namespace WV.Win.Classes
{
    public sealed class SubscriptionBag : IDisposable
    {
        private readonly List<Action> _unsubscribers = new();
        private readonly object _lock = new();
        private bool _disposed;

        public int Count
        {
            get { lock (_lock) return _unsubscribers.Count; }
        }

        public bool IsDisposed
        {
            get { lock (_lock) return _disposed; }
        }

        /// <summary>
        /// Registra una desuscripción. Si el bag ya fue liberado,
        /// la ejecuta inmediatamente (evita suscripciones zombies).
        /// </summary>
        public void Add(Action unsubscribe)
        {
            if (unsubscribe == null) 
                throw new ArgumentNullException(nameof(unsubscribe));

            bool runNow;

            lock (_lock)
            {
                runNow = _disposed;
                if (!runNow) 
                    _unsubscribers.Add(unsubscribe);
            }

            // Ya estamos liberados: desuscribe inmediatamente para no dejar basura.
            if (runNow)    
                SafeInvoke(unsubscribe);
            
        }

        /// <summary>
        /// Registra una suscripción y su desuscripción en una sola llamada.
        /// Ideal para usar con expresiones lambda que no puedes desuscribir de otra forma.
        /// </summary>
        public void Add(Action subscribe, Action unsubscribe)
        {
            if (subscribe == null) 
                throw new ArgumentNullException(nameof(subscribe));

            if (unsubscribe == null) 
                throw new ArgumentNullException(nameof(unsubscribe));

            // Si ya está liberado, no suscribas nada.
            lock (_lock)                
                if (_disposed) 
                    return;
            
            subscribe();
            Add(unsubscribe);
        }

        /// <summary>
        /// Libera todas las suscripciones registradas.
        /// Es seguro llamarlo múltiples veces.
        /// </summary>
        public void Dispose()
        {
            Action[] snapshot;
            lock (_lock)
            {
                if (_disposed) 
                    return;
                
                _disposed = true;
                snapshot = _unsubscribers.ToArray();
                _unsubscribers.Clear();
            }

            foreach (var unsub in snapshot)
                SafeInvoke(unsub);
        }

        private static void SafeInvoke(Action action)
        {
            try { action(); }
            catch
            {
                // Un unsubscribe fallido no debe romper el Dispose de los demás.
                // Si quieres trazarlo, aquí es el punto.
                System.Diagnostics.Debug.WriteLine($"[SubscriptionBag] Unsubscribe threw: {action}");
            }
        }
    }
}