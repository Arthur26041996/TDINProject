using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    class Client
    {
        [STAThread]
        static void Main(string[] args)
        {
            IDictionary props = new Hashtable();
            props["port"] = 0;                                                        // let the system choose a free port
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            TcpChannel chan = new TcpChannel(props, clientProvider, serverProvider);  // instantiate the channel
            ChannelServices.RegisterChannel(chan, false);                             // register the channel

            ChannelDataStore data = (ChannelDataStore)chan.ChannelData;
            int port = new Uri(data.ChannelUris[0]).Port;                            // get the port

            RemotingConfiguration.Configure("Client.exe.config", false);             // register the server objects
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Messenger), "Messenger", WellKnownObjectMode.Singleton);  // register my remote object for service
            // Convo convo = (Convo)RemotingServices.Connect(typeof(Convo), "tcp://localhost:"+port+"/Message");
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginWindow(port));
        }

    }

    public delegate Boolean ReceivedRequestHandler(User pair);
    public delegate void ReceivedMessageHandler(User from, String message);

    public interface IMessenger
    {
        void ProcessMessage(User user, String message);
        Boolean ProcessRequest(User pair);
    }

    public class Messenger : MarshalByRefObject, IMessenger
    {
        public event ReceivedMessageHandler ReceivedMessage;
        public event ReceivedRequestHandler ReceivedRequest;

        public override object InitializeLifetimeService()
        {
            Console.WriteLine("[Broker]: InitilizeLifetimeService");
            return null;
        }

        public Boolean ProcessRequest(User pair)
        {
            return ReceivedRequest(pair);
        }

        public void ProcessMessage(User user, String message)
        {
            ReceivedMessage(user, message);
        }
    }
    
}
