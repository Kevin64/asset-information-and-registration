﻿using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    public class CLIRegister : Form
    {
        public const int MAX_SIZE = 100;
        public const string WEBVIEW2_PATH = "runtimes\\win-x86";
        public string[] strArgs;
        public WebView2 webView2;
        public List<string> listPredio, listModo, listAD, listPadrao, listUso, listEtiq, listTipo, listPilha, listServer, listPorta;

        //Basic form for WebView2
        private void InitializeComponent()
        {
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
            this.Controls.Add(this.webView2);
            this.Name = "CLIRegister";
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
                serverSendInfo(strArgs);
                webView2.NavigationCompleted += webView2_NavigationCompleted;
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
                webView2.Dispose();
                Application.Exit();
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
