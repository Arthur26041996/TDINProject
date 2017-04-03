using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatWindow : Form
    {
        User self;

        public ChatWindow(User self, int port)
        {
            this.self = self;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(this.CloseWindow);
        }

        public Boolean ChatStarted(string nick)
        {
            return (tabControl1.TabPages.ContainsKey(nick));
        }

        public void NewTab(User self, User pair, IPEndPoint pairEndPoint)
        {
            this.tabControl1.TabPages.Add(new ChatTab(self, pair, pairEndPoint));
        }

        public void NewMessage(User pair, String message)
        {
            try
            {
                String formatted = String.Format("{0,20} | " + message, pair.Name);
                ((ChatTab)this.tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(pair.Nick)]).NewMessage(formatted);
            }
            catch { }
        }

        public void UpdateUsers(Dictionary<User, IPEndPoint> onlineUsers)
        {
            foreach (User user in onlineUsers.Keys)
            {
                if (self.Equals(user))
                {
                    self = user;
                }
            }
            foreach (User user in onlineUsers.Keys)
            {
                if (this.tabControl1.TabPages.ContainsKey(user.Nick))
                {
                    ((ChatTab)this.tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(user.Nick)]).UpdatePair(user, onlineUsers[user]);
                    ((ChatTab)this.tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(user.Nick)]).UpdateSelf(this.self);
                }
            }
            for(int i=0; i< this.tabControl1.TabCount; i++)
            {
                if (!onlineUsers.ContainsKey(((ChatTab)this.tabControl1.TabPages[i]).GetPair()))
                {
                    ((ChatTab)this.tabControl1.TabPages[i]).Disconnect();
                }
            }
            this.Refresh();
        }

        private void CloseWindow(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }

    public class ChatTab : TabPage
    {
        User self, pair;
        IPEndPoint pairEndPoint;
        IMessenger iMessenger;

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;

        public ChatTab(User self, User dest, IPEndPoint destEndPoint)
        {
            this.self = self;
            this.pair = dest;
            this.pairEndPoint = destEndPoint;
            iMessenger = (IMessenger)RemotingServices.Connect(typeof(IMessenger), Util.URL(pairEndPoint));

            #region tab layout

            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            this.Controls.Add(this.splitContainer1);
            this.Location = new System.Drawing.Point(4, 22);
            this.Name = dest.Nick;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(899, 563);
            this.Text = dest.Name;
            this.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox2);
            this.splitContainer1.Size = new System.Drawing.Size(893, 557);
            this.splitContainer1.SplitterDistance = 481;
            this.splitContainer1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(893, 481);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.Multiline = true;
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Font = new Font(FontFamily.GenericMonospace, this.richTextBox1.Font.Size);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox2.Location = new System.Drawing.Point(0, 0);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(893, 72);
            this.richTextBox2.TabIndex = 0;
            this.richTextBox2.Text = "";


            this.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);

            #endregion

            this.richTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SendMessage);
        }

        private void SendMessage(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    if(pairEndPoint == null)
                    {
                        MessageBox.Show(pair.Name + " is disconnected.");
                    }
                    else if (richTextBox2.Text == "" || richTextBox2.Text == "\n")
                    {
                    }
                    else
                    {
                        String formatted = String.Format("{0,20} | " + richTextBox2.Text, "[" + self.Name + "]");
                        iMessenger.ProcessMessage(self, richTextBox2.Text);
                        richTextBox1.AppendText(formatted);
                    }
                    richTextBox2.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -- " + ex.StackTrace);
            }
        }

        public void NewMessage(String message)
        {
            richTextBox1.AppendText(message);
        }

        public void UpdatePair(User pair, IPEndPoint pairEndPoint)
        {
            this.pair = pair;
            this.pairEndPoint = pairEndPoint;
            this.Text = pair.Name;
            iMessenger = (IMessenger)RemotingServices.Connect(typeof(IMessenger), Util.URL(pairEndPoint));
        }

        public void UpdateSelf(User self)
        {
            this.self = self;
        }

        public User GetPair()
        {
            return pair;
        }
        public void Disconnect()
        {
            pairEndPoint = null;
            iMessenger = null;
        }
    }
}
