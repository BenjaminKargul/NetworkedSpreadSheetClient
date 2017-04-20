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
            //server.SendCommand()
            files = new List<string>();
            files.Add("TestFile1.sprd");
            files.Add("TestFile2.sprd");
            files.Add("TestFile3.sprd");
            files.Add("TestFile4.sprd");
           

            displayFileList(files);
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
