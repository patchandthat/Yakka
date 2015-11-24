//using System;
//using Akka.Actor;

//namespace Yakka.Server.Actors
//{
//    //Probably need both server and client versions of these actors

//    class ConversationCoordinatorActor : ReceiveActor
//    {
//        //Todo: Root level actor responsible for instantiating conversations and delivering messages to the appropriate conversations
//    }

//    class ConversationActor : ReceiveActor
//    {
//        //Todo: A conversation between 2 or more connected users

//        //Messages will be passed in from the coordinator, which will be delivered to all participants of the conversation

//        //Should handle people joining and leaving the conversation
//        //Should probably also have a Guid identifier
//    }
//}