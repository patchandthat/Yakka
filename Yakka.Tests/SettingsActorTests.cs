using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Xunit;
using Akka.TestKit.Xunit2;
using Autofac;
using FakeItEasy;
using Ploeh.AutoFixture;
using Yakka.Actors;
using Yakka.DataLayer;
using Yakka.DataModels;

namespace Yakka.Tests
{
    public class SettingsActorTests : TestKit
    {
        //Autofac
        private IContainer _container;
        //Akka DI
        private IDependencyResolver _resolver;

        /// <summary>
        /// Autofixture to generate test data
        /// </summary>
        private readonly Fixture _fixture = new Fixture();

        public SettingsActorTests()
        {
            InitDependencies();
        }

        private void InitDependencies()
        {
            var builder = new ContainerBuilder();

            var fakeDb = A.Fake<IYakkaDb>();

            builder.RegisterInstance(fakeDb).As<IYakkaDb>();
            builder.RegisterType<SettingsPersistenceWorkerActor>();

            _container = builder.Build();
            _resolver = new AutoFacDependencyResolver(_container, Sys);
        }

        [Fact]
        public void On_save_settings_request_saves_data_to_database()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var dbFake = _container.Resolve<IYakkaDb>();
            var settingsToSave = _fixture.Create<YakkaSettings>().AsImmutable();

            actor.Tell(new SettingsActor.SaveSettingsRequest(settingsToSave, Sys.DeadLetters));
            ExpectNoMsg();

            //Verify correct data was passed
            A.CallTo(() =>
                dbFake.SaveSettings(
                    A<YakkaSettings>.That.Matches(s =>
                        s.ServerAddress == settingsToSave.ServerAddress
                        && s.ServerPort == settingsToSave.ServerPort
                        && s.Username == settingsToSave.Username
                        && s.ConnectAutomatically == settingsToSave.ConnectAutomatically
                        && s.LaunchOnStartup == settingsToSave.LaunchOnStartup
                        && s.RememberSettings == settingsToSave.RememberSettings)))
                .MustHaveHappened();
        }

        [Fact]
        public void On_save_settings_request_responds_to_ReplyTo_actor_with_correct_settings()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var settingsToSave = _fixture.Create<YakkaSettings>().AsImmutable();

            actor.Tell(new SettingsActor.SaveSettingsRequest(settingsToSave, respondTo: TestActor));
            var msg = ExpectMsg<ImmutableYakkaSettings>();

            //Verify response message matches saved data
            Assert.Equal(settingsToSave.ServerAddress, msg.ServerAddress);
            Assert.Equal(settingsToSave.ServerPort, msg.ServerPort);
            Assert.Equal(settingsToSave.Username, msg.Username);
            Assert.Equal(settingsToSave.ConnectAutomatically, msg.ConnectAutomatically);
            Assert.Equal(settingsToSave.LaunchOnStartup, msg.LaunchOnStartup);
            Assert.Equal(settingsToSave.RememberSettings, msg.RememberSettings);
        }

        [Fact]
        public void First_load_settings_request_fetches_from_database()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var dbFake = _container.Resolve<IYakkaDb>();
            var expectedSettings = _fixture.Create<YakkaSettings>();
            A.CallTo(() => dbFake.LoadSettings())
                .Returns(expectedSettings);

            actor.Tell(new SettingsActor.LoadSettingsRequest(respondTo: TestActor));
            var msg = ExpectMsg<ImmutableYakkaSettings>();

            A.CallTo(() => dbFake.LoadSettings())
                .MustHaveHappened();

            Assert.Equal(expectedSettings.ServerAddress, msg.ServerAddress);
            Assert.Equal(expectedSettings.ServerPort, msg.ServerPort);
            Assert.Equal(expectedSettings.Username, msg.Username);
            Assert.Equal(expectedSettings.ConnectAutomatically, msg.ConnectAutomatically);
            Assert.Equal(expectedSettings.LaunchOnStartup, msg.LaunchOnStartup);
            Assert.Equal(expectedSettings.RememberSettings, msg.RememberSettings);
        }

        [Fact] public void Subsequent_load_requests_do_not_hit_database()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var dbFake = _container.Resolve<IYakkaDb>();
            var expectedSettings = _fixture.Create<YakkaSettings>();
            A.CallTo(() => dbFake.LoadSettings())
                .Returns(expectedSettings);

            actor.Tell(new SettingsActor.LoadSettingsRequest(respondTo: TestActor));
            actor.Tell(new SettingsActor.LoadSettingsRequest(respondTo: TestActor));
            var msg = ExpectMsg<ImmutableYakkaSettings>();

            A.CallTo(() => dbFake.LoadSettings())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Loading_after_saving_does_not_hit_database()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var dbFake = _container.Resolve<IYakkaDb>();
            var settingsToSave = _fixture.Create<YakkaSettings>().AsImmutable();

            actor.Tell(new SettingsActor.SaveSettingsRequest(settingsToSave, Sys.DeadLetters));
            ExpectNoMsg();
            actor.Tell(new SettingsActor.LoadSettingsRequest(TestActor));
            var msg = ExpectMsg<ImmutableYakkaSettings>();

            A.CallTo(() => dbFake.LoadSettings())
                .MustNotHaveHappened();

            Assert.Same(settingsToSave, msg);
        }

        [Fact]
        public void Requesting_current_settings_responds_to_sender_with_last_known_settings()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var settingsToSave = _fixture.Create<YakkaSettings>().AsImmutable();
            var db = _container.Resolve<IYakkaDb>();

            actor.Tell(new SettingsActor.SaveSettingsRequest(settingsToSave, Sys.DeadLetters));
            ExpectNoMsg();

            actor.Tell(new SettingsActor.RequestCurrentSettingsRequest());
            var msg = ExpectMsg<SettingsActor.RequestCurrentSettingsResponse>();

            Assert.Same(settingsToSave, msg.Settings);
            A.CallTo(() => db.LoadSettings())
                .MustNotHaveHappened();
        }

        [Fact]
        public void Requesting_current_settings_loads_from_database_if_no_settings_known()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            var settingsToLoad = _fixture.Create<YakkaSettings>();
            var db = _container.Resolve<IYakkaDb>();
            A.CallTo(() => db.LoadSettings())
                .Returns(settingsToLoad);

            actor.Tell(new SettingsActor.RequestCurrentSettingsRequest());
            var msg = ExpectMsg<SettingsActor.RequestCurrentSettingsResponse>();

            A.CallTo(() => db.LoadSettings())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Some_fault_tolerance_behaviours()
        {
            Assert.True(false, "Todo");
        }
    }
}
