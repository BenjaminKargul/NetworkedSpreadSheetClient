using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        String user;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            user = textBoxUserName.Text;
            String hostname = textBoxHostName.Text;

            Controller server = new Controller(user, hostname);
            //SocketState state = NetworkController.Networking.ConnectToServer(ConnectionEstablished, hostname);

            //if program closes after spreadsheet window closes, make sure that works when we have multiple SS open
        }

        //private void ConnectionEstablished(NetworkController.SocketState ss)
        //{
        //    //Send the player name here
        //    Networking.Send(ss.theSocket, user + "\n");
        //}
    }
}
