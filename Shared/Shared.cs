using System;
using System.Collections.Generic;
using System.Net;

namespace Shared
{
    public enum Status { Online, Busy, Brb, Away, Call, Lunch };


    public delegate void UpdateOnlineUsersHandler(Dictionary<User, IPEndPoint> onlineUsers);
   // public delegate void ConvoRequestHandler(String nickSource, String nickDest, IPEndPoint endPoint);
   // public delegate void ConvoReplyHandler(String nickSource, String nickDest, Boolean answer);

    public interface IBroker
    {
        event UpdateOnlineUsersHandler UpdateOnlineUsers;
       // event ConvoRequestHandler ConvoRequest;
       // event ConvoReplyHandler ConvoReply;

        User Login(String nick, String pass, IPEndPoint endpoint);
        Boolean SignUp(String name, String nick, String pass);
        void Logout(String nick);
        Dictionary<User, IPEndPoint> GetOnlineUsers();
        void UpdateUserDetails(String name, String nick, String pass, Status status, IPEndPoint endpoint);
       // void RequestConvo(String nickSource, String nickDest, IPEndPoint endPoint);
       // void AnswerRequest(String nickSource, String nickDest, Boolean answer);
    }

    public class BrokerIntermediate : MarshalByRefObject
    {

        public event UpdateOnlineUsersHandler UpdateOnlineUsers;
        public void FireUpdateOnlineUsers(Dictionary<User, IPEndPoint> onlineUsers)
        {
            UpdateOnlineUsers(onlineUsers);
        }
        /*
        public event ConvoRequestHandler ConvoRequest;
        public void FireConvoRequest(String nickSource, String nickDest, IPEndPoint endPoint)
        {
            ConvoRequest(nickSource, nickDest, endPoint);
        }

        public event ConvoReplyHandler ConvoReply;
        public void FireConvoReply(String nickSource, String nickDest, Boolean answer)
        {
            ConvoReply(nickSource, nickDest, answer);
        }
        */
    }

    public interface IConvo
    {
        void ProcessMessage(User user, String message);
    }

    public class Convo : MarshalByRefObject, IConvo
    {
        public void ProcessMessage(User user, String message)
        {
            Console.WriteLine(message);
        }
    }

}
