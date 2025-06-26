using System.Runtime.InteropServices;
using WV.Interfaces;
using WV.Win.Invoke;

namespace WV.Win.Imp
{
    public class JSFunction : IJSFunction
    {
        public object? Raw { get; internal set; }
        public bool Disposed { get; private set; }

        public JSFunction(object? rawJS = null)
        {
            this.Raw = rawJS;
        }

        public void Execute(params object[] args)
        {
            if (this.Disposed)
                throw new Exception("IJSFunction disposed");

            Task.Run(() =>
            {
                if (!this.Disposed && this.Raw != null)
                    Invoker.ExecuteMethod(this.Raw, "", args);
            });
        }

        public void Dispose()
        {
            // Evitar que el Garbage Collector llame al destructor/Finalizador ~Plugin()
            GC.SuppressFinalize(this);
            this.Disposed = true;
            ReleaseComObject(this.Raw);
            this.Raw = null;
        }

        ~JSFunction()
        {
            this.Disposed = true;
            ReleaseComObject(this.Raw);
            this.Raw = null;
        }

        private static void ReleaseComObject(object? objCom)
        {
            if (objCom == null)
                return;

            try
            {
                Marshal.ReleaseComObject(objCom);
            }
            catch (Exception){  }
        }
    }
}