using System.Linq.Expressions;
using System.Reflection;
using WV.Interfaces;
using static WV.AppManager;

namespace WV
{
    public abstract class Plugin : IDisposable
    {
        #region STATICS

        public static void ThrowDispose(Plugin plugin)
        {
            if (plugin.Disposed)
                throw new ObjectDisposedException("This instance of the " + plugin.Name + " plugin is disposed");
        }

        private string WVEventName { get; } = typeof(WVEventHandler).Name;

        #endregion

        protected IWebView WebView { get; private set; }
        public string UID { get; }
        public string Name { get; }
        public bool Disposed { get; private set; }
        protected Plugin(IWebView webView)
        {
            this.WebView = webView;
            this.UID = Guid.NewGuid().ToString();
            this.Name = this.GetType().Name;
        }

        protected abstract void Dispose(bool disposing);

        #region EVENTS

        /// <summary>
        /// Appends an event listener for events whose type attribute value is type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddEventListener(string type, object callback)
        {
            Plugin.ThrowDispose(this);

            // 1. Obtener información del evento
            EventInfo? eventInfo = GetType().GetEvent(type);
            if (eventInfo == null)
                return;
                //throw new ArgumentException($"Evento '{eventName}' no encontrado");

            // 2. Obtener el tipo del delegado del evento
            Type? delegateType = eventInfo.EventHandlerType;
            if (delegateType == null || !delegateType.Name.StartsWith(WVEventName))
                return;
                //throw new ArgumentException($"No se ha encontrado el eventInfo.EventHandlerType");

            // IJSFunction su implementación validará si "raw" es una función JS
            IJSFunction fn = IJSFunction.Create(callback);

            ParameterInfo[] parameters = delegateType.GetMethod("Invoke")!.GetParameters();

            // Validar que hay al menos un parámetro (según el requerimiento)
            if (parameters.Length < 1)
                throw new InvalidOperationException("El evento debe tener al menos un parámetro");

            Type[] paramTypes = parameters
                .Skip(1)    // Excluir el primer parametro que será el sender (IWebView) normalmente
                .Select(p => p.ParameterType)
                .ToArray();

            // 3. Construir expresión lambda dinámica que llama a IJSFunction.Execute
            //ParameterExpression[] paramExpressions = paramTypes
            //    .Select((t, i) => Expression.Parameter(t, $"p{i}"))
            //    .ToArray();

            // 3. Construir parámetros para la expresión (todos los del evento)
            ParameterExpression[] paramExpressions = parameters
                .Select((p, i) => Expression.Parameter(p.ParameterType, $"p{i}"))
                .ToArray();

            // Convertir parámetros a object[] y llamar a Invocar
            //NewArrayExpression argsArray = Expression.NewArrayInit(
            //    typeof(object),
            //    paramExpressions.Select(p => Expression.Convert(p, typeof(object)))
            //);

            // Convertir parámetros a object[] (excluyendo el primero)
            NewArrayExpression argsArray = Expression.NewArrayInit(
                typeof(object),
                paramExpressions
                    .Skip(1)  // <--- Excluir el primer parámetro
                    .Select(p => Expression.Convert(p, typeof(object)))
            );

            MethodCallExpression executeCall = Expression.Call(
                Expression.Constant(fn),
                typeof(IJSFunction).GetMethod("Execute")!,
                argsArray
            );

            Delegate handler = Expression.Lambda(delegateType, executeCall, paramExpressions)
                .Compile();

            eventInfo.AddEventHandler(this, handler);
            TrackHandler(type, fn, handler);
        }

        /// <summary>
        /// Removes the event listener in target's event listener list with the same type and callback.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void RemoveEventListener(string type, object callback)
        {
            Plugin.ThrowDispose(this);

            if (!_rawFNs.TryGetValue(type, out var rawsIJSFn))
                return;

            if (!_eventHandlers.TryGetValue(type, out var handlers))
                return;

            // Para obtener el IJSFunction a travez del Raw
            IJSFunction? fn = rawsIJSFn.Find(x => x.Item1 == callback)?.Item2;
            if (fn == null)
                return;

            // Obtener el evento y remover todos los handlers asociados al IJSfunction
            EventInfo? eventInfo = GetType().GetEvent(type);
            if (eventInfo == null) 
                return;

            foreach (var handlerTuple in handlers.Where(t => t.Item1 == fn).ToList())
            {
                eventInfo.RemoveEventHandler(this, handlerTuple.Item2);
                handlers.Remove(handlerTuple);
            }

            foreach (var rawTuple in rawsIJSFn.Where(t => t.Item2 == fn).ToList())
            {
                rawsIJSFn.Remove(rawTuple);
                rawTuple.Item2.Dispose();
            }
        }

        private readonly Dictionary<string, List<Tuple<IJSFunction, Delegate>>> _eventHandlers = new();
        private readonly Dictionary<string, List<Tuple<object?, IJSFunction>>> _rawFNs = new();

        private void TrackHandler(string eventName, IJSFunction fn, Delegate handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
                _eventHandlers[eventName] = new List<Tuple<IJSFunction, Delegate>>();

            if(!_rawFNs.ContainsKey(eventName))
                _rawFNs[eventName] = new List<Tuple<object?, IJSFunction>>();

            _eventHandlers[eventName].Add(Tuple.Create(fn, handler));
            _rawFNs[eventName].Add(Tuple.Create(fn.Raw, fn));
        }


        #endregion

        protected void CleanJSEvents()
        {
            if(this.Disposed)
                return;

            // Eliminar todos los handlers de eventos
            foreach (var eventEntry in _eventHandlers)
            {
                EventInfo? eventInfo = GetType().GetEvent(eventEntry.Key);
                if (eventInfo == null) 
                    continue;

                foreach (var handlerTuple in eventEntry.Value)
                    eventInfo.RemoveEventHandler(this, handlerTuple.Item2);
                
                foreach (var rawTuple in _rawFNs[eventEntry.Key])
                    rawTuple.Item2.Dispose();
            }

            _eventHandlers.Clear();
            _rawFNs.Clear();
        }

        public virtual void Dispose()
        {
            if (this.Disposed)
                return;

            // Evitar que el Garbage Collector llame al destructor/Finalizador ~Plugin()
            GC.SuppressFinalize(this);

            DisposeClear(true);
        }

        // Destructor para limpieza de emergencia
        ~Plugin()
        {
            DisposeClear(false);
        }

        private void DisposeClear(bool disposing)
        {
            CleanJSEvents();
            Dispose(disposing);

            this.Disposed = true;

            object wv = (object)this.WebView;
            wv.GetType().GetMethod("PluginDisposed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(wv, new object[] { this.UID });
            
#pragma warning disable CS8625 // No se puede convertir un literal NULL en un tipo de referencia que no acepta valores NULL.
            this.WebView = null;
#pragma warning restore CS8625 // No se puede convertir un literal NULL en un tipo de referencia que no acepta valores NULL.
        }
    
    }
}