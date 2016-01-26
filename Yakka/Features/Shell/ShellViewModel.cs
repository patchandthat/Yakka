using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Akka.Actor;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Yakka.Actors;
using Yakka.Common.Paths;
using Yakka.Features.Conversations;
using Yakka.Features.HomeScreen;
using Yakka.Features.InfoPage;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private IScreen _activeContent;
        private ResourceDictionary DialogDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };

        private readonly Dictionary<Screens, Screen> _screens = new Dictionary<Screens, Screen>();

        public ShellViewModel(HomeViewModel home, SettingsViewModel settings, InfoPageViewModel infoPage, ConversationsViewModel convos, ActorSystem system, IWindowManager windowManager)
        {
            _windowManager = windowManager;
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

        public void DebugShowDialog()
        {
            var settings = new MetroDialogSettings()
            {
                CustomResourceDictionary = DialogDictionary,
                SuppressDefaultResources = true
            };

            try
            {
                Task<MessageDialogResult> tResult = DialogCoordinator.Instance.ShowMessageAsync(this, "DIALOG TITLE", "MESSAGE: TODO", MessageDialogStyle.Affirmative, settings);
            }
            catch (Exception ex)
            {
                //inspect me
                throw;
            }
        }

        public void DebugShowDialog2()
        {
            //Todo: read http://dragablz.net/2015/10/09/wpf-dialog-boxes-in-material-design-in-xaml-toolkit/
        }
    }
}
