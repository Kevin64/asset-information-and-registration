using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    public class CLIRegister : Form
    {
        public bool pass;
        private bool[] strAlertBool;
        private string[] strArgs, strAlert;
        private WebView2 webView2;

        //Basic form for WebView2
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CLIRegister));
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.SuspendLayout();
            // 
            // webView2
            // 
            this.webView2.AllowExternalDrop = true;
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Location = new System.Drawing.Point(12, 12);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(134, 29);
            this.webView2.TabIndex = 0;
            this.webView2.ZoomFactor = 1D;
            // 
            // CLIRegister
            // 
            this.ClientSize = new System.Drawing.Size(158, 53);
            this.ControlBox = false;
            this.Controls.Add(this.webView2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CLIRegister";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.ResumeLayout(false);

        }

        //Constructor
        public CLIRegister(string servidor, string porta, string modo, string patrimonio, string lacre, string sala, string predio, string ad, string padrao, string data, string pilha, string ticket, string uso, string etiqueta, string tipo)
        {
            InitializeComponent();
            initProc(servidor, porta, modo, patrimonio, lacre, sala, predio, ad, padrao, data, pilha, ticket, uso, etiqueta, tipo);
        }

        //When form closes
        public void CLIRegister_FormClosing(object sender, FormClosingEventArgs e)
        {
            webView2.Dispose();
            Application.Exit();
        }

        //Method that allocates a WebView2 instance and checks if args are within standard, then passes them to register method
        public async void initProc(string servidor, string porta, string modo, string patrimonio, string lacre, string sala, string predio, string ad, string padrao, string data, string pilha, string ticket, string uso, string etiqueta, string tipo)
        {
            strArgs = new string[35];
            
            strArgs[0] = servidor;
            strArgs[1] = porta;
            strArgs[2] = modo;
            strArgs[3] = patrimonio;
            strArgs[4] = lacre;
            strArgs[5] = sala;
            strArgs[6] = predio;
            strArgs[7] = ad;
            strArgs[8] = padrao;
            strArgs[9] = data;
            strArgs[10] = pilha;
            strArgs[11] = ticket;
            strArgs[12] = uso;
            strArgs[13] = etiqueta;
            strArgs[14] = tipo;

            strAlert = new string[9];
            strAlertBool = new bool[9];
            strAlert[0] = StringsAndConstants.CLI_HOSTNAME_ALERT;
            strAlert[1] = StringsAndConstants.CLI_MEDIA_OPERATION_ALERT;
            strAlert[2] = StringsAndConstants.CLI_SECURE_BOOT_ALERT;
            strAlert[3] = StringsAndConstants.CLI_DATABASE_REACH_ERROR;
            strAlert[4] = StringsAndConstants.CLI_BIOS_VERSION_ALERT;
            strAlert[5] = StringsAndConstants.CLI_FIRMWARE_TYPE_ALERT;
            strAlert[6] = StringsAndConstants.CLI_NETWORK_IP_ERROR;
            strAlert[7] = StringsAndConstants.CLI_NETWORK_MAC_ERROR;
            strAlert[8] = StringsAndConstants.CLI_VT_ALERT;

            webView2 = new WebView2();

            await loadWebView2();

            if (strArgs[0].Length <= 15 && strArgs[0].Length > 6 &&
                strArgs[1].Length <= 5 &&
                StringsAndConstants.listMode.Contains(strArgs[2]) &&
                strArgs[3].Length <= 6 && strArgs[3].Length > 0 &&
                strArgs[4].Length <= 10 &&
                strArgs[5].Length <= 4 && strArgs[5].Length > 0 &&
                StringsAndConstants.listBuilding.Contains(strArgs[6]) &&
                StringsAndConstants.listActiveDirectory.Contains(strArgs[7]) &&
                StringsAndConstants.listStandard.Contains(strArgs[8]) &&
                (strArgs[9].Length == 10 || strArgs[9].Equals(StringsAndConstants.today)) &&
                StringsAndConstants.listBattery.Contains(strArgs[10]) &&
                strArgs[11].Length <= 6 &&
                StringsAndConstants.listInUse.Contains(strArgs[12]) &&
                StringsAndConstants.listTag.Contains(strArgs[13]) &&
                StringsAndConstants.listType.Contains(strArgs[14]))
            {
                if (strArgs[2].Equals("f") || strArgs[2].Equals("F"))
                    strArgs[2] = StringsAndConstants.formatURL;
                else if(strArgs[2].Equals("m") || strArgs[2].Equals("M"))
                    strArgs[2] = StringsAndConstants.maintenanceURL;
                
                if (strArgs[8].Equals("A") || strArgs[8].Equals("a"))
                    strArgs[8] = StringsAndConstants.employee;
                else if (strArgs[8].Equals("F") || strArgs[8].Equals("f"))
                    strArgs[8] = StringsAndConstants.student;

                if (strArgs[10].Equals("Sim"))
                    strArgs[10] = StringsAndConstants.replacedBattery;
                else
                    strArgs[10] = StringsAndConstants.sameBattery;

                if (strArgs[9].Equals(StringsAndConstants.today))
                {
                    MiscMethods.regCreate(true, DateTime.Today.ToString());
                    strArgs[9] = DateTime.Today.ToString().Substring(0, 10);
                }
                else
                    MiscMethods.regCreate(true, strArgs[9]);
                collectThread();
                printHardwareData();
                if (pass)
                {
                    serverSendInfo(strArgs);
                    webView2.NavigationCompleted += webView2_NavigationCompleted;
                }
                else
                {
                    Console.WriteLine(StringsAndConstants.FIX_PROBLEMS);
                    for (int i = 0; i < strAlert.Length; i++)
                    {
                        if (strAlertBool[i])
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(strAlert[i]);
                            Console.ResetColor();
                        }
                    }
                    File.Delete(StringsAndConstants.@fileBios);
                    File.Delete(StringsAndConstants.@fileLogin);
                    webView2.Dispose();
                    Application.Exit();
                }
            }
            else
            {
                Console.WriteLine(StringsAndConstants.ARGS_ERROR);
                webView2.Dispose();
                Application.Exit();
            }
        }

        //Allocates WebView2 runtime
        public void webView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if(e.IsSuccess)
            {
                File.Delete(StringsAndConstants.@fileBios);
                File.Delete(StringsAndConstants.@fileLogin);
                webView2.Dispose();
                Application.Exit();
            }
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private void printHardwareData()
        {
            pass = true;
            string[] str = BIOSFileReader.fetchInfo(strArgs[17], strArgs[18], strArgs[28], strArgs[0], strArgs[1]);

            if (strArgs[24].Equals(StringsAndConstants.DEFAULT_HOSTNAME))
            {
                pass = false;
                strAlert[0] += strArgs[24] + StringsAndConstants.HOSTNAME_ALERT;
                strAlertBool[0] = true;
            }
            //The section below contains the exception cases for AHCI enforcement
            if (!strArgs[18].Contains(StringsAndConstants.nonAHCImodel1) &&
                !strArgs[18].Contains(StringsAndConstants.nonAHCImodel2) &&
                !strArgs[18].Contains(StringsAndConstants.nonAHCImodel3) &&
                !strArgs[18].Contains(StringsAndConstants.nonAHCImodel4) &&
                !strArgs[18].Contains(StringsAndConstants.nonAHCImodel5) &&
                Environment.Is64BitOperatingSystem &&
                strArgs[31].Equals(StringsAndConstants.MEDIA_OPERATION_IDE_RAID))
            {
                if (strArgs[18].Contains(StringsAndConstants.nvmeModel1))
                {
                    strArgs[31] = StringsAndConstants.MEDIA_OPERATION_NVME;
                }
                else
                {
                    pass = false;
                    strAlert[1] += strArgs[31] + StringsAndConstants.MEDIA_OPERATION_ALERT;
                    strAlertBool[1] = true;
                }
            }
            //The section below contains the exception cases for Secure Boot enforcement
            if (strArgs[32].Equals(StringsAndConstants.deactivated) &&
                !strArgs[30].Contains(StringsAndConstants.nonSecBootGPU1) &&
                !strArgs[30].Contains(StringsAndConstants.nonSecBootGPU2))
            {
                pass = false;
                strAlert[2] += strArgs[32] + StringsAndConstants.SECURE_BOOT_ALERT;
                strAlertBool[2] = true;
            }
            if (str == null)
            {
                pass = false;
                strAlert[3] += StringsAndConstants.DATABASE_REACH_ERROR;
                strAlertBool[3] = true;
            }
            if (str != null && !strArgs[25].Contains(str[0]))
            {
                if (!str[0].Equals("-1"))
                {
                    pass = false;
                    strAlert[4] += strArgs[25] + StringsAndConstants.BIOS_VERSION_ALERT;
                    strAlertBool[4] = true;
                }
            }
            if (str != null && str[1].Equals("false"))
            {
                pass = false;
                strAlert[5] += strArgs[28] + StringsAndConstants.FIRMWARE_TYPE_ALERT;
                strAlertBool[5] = true;
            }
            if (strArgs[26] == "")
            {
                pass = false;
                strAlert[6] += strArgs[26] + StringsAndConstants.NETWORK_ERROR;
                strAlert[7] += strArgs[27] + StringsAndConstants.NETWORK_ERROR;
                strAlertBool[6] = true;
                strAlertBool[7] = true;
            }
            if (strArgs[33] == StringsAndConstants.deactivated)
            {
                pass = false;
                strAlert[8] += strArgs[33] + StringsAndConstants.VT_ALERT;
                strAlertBool[8] = true;
            }
        }

        //Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables,
        // while reporting the progress to the progressbar
        public void collectThread()
        {
            strArgs[17] = HardwareInfo.GetBoardMaker();

            strArgs[18] = HardwareInfo.GetModel();

            strArgs[19] = HardwareInfo.GetBoardProductId();

            strArgs[20] = HardwareInfo.GetProcessorCores();

            strArgs[21] = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";

            strArgs[22] = HardwareInfo.GetHDSize();

            strArgs[29] = HardwareInfo.GetStorageType();

            strArgs[31] = HardwareInfo.GetStorageOperation();

            strArgs[30] = HardwareInfo.GetGPUInfo();

            strArgs[23] = HardwareInfo.GetOSInformation();

            strArgs[24] = HardwareInfo.GetComputerName();

            strArgs[26] = HardwareInfo.GetMACAddress();

            strArgs[27] = HardwareInfo.GetIPAddress();

            strArgs[28] = HardwareInfo.GetBIOSType();

            strArgs[32] = HardwareInfo.GetSecureBoot();

            strArgs[25] = HardwareInfo.GetComputerBIOS();

            strArgs[33] = HardwareInfo.GetVirtualizationTechnology();

            strArgs[34] = HardwareInfo.GetTPMStatus();
        }

        //Loads webView2 component
        public async Task loadWebView2()
        {
            CoreWebView2Environment webView2Environment = await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_PATH, Path.GetTempPath());
            await webView2.EnsureCoreWebView2Async(webView2Environment);
        }

        //Sends hardware info to the specified server
        public void serverSendInfo(string[] serverArgs)
        {
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[17] + "&modelo=" + serverArgs[18] + "&numeroSerial=" + serverArgs[19] + "&processador=" + serverArgs[20] + "&memoria=" + serverArgs[21] + "&hd=" + serverArgs[22] + "&sistemaOperacional=" + serverArgs[23] + "&nomeDoComputador=" + serverArgs[24] + "&bios=" + serverArgs[25] + "&mac=" + serverArgs[26] + "&ip=" + serverArgs[27] + "&emUso=" + serverArgs[12] + "&etiqueta=" + serverArgs[13] + "&tipo=" + serverArgs[14] + "&tipoFW=" + serverArgs[28] + "&tipoArmaz=" + serverArgs[29] + "&gpu=" + serverArgs[30] + "&modoArmaz=" + serverArgs[31] + "&secBoot=" + serverArgs[32] + "&vt=" + serverArgs[33] + "&tpm=" + serverArgs[34] + "&trocaPilha=" + serverArgs[10] + "&ticketNum=" + serverArgs[11]);
        }
    }
}
