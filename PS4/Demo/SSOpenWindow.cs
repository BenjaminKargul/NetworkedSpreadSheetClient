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
        public SSOpenWindow(Controller server)
        {
            InitializeComponent();

            this.server = server;
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
    }
}
