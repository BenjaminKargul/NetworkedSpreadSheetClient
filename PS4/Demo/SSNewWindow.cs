using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{

    public partial class SSNewWindow : Form
    {
        string text;
        Controller server;
        public SSNewWindow(Controller theServer)
        {
            InitializeComponent();
            this.server = theServer;
            server.SendCommand("0\n");
            text = NameBox.Text;
        }

        private void CreateNewButton_Click(object sender, EventArgs e)
        {
            server.SendCommand("1\t" + text + "\n");
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            text = NameBox.Text;
        }
    }
}
