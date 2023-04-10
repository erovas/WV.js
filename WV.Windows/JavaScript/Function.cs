using WV.Windows.Utils;

namespace WV.Windows.JavaScript
{
    public class Function : WV.JavaScript.Function
    {
        public Function(object fn, string stringified, bool isAsync = false) : base()
        {
            _JSValue = fn;
            _Stringified = stringified;
            _Async = isAsync;

            if (_Async)
                _JSType = WV.JavaScript.Enums.JSType.AsyncFunction;
        }

        public override void Execute(params object[] args)
        {
            try
            {
                InvokeMemberHelper.InvokeDispMethod(this.JSValue, null, args);
                return;
            }
            catch (Exception) { }

            try
            {
                Task.Run(() => { InvokeMemberHelper.InvokeDispMethod(this.JSValue, null, args); });
                return;
            }
            catch (Exception) { }
        }
    }
}