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
        public SSOpenWindow(Controller Server)
        {
            InitializeComponent();
            this.server = Server;
            server.SendCommand("0\n");
            server.fileListRecieved += displayFileList;
            listBoxSpreadsheets.DataSource = files;
        }

        public void displayFileList(List<String> files)
        {
            this.files = files;
            this.Invoke((MethodInvoker)delegate () { foreach (string item in files) { listBoxSpreadsheets.Items.Add(item); } });
        }

        private void listBoxSpreadsheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectionBox.Text = listBoxSpreadsheets.GetItemText(listBoxSpreadsheets.SelectedItem);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //tbd
            server.SendCommand("2\t" + listBoxSpreadsheets.GetItemText(listBoxSpreadsheets.SelectedItem) + "\n");
        }

        private void SSOpenWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
<<<<<<< HEAD
            server.fileListRecieved -= displayFileList;
            server.CloseAllOpenForms -= Close;
=======
           server.fileListRecieved -= displayFileList;
            
>>>>>>> 16460ac2147b54bcf7dbebf981635be27005a2ac
        }
    }
}
