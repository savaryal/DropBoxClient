///ETML
///Auteur : Alison Savary
///Date   : 11.05.2017
///Description : Client Dropbox en C#
///   
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropBoxClient
{
    public partial class DropBoxClientForm : Form
    {
        Bitmap bitmapHomeIcon = DropBoxClient.Properties.Resources.homeIcon;
        Bitmap bitmapParametersIcon = DropBoxClient.Properties.Resources.parametersIcon;

        public DropBoxClientForm()
        {
            InitializeComponent();
        }


        private void parametersHomePictureBox_Click(object sender, EventArgs e)
        {
            if(parametersHomePictureBox.Image == bitmapParametersIcon)
            {
                parametersHomePictureBox.Image = bitmapHomeIcon;
                homePanel.Visible = false;
                parametersPanel.Visible = true;
            }
            else
            {
                parametersHomePictureBox.Image = bitmapParametersIcon;
                parametersPanel.Visible = false;
                homePanel.Visible = true;
            }
        }
    }
}
