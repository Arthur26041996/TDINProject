using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;

namespace Shared
{
    public enum Status { Online, Busy, Brb, Away, Call, Lunch };


    public delegate void UpdateOnlineUsersHandler(Dictionary<User, IPEndPoint> onlineUsers);

    public interface IBroker
    {
        event UpdateOnlineUsersHandler UpdateOnlineUsers;

        User Login(String nick, String pass, IPEndPoint endpoint);
        Boolean SignUp(String name, String nick, String pass);
        void Logout(String nick);
        Dictionary<User, IPEndPoint> GetOnlineUsers();
        void UpdateUserDetails(String name, String nick, String pass, Status status, IPEndPoint endpoint);
    }

    public class BrokerIntermediate : MarshalByRefObject
    {

        public event UpdateOnlineUsersHandler UpdateOnlineUsers;
        public void FireUpdateOnlineUsers(Dictionary<User, IPEndPoint> onlineUsers)
        {
            UpdateOnlineUsers(onlineUsers);
        }
    }

    public class GetRemote
    {
        private static IDictionary wellKnownTypes;

        public static object New(Type type)
        {
            if (wellKnownTypes == null)
                InitTypeCache();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)wellKnownTypes[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return Activator.GetObject(type, entry.ObjectUrl);
        }

        public static void InitTypeCache()
        {
            Hashtable types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
            {
                if (entry.ObjectType == null)
                    throw new RemotingException("A configured type could not be found!");
                types.Add(entry.ObjectType, entry);
            }
            wellKnownTypes = types;
        }
    }

    public class Util
    {
        public static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }


        public static string URL(IPEndPoint endPoint)
        {
            return "tcp://" + endPoint.Address + ":" + endPoint.Port + "/Messenger";
        }
    }

}
