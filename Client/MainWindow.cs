using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace Client
{
    public partial class MainWindow : Form
    {
        IBroker broker;
        BrokerIntermediate brokerIntermediate;
        Messenger messenger;
        User self;
        int port;
        Dictionary<User, IPEndPoint> onlineUsers;
        ChatWindow chatWindow;

        public MainWindow(User user, int port)
        {
            //intermediate to handle notifications of updates
            brokerIntermediate = new BrokerIntermediate();
            brokerIntermediate.UpdateOnlineUsers += OnUpdateOnlineUsers;
            //remote object
            broker = (IBroker)GetRemote.New(typeof(IBroker));
            broker.UpdateOnlineUsers += brokerIntermediate.FireUpdateOnlineUsers;
            this.self = user;
            this.port = port;
            InitializeComponent();
            this.textBox1.Text = user.Name;
            this.comboBox1.SelectedIndex = this.comboBox1.Items.IndexOf(user.Status);
            onlineUsers = broker.GetOnlineUsers();
            UpdateUserList();

            this.button1.Click += new System.EventHandler(this.RequestConvo);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ChangeStatus);
            this.button3.Click += new System.EventHandler(this.Logout);
            this.textBox1.TextChanged += new System.EventHandler(this.ChangeName);
            this.FormClosing += new FormClosingEventHandler(this.CloseWindow);
            chatWindow = new ChatWindow(self, port);
            messenger = (Messenger)RemotingServices.Connect(typeof(Messenger), "tcp://localhost:" + port + "/Messenger");
            messenger.ReceivedRequest += OnReceivedRequest;
            messenger.ReceivedMessage += OnReceivedMessage;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void RequestConvo(object sender, EventArgs e)
        {
            try
            {
                KeyValuePair<User, IPEndPoint> keyVal = onlineUsers.ElementAt(this.dataGridView1.CurrentCell.RowIndex);
                if (keyVal.Key.Nick != null && keyVal.Key.Nick != "" && keyVal.Key.Nick != self.Nick)
                {
                    if (chatWindow.ChatStarted(keyVal.Key.Nick))
                    {
                        chatWindow.Show();
                    }
                    else
                    {
                        IMessenger iMessenger = (IMessenger)RemotingServices.Connect(typeof(IMessenger), Util.URL(keyVal.Value));
                        if (iMessenger.ProcessRequest(self))
                        {
                            chatWindow.NewTab(self, keyVal.Key, keyVal.Value);
                            chatWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show(keyVal.Key.Nick + " refused your request", "Refused", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
            }
        }

        private Boolean OnReceivedRequest(User pair)
        {
            try
            {
                DialogResult result = MessageBox.Show(pair.Name + " wants to start a chat with you. Accept?", "New Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    IPEndPoint pairEndPoint = onlineUsers[pair];
                    this.BeginInvoke((Action)(() =>
                    {
                        chatWindow.NewTab(self, pair, pairEndPoint);
                        chatWindow.Show();
                    }));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
                return false;
            }
        }

        public void OnReceivedMessage(User pair, String message)
        {
                this.BeginInvoke((Action)(() =>
                {
                    if (!chatWindow.ChatStarted(pair.Nick))
                    {
                        IPEndPoint pairEndPoint = onlineUsers[pair];
                        chatWindow.NewTab(self, pair, pairEndPoint);
                    }
                        chatWindow.NewMessage(pair, message);
                    chatWindow.Show();
                }));
        }

        private void Logout(object sender, EventArgs e)
        {
            messenger.ReceivedRequest -= OnReceivedRequest;
            messenger.ReceivedMessage -= OnReceivedMessage;
            broker.UpdateOnlineUsers -= brokerIntermediate.FireUpdateOnlineUsers;
            broker.Logout(self.Nick);
            Application.Exit();
        }

        private void ChangeStatus(object sender, EventArgs e)
        {
            try
            {
                broker.UpdateUserDetails(null, self.Nick, null, (Status)Enum.Parse(typeof(Status), this.comboBox1.SelectedItem.ToString()), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
            }
        }

        private void ChangeName(object sender, EventArgs e)
        {
            try
            {
                broker.UpdateUserDetails(this.textBox1.Text, self.Nick, null, self.Status, null);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
            }
        }

        private void UpdateUserList()
        {
            try
            {
               this.dataGridView1.Rows.Clear();
                if (onlineUsers.Count > 1)
                {
                    this.dataGridView1.Rows.Add(onlineUsers.Count - 1);
                }
                for (int i = 0; i < onlineUsers.Count; i++)
                {
                    if (onlineUsers.ElementAt(i).Key.Nick != self.Nick)
                    {
                        this.dataGridView1.Rows[i].Cells[0].Value = onlineUsers.ElementAt(i).Key.Name + " (" + onlineUsers.ElementAt(i).Key.Nick + ")";
                        this.dataGridView1.Rows[i].Cells[1].Value = onlineUsers.ElementAt(i).Key.Status;
                    }
                    else
                    {
                        this.dataGridView1.Rows[i].Cells[0].Value = "[" + onlineUsers.ElementAt(i).Key.Name + " (" + onlineUsers.ElementAt(i).Key.Nick + ")]";
                        this.dataGridView1.Rows[i].Cells[1].Value = onlineUsers.ElementAt(i).Key.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
            }
        }

        public void OnUpdateOnlineUsers(Dictionary<User, IPEndPoint> onlineUsers)
        {
            try
            {
                this.onlineUsers = onlineUsers;
                UpdateUserList();
                this.BeginInvoke((Action)(() =>
                {
                    chatWindow.UpdateUsers(onlineUsers);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
            }
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            messenger.ReceivedRequest -= OnReceivedRequest;
            messenger.ReceivedMessage -= OnReceivedMessage;
            broker.UpdateOnlineUsers -= brokerIntermediate.FireUpdateOnlineUsers;
            broker.Logout(self.Nick);
            Application.Exit();
        }
    }
}