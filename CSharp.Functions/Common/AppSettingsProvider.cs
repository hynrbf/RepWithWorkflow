namespace Common
{
    public class AppSettingsProvider
    {
        private static AppSettingsProvider _instanceObj;

        public static AppSettingsProvider Instance => _instanceObj ??= new AppSettingsProvider();

        private Func<Lazy<Dictionary<string, string>>> _defaultSettingsDelegate;

        private Lazy<Dictionary<string, string>> _appSettings;

        public void SetAppSettings(Func<Lazy<Dictionary<string, string>>> action)
            => _defaultSettingsDelegate = action;

        public string GetValue(string key)
        {
            _appSettings ??= _defaultSettingsDelegate();
            var theSettings = _appSettings.Value;
            return !theSettings.TryGetValue(key, out var value) ? string.Empty : value;
        }

        public void SetValue(string key, string value)
        {
            _appSettings ??= _defaultSettingsDelegate();
            var theSettings = _appSettings.Value;
            theSettings[key] = value;
        }
    }
}