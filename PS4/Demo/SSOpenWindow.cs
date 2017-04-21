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
            server.handleRecieveFileList("0\tfile1.sprd\tfile2.sprd\tfile3.sprd\tfile4.sprd\tfile5.sprd\n");
        }

        public void displayFileList(List<String> files)
        {
            foreach(string fileName in files)
            {
                listBoxSpreadsheets.Items.Add(fileName);
            }
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
    }
}
