using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    public class CLIRegister : Form
    {
        private string fileBios = "bios.json";
        private string fileLogin = "login.json";
        public bool pass;
        public const int MAX_SIZE = 100;
        public const string WEBVIEW2_PATH = "runtimes\\win-x86";
        private const string DEFAULT_HOSTNAME = "MUDAR-NOME";
        private const string HOSTNAME_ALERT = " (Nome incorreto, alterar)";
        private const string MEDIA_OPERATION_NVME = "NVMe";
        private const string MEDIA_OPERATION_IDE_RAID = "IDE/Legacy ou RAID";
        private const string MEDIA_OPERATION_ALERT = " (Modo de operação incorreto, alterar)";
        private const string SECURE_BOOT_ALERT = " (Ativar boot seguro)";
        private const string DATABASE_REACH_ERROR = "Erro ao contatar o banco de dados, verifique a sua conexão com a intranet e se o servidor web está ativo!";
        private const string BIOS_VERSION_ALERT = " (Atualizar BIOS/UEFI)";
        private const string FIRMWARE_TYPE_ALERT = " (PC suporta UEFI, fazer a conversão do sistema)";
        private const string NETWORK_ERROR = "Computador sem conexão com a Intranet";
        private const string VT_ALERT = " (Ativar Tecnologia de Virtualização na BIOS/UEFI)";
        public string[] strArgs, strAlert;
        public bool[] strAlertBool;
        public WebView2 webView2;
        public List<string> listPredio, listModo, listAD, listPadrao, listUso, listEtiq, listTipo, listPilha, listServer, listPorta;

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
            strAlert[0] = "Hostname: ";
            strAlert[1] = "Modo de operação SATA/M.2: ";
            strAlert[2] = "Secure Boot: ";
            strAlert[3] = "Conectividade com o banco de dados: ";
            strAlert[4] = "Versão da BIOS/UEFI: ";
            strAlert[5] = "Tipo de firmware: ";
            strAlert[6] = "Endereço IP: ";
            strAlert[7] = "Endereço MAC: ";
            strAlert[8] = "Tecnologia de Virtualização: ";

            listPredio = new List<string>();
            listModo = new List<string>();
            listAD = new List<string>();
            listPadrao = new List<string>();
            listUso = new List<string>();
            listEtiq = new List<string>();
            listTipo = new List<string>();
            listPilha = new List<string>();
            listServer = new List<string>();
            listPorta = new List<string>();
            webView2 = new WebView2();

            listPredio.Add("21");
            listPredio.Add("67");
            listPredio.Add("74A");
            listPredio.Add("74B");
            listPredio.Add("74C");
            listPredio.Add("74D");
            listPredio.Add("AR");
            listModo.Add("f");
            listModo.Add("F");
            listModo.Add("m");
            listModo.Add("M");
            listAD.Add("Não");
            listAD.Add("Sim");
            listPadrao.Add("Aluno");
            listPadrao.Add("Funcionário");
            listUso.Add("Não");
            listUso.Add("Sim");
            listEtiq.Add("Não");
            listEtiq.Add("Sim");
            listTipo.Add("Desktop");
            listTipo.Add("Notebook");
            listTipo.Add("Tablet");
            listPilha.Add("C/ troca de pilha");
            listPilha.Add("S/ troca de pilha");
            listServer.Add("192.168.76.103");
            listPorta.Add("8081");
            await loadWebView2();

            if (strArgs[0].Length <= 15 && strArgs[0].Length > 6 &&
                strArgs[1].Length <= 5 &&
                listModo.Contains(strArgs[2]) &&
                strArgs[3].Length <= 6 && strArgs[3].Length > 0 &&
                strArgs[4].Length <= 10 &&
                strArgs[5].Length <= 4 && strArgs[5].Length > 0 &&
                listPredio.Contains(strArgs[6]) &&
                listAD.Contains(strArgs[7]) &&
                listPadrao.Contains(strArgs[8]) &&
                (strArgs[9].Length == 10 || strArgs[9].Equals("hoje")) &&
                listPilha.Contains(strArgs[10]) &&
                strArgs[11].Length <= 6 &&
                listUso.Contains(strArgs[12]) &&
                listEtiq.Contains(strArgs[13]) &&
                listTipo.Contains(strArgs[14]))
            {
                if (strArgs[2].Equals("f") || strArgs[2].Equals("F"))
                    strArgs[2] = "recebeDadosFormatacao";
                else if(strArgs[2].Equals("m") || strArgs[2].Equals("M"))
                    strArgs[2] = "recebeDadosManutencao";

                if (strArgs[9].Equals("hoje"))
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
                    Console.WriteLine("Corrija o problemas a seguir antes de prosseguir:");
                    for (int i = 0; i < strAlert.Length; i++)
                    {
                        if (strAlertBool[i])
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(strAlert[i]);
                            Console.ResetColor();
                        }
                    }
                    File.Delete(@fileBios);
                    File.Delete(@fileLogin);
                    webView2.Dispose();
                    Application.Exit();
                }
            }
            else
            {
                Console.WriteLine("Um ou mais argumentos contém erros! Saindo do programa...");
                webView2.Dispose();
                Application.Exit();
            }
        }

        //Allocates WebView2 runtime
        public void webView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if(e.IsSuccess)
            {
                File.Delete(@fileBios);
                File.Delete(@fileLogin);
                webView2.Dispose();
                Application.Exit();
            }
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private void printHardwareData()
        {
            pass = true;
            string[] str = BIOSFileReader.fetchInfo(strArgs[17], strArgs[18], strArgs[28], strArgs[0], strArgs[1]);

            if (strArgs[24].Equals(DEFAULT_HOSTNAME))
            {
                pass = false;
                strAlert[0] += strArgs[24] + HOSTNAME_ALERT;
                strAlertBool[0] = true;
            }
            //The section below contains the exception cases for AHCI enforcement
            if (!strArgs[18].Contains("7057") &&
                !strArgs[18].Contains("8814") &&
                !strArgs[18].Contains("6078") &&
                !strArgs[18].Contains("560s") &&
                Environment.Is64BitOperatingSystem &&
                strArgs[31].Equals(MEDIA_OPERATION_IDE_RAID))
            {
                if (strArgs[18].Contains("A315-56"))
                {
                    strArgs[31] = MEDIA_OPERATION_NVME;
                }
                else
                {
                    pass = false;
                    strAlert[1] += strArgs[31] + MEDIA_OPERATION_ALERT;
                    strAlertBool[1] = true;
                }
            }
            //The section below contains the exception cases for Secure Boot enforcement
            if (strArgs[32].Equals("Desativado") &&
                !strArgs[30].Contains("210") &&
                !strArgs[30].Contains("430"))
            {
                pass = false;
                strAlert[2] += strArgs[32] + SECURE_BOOT_ALERT;
                strAlertBool[2] = true;
            }
            if (str == null)
            {
                pass = false;
                strAlert[3] += DATABASE_REACH_ERROR;
                strAlertBool[3] = true;
            }
            if (str != null && !strArgs[25].Contains(str[0]))
            {
                if (!str[0].Equals("-1"))
                {
                    pass = false;
                    strAlert[4] += strArgs[25] + BIOS_VERSION_ALERT;
                    strAlertBool[4] = true;
                }
            }
            if (str != null && str[1].Equals("false"))
            {
                pass = false;
                strAlert[5] += strArgs[28] + FIRMWARE_TYPE_ALERT;
                strAlertBool[5] = true;
            }
            if (strArgs[26] == "")
            {
                pass = false;
                strAlert[6] += strArgs[26] + NETWORK_ERROR;
                strAlert[7] += strArgs[27] + NETWORK_ERROR;
                strAlertBool[6] = true;
                strAlertBool[7] = true;
            }
            if (strArgs[33] == "Desativado")
            {
                pass = false;
                strAlert[8] += strArgs[33] + VT_ALERT;
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
            CoreWebView2Environment webView2Environment = await CoreWebView2Environment.CreateAsync(WEBVIEW2_PATH, System.IO.Path.GetTempPath());
            await webView2.EnsureCoreWebView2Async(webView2Environment);
        }

        //Sends hardware info to the specified server
        public void serverSendInfo(string[] serverArgs)
        {
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[17] + "&modelo=" + serverArgs[18] + "&numeroSerial=" + serverArgs[19] + "&processador=" + serverArgs[20] + "&memoria=" + serverArgs[21] + "&hd=" + serverArgs[22] + "&sistemaOperacional=" + serverArgs[23] + "&nomeDoComputador=" + serverArgs[24] + "&bios=" + serverArgs[25] + "&mac=" + serverArgs[26] + "&ip=" + serverArgs[27] + "&emUso=" + serverArgs[12] + "&etiqueta=" + serverArgs[13] + "&tipo=" + serverArgs[14] + "&tipoFW=" + serverArgs[28] + "&tipoArmaz=" + serverArgs[29] + "&gpu=" + serverArgs[30] + "&modoArmaz=" + serverArgs[31] + "&secBoot=" + serverArgs[32] + "&vt=" + serverArgs[33] + "&tpm=" + serverArgs[34] + "&trocaPilha=" + serverArgs[10] + "&ticketNum=" + serverArgs[11]);
        }
    }
}
