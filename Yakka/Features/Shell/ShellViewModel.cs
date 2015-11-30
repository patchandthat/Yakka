using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using Yakka.Features.HomeScreen;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private IScreen _activeContent;
        private IScreen _flyoutContent;

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

        public IScreen FlyoutContent
        {
            get { return _flyoutContent; }
            set
            {
                if (Equals(value, _flyoutContent)) return;
                _flyoutContent = value;
                NotifyOfPropertyChange(() => FlyoutContent);
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
            FlyoutContent = _settings;

            base.OnInitialize();
        }

        public void GitHubButton()
        {
            Process.Start("https://github.com/patchandthat/Yakka");
        }

        public void ShowLeftFlyout()
        {
            ((ShellView)Application.Current.MainWindow).LeftFlyout.IsOpen = true;
        }
    }
}
