namespace WV.WebView.Entities
{
    public class Config
    {
        #region DEFAULT VALUES

        private string[] _Plugins = Array.Empty<string>();
        private Parameters _Parameters = new();

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public string[] Plugins 
        {
            get => _Plugins;
            set => _Plugins = value?? _Plugins;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Parameters Parameters 
        { 
            get => _Parameters; 
            set => _Parameters = value ?? _Parameters; 
        }
    }
}