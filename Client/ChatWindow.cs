using Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatWindow : Form
    {
        User user;
        Dictionary<Tuple<User, User>, List<String>> conversations;
        Dictionary<User, IPEndPoint> onlineUsers;
        User currentRecipient;
        IConvo iConv;
        Convo conv;

        public Dictionary<User, IPEndPoint> OnlineUsers
        {
            get
            {
                return onlineUsers;
            }

            set
            {
                onlineUsers = value;
            }
        }

        public User CurrentRecipient
        {
            get
            {
                return currentRecipient;
            }

            set
            {
                currentRecipient = value;
            }
        }

        public ChatWindow(User user, Dictionary<User, IPEndPoint> onlineUsers)
        {
            this.user = user;
            this.onlineUsers = onlineUsers;
            conversations = new Dictionary<Tuple<User, User>, List<String>>();
            InitializeComponent();
            this.richTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SendMessage);
            this.FormClosing += new FormClosingEventHandler(this.CloseWindow);
            //conv = (Convo)RemotingServices.Connect(typeof(Convo), "tcp://localhost:" + onlineUsers[this.user].Port + "/Message");
            //selfConv = (IConvo)RemotingServices.Connect(typeof(IConvo), URL(this.onlineUsers[this.user]));
        }

        private void SendMessage(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar);
            if (e.KeyChar == (char)Keys.Return)
            {
                iConv = (IConvo)RemotingServices.Connect(typeof(IConvo), URL(this.onlineUsers[this.currentRecipient]));
                iConv.ProcessMessage(user, richTextBox2.Text);
                richTextBox1.AppendText("\n\n me :" + richTextBox2.Text);
                if (!conversations.ContainsKey(Tuple.Create(this.user, user)))
                {
                    conversations.Add(Tuple.Create(this.user, user), new List<String>());
                }
                conversations[Tuple.Create(this.user, user)].Add("me: " + richTextBox2.Text);
                richTextBox2.Clear();
            }
        }

        public void ProcessMessage(User user, String message)
        {
            richTextBox1.AppendText("\n\n" + user.Name + " : " + message);
            if (!conversations.ContainsKey(Tuple.Create(this.user, user)))
            {
                conversations.Add(Tuple.Create(this.user, user), new List<String>());
            }
            conversations[Tuple.Create(this.user, user)].Add(user.Name + " : " + message);
        }

        private string URL(IPEndPoint endPoint)
        {
            return "tcp://" + endPoint.Address + ":" + endPoint.Port + "/Message";
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
