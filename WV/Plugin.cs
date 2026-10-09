using System.Linq.Expressions;
using System.Reflection;
using WV.Interfaces;
using static WV.App.Delegates;

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

        private readonly PluginContext _context;
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

        protected Plugin(IPluginContext context)
        {
            if(App.IsInitialized && context.WebView is null)
                throw new NullReferenceException("Context.WebView cannot be null.");

            this._context = (PluginContext)context;
            this._context._webview ??= (IWebView)this;
        }

        /// <summary>
        /// Appends an event listener for events whose type attribute value is type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void AddEventListener(string type, object callback)
        {
            var txt = $"{nameof(AddEventListener)}(\"{type}\", {nameof(callback)}) method.";
            Logger.Info("##. Calling " + txt);

            try
            {
                Logger.Debug("0. Verifying that the plugin is disposed.");
                ThrowIfDisposed();

                Logger.Debug("1. Retrieving event information (EventInfo).");
                EventInfo? eventInfo = GetType().GetEvent(type);
                if (eventInfo == null)
                {
                    Logger.Debug($"1.1 The (\"{type}\") event does not exist.");
                    return;
                }
                    
                Logger.Debug("2. Obtaining the event delegate type.");
                Type? delegateType = eventInfo.EventHandlerType;
                if (delegateType == null || !delegateType.Name.StartsWith(Plugin.WVEventName))
                {
                    Logger.Debug($"2.1 There is no delegate for the (\"{type}\") event.");
                    return;
                }

                Logger.Debug("3. Creating JSFunction.");
                IJSFunction fn = IJSFunction.Create(callback);

                Logger.Debug("4. Retrieving information from the delegate parameters.");
                ParameterInfo[] parameters = delegateType.GetMethod("Invoke")!.GetParameters();

                Logger.Debug("5. Validating that there is at least one parameter in the delegate.");
                if (parameters.Length < 1)  // Primer parametro IWebView obligatorio
                    throw new InvalidOperationException("The event must have at least one parameter.");

                Logger.Debug("6. Constructing parameters for the expression (all those from the event).");
                ParameterExpression[] paramExpressions = parameters
                    .Select((p, i) => Expression.Parameter(p.ParameterType, $"p{i}"))
                    .ToArray();

                Logger.Debug("7. Creating IEnumerable<Expression>");
                IEnumerable<Expression> IEExpression = paramExpressions
                    .Skip(1)  // Excluir el primer parametro que será el sender (IWebView).
                    .Select(p => Expression.Convert(p, typeof(object)));

                Logger.Debug("8. Creating NewArrayExpression");
                NewArrayExpression argsArray = Expression.NewArrayInit(typeof(object), IEExpression);

                Logger.Debug("9. Creating MethodCallExpression");
                MethodCallExpression executeCall = Expression.Call(
                    Expression.Constant(fn),
                    typeof(IJSFunction).GetMethod("Execute")!,
                    argsArray
                );

                Logger.Debug("10. Creating the delegate handler.");
                Delegate handler = Expression.Lambda(delegateType, executeCall, paramExpressions).Compile();

                Logger.Debug("11. Adding the created delegate handler to the event (EventInfo).");
                eventInfo.AddEventHandler(this, handler);

                Logger.Debug("12. Handler tracking.");
                if (!_eventHandlers.ContainsKey(type))
                    _eventHandlers[type] = new List<Tuple<IJSFunction, Delegate>>();

                if (!_rawFNs.ContainsKey(type))
                    _rawFNs[type] = new List<Tuple<object?, IJSFunction>>();

                _eventHandlers[type].Add(Tuple.Create(fn, handler));
                _rawFNs[type].Add(Tuple.Create(fn.Raw, fn));
            }
            catch (Exception ex)
            {
                Logger.Error("@@. Fail calling " + txt, ex);
                throw;
            }
        }

        /// <summary>
        /// Removes the event listener in target's event listener list with the same type and callback.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public virtual void RemoveEventListener(string type, object callback)
        {
            var txt = $"{nameof(RemoveEventListener)}(\"{type}\", {nameof(callback)}) method.";
            Logger.Info($"##. Calling " + txt);

            try
            {
                Logger.Debug("0. Verifying that the plugin is disposed.");
                ThrowIfDisposed();

                Logger.Debug("1. Retrieving event information (EventInfo).");
                EventInfo? eventInfo = GetType().GetEvent(type);

                if (eventInfo == null)
                {
                    Logger.Debug($"1.1 The (\"{type}\") event does not exist.");
                    return;
                }

                Logger.Debug("2. Obtaining the <Raw, IJSFunction> associated with the event.");
                if (!_rawFNs.TryGetValue(type, out var rawsIJSFn))
                {
                    Logger.Debug($"2.1 The <Raw, IJSFunction> associated with the (\"{type}\") event does not exist.");
                    return;
                }

                Logger.Debug("3. Obtaining the <IJSFunction, Delegate> associated with the event.");
                if (!_eventHandlers.TryGetValue(type, out var handlers))
                {
                    Logger.Debug($"3.1 There is no <IJSFunction, Delegate> associated with the (\"{type}\") event.");
                    return;
                }

                Logger.Debug($"4. Obtaining the IJSFunction via the Raw object ({nameof(callback)}).");
                IJSFunction? fn = rawsIJSFn.Find(x => x.Item1 == callback)?.Item2;

                if (fn == null)
                {
                    Logger.Debug($"4.1 There is no IJSFunction associated with the (\"{type}\") event via the Raw ({nameof(callback)}).");
                    return;
                }

                Logger.Debug("5. Removing all handlers (delegates) from the event (EventInfo) and the tracking mechanism.");
                foreach (var handlerTuple in handlers.Where(t => t.Item1 == fn).ToList())
                {
                    eventInfo.RemoveEventHandler(this, handlerTuple.Item2);
                    handlers.Remove(handlerTuple);
                }

                Logger.Debug("6. Removing all IJSFunctions from the tracking.");
                foreach (var rawTuple in rawsIJSFn.Where(t => t.Item2 == fn).ToList())
                {
                    rawsIJSFn.Remove(rawTuple);
                    rawTuple.Item2.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("@@. Fail calling " + txt, ex);
                throw;
            }
        }

        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        ~Plugin()
        {
            CleanUp(false);
        }

        private void CleanUp(bool disposing)
        {
            Logger.Info($"##. Calling {nameof(Dispose)}({disposing}) method.");

            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            try
            {
                if (disposing)
                {
                    ClearListeners();
                    ClearEvents();
                }
                Dispose(disposing);
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

        protected abstract void Dispose(bool disposing);

        //=======================================//

        protected void ClearListeners()
        {
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

        protected void ClearEvents()
        {
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