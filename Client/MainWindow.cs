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
        User user;
        Dictionary<User, IPEndPoint> onlineUsers;
        ChatWindow chatWindow;

        public MainWindow(User user)
        {
            brokerIntermediate = new BrokerIntermediate();
            brokerIntermediate.UpdateOnlineUsers += OnUpdateOnlineUsers;
            broker = (IBroker)GetRemote.New(typeof(IBroker));
            broker.UpdateOnlineUsers += brokerIntermediate.FireUpdateOnlineUsers;
            this.user = user;
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
            chatWindow = new ChatWindow(user, onlineUsers);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void RequestConvo(object sender, EventArgs e)
        {

            chatWindow.Show();
            /*
            try
            {
                String nickDestination = onlineUsers[this.dataGridView1.CurrentCell.RowIndex].Nick;
                if (nickDestination != null && nickDestination != "" && nickDestination != user.Nick)
                {
                    chatWindow.Show();
                    //sbroker.RequestConvo(user.Nick, nickDestination, chatWindow.PrepareNewConvo(user.Endpoint));
                }
            }
            catch { }
            */
        }

        private void Logout(object sender, EventArgs e)
        {
            //chatWindow.Disconnect();
            broker.UpdateOnlineUsers -= brokerIntermediate.FireUpdateOnlineUsers;
            broker.Logout(user.Nick);
            Application.Exit();
        }

        private void ChangeStatus(object sender, EventArgs e)
        {
            broker.UpdateUserDetails(null, user.Nick, null, (Status)Enum.Parse(typeof(Status), this.comboBox1.SelectedItem.ToString()), null);
        }

        private void ChangeName(object sender, EventArgs e)
        {
            broker.UpdateUserDetails(this.textBox1.Text, user.Nick, null, user.Status, null);
        }

        private void UpdateUserList()
        {
            this.dataGridView1.Rows.Clear();
            if (onlineUsers.Count > 1)
            {
                this.dataGridView1.Rows.Add(onlineUsers.Count - 1);
            }
            for (int i = 0; i < onlineUsers.Count; i++)
            {
                if (onlineUsers.ElementAt(i).Key.Nick != user.Nick)
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

        public void OnUpdateOnlineUsers(Dictionary<User, IPEndPoint> onlineUsers)
        {
            this.onlineUsers = onlineUsers;
            this.chatWindow.OnlineUsers = onlineUsers;
            this.chatWindow.CurrentRecipient = onlineUsers.ElementAt(0).Key;
            UpdateUserList();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            //chatWindow.Disconnect();
            broker.UpdateOnlineUsers -= brokerIntermediate.FireUpdateOnlineUsers;
            broker.Logout(user.Nick);
            Application.Exit();
        }
    }
}