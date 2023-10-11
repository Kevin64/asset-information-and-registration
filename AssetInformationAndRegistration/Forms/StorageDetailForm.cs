using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    internal partial class StorageDetailForm : Form, ITheming
    {
        private static double totalSize = 0;
        public StorageDetailForm(List<List<string>> str, List<string[]> parametersList, bool isSystemDarkModeEnabled)
        {
            double individualSize;
            string individualSizeStr, totalSizeStr;
            InitializeComponent();

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in str)
            {
                if (Convert.ToDouble(s[2].TrimEnd('K', 'M', 'G', 'T', 'B')) > 1000)
                {
                    individualSize = Convert.ToInt64(s[2]);
                    if (individualSize / 1000 / 1000 / 1000 >= 1000)
                        individualSizeStr = Math.Round(individualSize / 1000 / 1000 / 1000 / 1000, 0) + " " + ConstantsDLL.Properties.Resources.TB;
                    else if (individualSize / 1000 / 1000 / 1000 < 1000 && individualSize / 1000 / 1000 / 1000 >= 1)
                        individualSizeStr = Math.Round(individualSize / 1000 / 1000 / 1000, 0) + " " + ConstantsDLL.Properties.Resources.GB;
                    else
                        individualSizeStr = Math.Round(individualSize / 1000 / 1000, 0) + " " + ConstantsDLL.Properties.Resources.MB;
                    s[2] = individualSizeStr;
                    totalSize += individualSize;
                }
                _ = dataGridView1.Rows.Add(s.ToArray());
            }
            //Shows the total storage size
            if (totalSize / 1000 / 1000 / 1000 >= 1000)
                totalSizeStr = Math.Round(totalSize / 1000 / 1000 / 1000 / 1000, 2) + " " + ConstantsDLL.Properties.Resources.TB;
            else if (totalSize / 1000 / 1000 / 1000 < 1000 && totalSize / 1000 / 1000 / 1000 >= 1)
                totalSizeStr = Math.Round(totalSize / 1000 / 1000 / 1000, 2) + " " + ConstantsDLL.Properties.Resources.GB;
            else
                totalSizeStr = Math.Round(totalSize / 1000 / 1000, 2) + " " + ConstantsDLL.Properties.Resources.MB;
            lblTotalSize.Text = totalSizeStr;

            //Sorts the ID column
            dataGridView1.Sort(dataGridView1.Columns["storageId"], ListSortDirection.Ascending);

            //Define theming according to ini file provided info
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(parametersList, isSystemDarkModeEnabled);
            switch (themeFileSet)
            {
                case 0:
                    LightTheme();
                    break;
                case 1:
                    DarkTheme();
                    break;
            }

            //Paints cell in red if SMART status equals a 'Pred Fail'
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell cell = row.Cells[6];
                if (cell.Value != null && cell.Value.Equals(ConstantsDLL.Properties.Resources.PRED_FAIL))
                {
                    cell.Style.BackColor = Color.Red;
                    cell.Style.ForeColor = Color.White;
                }
            }
        }

        public void LightTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);

            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            foreach (Button b in Controls.OfType<Button>())
            {
                b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                b.FlatStyle = System.Windows.Forms.FlatStyle.System;
            }
            foreach (CheckBox cb in Controls.OfType<CheckBox>())
            {
                cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (CustomFlatComboBox c in Controls.OfType<CustomFlatComboBox>())
            {
                c.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                c.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                c.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
                c.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            }
            foreach (DataGridView dgv in Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (Label l in Controls.OfType<Label>())
            {
                if (l.Name.Contains("Separator"))
                    l.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
                if (l.Name.Contains("Mandatory"))
                    l.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
                else if (l.Name.Contains("Fixed"))
                    l.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                else
                    l.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            foreach (RadioButton rb in Controls.OfType<RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (RichTextBox rtb in Controls.OfType<RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (TextBox t in Controls.OfType<TextBox>())
            {
                t.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                t.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }

            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_DISK_SIZE_LIGHT_PATH));
        }

        public void DarkTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);

            BackColor = StringsAndConstants.DARK_BACKGROUND;

            foreach (Button b in Controls.OfType<Button>())
            {
                b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            }
            foreach (CheckBox cb in Controls.OfType<CheckBox>())
            {
                cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (CustomFlatComboBox c in Controls.OfType<CustomFlatComboBox>())
            {
                c.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                c.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                c.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                c.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            }
            foreach (DataGridView dgv in Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (Label l in Controls.OfType<Label>())
            {
                if (l.Name.Contains("Separator"))
                    l.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
                if (l.Name.Contains("Mandatory"))
                    l.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
                else if (l.Name.Contains("Fixed"))
                    l.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                else
                    l.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            }
            foreach (RadioButton rb in Controls.OfType<RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (RichTextBox rtb in Controls.OfType<RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (TextBox t in Controls.OfType<TextBox>())
            {
                t.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                t.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }

            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_DISK_SIZE_DARK_PATH));
        }
    }
}