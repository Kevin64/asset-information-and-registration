using HardwareInformation;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Program : Form
{
    private Microsoft.Web.WebView2.WinForms.WebView2 webView2_1;

    private void InitializeComponent()
    {
        this.webView2_1 = new Microsoft.Web.WebView2.WinForms.WebView2();
        ((System.ComponentModel.ISupportInitialize)(this.webView2_1)).BeginInit();
        this.SuspendLayout();
        // 
        // webView2_1
        // 
        this.webView2_1.CreationProperties = null;
        this.webView2_1.DefaultBackgroundColor = System.Drawing.Color.White;
        this.webView2_1.Location = new System.Drawing.Point(92, 73);
        this.webView2_1.Name = "webView2_1";
        this.webView2_1.Size = new System.Drawing.Size(75, 23);
        this.webView2_1.TabIndex = 0;
        this.webView2_1.ZoomFactor = 1D;
        // 
        // Program
        // 
        this.ClientSize = new System.Drawing.Size(284, 261);
        this.Controls.Add(this.webView2_1);
        this.Name = "Program";
        ((System.ComponentModel.ISupportInitialize)(this.webView2_1)).EndInit();
        this.ResumeLayout(false);

    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static async Task Main()
    {
//        string[] args = Environment.GetCommandLineArgs();
//        if (args == null)
//        {
            //Runs Form2 (login)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
//        }
//        else
//        {
            /*
            Application.Run(new Program());
            var webView2Environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync("runtimes\\win-x86", System.IO.Path.GetTempPath());
            webView2_1.EnsureCoreWebView2Async(webView2Environment);

            webView2_1.CoreWebView2.Navigate("http://" + args[1] + "/recebeDadosManutencao.php?patrimonio=" + args[1] + "&lacre=" + args[1] +
                        "&sala=" + args[1] + "&predio=" + args[1] + "&ad=" + args[1] + "&padrao=" + args[1] + "&formatacao=" + DateTime.Now.ToString() + "&formatacoesAnteriores=" + DateTime.Now.ToString() +
                        "&marca=" + HardwareInfo.GetBoardMaker() + "&modelo=" + HardwareInfo.GetModel() + "&numeroSerial=" + HardwareInfo.GetBoardProductId() + "&processador=" + HardwareInfo.GetProcessorCores() + "&memoria=" + HardwareInfo.GetPhysicalMemory() +
                        "&hd=" + HardwareInfo.GetHDSize() + "&sistemaOperacional=" + HardwareInfo.GetOSInformation() + "&nomeDoComputador=" + HardwareInfo.GetComputerName() + "&bios=" + HardwareInfo.GetComputerBIOS() + "&mac=" + HardwareInfo.GetMACAddress() + "&ip=" + HardwareInfo.GetIPAddress() + "&emUso=" + args[1] +
                        "&etiqueta=" + args[1] + "&tipo=" + args[1] + "&tipoFW=" + HardwareInfo.GetBIOSType() + "&tipoArmaz=" + HardwareInfo.GetStorageType() + "&gpu=" + HardwareInfo.GetGPUInfo() + "&modoArmaz=" + HardwareInfo.GetStorageOperation() +
                        "&secBoot=" + HardwareInfo.GetSecureBoot());
            
        }*/
    }
}