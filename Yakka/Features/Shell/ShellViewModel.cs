using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Akka.Actor;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Yakka.Actors;
using Yakka.Actors.UI;
using Yakka.Common.Paths;
using Yakka.Features.Conversations;
using Yakka.Features.Dialogs;
using Yakka.Features.HomeScreen;
using Yakka.Features.Settings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private IScreen _activeContent;

        private readonly Dictionary<Screens, Screen> _screens = new Dictionary<Screens, Screen>();
        private readonly ConcurrentQueue<ErrorDialogActor.ErrorMessage> _errorDialogs = new ConcurrentQueue<ErrorDialogActor.ErrorMessage>();

        private readonly IActorRef _shellViewModelActor;
        private bool _isConnected;

        public ShellViewModel(HomeViewModel home, SettingsViewModel settings, ConversationsViewModel convos, IActorRefFactory system)
        {
            _screens.Add(Screens.Home, home);
            _screens.Add(Screens.Settings, settings);
            _screens.Add(Screens.Conversations, convos);

            system.ActorSelection(ClientActorPaths.ErrorDialogActor.Path)
                .Tell(new ErrorDialogActor.RegisterShell(this));

            _shellViewModelActor = system.ActorOf(Props.Create(() => new ShellViewModelActor(this)),
                ClientActorPaths.ShellViewModelActor.Name);
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

        public string ConnectionState => IsConnected ? "Connected" : "Not connected";

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value == _isConnected) return;
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => ConnectionState);
            }
        }

        protected override void OnInitialize()
        {
            DisplayName = "Yakka";

            ActiveContent = _screens[Screens.Home];

            //Do first load initialisation
            var settings = (SettingsViewModel) _screens[Screens.Settings];
            settings.CancelButton();

            base.OnInitialize();

            Task.Run(() => ProcessErrorDialogQueue());
        }
        
        public void ConnectButton()
        {
            object request = IsConnected
                ? (object)new ConnectionActor.Disconnect()
                : (object)new ConnectionActor.ConnectRequest();
            _shellViewModelActor.Tell(request);
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

        public void QueueErrorDialog(ErrorDialogActor.ErrorMessage errorMessage)
        {
            _errorDialogs.Enqueue(errorMessage);
        }

        private async void ProcessErrorDialogQueue()
        {
            while (true)
            {
                if (!_errorDialogs.IsEmpty)
                {
                    ErrorDialogActor.ErrorMessage message;
                    if (_errorDialogs.TryDequeue(out message))
                    {
                        await Application.Current.Dispatcher.Invoke(
                            async () =>
                            {
                                var d = message;
                                await ShowDialog(d);
                            });
                    }
                }

                await Task.Delay(500);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private Task<object> ShowDialog(ErrorDialogActor.ErrorMessage errorMessage)
        {
            var dialog = new ErrorDialog()
            {
                MessageTextBlock =
                {
                    Text = errorMessage.UserFriendlyMessage
                }
            };

            return DialogHost.Show(dialog);
        }
    }
}
