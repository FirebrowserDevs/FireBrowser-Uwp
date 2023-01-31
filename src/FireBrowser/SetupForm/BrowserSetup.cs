using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireBrowser;

namespace FireBrowser.SetupForm
{
    public partial class BrowserSetup : Form
    {
        public BrowserSetup()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void next1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void next2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void BrowserSetup_Load(object sender, EventArgs e)
        {
            cckpeek.Checked = true;
            cckscript.Checked = true;
            cckdefault.Checked = true;
            urlengien.Enabled = false;
            ckcls.Checked = true;
            ppPop.Text = "Classic";
        }

        private void cckdefault_CheckedChanged(object sender, EventArgs e)
        {
            if(cckdefault.Checked == true)
            {
                urlengien.Enabled = false;
                cnfrdf.Checked = true;
                lbcos.Text = "Costum : No";
            }
            else
            {
                urlengien.Enabled = true;
                cnfrdf.Checked = false;
                lbcos.Text = urlengien.Text;
            }
        }

        private void next3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            gpconf.Enabled = false;
            mainconf.Enabled = false;
            uiconf.Enabled = false;
        }

        private void urlengien_TextChange(object sender, EventArgs e)
        {

            if (cckdefault.Checked == true)
            {
                urlengien.Enabled = false;
                cnfrdf.Checked = true;
                lbcos.Text = "Costum : No";
            }
            else
            {
                urlengien.Enabled = true;
                cnfrdf.Checked = false;
                lbcos.Text = urlengien.Text;
            }
        }

        private void profilebox_TextChange(object sender, EventArgs e)
        {
            prfconf.Text = profilebox.Text;
        }

        private void uiprofilename_TextChange(object sender, EventArgs e)
        {
            bruconf.Text = uiprofilename.Text;
        }

        private void ckcls_CheckedChanged(object sender, EventArgs e)
        {
            ckblc.Checked = false;
            ckmod.Checked = false;
            ppPop.Text = "Classic";
        }

        private void ckmod_CheckedChanged(object sender, EventArgs e)
        {
            ckblc.Checked = false;
            ckcls.Checked = false;
            ppPop.Text = "Modern";
        }

        private void ckblc_CheckedChanged(object sender, EventArgs e)
        {
            ckmod.Checked = false;
            ppPop.Text = "Block";
            ckcls.Checked = false;
        }

        private void cckDev_CheckedChanged(object sender, EventArgs e)
        {
            if(cckDev.Checked == true)
            {
                ckdevconf.Checked = true;
            }
            else
            {
                ckdevconf.Checked = false;
            }
        }

        private void cckicons_CheckedChanged(object sender, EventArgs e)
        {
            if (cckicons.Checked == true)
            {
                ckicconf.Checked = true;
            }
            else
            {
                ckicconf.Checked = false;
            }
        }

        private void cckscript_CheckedChanged(object sender, EventArgs e)
        {
            if (cckscript.Checked == true)
            {
                ckscrconf.Checked = true;
            }
            else
            {
                ckscrconf.Checked = false;
            }
        }

        private void ckpeekconf_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void cckpeek_CheckedChanged(object sender, EventArgs e)
        {

            if (cckpeek.Checked == true)
            {
                ckpeekconf.Checked = true;
            }
            else
            {
                ckpeekconf.Checked = false;
            }
        }

        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            txufconf.Text = bunifuTextBox1.Text;
        }

        private void conf_Click(object sender, EventArgs e)
        {
            FireBrowser.Properties.Settings.Default.ForeColor = Color.FromName(txufconf.Text.ToString());
            FireBrowser.Properties.Settings.Default.userProfile = prfconf.Text.ToString();
            FireBrowser.Properties.Settings.Default.PopUp = ppPop.Text.ToString();
            FireBrowser.Properties.Settings.Default.usrnm = bruconf.Text.ToString();
            FireBrowser.Properties.Settings.Default.SetupDone = true;
            setupsbools();
            FireBrowser.Properties.Settings.Default.Save();
            this.Close();
            Process.Start("FireRestartSetup.exe");
            foreach (var process in Process.GetProcessesByName("FireBrowser"))
            {
                process.Kill();         
            }               
        }

       public void setupsbools()
        {
            if (cnfrdf.Checked == true)
            {
                FireBrowser.Properties.Settings.Default.EngineDefault = "https://www.google.com/?f=ssl";
                FireBrowser.Properties.Settings.Default.Engine = true;
            }
            else
            {
                FireBrowser.Properties.Settings.Default.EngineDefault = lbcos.Text.ToString();
                FireBrowser.Properties.Settings.Default.Engine = false;
            }
            if (ckscrconf.Checked == true)
            {
                FireBrowser.Properties.Settings.Default.Scripts = true;
            }
            else
            {
                FireBrowser.Properties.Settings.Default.Scripts = false;
            }
            if (ckpeekconf.Checked == true)
            {
                FireBrowser.Properties.Settings.Default.AeroPeek = true;
            }
            else
            {
                FireBrowser.Properties.Settings.Default.AeroPeek = false;
            }
            if (ckdevconf.Checked == true)
            {
                FireBrowser.Properties.Settings.Default.dev = true;
            }
            else
            {
                FireBrowser.Properties.Settings.Default.dev = false;
            }
            if (ckicconf.Checked == true)
            {
                FireBrowser.Properties.Settings.Default.icons = true;
            }
            else
            {
                FireBrowser.Properties.Settings.Default.icons = false;
            }
            FireBrowser.Properties.Settings.Default.Save();
        }

        private void BrowserSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
