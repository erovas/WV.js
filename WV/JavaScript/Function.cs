using WV.JavaScript.Enums;

namespace WV.JavaScript
{
    public abstract class Function : Value
    {
        protected bool _Async;
        public bool Async => _Async;

        /// <summary>
        /// Execute function asynchronously
        /// </summary>
        /// <param name="args"></param>
        public abstract void Execute(params object[] args);

        protected Function() 
        {
            _JSType = JSType.Function;
        }
    }
}
