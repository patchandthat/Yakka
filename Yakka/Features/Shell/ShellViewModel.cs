using System.Collections.Generic;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.ClientActorSystem;
using Yakka.ClientActorSystem.Actors.UI.Input;
using Yakka.ClientActorSystem.Actors.UI.Update;
using Yakka.Features.Conversations;
using Yakka.Features.HomeScreen;
using Yakka.Features.InfoPage;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private IScreen _activeContent;

        private readonly Dictionary<Screens, Screen> _screens = new Dictionary<Screens, Screen>();
        private readonly IActorRef _inputActor;

        public ShellViewModel(HomeViewModel home, SettingsViewModel settings, InfoPageViewModel infoPage, ConversationsViewModel convos, ActorSystem system)
        {
            _screens.Add(Screens.Home, home);
            _screens.Add(Screens.Settings, settings);
            _screens.Add(Screens.Info, infoPage);
            _screens.Add(Screens.Conversations, convos);

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new ShellInputActor()), ActorPaths.ShellInputActor.Name);

            //UI updating actor
            system.ActorOf(Props.Create(() => new ShellUpdateActor(this)), ActorPaths.ShellViewModelActor.Name);
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
