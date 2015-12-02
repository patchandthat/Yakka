using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using Yakka.Features.Conversations;
using Yakka.Features.HomeScreen;
using Yakka.Features.InfoPage;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private IScreen _activeContent;

        private readonly IEventAggregator _aggregator;

        private readonly Dictionary<Screens, Screen> _screens = new Dictionary<Screens, Screen>();

        private enum Screens
        {
            Home,
            Settings,
            Info,
            Conversations
        }

        public IScreen ActiveContent
        {
            get { return _activeContent; }
            set
            {
                if (Equals(value, _activeContent)) return;
                _activeContent = value;
                NotifyOfPropertyChange(() => ActiveContent);
                NotifyOfPropertyChange(() => ActiveContentName);
            }
        }

        public string ActiveContentName { get { return ActiveContent == null ? "" : ActiveContent.DisplayName ?? ""; } }

        public string ConnectionState { get { return "Not connected"; } }

        public ShellViewModel(IEventAggregator agg, HomeViewModel home, SettingsViewModel settings, InfoPageViewModel infoPage, ConversationsViewModel convos)
        {
            _aggregator = agg;

            _screens.Add(Screens.Home, home);
            _screens.Add(Screens.Settings, settings);
            _screens.Add(Screens.Info, infoPage);
            _screens.Add(Screens.Conversations, convos);
        }

        protected override void OnInitialize()
        {
            DisplayName = "Yakka";

            ActiveContent = _screens[Screens.Home];

            base.OnInitialize();
        }

        public void ConnectButton()
        {
            
        }

        public void HomeButton()
        {
            ActiveContent = _screens[Screens.Home];
        }

        public void ConversationsButton()
        {
            ActiveContent = _screens[Screens.Conversations];
        }

        public void SettingsButton()
        {
            ActiveContent = _screens[Screens.Settings];
        }

        public void InfoButton()
        {
            ActiveContent = _screens[Screens.Info];
        }
    }
}
