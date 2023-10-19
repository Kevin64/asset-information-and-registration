﻿using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    internal partial class RamDetailForm : Form, ITheming
    {
        static List<List<string>> auxList;

        public RamDetailForm(List<List<string>> str, List<string[]> parametersList, bool isSystemDarkModeEnabled)
        {
            double individualRam;
            string individualRamStr;
            auxList = str;
            InitializeComponent();
            this.KeyDown += RamDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in auxList)
            {
                if (Convert.ToDouble(s[1].TrimEnd('K', 'M', 'G', 'T', 'B')) > 1024)
                {
                    individualRam = Convert.ToInt64(s[1]);
                    if (individualRam / 1024 / 1024 / 1024 >= 1024)
                        individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024 / 1024, 0) + " " + ConstantsDLL.Properties.Resources.TB;
                    else if (individualRam / 1024 / 1024 / 1024 < 1024 && individualRam / 1024 / 1024 / 1024 >= 1)
                        individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024, 0) + " " + ConstantsDLL.Properties.Resources.GB;
                    else
                        individualRamStr = Math.Round(individualRam / 1024 / 1024, 0) + " " + ConstantsDLL.Properties.Resources.MB;
                    s[1] = individualRamStr;

                    if(s[2] == ConstantsDLL.Properties.Resources.DDR4_SMBIOS)
                        s[2] = ConstantsDLL.Properties.Resources.DDR4;
                    else if (s[2] == ConstantsDLL.Properties.Resources.DDR3_SMBIOS)
                        s[2] = ConstantsDLL.Properties.Resources.DDR3;
                    else
                        s[2] = ConstantsDLL.Properties.Resources.DDR2;

                    s[3] = s[3] + " " + ConstantsDLL.Properties.Resources.MHZ;
                }

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by slot column
            dataGridView1.Sort(dataGridView1.Columns["ramSlot"], ListSortDirection.Ascending);

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
        }

        private void RamDetailForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
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
        }
    }
}