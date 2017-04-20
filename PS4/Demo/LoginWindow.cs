using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SS.NetworkController;

namespace SS
{
    public partial class LoginWindow : Form
    {
        public LoginWindow()
        {
            InitializeComponent();
            buttonOpen.Hide();
            buttonNew.Hide();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            String user = textBoxUserName.Text;
            String hostname = textBoxHostName.Text;

            if (user == "" || hostname == "")
            {
                
                MessageBox.Show(this, "Not all fields are properly filled out", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Stopwatch t = new Stopwatch();
                t.Start();
                textBoxUserName.Enabled = false;
                textBoxHostName.Enabled = false;
                buttonConnect.Text = "Connecting...";
                buttonConnect.Enabled = false;
                Controller server = new Controller(user, hostname);
                while (!server.ssReady)
                {
                    if (t.Elapsed.Seconds > 5)
                    {
                        MessageBox.Show(this, "Unable to connect to server, please check for valid server and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Enabled = true;
                        textBoxUserName.Enabled = true;
                        textBoxHostName.Enabled = true;
                        return;
                    }
                }
                buttonConnect.Text = "Connected";
                buttonNew.Show();
                buttonOpen.Show();
            }

            //if program closes after spreadsheet window closes, make sure that works when we have multiple SS open
        }
    }
}
