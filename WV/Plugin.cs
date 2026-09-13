using System.Linq.Expressions;
using System.Reflection;
using WV.Interfaces;
using static WV.AppManager;

namespace WV
{
    public abstract class Plugin : IDisposable
    {
        #region Statics

        public static void ThrowIfDisposed(Plugin plugin)
        {
            if (plugin is null) 
                throw new ArgumentNullException(nameof(plugin));

            if (plugin.Disposed)
                throw new ObjectDisposedException($"This instance of the {plugin.Name} plugin is disposed");
        }

        private static string WVEventName { get; } = typeof(WVEventHandler).Name;

        private static FieldInfo[] DiscoverEventFields(Type type)
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.DeclaredOnly;

            var result = new System.Collections.Generic.List<FieldInfo>();

            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                foreach (var evt in t.GetEvents(flags))
                {
                    // Para un evento field-like, el campo de respaldo se llama igual.
                    var field = t.GetField(evt.Name, flags);
                    if (field != null)
                        result.Add(field);
                }
            }

            return result.ToArray();
        }

        #endregion

        #region Fields

        private readonly IContext _context;
        private int _disposed;
        private readonly Dictionary<string, List<Tuple<IJSFunction, Delegate>>> _eventHandlers = new();
        private readonly Dictionary<string, List<Tuple<object?, IJSFunction>>> _rawFNs = new();

        #endregion

        #region Properties

        protected IWebView WebView => _context.WebView!;
        protected ILogger Logger => _context.Logger!;
        public string UID => _context.UID;
        public string Name => _context.Name;
        public bool Disposed => Volatile.Read(ref _disposed) == 1;

        #endregion

        protected Plugin(IContext context)
        {
            if(AppManager.IsInitialized && context.WebView is null)
                throw new NullReferenceException("Context.WebView cannot be null.");

            this._context = context;
        }

        /// <summary>
        /// Appends an event listener for events whose type attribute value is type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddEventListener(string type, object callback)
        {
            Logger.Debug($"##. Ejecutando AddEventListener(\"{type}\", {nameof(callback)})");

            try
            {
                Logger.Debug("0. Comprobando que el plugin está Disposed");
                ThrowIfDisposed();

                Logger.Debug("1. Obteniendo información del evento (EventInfo)");
                EventInfo? eventInfo = GetType().GetEvent(type);
                if (eventInfo == null)
                {
                    Logger.Debug($"1.1 No existe el evento (\"{type}\")");
                    return;
                }
                    
                Logger.Debug("2. Obteniendo el tipo del delegado del evento");
                Type? delegateType = eventInfo.EventHandlerType;
                if (delegateType == null || !delegateType.Name.StartsWith(Plugin.WVEventName))
                {
                    Logger.Debug($"2.1 No existe el delegado para el evento (\"{type}\")");
                    return;
                }

                Logger.Debug("3. Creando JSFunction");
                IJSFunction fn = IJSFunction.Create(callback);

                Logger.Debug("4. Obteniendo información de los parametros del delegado");
                ParameterInfo[] parameters = delegateType.GetMethod("Invoke")!.GetParameters();

                Logger.Debug("5. Validando que hay al menos un parametro en el delegado");
                if (parameters.Length < 1)  // Primer parametro IWebView obligatorio
                    throw new InvalidOperationException("El evento debe tener al menos un parámetro");

                Logger.Debug("6. Construyendo parámetros para la expresión (todos los del evento)");
                ParameterExpression[] paramExpressions = parameters
                    .Select((p, i) => Expression.Parameter(p.ParameterType, $"p{i}"))
                    .ToArray();

                Logger.Debug("7. Creando IEnumerable<Expression>");
                IEnumerable<Expression> IEExpression = paramExpressions
                    .Skip(1)  // Excluir el primer parametro que será el sender (IWebView).
                    .Select(p => Expression.Convert(p, typeof(object)));

                Logger.Debug("8. Creando NewArrayExpression");
                NewArrayExpression argsArray = Expression.NewArrayInit(typeof(object), IEExpression);

                Logger.Debug("9. Creando MethodCallExpression");
                MethodCallExpression executeCall = Expression.Call(
                    Expression.Constant(fn),
                    typeof(IJSFunction).GetMethod("Execute")!,
                    argsArray
                );

                Logger.Debug("10. Creando manejador del delegado (Delegate)");
                Delegate handler = Expression.Lambda(delegateType, executeCall, paramExpressions).Compile();

                Logger.Debug("11. Agregando el manejador del delegado creado (Delegate) al evento (EventInfo)");
                eventInfo.AddEventHandler(this, handler);

                Logger.Debug("12. Seguimiento de los manejadores");
                if (!_eventHandlers.ContainsKey(type))
                    _eventHandlers[type] = new List<Tuple<IJSFunction, Delegate>>();

                if (!_rawFNs.ContainsKey(type))
                    _rawFNs[type] = new List<Tuple<object?, IJSFunction>>();

                _eventHandlers[type].Add(Tuple.Create(fn, handler));
                _rawFNs[type].Add(Tuple.Create(fn.Raw, fn));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Removes the event listener in target's event listener list with the same type and callback.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void RemoveEventListener(string type, object callback)
        {
            Logger.Debug($"##. Ejecutando RemoveEventListener(\"{type}\", {nameof(callback)})");

            try
            {
                Logger.Debug("0. Comprobando que el plugin está Disposed");
                ThrowIfDisposed();

                Logger.Debug("1. Obteniendo información del evento (EventInfo)");
                EventInfo? eventInfo = GetType().GetEvent(type);

                if (eventInfo == null)
                {
                    Logger.Debug($"1.1 No existe el evento (\"{type}\")");
                    return;
                }

                Logger.Debug("2. Obteniendo el <Raw, IJSFunction> asociado al evento");
                if (!_rawFNs.TryGetValue(type, out var rawsIJSFn))
                {
                    Logger.Debug($"2.1 No existe el <Raw, IJSFunction> asociado al evento (\"{type}\")");
                    return;
                }

                Logger.Debug("3. Obteniendo el <IJSFunction, Delegate> asociado al evento");
                if (!_eventHandlers.TryGetValue(type, out var handlers))
                {
                    Logger.Debug($"3.1 No existe el <IJSFunction, Delegate> asociado al evento (\"{type}\")");
                    return;
                }

                Logger.Debug($"4. Obteniendo el IJSFunction a travez del Raw ({nameof(callback)})");
                IJSFunction? fn = rawsIJSFn.Find(x => x.Item1 == callback)?.Item2;

                if (fn == null)
                {
                    Logger.Debug($"4.1 No existe un IJSFunction asociado al evento (\"{type}\") a travez del Raw ({nameof(callback)})");
                    return;
                }

                Logger.Debug("5. Removiendo todos los manejadores (Delegate) del evento (EventInfo) y del seguimiento");
                foreach (var handlerTuple in handlers.Where(t => t.Item1 == fn).ToList())
                {
                    eventInfo.RemoveEventHandler(this, handlerTuple.Item2);
                    handlers.Remove(handlerTuple);
                }

                Logger.Debug("6. Removiendo todas los IJSFunction del seguimiento");
                foreach (var rawTuple in rawsIJSFn.Where(t => t.Item2 == fn).ToList())
                {
                    rawsIJSFn.Remove(rawTuple);
                    rawTuple.Item2.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            try
            {
                ClearListeners();
                ClearEvents();
                Dispose(true);
            }
            catch (Exception ex)
            {
                Logger.Error($"Disposal failed for {Name} ({UID})", ex);
            }
            finally
            {
                try 
                { 
                    _context.OnDisposed?.Invoke(UID); 
                }
                catch (Exception ex) 
                { 
                    Logger.Error($"Notify failed for {UID}", ex); 
                }

                _context.Release();
                Logger.Debug($"Plugin disposed: {UID} ({Name})");
            }
        }

        //=======================================//

        protected virtual void Dispose(bool disposing)
        {
            // Do nothing
        }

        protected void ClearListeners()
        {
            if (this.Disposed)
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

        /// <summary>
        /// Pone a null el campo de respaldo de todos los eventos field-like
        /// declarados en esta instancia y en toda su jerarquía.
        /// No pasa por add/remove.
        /// </summary>
        protected virtual void ClearEvents()
        {
            if (this.Disposed)
                return;

            var fields = DiscoverEventFields(this.GetType());

            // Asignación directa al campo: no invoca remove().
            foreach (var f in fields)    
                f.SetValue(this, null);
        }

        protected virtual void ThrowIfDisposed()
        {
            Plugin.ThrowIfDisposed(this);
        }

    }
}