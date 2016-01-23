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
            var settingsToSave = new YakkaSettings()
            {
                ServerAddress = _fixture.Create<string>(),
                ServerPort = _fixture.Create<int>(),
                Username = _fixture.Create<string>(),
                ConnectAutomatically = _fixture.Create<bool>(),
                RememberSettings = _fixture.Create<bool>(),
                LaunchOnStartup = _fixture.Create<bool>()
            }.AsImmutable();

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
            var settingsToSave = new YakkaSettings()
            {
                ServerAddress = _fixture.Create<string>(),
                ServerPort = _fixture.Create<int>(),
                Username = _fixture.Create<string>(),
                ConnectAutomatically = _fixture.Create<bool>(),
                RememberSettings = _fixture.Create<bool>(),
                LaunchOnStartup = _fixture.Create<bool>()
            }.AsImmutable();

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
        public void SubsequentSaveRequestsDoNotHitDB()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void FirstLoadSettingsRequestLoadsFromDB()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void SubsequentLoadRequestsDoNotHitDB()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void LoadingAfterSavingDoesNotHitDB()
        {
            Assert.True(false, "Todo");
        }
    }
}
