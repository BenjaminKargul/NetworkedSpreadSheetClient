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
    public partial class InvalidAction : Form
    {
        
        public InvalidAction(string type)
        {
            InitializeComponent();
            if (type == "edit")
            {
                ErrorMessage.Text = "Invalid Edit sent to server, you likely sent a formula that caused a circular exception";
            }
            else
            {
                ErrorMessage.Text = "Invalid Rename sent to server, the name you sent most likely already exists";
            }
        }
    }
}
