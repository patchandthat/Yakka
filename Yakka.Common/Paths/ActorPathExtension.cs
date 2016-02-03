using Akka.Actor;

namespace Yakka.Common
{
    public static class ActorPathExtension
    {
        public static ActorPath Sibling(this ActorPath actor, string siblingName)
        {
            return actor.Parent.Child(siblingName);
        }
    }
}
