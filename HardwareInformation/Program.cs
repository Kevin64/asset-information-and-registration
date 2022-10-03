using CommandLine;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConstantsDLL;
using JsonFileReaderDLL;

namespace HardwareInformation
{
	public class Program
    {
        //Command line switch options specification
        public class Options
        {
            [Option("servidor", Required = false, HelpText = StringsAndConstants.cliHelpTextServer, Default = "192.168.76.103")]
            public string Servidor { get; set; }
            [Option("porta", Required = false, HelpText = StringsAndConstants.cliHelpTextPort, Default = "8081")]
            public string Porta { get; set; }
            [Option("modo", Required = false, HelpText = StringsAndConstants.cliHelpTextMode, Default = "m")]
            public string TipoDeServico { get; set; }
            [Option("patrimonio", Required = true, HelpText = StringsAndConstants.cliHelpTextPatrimony)]
            public string Patrimonio { get; set; }
            [Option("lacre", Required = false, HelpText = StringsAndConstants.cliHelpTextSeal, Default = "")]
            public string Lacre { get; set; }
            [Option("sala", Required = true, HelpText = StringsAndConstants.cliHelpTextRoom)]
            public string Sala { get; set; }
            [Option("predio", Required = true, HelpText = StringsAndConstants.cliHelpTextBuilding)]
            public string Predio { get; set; }
            [Option("ad", Required = false, HelpText = StringsAndConstants.cliHelpTextActiveDirectory, Default = "Sim")]
            public string AD { get; set; }
            [Option("padrao", Required = false, HelpText = StringsAndConstants.cliHelpTextStandard, Default = "Aluno")]
            public string Padrao { get; set; }
            [Option("data", Required = false, HelpText = StringsAndConstants.cliHelpTextDate, Default = "hoje")]
            public string Data { get; set; }
            [Option("pilha", Required = true, HelpText = StringsAndConstants.cliHelpTextBattery)]
            public string Pilha { get; set; }
            [Option("ticket", Required = true, HelpText = StringsAndConstants.cliHelpTextTicket)]
            public string Ticket { get; set; }
            [Option("uso", Required = false, HelpText = StringsAndConstants.cliHelpTextInUse, Default = "Sim")]
            public string Uso { get; set; }
            [Option("etiqueta", Required = true, HelpText = StringsAndConstants.cliHelpTextTag)]
            public string Etiqueta { get; set; }
            [Option("tipo", Required = false, HelpText = StringsAndConstants.cliHelpTextType, Default = "Desktop")]
            public string TipoHardware { get; set; }
            [Option("usuario", Required = true, HelpText = StringsAndConstants.cliHelpTextUser)]
            public string Usuario { get; set; }
            [Option("senha", Required = true, HelpText = StringsAndConstants.cliHelpTextPassword)]
            public string Senha { get; set; }
        }

        //Passes args to auth method and then to register class, otherwise informs auth error and closes the program
        public static async void RunOptions(Options opts)
        {
            string[] str = await LoginFileReader.fetchInfo(opts.Usuario, opts.Senha, opts.Servidor, opts.Porta);
            if (str[0] == "true")
                Application.Run(new CLIRegister(opts.Servidor, opts.Porta, opts.TipoDeServico, opts.Patrimonio, opts.Lacre, opts.Sala, opts.Predio, opts.AD, opts.Padrao, opts.Data, opts.Pilha, opts.Ticket, opts.Uso, opts.Etiqueta, opts.TipoHardware, opts.Usuario));
            else
            {
                Console.WriteLine(StringsAndConstants.AUTH_ERROR);
                Application.Exit();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                FreeConsole();
                Application.Run(new Form2()); //If given no args, runs Form2 (login)
            }
            else
            {
                //If given args, parses them
                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(RunOptions);
            }
        }
    }
}
