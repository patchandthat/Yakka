using Caliburn.Micro;
using Yakka.Features.HomeScreen;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private IScreen _activeContent;
        private readonly IEventAggregator _aggregator;
        private readonly HomeViewModel _home;
        private readonly SettingsViewModel _settings;

        public IScreen ActiveContent
        {
            get { return _activeContent; }
            set
            {
                if (Equals(value, _activeContent)) return;
                _activeContent = value;
                NotifyOfPropertyChange(() => ActiveContent);
            }
        }

        public ShellViewModel(IEventAggregator agg, HomeViewModel home, SettingsViewModel settings)
        {
            _aggregator = agg;
            _home = home;
            _settings = settings;
        }

        protected override void OnInitialize()
        {
            DisplayName = "Yakka";

            ActiveContent = _home;

            base.OnInitialize();
        }
    }
}
