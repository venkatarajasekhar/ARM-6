using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HMI_MT
{
    public partial class MenuItemNewFielDlg : Form
    {
        public MenuItemNewFielDlg( )
        {
            InitializeComponent();
        }

        private void button1_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.OK;
        }

        private void button2_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}