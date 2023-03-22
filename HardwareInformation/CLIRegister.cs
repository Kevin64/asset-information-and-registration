using ConstantsDLL;
using HardwareInfoDLL;
using HardwareInformation.Properties;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    public class CLIRegister : Form
    {
        public bool pass, serverOnline;
        private bool[] strAlertBool;
        private string[] strArgs, strAlert;
        private WebView2 webView2;
        private readonly LogGenerator log;

        //Basic form for WebView2
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CLIRegister));
            webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webView2).BeginInit();
            SuspendLayout();
            // 
            // webView2
            // 
            webView2.AllowExternalDrop = true;
            webView2.CreationProperties = null;
            webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            webView2.Location = new System.Drawing.Point(12, 12);
            webView2.Name = "webView2";
            webView2.Size = new System.Drawing.Size(134, 29);
            webView2.TabIndex = 0;
            webView2.ZoomFactor = 1D;
            // 
            // CLIRegister
            // 
            ClientSize = new System.Drawing.Size(158, 53);
            ControlBox = false;
            Controls.Add(webView2);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CLIRegister";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)webView2).EndInit();
            ResumeLayout(false);
        }

        //Form constructor
        public CLIRegister(string servidor, string porta, string modo, string patrimonio, string lacre, string sala, string predio, string ad, string padrao, string data, string pilha, string ticket, string uso, string etiqueta, string tipo, string usuario, LogGenerator l, List<string[]> definitionList)
        {
            //Inits WinForms components
            InitializeComponent();

            log = l;

            InitProc(servidor, porta, modo, patrimonio, lacre, sala, predio, ad, padrao, data, pilha, ticket, uso, etiqueta, tipo, usuario, definitionList);
        }

        //Method that allocates a WebView2 instance and checks if args are within standard, then passes them to register method
        public async void InitProc(string servidor, string porta, string modo, string patrimonio, string lacre, string sala, string predio, string ad, string padrao, string data, string pilha, string ticket, string uso, string etiqueta, string tipo, string usuario, List<string[]> definitionList)
        {
            #region

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
            strAlert[0] = Strings.CLI_HOSTNAME_ALERT;
            strAlert[1] = Strings.CLI_MEDIA_OPERATION_ALERT;
            strAlert[2] = Strings.CLI_SECURE_BOOT_ALERT;
            strAlert[3] = Strings.CLI_DATABASE_REACH_ERROR;
            strAlert[4] = Strings.CLI_BIOS_VERSION_ALERT;
            strAlert[5] = Strings.CLI_FIRMWARE_TYPE_ALERT;
            strAlert[6] = Strings.CLI_NETWORK_IP_ERROR;
            strAlert[7] = Strings.CLI_NETWORK_MAC_ERROR;
            strAlert[8] = Strings.CLI_VT_ALERT;
            strAlert[9] = Strings.CLI_TPM_ALERT;
            strAlert[10] = Strings.CLI_MEMORY_ALERT;

            #endregion

            //Fetch building and hw types info from the specified server
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            List<string[]> jsonServerSettings = ConfigFileReader.FetchInfoST(servidor, porta);
            definitionList[2] = jsonServerSettings[0];
            definitionList[3] = jsonServerSettings[1];

            webView2 = new WebView2();

            await LoadWebView2();

            string[] dateFormat = new string[] { "dd/MM/yyyy" };

            if (strArgs[0].Length <= 15 && strArgs[0].Length > 6 && //Servidor
                strArgs[1].Length <= 5 && strArgs[1].All(char.IsDigit) && //Porta
                StringsAndConstants.listModeCLI.Contains(strArgs[2]) && //Modo
                strArgs[3].Length <= 6 && strArgs[3].Length >= 0 && strArgs[3].All(char.IsDigit) && //Patrimonio
                ((strArgs[4].Length <= 10 && strArgs[4].All(char.IsDigit)) || strArgs[4].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //Lacre
                ((strArgs[5].Length <= 4 && strArgs[5].Length > 0 && strArgs[5].All(char.IsDigit)) || strArgs[5].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //Sala
                (definitionList[2].Contains(strArgs[6]) || strArgs[6].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //Predio
                (StringsAndConstants.listActiveDirectoryCLI.Contains(strArgs[7]) || strArgs[7].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //AD
                (StringsAndConstants.listStandardCLI.Contains(strArgs[8]) || strArgs[8].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //Padrao
                ((strArgs[9].Length == 10 && DateTime.TryParseExact(strArgs[9], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime datetime)) || strArgs[9].Equals(ConstantsDLL.Properties.Resources.today)) && //Data
                StringsAndConstants.listBatteryCLI.Contains(strArgs[10]) && //Pilha
                strArgs[11].Length <= 6 && strArgs[11].All(char.IsDigit) && //Ticket
                (StringsAndConstants.listInUseCLI.Contains(strArgs[12]) || strArgs[12].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //Uso
                (StringsAndConstants.listTagCLI.Contains(strArgs[13]) || strArgs[13].Equals(ConstantsDLL.Properties.Resources.sameWord)) && //Etiqueta
                (definitionList[3].Contains(strArgs[14]) || strArgs[14].Equals(ConstantsDLL.Properties.Resources.sameWord))) //Tipo
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                serverOnline = await BIOSFileReader.CheckHostMT(strArgs[0], strArgs[1]);
                if (serverOnline && strArgs[1] != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERVER_DETAIL, strArgs[0] + ":" + strArgs[1], true);
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                    CollectThread();

                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), MiscMethods.SinceLabelUpdate(true), string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), MiscMethods.SinceLabelUpdate(false), string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    //Patrimonio
                    if (strArgs[3].Equals(string.Empty))
                    {
                        strArgs[3] = System.Net.Dns.GetHostName().Substring(3);
                    }

                    string[] pcJsonStr = PCFileReader.FetchInfoST(strArgs[3], strArgs[0], strArgs[1]);
                    //If PC Json does not exist and there are some 'mesmo' cmd switch word
                    if (pcJsonStr[0] == "false" && strArgs.Contains(ConstantsDLL.Properties.Resources.sameWord))
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_SAMEWORD_NOFIRSTREGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                        webView2.Dispose();
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                    }
                    else if (pcJsonStr[0] == "false") //If PC Json does not exist
                    {
                        //Modo
                        if (strArgs[2].Equals("f") || strArgs[2].Equals("F"))
                        {
                            strArgs[2] = ConstantsDLL.Properties.Resources.formatURL;
                        }
                        else if (strArgs[2].Equals("m") || strArgs[2].Equals("M"))
                        {
                            strArgs[2] = ConstantsDLL.Properties.Resources.maintenanceURL;
                        }
                        //AD
                        if (strArgs[7].Equals("N") || strArgs[7].Equals("n"))
                        {
                            strArgs[7] = ConstantsDLL.Properties.Resources.NO;
                        }
                        else if (strArgs[7].Equals("S") || strArgs[7].Equals("s"))
                        {
                            strArgs[7] = ConstantsDLL.Properties.Resources.YES;
                        }
                        //Padrao
                        if (strArgs[8].Equals("F") || strArgs[8].Equals("f"))
                        {
                            strArgs[8] = Strings.employee;
                        }
                        else if (strArgs[8].Equals("A") || strArgs[8].Equals("a"))
                        {
                            strArgs[8] = Strings.student;
                        }
                        //Pilha
                        if (strArgs[10].Equals("N") || strArgs[10].Equals("n"))
                        {
                            strArgs[10] = Strings.sameBattery;
                        }
                        else if (strArgs[10].Equals("S") || strArgs[10].Equals("s"))
                        {
                            strArgs[10] = Strings.replacedBattery;
                        }
                        //Uso
                        if (strArgs[12].Equals("N") || strArgs[12].Equals("n"))
                        {
                            strArgs[12] = ConstantsDLL.Properties.Resources.NO;
                        }
                        else if (strArgs[12].Equals("S") || strArgs[12].Equals("s"))
                        {
                            strArgs[12] = ConstantsDLL.Properties.Resources.YES;
                        }
                        //Etiqueta
                        if (strArgs[13].Equals("N") || strArgs[13].Equals("n"))
                        {
                            strArgs[13] = ConstantsDLL.Properties.Resources.NO;
                        }
                        else if (strArgs[13].Equals("S") || strArgs[13].Equals("s"))
                        {
                            strArgs[13] = ConstantsDLL.Properties.Resources.YES;
                        }
                    }
                    else //If PC Json does exist
                    {
                        //If PC is discarded
                        if (pcJsonStr[9] == "1")
                        {
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.PC_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                            webView2.Dispose();
                            Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                        }
                        //Modo
                        if (strArgs[2].Equals("f") || strArgs[2].Equals("F"))
                        {
                            strArgs[2] = ConstantsDLL.Properties.Resources.formatURL;
                        }
                        else if (strArgs[2].Equals("m") || strArgs[2].Equals("M"))
                        {
                            strArgs[2] = ConstantsDLL.Properties.Resources.maintenanceURL;
                        }
                        //Lacre
                        if (strArgs[4].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[4] = pcJsonStr[6];
                        }
                        //Sala
                        if (strArgs[5].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[5] = pcJsonStr[2];
                        }
                        //Predio
                        if (strArgs[6].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[6] = pcJsonStr[1];
                        }
                        //AD
                        if (strArgs[7].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[7] = pcJsonStr[4];
                        }
                        else if (strArgs[7].Equals("N") || strArgs[7].Equals("n"))
                        {
                            strArgs[7] = ConstantsDLL.Properties.Resources.NO;
                        }
                        else if (strArgs[7].Equals("S") || strArgs[7].Equals("s"))
                        {
                            strArgs[7] = ConstantsDLL.Properties.Resources.YES;
                        }
                        //Padrao
                        if (strArgs[8].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[8] = pcJsonStr[3];
                        }
                        else if (strArgs[8].Equals("F") || strArgs[8].Equals("f"))
                        {
                            strArgs[8] = Strings.employee;
                        }
                        else if (strArgs[8].Equals("A") || strArgs[8].Equals("a"))
                        {
                            strArgs[8] = Strings.student;
                        }
                        //Pilha
                        if (strArgs[10].Equals("N") || strArgs[10].Equals("n"))
                        {
                            strArgs[10] = Strings.sameBattery;
                        }
                        else if (strArgs[10].Equals("S") || strArgs[10].Equals("s"))
                        {
                            strArgs[10] = Strings.replacedBattery;
                        }
                        //Uso
                        if (strArgs[12].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[12] = pcJsonStr[5];
                        }
                        else if (strArgs[12].Equals("N") || strArgs[12].Equals("n"))
                        {
                            strArgs[12] = ConstantsDLL.Properties.Resources.NO;
                        }
                        else if (strArgs[12].Equals("S") || strArgs[12].Equals("s"))
                        {
                            strArgs[12] = ConstantsDLL.Properties.Resources.YES;
                        }
                        //Etiqueta
                        if (strArgs[13].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[13] = pcJsonStr[7];
                        }
                        else if (strArgs[13].Equals("N") || strArgs[13].Equals("n"))
                        {
                            strArgs[13] = ConstantsDLL.Properties.Resources.NO;
                        }
                        else if (strArgs[13].Equals("S") || strArgs[13].Equals("s"))
                        {
                            strArgs[13] = ConstantsDLL.Properties.Resources.YES;
                        }
                        //Tipo
                        if (strArgs[14].Equals(ConstantsDLL.Properties.Resources.sameWord))
                        {
                            strArgs[14] = pcJsonStr[8];
                        }
                    }
                    PrintHardwareData();

                    //If there are no pendencies
                    if (pass)
                    {
                        DateTime d = new DateTime();
                        DateTime todayDate = DateTime.Today;
                        bool tDay;

                        try //If there is database record of the patrimony
                        {
                            //If chosen date is 'hoje'
                            if (strArgs[9].Equals(ConstantsDLL.Properties.Resources.today))
                            {
                                strArgs[9] = DateTime.Today.ToString("yyyy-MM-dd").Substring(0, 10);
                                tDay = true;
                            }
                            else //If chosen date is not 'hoje'
                            {
                                d = DateTime.Parse(strArgs[9]);
                                strArgs[9] = d.ToString("yyyy-MM-dd");
                                tDay = false;
                            }

                            //Calculates last registered date with chosen date
                            DateTime registerDate = DateTime.ParseExact(strArgs[9], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(pcJsonStr[10], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            //If chosen date is later than the registered date
                            if (registerDate >= lastRegisterDate)
                            {
                                if (tDay) //If today
                                {
                                    strArgs[9] = todayDate.ToString().Substring(0, 10);
                                }
                                else if (registerDate <= todayDate) //If today is greater or equal than registered date
                                {
                                    strArgs[9] = DateTime.Parse(strArgs[9]).ToString().Substring(0, 10);
                                }
                                else //Forbids future registering
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.INCORRECT_FUTURE_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    webView2.Dispose();
                                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                                }

                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                ServerSendInfo(strArgs); //Send info to server
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                                //Resets host install date
                                if (modo == "f" || modo == "F")
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    MiscMethods.RegCreate(true, strArgs[9]);
                                }
                                if (modo == "m" || modo == "M") //Resets host maintenance date
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    MiscMethods.RegCreate(false, strArgs[9]);
                                }
                            }
                            else //If chosen date is earlier than the registered date, show an error
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                webView2.Dispose(); //Kills WebView2 instance
                                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR)); //Exits
                            }
                        }
                        catch //If there is no database record of the patrimony
                        {
                            if (strArgs[9].Equals(ConstantsDLL.Properties.Resources.today))
                            {
                                strArgs[9] = todayDate.ToString().Substring(0, 10);
                            }
                            else
                            {
                                d = DateTime.Parse(strArgs[9]);
                                strArgs[9] = d.ToString().Substring(0, 10);
                            }

                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                            ServerSendInfo(strArgs); //Send info to server
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                            //Resets host install date
                            if (modo == "f" || modo == "F")
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                MiscMethods.RegCreate(true, strArgs[9]);
                            }
                            else if (modo == "m" || modo == "M") //Resets host maintenance date
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                MiscMethods.RegCreate(false, strArgs[9]);
                            }
                        }
                    }
                    else //If there are pendencies
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.FIX_PROBLEMS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                        for (int i = 0; i < strAlert.Length; i++)
                        {
                            if (strAlertBool[i])
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), strAlert[i], string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                            }
                        }
                        webView2.Dispose(); //Kills WebView2 instance
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_WARNING)); //Exits
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    webView2.Dispose(); //Kills WebView2 instance
                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR)); //Exits
                }
            }
            else
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.ARGS_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                webView2.Dispose(); //Kills WebView2 instance
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR)); //Exits
            }
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING_CLI, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Deletes downloaded json files
            File.Delete(ConstantsDLL.Properties.Resources.biosPath);
            File.Delete(ConstantsDLL.Properties.Resources.loginPath);
            File.Delete(ConstantsDLL.Properties.Resources.pcPath);
            File.Delete(ConstantsDLL.Properties.Resources.configPath);
            webView2.NavigationCompleted += WebView2_NavigationCompleted;
            Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS)); //Exits
        }

        //When WebView2 navigation is finished
        public void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView2.Dispose(); //Kills instance
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS)); //Exits
            }
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private void PrintHardwareData()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_BIOSFILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            pass = true;

            try
            {
                //Feches model info from server
                string[] biosJsonStr = BIOSFileReader.FetchInfoST(strArgs[17], strArgs[18], strArgs[28], strArgs[34], strArgs[31], strArgs[0], strArgs[1]);

                //Scan if hostname is the default one
                if (strArgs[24].Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    strAlert[0] += strArgs[24] + Strings.HOSTNAME_ALERT;
                    strAlertBool[0] = true;
                }
                //If model Json file does exist and the Media Operation is incorrect
                if (biosJsonStr != null && biosJsonStr[3].Equals("false"))
                {
                    pass = false;
                    strAlert[1] += strArgs[31] + Strings.MEDIA_OPERATION_ALERT;
                    strAlertBool[1] = true;
                }
                //The section below contains the exception cases for Secure Boot enforcement
                if (strArgs[32].Equals(ConstantsDLL.Properties.Strings.deactivated) &&
                    !strArgs[30].Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU1) &&
                    !strArgs[30].Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU2))
                {
                    pass = false;
                    strAlert[2] += strArgs[32] + Strings.SECURE_BOOT_ALERT;
                    strAlertBool[2] = true;
                }
                //If model Json file does not exist and server is unreachable
                if (biosJsonStr == null)
                {
                    pass = false;
                    strAlert[3] += Strings.DATABASE_REACH_ERROR;
                    strAlertBool[3] = true;
                }
                //If model Json file does exist and BIOS/UEFI version is incorrect
                if (biosJsonStr != null && !strArgs[25].Contains(biosJsonStr[0]))
                {
                    if (!biosJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        strAlert[4] += strArgs[25] + Strings.BIOS_VERSION_ALERT;
                        strAlertBool[4] = true;
                    }
                }
                //If model Json file does exist and firmware type is incorrect
                if (biosJsonStr != null && biosJsonStr[1].Equals("false"))
                {
                    pass = false;
                    strAlert[5] += strArgs[28] + Strings.FIRMWARE_TYPE_ALERT;
                    strAlertBool[5] = true;
                }
                //If there is no MAC address assigned
                if (strArgs[26] == string.Empty)
                {
                    pass = false;
                    strAlert[6] += strArgs[26] + Strings.NETWORK_ERROR; //Prints a network error
                    strAlert[7] += strArgs[27] + Strings.NETWORK_ERROR; //Prints a network error
                    strAlertBool[6] = true;
                    strAlertBool[7] = true;
                }
                //If Virtualization Technology is disabled
                if (strArgs[33] == ConstantsDLL.Properties.Strings.deactivated)
                {
                    pass = false;
                    strAlert[8] += strArgs[33] + Strings.VT_ALERT;
                    strAlertBool[8] = true;
                }
                //If model Json file does exist and TPM is not enabled
                if (biosJsonStr != null && biosJsonStr[2].Equals("false"))
                {
                    pass = false;
                    strAlert[9] += strArgs[34] + Strings.TPM_ERROR;
                    strAlertBool[9] = true;
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetPhysicalMemoryAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, shows an alert
                if (d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    strAlert[10] += strArgs[21] + Strings.NOT_ENOUGH_MEMORY;
                    strAlertBool[10] = true;
                }
                //If RAM is more than 4GB and OS is x86, shows an alert
                if (d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    strAlert[10] += strArgs[21] + Strings.TOO_MUCH_MEMORY;
                    strAlertBool[10] = true;
                }
                if (pass)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
        }

        //Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables,
        // while reporting the progress to the progressbar
        public void CollectThread()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for PC maker
            strArgs[17] = HardwareInfo.GetBoardMaker();
            if (strArgs[17] == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || strArgs[17] == string.Empty)
            {
                strArgs[17] = HardwareInfo.GetBoardMakerAlt();
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BM, strArgs[17], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for PC model
            strArgs[18] = HardwareInfo.GetModel();
            if (strArgs[18] == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || strArgs[18] == string.Empty)
            {
                strArgs[18] = HardwareInfo.GetModelAlt();
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MODEL, strArgs[18], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for motherboard Serial number
            strArgs[19] = HardwareInfo.GetBoardProductId();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERIALNO, strArgs[19], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for CPU information
            strArgs[20] = HardwareInfo.GetProcessorCores();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PROCNAME, strArgs[20], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for RAM amount and total number of slots
            strArgs[21] = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PM, strArgs[21], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Storage size
            strArgs[22] = HardwareInfo.GetHDSize();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HDSIZE, strArgs[22], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Storage type
            strArgs[29] = HardwareInfo.GetStorageType();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIATYPE, strArgs[29], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Media Operation (IDE/AHCI/NVME)
            strArgs[31] = HardwareInfo.GetStorageOperation();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIAOP, strArgs[31], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for GPU information
            strArgs[30] = HardwareInfo.GetGPUInfo();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_GPUINFO, strArgs[30], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for OS infomation
            strArgs[23] = HardwareInfo.GetOSInformation();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OS, strArgs[23], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Hostname
            strArgs[24] = HardwareInfo.GetComputerName();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HOSTNAME, strArgs[24], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for MAC Address
            strArgs[26] = HardwareInfo.GetMACAddress();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MAC, strArgs[26], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for IP Address
            strArgs[27] = HardwareInfo.GetIPAddress();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_IP, strArgs[27], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for firmware type
            strArgs[28] = HardwareInfo.GetBIOSType();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOSTYPE, strArgs[28], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Secure Boot status
            strArgs[32] = HardwareInfo.GetSecureBoot();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SECBOOT, strArgs[32], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for BIOS version
            strArgs[25] = HardwareInfo.GetComputerBIOS();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOS, strArgs[25], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for VT status
            strArgs[33] = HardwareInfo.GetVirtualizationTechnology();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_VT, strArgs[33], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for TPM status
            strArgs[34] = HardwareInfo.GetTPMStatus();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_TPM, strArgs[34], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
        }

        //Loads webView2 component
        public async Task LoadWebView2()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            CoreWebView2Environment webView2Environment = Environment.Is64BitOperatingSystem
                ? await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version(), Environment.GetEnvironmentVariable("TEMP"))
                : await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version(), Environment.GetEnvironmentVariable("TEMP"));
            await webView2.EnsureCoreWebView2Async(webView2Environment);
            webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView2.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
        }

        //Sends hardware info to the specified server
        public void ServerSendInfo(string[] serverArgs)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[17] + "&modelo=" + serverArgs[18] + "&numeroSerial=" + serverArgs[19] + "&processador=" + serverArgs[20] + "&memoria=" + serverArgs[21] + "&hd=" + serverArgs[22] + "&sistemaOperacional=" + serverArgs[23] + "&nomeDoComputador=" + serverArgs[24] + "&bios=" + serverArgs[25] + "&mac=" + serverArgs[26] + "&ip=" + serverArgs[27] + "&emUso=" + serverArgs[12] + "&etiqueta=" + serverArgs[13] + "&tipo=" + serverArgs[14] + "&tipoFW=" + serverArgs[28] + "&tipoArmaz=" + serverArgs[29] + "&gpu=" + serverArgs[30] + "&modoArmaz=" + serverArgs[31] + "&secBoot=" + serverArgs[32] + "&vt=" + serverArgs[33] + "&tpm=" + serverArgs[34] + "&trocaPilha=" + serverArgs[10] + "&ticketNum=" + serverArgs[11] + "&agent=" + serverArgs[35]);
        }
    }
}
