using Shared;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Net;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("Server.exe.config", false);
            Console.WriteLine("Press return to exit");
            Console.ReadLine();
        }
    }

    public class Broker : MarshalByRefObject, IBroker
    {
        List<User> users;
        Dictionary<User, IPEndPoint> onlineUsers;

        public event UpdateOnlineUsersHandler UpdateOnlineUsers;

        public Broker()
        {
            onlineUsers = new Dictionary<User, IPEndPoint>();
            users = new List<User>();
            //testing
            users.Add(new User("q", "q", "q"));
            users.Add(new User("w", "w", "w"));
            users.Add(new User("e", "e", "e"));
        }

        public override object InitializeLifetimeService()
        {
            Console.WriteLine("[Broker]: InitilizeLifetimeService");
            return null;
        }

        public User Login(string nick, string pass, IPEndPoint endpoint)
        {
            foreach (User user in users)
            {
                if (user.Nick.Equals(nick) && user.Pass.Equals(pass) && !onlineUsers.ContainsKey(user))
                {
                    user.Endpoint = endpoint;
                    onlineUsers.Add(user, endpoint);
                    UpdateOnlineUsers?.Invoke(onlineUsers);
                    return user;
                }
            }
            return null;
        }

        public void Logout(string nick)
        {
            foreach (KeyValuePair<User, IPEndPoint> user in onlineUsers)
            {
                if (user.Key.Nick.Equals(nick))
                {
                    onlineUsers.Remove(user.Key);
                    UpdateOnlineUsers?.Invoke(GetOnlineUsers());
                    return;
                }
            }
            return;
        }

        public Boolean SignUp(string name, string nick, string pass)
        {
            foreach (User user in users)
            {
                if (user.Nick.Equals(nick))
                {
                    return false;
                }
            }
            users.Add(new User(name, nick, pass));
            return true;
        }

        public Dictionary<User, IPEndPoint> GetOnlineUsers()
        {
            Dictionary<User, IPEndPoint> list = new Dictionary<User, IPEndPoint>();
            foreach (KeyValuePair<User, IPEndPoint> user in onlineUsers)
            {
                list.Add(new User(user.Key.Name, user.Key.Nick, null, user.Key.Status), user.Value);
            }
            return list;
        }

        public void UpdateUserDetails(string name, string nick, string pass, Status status, IPEndPoint endpoint)
        {
            foreach (User user in users)
            {
                if (user.Nick.Equals(nick))
                {
                    if (name != null)
                    {
                        user.Name = name;
                    }
                    if (pass != null)
                    {
                        user.Pass = pass;
                    }

                    user.Status = status;

                    if (endpoint != null)
                    {
                        user.Endpoint = endpoint;
                    }

                    if (UpdateOnlineUsers != null)
                    {
                        UpdateOnlineUsers(GetOnlineUsers());
                    }
                    return;
                }
            }
        }
    }
}
