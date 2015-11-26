using Caliburn.Micro;
using Yakka.Features.Flyouts;
using Yakka.Features.Flyouts.ServerSettings;

namespace Yakka.Features.Shell
{
    class ShellViewModel : Screen
    {
        private readonly IObservableCollection<FlyoutBaseViewModel> _flyoutsViewModels =
            new BindableCollection<FlyoutBaseViewModel>();

        private IEventAggregator _aggregator;
        private FlyoutBaseViewModel _flyout;

        public IObservableCollection<FlyoutBaseViewModel> FlyoutsViewModels
        {
            get { return _flyoutsViewModels; }
        }

        public FlyoutBaseViewModel Flyout { get { return _flyout; } }

        public ShellViewModel(IEventAggregator agg, ServerSettingsFlyoutViewModel flyout)
        {
            _aggregator = agg;
            _flyout = flyout;
            _flyoutsViewModels.Add(flyout);
        }

        protected override void OnInitialize()
        {
            DisplayName = "Yakka";

            base.OnInitialize();
        }

        public void ToggleFlyout()
        {
            var flyout = _flyoutsViewModels[0];
            flyout.IsOpen = !flyout.IsOpen;
            NotifyOfPropertyChange(() => FlyoutsViewModels);
            NotifyOfPropertyChange(() => Flyout);
        }
    }
}
