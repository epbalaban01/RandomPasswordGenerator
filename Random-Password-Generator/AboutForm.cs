using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Random_Password_Generator
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

    

        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            lblProgramName.Text = fvi.ProductName;
            lblVersion.Text = fvi.ProductVersion;

            lblCopyright.Text = fvi.LegalCopyright;

          

        }

        private void projectLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

       

       

        private void VisitLink()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            projectLinkLabel.LinkVisited = true;
            //Call the Process.Start method to open the default browser
            //with a URL:
            System.Diagnostics.Process.Start("https://www.eyupcanbalaban.com");
        }

        private void licenseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string licensePath = Path.Combine(Application.StartupPath, "License.html");

            if (File.Exists(licensePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = licensePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("License.html dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

      
}
