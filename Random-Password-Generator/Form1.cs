using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Random_Password_Generator
{
    public partial class Form1 : Form
    {
      

        public Form1()
        {
            InitializeComponent();

            bool isDarkTheme = Properties.Settings.Default.AppTheme == "dark";
            ApplyTheme(isDarkTheme);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lstHistory.ContextMenuStrip = contextMenuStrip1;
            //LoadPasswordHistory();  // Program başladığında geçmişi yükle

            // Kaydedilen verileri yükle
            InitData();
            UpdateEmptyMessage();




        }

        private void LoadPasswordHistory()
        {
            // Settings'ten geçmişi yükle
            if (Properties.Settings.Default.PasswordHistory != null)
            {
                foreach (var password in Properties.Settings.Default.PasswordHistory)
                {
                    lstHistory.Items.Add(password);  // ListBox'a ekle
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

            int length = (int)numLength.Value;
            bool useUpper = chkUpper.Checked;
            bool useLower = chkLower.Checked;
            bool useDigits = chkDigits.Checked;
            bool useSpecial = chkSpecial.Checked;

            string password = GeneratePassword(length, useUpper, useLower, useDigits, useSpecial);
            txtPassword.Text = password;

            ShowStrength(password);

            if (!string.IsNullOrWhiteSpace(password) && !password.StartsWith("Karakter"))
                Clipboard.SetText(password); // otomatik kopyalama

            
            UpdateEmptyMessage();
        }

        private string GeneratePassword(int length, bool useUpper, bool useLower, bool useDigits, bool useSpecial)
        {
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string digits = "0123456789";
            string special = "!@#$%^&*()-_=+<>?";

            string charPool = "";
            if (useUpper) charPool += upper;
            if (useLower) charPool += lower;
            if (useDigits) charPool += digits;
            if (useSpecial) charPool += special;

            if (string.IsNullOrEmpty(charPool))
                return "Karakter türü seçin!";

            Random rand = new Random();
            string password = new string(Enumerable.Repeat(charPool, length)
                .Select(s => s[rand.Next(s.Length)]).ToArray());

            // Şifreyi önce kopyala ve listeye ekle
            if (!string.IsNullOrWhiteSpace(password) && !password.StartsWith("Karakter"))
            {
                Clipboard.SetText(password);
                lstHistory.Items.Insert(0, $"{DateTime.Now:T} → {password}");
            }

            return password;  // Şifreyi döndür
        }


        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                Clipboard.SetText(txtPassword.Text);
                MessageBox.Show("Şifre panoya kopyalandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowStrength(string password)
        {
            int score = 0;

            if (password.Length >= 8) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(ch => "!@#$%^&*()-_=+<>?".Contains(ch))) score++;

            string[] levels = { "Çok Zayıf", "Zayıf", "Orta", "Güçlü", "Çok Güçlü" };
            Color[] colors = { Color.DarkRed, Color.OrangeRed, Color.Goldenrod, Color.ForestGreen, Color.LimeGreen };

            int index = Math.Min(score, levels.Length - 1);
            lblStrength.Text = levels[index];
            lblStrength.ForeColor = colors[index];

 
        }

   

        private void ApplyTheme(bool isDarkTheme)
        {
            Color backColor = isDarkTheme ? Color.FromArgb(30, 30, 30) : Color.White;
            Color foreColor = isDarkTheme ? Color.White : Color.Black;
            Color boxBack = isDarkTheme ? Color.FromArgb(45, 45, 45) : Color.White;

            this.BackColor = backColor;

            foreach (Control ctrl in this.Controls)
            {
                ctrl.ForeColor = foreColor;

                if (ctrl is TextBox tb)
                {
                    tb.BackColor = boxBack;
                    tb.ForeColor = foreColor;
                }
                else if (ctrl is CheckBox cb)
                {
                    cb.BackColor = backColor;
                    cb.ForeColor = foreColor;
                }
                else if (ctrl is Button btn)
                {
                    btn.BackColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightGray;
                    btn.ForeColor = foreColor;
                    btn.FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is NumericUpDown num)
                {
                    num.BackColor = boxBack;
                    num.ForeColor = foreColor;
                    num.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (ctrl is ListBox list)
                {
                    list.BackColor = boxBack;
                    list.ForeColor = foreColor;
                }
                else if (ctrl is MenuStrip menu)
                {
                    menu.BackColor = boxBack; // MenuStrip arka plan rengini ayarlıyoruz
                    menu.ForeColor = foreColor; // MenuStrip yazı rengini ayarlıyoruz

                    // Menülerdeki her item için renkleri ayarlıyoruz
                    foreach (ToolStripMenuItem item in menu.Items)
                    {
                        item.BackColor = boxBack; // Menu öğelerinin arka plan rengini ayarlıyoruz
                        item.ForeColor = foreColor; // Menu öğelerinin yazı rengini ayarlıyoruz

                        foreach (ToolStripItem subItem in item.DropDownItems)
                        {
                            if (subItem is ToolStripMenuItem subMenuItem)
                            {
                                subMenuItem.BackColor = boxBack; // Alt menü öğelerinin arka plan rengini ayarlıyoruz
                                subMenuItem.ForeColor = foreColor; // Alt menü öğelerinin yazı rengini ayarlıyoruz
                            }
                        }
                    }

                    // Menüde açılır (drop-down) menülerde stil ve renk değişiklikleri yapmak için:
                    menu.Renderer = new ToolStripProfessionalRenderer(new CustomMenuColorTable(isDarkTheme));
                }

            }
        }


        // Tema durumuna göre renkleri ayarlamak için özel bir MenuColorTable oluşturuyoruz.
        public class CustomMenuColorTable : ProfessionalColorTable
        {
            private bool isDarkTheme;

            public CustomMenuColorTable(bool isDarkTheme)
            {
                this.isDarkTheme = isDarkTheme;
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightGray;
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightGray;
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightGray;
                }
            }

            public override Color MenuItemBorder
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(100, 100, 100) : Color.DarkGray;
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(100, 100, 100) : Color.DarkGray;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return isDarkTheme ? Color.FromArgb(100, 100, 100) : Color.DarkGray;
                }
            }
        }

        //private void UygulaDarkTema()
        //{
        //    Color backColor = isDarkTheme ? Color.FromArgb(30, 30, 30) : Color.White;
        //    Color foreColor = isDarkTheme ? Color.White : Color.Black;
        //    Color boxBack = isDarkTheme ? Color.FromArgb(45, 45, 45) : Color.White;

        //    this.BackColor = backColor;
        //    foreach (Control ctrl in this.Controls)
        //    {
        //        ctrl.ForeColor = foreColor;
        //        if (ctrl is TextBox tb)
        //        {
        //            tb.BackColor = boxBack;
        //            tb.ForeColor = foreColor;
        //        }
        //        if (ctrl is CheckBox cb)
        //        {
        //            cb.BackColor = backColor;
        //            cb.ForeColor = foreColor;
        //        }
        //        if (ctrl is Button btn)
        //        {
        //            btn.BackColor = isDarkTheme ? Color.FromArgb(60, 60, 60) : Color.LightGray;
        //            btn.ForeColor = foreColor;
        //            btn.FlatStyle = FlatStyle.Flat;
        //        }
        //        if (ctrl is NumericUpDown num)
        //        {
        //            num.BackColor = boxBack;
        //            num.ForeColor = foreColor;
        //            num.BorderStyle = BorderStyle.FixedSingle;
        //        }
        //        if (ctrl is ListBox list)
        //        {
        //            list.BackColor = boxBack;
        //            list.ForeColor = foreColor;
        //        }
        //    }
        //}



        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            bool isDarkTheme = Properties.Settings.Default.AppTheme == "dark";
            bool newTheme = !isDarkTheme;

            ApplyTheme(newTheme);
            Properties.Settings.Default.AppTheme = newTheme ? "dark" : "light";
            Properties.Settings.Default.Save();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            lstHistory.Items.Clear();
            Properties.Settings.Default.pwdhistory = new System.Collections.Specialized.StringCollection();
            Properties.Settings.Default.Save();
            UpdateEmptyMessage();

            // Seçilen öğeyi ListBox'tan al
            var selectedItem = lstHistory.SelectedItem;

            if (selectedItem != null)
            {
                // ListBox'tan öğeyi sil
                lstHistory.Items.Remove(selectedItem);

                // (Opsiyonel) Silinen öğe üzerinde işlem yapabilirsiniz (Settings vb.),
                // Ama bu durumda sadece ListBox'tan siliyoruz, Settings'e kaydetmiyoruz.
            }
        }

        private void lstHistory_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstHistory.SelectedItem != null)
            {
                Clipboard.SetText(lstHistory.SelectedItem.ToString().Split('→')[1].Trim());
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItem != null)
            {
                Clipboard.SetText(lstHistory.SelectedItem.ToString().Split('→')[1].Trim());
                MessageBox.Show("Şifre başarıyla kopyalandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItem != null)
            {
                lstHistory.Items.Remove(lstHistory.SelectedItem);
            }
        }

        private void btnExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstHistory.Items.Count == 0)
            {
                MessageBox.Show("Liste boş!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text File (*.txt)|*.txt";
                saveFileDialog.FileName = "PasswordHistory.txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (var item in lstHistory.Items)
                        {
                            writer.WriteLine(item.ToString());
                        }
                    }

                    MessageBox.Show("Liste başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Verileri kaydet
            SaveData();
        }

        private void SaveData()
        {
            var collection = new System.Collections.Specialized.StringCollection();
            foreach (var item in lstHistory.Items)
            {
                collection.Add(item.ToString());
            }

            Properties.Settings.Default.pwdhistory = collection;
            Properties.Settings.Default.Save();
        }

        private void InitData()
        {
            // Şifre geçmişini Settings'ten yükleyelim
            var collection = Properties.Settings.Default.pwdhistory;
            if (collection != null)
            {
                foreach (string item in collection)
                {
                    lstHistory.Items.Add(item);
                }
            }
        }

        private void UpdateEmptyMessage()
        {
            lblEmptyMessage.Visible = lstHistory.Items.Count == 0;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide(); // formu gizle
                notifyIcon1.Visible = true;

                notifyIcon1.BalloonTipTitle = "Şifre Oluşturucu Aktif";
                notifyIcon1.BalloonTipText = "Uygulama sistem çubuğunda çalışıyor.";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.ShowBalloonTip(3000); // 3 saniye göster
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(); // Modal olarak aç
        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isDarkTheme = Properties.Settings.Default.AppTheme == "dark";
            bool newTheme = !isDarkTheme;

            ApplyTheme(newTheme);
            Properties.Settings.Default.AppTheme = newTheme ? "dark" : "light";
            Properties.Settings.Default.Save();
        }
    }
}
