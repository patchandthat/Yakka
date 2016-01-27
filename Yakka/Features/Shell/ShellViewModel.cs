using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Yakka.Actors;
using Yakka.Common.Paths;
using Yakka.Features.Conversations;
using Yakka.Features.Dialogs;
using Yakka.Features.HomeScreen;
using Yakka.Features.InfoPage;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private IScreen _activeContent;

        private readonly Dictionary<Screens, Screen> _screens = new Dictionary<Screens, Screen>();

        public ShellViewModel(HomeViewModel home, SettingsViewModel settings, InfoPageViewModel infoPage, ConversationsViewModel convos, ActorSystem system)
        {
            _screens.Add(Screens.Home, home);
            _screens.Add(Screens.Settings, settings);
            _screens.Add(Screens.Info, infoPage);
            _screens.Add(Screens.Conversations, convos);

            system.ActorSelection(ClientActorPaths.ErrorDialogActor.Path)
                .Tell(new ErrorDialogActor.RegisterShell(this));
        }

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
        
        protected override void OnInitialize()
        {
            DisplayName = "Yakka";

            ActiveContent = _screens[Screens.Home];

            //Do first load initialisation
            var settings = _screens[Screens.Settings] as SettingsViewModel;
            settings.CancelButton();

            base.OnInitialize();
        }

        public void ConnectButton()
        {
            DebugShowDialog();
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

        public async void DebugShowDialog()
        {
            //Todo: read http://dragablz.net/2015/10/09/wpf-dialog-boxes-in-material-design-in-xaml-toolkit/
            //Take a look at https://github.com/Codeusa/SteamCleaner - uses this dialog method nicely. Look at CleanerUtilities.CleanData()

            const string myText = "Todo: this stuff is not yet supported.";

            var dialog = new ErrorDialog()
            {
                MessageTextBlock =
                {
                    Text = myText
                }
            };

            //Todo: Need to take protective steps, if this is re-entered while the dialog is open it's gonna go bang
            var result = await DialogHost.Show(dialog);
        }
    }
}
