using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HardwareInfoDLL;
using ConstantsDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using System.Globalization;

namespace HardwareInformation
{
    public class CLIRegister : Form
    {
        public bool pass, serverOnline;
        private bool[] strAlertBool;
        private string[] strArgs, strAlert;
        private WebView2 webView2;
        private LogGenerator log;

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
        public CLIRegister(string servidor, string porta, string modo, string patrimonio, string lacre, string sala, string predio, string ad, string padrao, string data, string pilha, string ticket, string uso, string etiqueta, string tipo, string usuario, LogGenerator l)
        {
            InitializeComponent();
            log = l;
            initProc(servidor, porta, modo, patrimonio, lacre, sala, predio, ad, padrao, data, pilha, ticket, uso, etiqueta, tipo, usuario);
        }

        //Method that allocates a WebView2 instance and checks if args are within standard, then passes them to register method
        public async void initProc(string servidor, string porta, string modo, string patrimonio, string lacre, string sala, string predio, string ad, string padrao, string data, string pilha, string ticket, string uso, string etiqueta, string tipo, string usuario)
        {
            /** !!Labels!!
             * strArgs[0](servidor)
             * strArgs[1](porta)
             * strArgs[2](modo)
             * strArgs[3](patrimonio)
             * strArgs[4](lacre)
             * strArgs[5](sala)
             * strArgs[6](predio)
             * strArgs[7](ad)
             * strArgs[8](padrao)
             * strArgs[9](data)
             * strArgs[10](pilha)
             * strArgs[11](ticket)
             * strArgs[12](uso)
             * strArgs[13](etiqueta)
             * strArgs[14](tipo)
             * strArgs[15]()
             * strArgs[16]()
             * strArgs[17](BM)
             * strArgs[18](Model)
             * strArgs[19](SerialNo)
             * strArgs[20](ProcName)
             * strArgs[21](PM)
             * strArgs[22](HDSize)
             * strArgs[23](OS)
             * strArgs[24](Hostname)
             * strArgs[25](BIOS)
             * strArgs[26](Mac)
             * strArgs[27](IP)
             * strArgs[28](BIOSType)
             * strArgs[29](MediaType)
             * strArgs[30](GPUInfo)
             * strArgs[31](MediaOperation)
             * strArgs[32](SecBoot)
             * strArgs[33](VT)
             * strArgs[34](TPM)
             * strArgs[35](usuario)
             */
            strArgs = new string[36];

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
            strArgs[35] = usuario;

            strAlert = new string[11];
            strAlertBool = new bool[11];
            strAlert[0] = StringsAndConstants.CLI_HOSTNAME_ALERT;
            strAlert[1] = StringsAndConstants.CLI_MEDIA_OPERATION_ALERT;
            strAlert[2] = StringsAndConstants.CLI_SECURE_BOOT_ALERT;
            strAlert[3] = StringsAndConstants.CLI_DATABASE_REACH_ERROR;
            strAlert[4] = StringsAndConstants.CLI_BIOS_VERSION_ALERT;
            strAlert[5] = StringsAndConstants.CLI_FIRMWARE_TYPE_ALERT;
            strAlert[6] = StringsAndConstants.CLI_NETWORK_IP_ERROR;
            strAlert[7] = StringsAndConstants.CLI_NETWORK_MAC_ERROR;
            strAlert[8] = StringsAndConstants.CLI_VT_ALERT;
            strAlert[9] = StringsAndConstants.CLI_TPM_ALERT;
            strAlert[10] = StringsAndConstants.CLI_MEMORY_ALERT;

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
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PINGGING_SERVER, string.Empty, StringsAndConstants.consoleOutCLI);
                serverOnline = await BIOSFileReader.checkHostMT(strArgs[0], strArgs[1]);
                if (serverOnline && strArgs[1] != "")
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERVER_DETAIL, strArgs[0] + ":" + strArgs[1], true);
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_ONLINE_SERVER, string.Empty, StringsAndConstants.consoleOutCLI);

                    collectThread();

                    log.LogWrite(StringsAndConstants.LOG_INFO, MiscMethods.sinceLabelUpdate(true), string.Empty, StringsAndConstants.consoleOutCLI);
                    log.LogWrite(StringsAndConstants.LOG_INFO, MiscMethods.sinceLabelUpdate(false), string.Empty, StringsAndConstants.consoleOutCLI);

                    if (strArgs[2].Equals("f") || strArgs[2].Equals("F"))
                        strArgs[2] = StringsAndConstants.formatURL;
                    else if (strArgs[2].Equals("m") || strArgs[2].Equals("M"))
                        strArgs[2] = StringsAndConstants.maintenanceURL;

                    if (strArgs[8].Equals("A") || strArgs[8].Equals("a"))
                        strArgs[8] = StringsAndConstants.employee;
                    else if (strArgs[8].Equals("F") || strArgs[8].Equals("f"))
                        strArgs[8] = StringsAndConstants.student;

                    if (strArgs[10].Equals("Sim"))
                        strArgs[10] = StringsAndConstants.replacedBattery;
                    else
                        strArgs[10] = StringsAndConstants.sameBattery;

                    printHardwareData();

                    if (pass)
                    {
                        if (strArgs[9].Equals(StringsAndConstants.today))
                        {
                            strArgs[9] = DateTime.Today.ToString().Substring(0, 10);

                            if (modo == "f" || modo == "F")
                                MiscMethods.regCreate(true, DateTime.Today.ToString());
                            if (modo == "m" || modo == "M")
                                MiscMethods.regCreate(false, DateTime.Today.ToString());
                        }
                        else
                        {
                            if (modo == "f" || modo == "F")
                                MiscMethods.regCreate(true, strArgs[9]);
                            if (modo == "m" || modo == "M")
                                MiscMethods.regCreate(false, strArgs[9]);
                        }

                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_REGISTRY, string.Empty, StringsAndConstants.consoleOutCLI);
                        serverSendInfo(strArgs);
                        webView2.NavigationCompleted += webView2_NavigationCompleted;
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTRY_FINISHED, string.Empty, StringsAndConstants.consoleOutCLI);
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.FIX_PROBLEMS, string.Empty, StringsAndConstants.consoleOutCLI);
                        for (int i = 0; i < strAlert.Length; i++)
                        {
                            if (strAlertBool[i])
                                log.LogWrite(StringsAndConstants.LOG_WARNING, strAlert[i], string.Empty, StringsAndConstants.consoleOutCLI);
                        }
                        webView2.Dispose();
                        Environment.Exit(StringsAndConstants.RETURN_WARNING);
                    }

                    if (modo == "f" || modo == "F")
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_INSTALLDATE, string.Empty, StringsAndConstants.consoleOutCLI);
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutCLI);
                    }
                    else
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutCLI);
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_OFFLINE_SERVER, string.Empty, StringsAndConstants.consoleOutCLI);
                    Console.WriteLine(StringsAndConstants.LOG_OFFLINE_SERVER);
                    webView2.Dispose();
                    Environment.Exit(StringsAndConstants.RETURN_ERROR);
                }
            }
            else
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_ARGS_ERROR, string.Empty, StringsAndConstants.consoleOutCLI);
                Console.WriteLine(StringsAndConstants.ARGS_ERROR);
                webView2.Dispose();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING_CLI, string.Empty, StringsAndConstants.consoleOutCLI);
            log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutCLI);
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            webView2.Dispose();
            Environment.Exit(StringsAndConstants.RETURN_SUCCESS);
        }

        //Allocates WebView2 runtime
        public void webView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView2.Dispose();
                Environment.Exit(StringsAndConstants.RETURN_SUCCESS);
            }
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private void printHardwareData()
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_FETCHING_BIOSFILE, string.Empty, StringsAndConstants.consoleOutCLI);

            pass = true;

            try
            {
                string[] str = BIOSFileReader.fetchInfoST(strArgs[17], strArgs[18], strArgs[28], strArgs[34], strArgs[31], strArgs[0], strArgs[1]);

                if (strArgs[24].Equals(StringsAndConstants.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    strAlert[0] += strArgs[24] + StringsAndConstants.HOSTNAME_ALERT;
                    strAlertBool[0] = true;
                }
                if (str != null && str[3].Equals("false"))
                {
                    pass = false;
                    strAlert[1] += strArgs[31] + StringsAndConstants.MEDIA_OPERATION_ALERT;
                    strAlertBool[1] = true;
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
                if (str != null && str[2].Equals("false"))
                {
                    pass = false;
                    strAlert[9] += strArgs[34] + StringsAndConstants.TPM_ERROR;
                    strAlertBool[9] = true;
                }
                double d = Convert.ToDouble(HardwareInfo.GetPhysicalMemoryAlt(), CultureInfo.CurrentCulture.NumberFormat);
                if (d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    strAlert[10] += strArgs[21] + StringsAndConstants.NOT_ENOUGH_MEMORY;
                    strAlertBool[10] = true;
                }
                if (d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    strAlert[10] += strArgs[21] + StringsAndConstants.TOO_MUCH_MEMORY;
                    strAlertBool[10] = true;
                }
                if (pass)
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HARDWARE_PASSED, string.Empty, StringsAndConstants.consoleOutCLI);
            }
            catch (Exception e)
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, e.Message, string.Empty, StringsAndConstants.consoleOutCLI);
            }
        }

        //Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables,
        // while reporting the progress to the progressbar
        public void collectThread()
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_COLLECTING, string.Empty, StringsAndConstants.consoleOutCLI);

            strArgs[17] = HardwareInfo.GetBoardMaker();
            if (strArgs[17] == StringsAndConstants.ToBeFilledByOEM || strArgs[17] == "")
                strArgs[17] = HardwareInfo.GetBoardMakerAlt();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BM, strArgs[17], StringsAndConstants.consoleOutCLI);

            strArgs[18] = HardwareInfo.GetModel();
            if (strArgs[18] == StringsAndConstants.ToBeFilledByOEM || strArgs[18] == "")
                strArgs[18] = HardwareInfo.GetModelAlt();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MODEL, strArgs[18], StringsAndConstants.consoleOutCLI);

            strArgs[19] = HardwareInfo.GetBoardProductId();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERIALNO, strArgs[19], StringsAndConstants.consoleOutCLI);

            strArgs[20] = HardwareInfo.GetProcessorCores();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PROCNAME, strArgs[20], StringsAndConstants.consoleOutCLI);

            strArgs[21] = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PM, strArgs[21], StringsAndConstants.consoleOutCLI);

            strArgs[22] = HardwareInfo.GetHDSize();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HDSIZE, strArgs[22], StringsAndConstants.consoleOutCLI);

            strArgs[29] = HardwareInfo.GetStorageType();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MEDIATYPE, strArgs[29], StringsAndConstants.consoleOutCLI);

            strArgs[31] = HardwareInfo.GetStorageOperation();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MEDIAOP, strArgs[31], StringsAndConstants.consoleOutCLI);

            strArgs[30] = HardwareInfo.GetGPUInfo();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_GPUINFO, strArgs[30], StringsAndConstants.consoleOutCLI);

            strArgs[23] = HardwareInfo.GetOSInformation();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OS, strArgs[23], StringsAndConstants.consoleOutCLI);

            strArgs[24] = HardwareInfo.GetComputerName();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HOSTNAME, strArgs[24], StringsAndConstants.consoleOutCLI);

            strArgs[26] = HardwareInfo.GetMACAddress();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MAC, strArgs[26], StringsAndConstants.consoleOutCLI);

            strArgs[27] = HardwareInfo.GetIPAddress();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_IP, strArgs[27], StringsAndConstants.consoleOutCLI);

            strArgs[28] = HardwareInfo.GetBIOSType();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BIOSTYPE, strArgs[28], StringsAndConstants.consoleOutCLI);

            strArgs[32] = HardwareInfo.GetSecureBoot();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SECBOOT, strArgs[32], StringsAndConstants.consoleOutCLI);

            strArgs[25] = HardwareInfo.GetComputerBIOS();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BIOS, strArgs[25], StringsAndConstants.consoleOutCLI);

            strArgs[33] = HardwareInfo.GetVirtualizationTechnology();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_VT, strArgs[33], StringsAndConstants.consoleOutCLI);

            strArgs[34] = HardwareInfo.GetTPMStatus();
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_TPM, strArgs[34], StringsAndConstants.consoleOutCLI);

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_COLLECTING, string.Empty, StringsAndConstants.consoleOutCLI);
        }

        //Loads webView2 component
        public async Task loadWebView2()
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutCLI);
            CoreWebView2Environment webView2Environment;
            if (Environment.Is64BitOperatingSystem)
                webView2Environment = await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.getWebView2Version(), Path.GetTempPath());
            else
                webView2Environment = await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.getWebView2Version(), Path.GetTempPath());
            await webView2.EnsureCoreWebView2Async(webView2Environment);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutCLI);
        }

        //Sends hardware info to the specified server
        public void serverSendInfo(string[] serverArgs)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING, string.Empty, StringsAndConstants.consoleOutCLI);
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[17] + "&modelo=" + serverArgs[18] + "&numeroSerial=" + serverArgs[19] + "&processador=" + serverArgs[20] + "&memoria=" + serverArgs[21] + "&hd=" + serverArgs[22] + "&sistemaOperacional=" + serverArgs[23] + "&nomeDoComputador=" + serverArgs[24] + "&bios=" + serverArgs[25] + "&mac=" + serverArgs[26] + "&ip=" + serverArgs[27] + "&emUso=" + serverArgs[12] + "&etiqueta=" + serverArgs[13] + "&tipo=" + serverArgs[14] + "&tipoFW=" + serverArgs[28] + "&tipoArmaz=" + serverArgs[29] + "&gpu=" + serverArgs[30] + "&modoArmaz=" + serverArgs[31] + "&secBoot=" + serverArgs[32] + "&vt=" + serverArgs[33] + "&tpm=" + serverArgs[34] + "&trocaPilha=" + serverArgs[10] + "&ticketNum=" + serverArgs[11] + "&agent=" + serverArgs[35]);
        }
    }
}
