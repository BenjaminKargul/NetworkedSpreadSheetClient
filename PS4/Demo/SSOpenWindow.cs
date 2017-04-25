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
    public partial class SSOpenWindow : Form
    {
        Controller server;
        List<String> files;
        string startFile ="";
        bool beenInitialized = false;
      
        public SSOpenWindow(Controller Server)
        {
            InitializeComponent();
            this.server = Server;
            server.SendCommand("0\n");
            server.fileListRecieved += displayFileList;
            listBoxSpreadsheets.DataSource = files;
            beenInitialized = true;
        }
        public SSOpenWindow(Controller Server, string sourceFile)
        {
            InitializeComponent();
            this.server = Server;
            server.SendCommand("0\n");
            server.fileListRecieved += displayFileList;
            listBoxSpreadsheets.DataSource = files;
            startFile = sourceFile;
            beenInitialized = true;
        }
       
        public void displayFileList(List<String> files)
        {
            while (!beenInitialized)
            {
                //wait
            }
            this.files = files;
            this.Invoke((MethodInvoker)delegate () {
                foreach (string item in files)
                {
                    if (listBoxSpreadsheets.Items.Contains(item))
                    {

                    }
                    else
                    {
                        listBoxSpreadsheets.Items.Add(item);
                    }
                    
                }
            });
        }

        private void listBoxSpreadsheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectionBox.Text = listBoxSpreadsheets.GetItemText(listBoxSpreadsheets.SelectedItem);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = listBoxSpreadsheets.GetItemText(listBoxSpreadsheets.SelectedItem);
            if (startFile == name )
            {
                Close();
            }
            server.SendCommand("2\t" + name + "\n");
            server.filename = name;
        }

        private void SSOpenWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.fileListRecieved -= displayFileList;
        }
    }
}
