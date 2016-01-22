using Akka.Actor;
using Xunit;
using Akka.TestKit.Xunit2;
using Yakka.Actors;

namespace Yakka.Tests
{
    public class SettingsActorTests : TestKit
    {
        [Fact]
        public void Demo()
        {
            var actor = Sys.ActorOf(Props.Create(() => new SettingsActor()));
            actor.Tell("message");

            bool result = ExpectMsg<bool>();

            Assert.True(result);
        }
    }
}
