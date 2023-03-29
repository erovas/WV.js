using WV.Windows.Utils;

namespace WV.Windows.JavaScript
{
    public class Function : WV.JavaScript.Function
    {
        public Function(object fn, string stringified) : base()
        {
            _JSValue = fn;
            _Stringified = stringified;
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