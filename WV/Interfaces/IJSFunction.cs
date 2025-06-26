namespace WV.Interfaces
{
    public interface IJSFunction : IDisposable
    {
        private static Type? _type;

        public static IJSFunction Create(object JSFunction)
        {
            if (JSFunction == null)
                throw new ArgumentNullException(nameof(JSFunction) + " cannot be null");

            if (!JSFunction.GetType().IsCOMObject)
                throw new ArgumentException("Invalid " + nameof(JSFunction));

            if (_type == null)
            {
                Type itype = typeof(IJSFunction);
                List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetTypes())
                                    .Where(p => itype.IsAssignableFrom(p) && p != itype).ToList();

                _type = types.FirstOrDefault();
            }

            if (_type == null)
                throw new Exception("IJSFunction is not implemented");

            object? value = Activator.CreateInstance(_type, new object[] { JSFunction });

            if (value == null)
                throw new Exception("Impossible create instance");

            return (IJSFunction)value;
        }

        object? Raw { get; }
        bool Disposed { get; }

        /// <summary>
        /// Execute JavaScript Function
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        void Execute(params object[] args);

    }
}