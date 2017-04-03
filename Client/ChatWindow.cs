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
        User self;
        Messenger messenger;

        public ChatWindow(User self, int port)
        {
            this.self = self;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(this.CloseWindow);
            //own remote object
            messenger = (Messenger)RemotingServices.Connect(typeof(Messenger), "tcp://localhost:" + port + "/Messenger");
            //messenger.ReceivedRequest += OnReceivedRequest;
        }

        public Boolean ChatStarted(string nick)
        {
            return (tabControl1.TabPages.ContainsKey(nick));
        }

        public void NewTab(User self, User pair, IPEndPoint pairEndPoint)
        {
            //this.BeginInvoke((Action)(() =>
            //{
            //    this.tabControl1.TabPages.Add(new ChatTab(self, this.messenger, pair, pairEndPoint));
            //}));
            this.tabControl1.TabPages.Add(new ChatTab(self, this.messenger, pair, pairEndPoint));
        }

        public void NewMessage(String nick, String message)
        {
            try
            {
                ((ChatTab)this.tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(nick)]).NewMessage(message);
            }
            catch { }
        }

        public Boolean OnReceivedRequest(User pair, IPEndPoint pairEndPoint)
        {
            try
            {
                DialogResult result = MessageBox.Show(pair.Name + " wants to start a chat with you. Accept?", "New Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    this.tabControl1.TabPages.Add(new ChatTab(self, this.messenger, pair, pairEndPoint));
                    //this.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + ex.StackTrace, "Error Request");
                return false;
            }
        }

        //public void OnReceivedMessage(User pair, String message)
        //{
        //    try
        //    {
        //        int index;
        //        if (this.InvokeRequired)
        //        {
        //            this.BeginInvoke((MethodInvoker)delegate ()
        //            {
        //                index = tabControl1.TabPages.IndexOfKey(pair.Nick);
        //                if (!this.Visible)
        //                {
        //                    tabControl1.SelectTab(index);
        //                    this.Show();
        //                }
        //        ((ChatTab)tabControl1.TabPages[index]).AddMessage("\n\n" + pair.Name + " : " + message);
        //            });
        //        }
        //        else
        //        {
        //            index = tabControl1.TabPages.IndexOfKey(pair.Nick);
        //            if (!this.Visible)
        //            {
        //                tabControl1.SelectTab(index);
        //                this.Show();
        //            }
        //        ((ChatTab)tabControl1.TabPages[index]).AddMessage("\n\n" + pair.Name + " : " + message);
        //        }
        //        //int index = tabControl1.TabPages.IndexOfKey(pair.Nick);
        //        //if (!this.Visible)
        //        //{
        //        //    tabControl1.SelectTab(index);
        //        //    this.Show();
        //        //}
        //        //((ChatTab)tabControl1.TabPages[index]).AddMessage("\n\n" + pair.Name + " : " + message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + " - " + ex.StackTrace, "Error Message "+self.Name);
        //    }
        //}

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
        Messenger messenger;

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        
        public ChatTab(User self, Messenger messenger, User dest, IPEndPoint destEndPoint)
        {
            this.self = self;
            this.messenger = messenger;
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
            //messenger.ReceivedMessage += OnReceivedMessage;
        }

        private void SendMessage(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return && richTextBox2.Text != "")
                {
                    iMessenger.ProcessMessage(self, richTextBox2.Text);
                    richTextBox1.AppendText("\n me :" + richTextBox2.Text);
                    richTextBox2.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + ex.StackTrace, "Error Send Message");
            }
        }

        public void NewMessage(String message)
        {
            richTextBox1.AppendText(message);
        }

        //public void OnReceivedMessage(User pair, String message)
        //{
        //    if (this.pair.Equals(pair))
        //    {
        //        richTextBox1.AppendText(message);
        //    }
        //}
    }
}
