using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class SignUpWindow : Form
    {

        IBroker broker;
        BrokerIntermediate brokerIntermediate;
        int port;

        public SignUpWindow(int port)
        {
            brokerIntermediate = new BrokerIntermediate();
            broker = (IBroker)GetRemote.New(typeof(IBroker));
            InitializeComponent();
            this.port = port;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            (new LoginWindow(port)).Show();
            this.Dispose();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Invalid Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Invalid Nickname", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Invalid Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (broker.SignUp(textBox1.Text, textBox2.Text, textBox3.Text))
                    {
                        //this.Hide();
                        MessageBox.Show("SignUp successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        (new LoginWindow(port)).Show();
                        this.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("Nickname invalid or already in use.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Could not contact the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
