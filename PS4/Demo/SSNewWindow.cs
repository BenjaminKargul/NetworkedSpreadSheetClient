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
        string theType;
        int docId;
        public SSNewWindow(Controller theServer, string type, int id)
        {
            InitializeComponent();
            this.server = theServer;
            server.CloseAllOpenForms += Close;
            if (type == "new")
            {
                CreateNewButton.Text = "Create New";
                server.SendCommand("0\n");
            }
            else
            {
                CreateNewButton.Text = "Rename";
            }
            theType = type;
            docId = id;
            
            text = NameBox.Text;
        }

        private void CreateNewButton_Click(object sender, EventArgs e)
        {
            if (theType == "new")
            {
                server.SendCommand("1\t" + text + "\n");
            }
            else
            {
                server.SendCommand("7\t"+docId+"\t" + text + "\n");
            }
            
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            text = NameBox.Text;
        }

        private void SSNewWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.CloseAllOpenForms -= Close;
        }
    }
}
