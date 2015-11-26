using Caliburn.Micro;

namespace Yakka.Features.Flyouts.ServerSettings
{
    public class ServerSettingsFlyoutViewModel : FlyoutBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public ServerSettingsFlyoutViewModel(IEventAggregator aggregator)
        {
            Header = "Server settings";
            Position = MahApps.Metro.Controls.Position.Right;
            _eventAggregator = aggregator;
        }
    }
}
