﻿using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    internal partial class StorageDetailForm : Form, ITheming
    {
        public StorageDetailForm(List<List<string>> str, List<string[]> parametersList, bool themeBool)
        {
            InitializeComponent();

            foreach (List<string> s in str)
            {
                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Define theming according to ini file provided info
            //themeBool = MiscMethods.GetSystemThemeMode();
            int themeFileSet = MiscMethods.GetFileThemeMode(parametersList, themeBool);
            switch (themeFileSet)
            {
                case 0:
                    DarkTheme();
                    break;
                case 1:
                    LightTheme();
                    break;
                case 2:
                    LightTheme();
                    break;
                case 3:
                    DarkTheme();
                    break;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell cell = row.Cells[5];
                if (cell.Value != null && cell.Value.Equals(ConstantsDLL.Properties.Resources.PRED_FAIL))
                {
                    cell.Style.BackColor = Color.Red;
                    cell.Style.ForeColor = Color.White;
                }
            }
        }

        public void LightTheme()
        {
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            dataGridView1.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
            }
        }

        public void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            dataGridView1.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
            }
        }
    }
}
