using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SS.NetworkController;

namespace SS
{
    public partial class LoginWindow : Form
    {
        Thread workerThread;
        Controller server;
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

                CancelButton.Enabled = true;
                textBoxUserName.Enabled = false;
                textBoxHostName.Enabled = false;
                buttonConnect.Text = "Connecting...";
                buttonConnect.Enabled = false;
                workerThread = new Thread(() =>
                {
                    Stopwatch t = new Stopwatch();
                    t.Start();
                    server = new Controller(user, hostname);
                    while (!server.ssReady)
                    {
                        if (t.Elapsed.Seconds > 30)
                        {
                            //invoker
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                MessageBox.Show(this, "Unable to connect to server, please check for valid server and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //this.Enabled = true;
                                textBoxUserName.Enabled = true;
                                textBoxHostName.Enabled = true;
                                buttonConnect.Text = "Connect";
                                buttonConnect.Enabled = true;
                                return;
                            });
                        }
                    }
                    //invoker
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        buttonConnect.Text = "Connected";
                        buttonNew.Show();
                        buttonOpen.Show();
                        CancelButton.Enabled = false;
                    });

                });
                workerThread.Start();
            }
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            SSNewWindow openWindow = new SSNewWindow(server, "new", -1);
            openWindow.ShowDialog();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            SSOpenWindow openWindow = new SSOpenWindow(server);
            openWindow.ShowDialog();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            workerThread.Abort();
            textBoxUserName.Enabled = true;
            textBoxHostName.Enabled = true;
            buttonConnect.Text = "Connect";
            buttonConnect.Enabled = true;
            CancelButton.Enabled = false;
        }
    }
}
