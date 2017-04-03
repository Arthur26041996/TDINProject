using Shared;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Net;
using System.IO;

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
            ////testing
            //users.Add(new User("q", "q", "q"));
            //users.Add(new User("w", "w", "w"));
            //users.Add(new User("e", "e", "e"));
            LoadUsers();
        }

        public override object InitializeLifetimeService()
        {
            Console.WriteLine("[Broker]: InitilizeLifetimeService");
            return null;
        }

        private void LoadUsers()
        {
            if (File.Exists("users.txt"))
            {
                string[] lines = System.IO.File.ReadAllLines(@"users.txt");
                foreach (String line in lines)
                {
                    if (line != "" && line != "\n")
                    {
                        String[] tokens = line.Split(',');
                        users.Add(new User(tokens[0], tokens[1], tokens[2]));
                    }
                }
            }
        }

        public User Login(string nick, string pass, IPEndPoint endpoint)
        {
            try
            {
                foreach (User user in users)
                {
                    if (user.Nick.Equals(nick) && user.Pass.Equals(pass))
                    {
                        if (onlineUsers.ContainsKey(user))
                        {
                            Logout(user.Nick);
                        }
                        onlineUsers.Add(user, endpoint);
                        UpdateOnlineUsers?.Invoke(onlineUsers);
                        return user;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
                return null;
            }
        }

        public void Logout(string nick)
        {
            try
            {
                foreach (User user in onlineUsers.Keys)
                {
                    if (user.Nick.Equals(nick))
                    {
                        onlineUsers.Remove(user);
                        UpdateOnlineUsers?.Invoke(GetOnlineUsers());
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
            }
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
            System.IO.File.AppendAllText("users.txt", name + ',' + nick + ',' + pass + '\n');
            return true;
        }

        public Dictionary<User, IPEndPoint> GetOnlineUsers()
        {
            try
            {
                Dictionary<User, IPEndPoint> list = new Dictionary<User, IPEndPoint>();
                foreach (KeyValuePair<User, IPEndPoint> user in onlineUsers)
                {
                    list.Add(new User(user.Key.Name, user.Key.Nick, "", user.Key.Status), user.Value);
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }

        public void UpdateUserDetails(string name, string nick, string pass, Status status, IPEndPoint endpoint)
        {
            try
            {
                File.Delete("users.txt");
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

                        if (UpdateOnlineUsers != null)
                        {
                            UpdateOnlineUsers(GetOnlineUsers());
                        }
                    }
                    System.IO.File.AppendAllText("users.txt", user.Name + ',' + user.Nick + ',' + user.Pass + '\n');
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
            }
        }
    }
}
