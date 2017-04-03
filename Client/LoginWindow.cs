using Shared;
using System;
using System.Net;
using System.Windows.Forms;

namespace Client
{
    public partial class LoginWindow : Form
    {
        IBroker broker;
        BrokerIntermediate brokerIntermediate;
        User user;
        int port;

        public LoginWindow(int port)
        {
            brokerIntermediate = new BrokerIntermediate();
            broker = (IBroker)GetRemote.New(typeof(IBroker));
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(this.CloseWindow);
            this.port = port;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Invalid Nickname", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Invalid Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if ((user = broker.Login(textBox1.Text, textBox2.Text, new IPEndPoint(Util.LocalIPAddress(), port))) != null)
                {
                    this.Hide();
                    (new MainWindow(user, port)).Show();
                }
                else
                {
                    MessageBox.Show("Invalid nickname or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                //broker.Logout(textBox1.Text);
                user = null;
                Console.WriteLine(ex.Message + " - "+ex.StackTrace);
                MessageBox.Show("Error contacting the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            (new SignUpWindow(port)).Show();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
