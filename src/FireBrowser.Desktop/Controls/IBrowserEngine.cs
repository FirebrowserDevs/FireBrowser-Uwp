namespace FireBrowser.Controls
{
    /// <summary>
    /// Work in progress class in order to allow using a custom engine.
    /// </summary>
    interface IBrowserEngine
    {
        public bool CanGoBack { get; }
        public bool CanGoForward { get; }
    }
}
