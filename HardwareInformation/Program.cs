using CommandLine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HardwareInformation
{
	public class Program
    {
        //Command line switch options specification
        public class Options
        {
            [Option("servidor", Required = false, HelpText = "Servidor do sistema de patrimônio", Default = "192.168.76.103")]
            public string Servidor { get; set; }
            [Option("porta", Required = false, HelpText = "Porta do sistema de patrimônio", Default = "8081")]
            public string Porta { get; set; }
            [Option("modo", Required = false, HelpText = "Tipo de serviço realizado", Default = "m")]
            public string TipoDeServico { get; set; }
            [Option("patrimonio", Required = true, HelpText = "Patrimônio do equipamento")]
            public string Patrimonio { get; set; }
            [Option("lacre", Required = false, HelpText = "Lacre do equipamento (se houver)", Default = "")]
            public string Lacre { get; set; }
            [Option("sala", Required = true, HelpText = "Sala onde o equipamento estará")]
            public string Sala { get; set; }
            [Option("predio", Required = true, HelpText = "Prédio onde o equipamento estará")]
            public string Predio { get; set; }
            [Option("ad", Required = false, HelpText = "Cadastrado no Active Directory", Default = "Sim")]
            public string AD { get; set; }
            [Option("padrao", Required = false, HelpText = "Padrão da imagem implantado no equipamento", Default = "Aluno")]
            public string Padrao { get; set; }
            [Option("data", Required = false, HelpText = "Data do serviço realizado", Default = "hoje")]
            public string Data { get; set; }
            [Option("pilha", Required = true, HelpText = "Realizada troca de pilha?")]
            public string Pilha { get; set; }
            [Option("ticket", Required = true, HelpText = "Número do chamado aberto")]
            public string Ticket { get; set; }
            [Option("uso", Required = false, HelpText = "Equipamento em uso?", Default = "Sim")]
            public string Uso { get; set; }
            [Option("etiqueta", Required = true, HelpText = "Equipamento possui etiqueta?")]
            public string Etiqueta { get; set; }
            [Option("tipo", Required = false, HelpText = "Categoria do equipamento", Default = "Desktop")]
            public string TipoHardware { get; set; }
            [Option("usuario", Required = true, HelpText = "Usuário de login")]
            public string Usuario { get; set; }
            [Option("senha", Required = true, HelpText = "Senha de login")]
            public string Senha { get; set; }
        }

        //Passes args to auth method and then to register class, otherwise informs auth error and closes the program
        public static void RunOptions(Options opts)
        {
            string[] str = LoginFileReader.fetchInfo(opts.Usuario, opts.Senha, opts.Servidor, opts.Porta);
            if (str[0] == "true")
                Application.Run(new CLIRegister(opts.Servidor, opts.Porta, opts.TipoDeServico, opts.Patrimonio, opts.Lacre, opts.Sala, opts.Predio, opts.AD, opts.Padrao, opts.Data, opts.Pilha, opts.Ticket, opts.Uso, opts.Etiqueta, opts.TipoHardware));
            else
            {
                Console.WriteLine("Erro de autenticação! Saindo do programa...");
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

                //Parser.Default.ParseArguments<Options>(args)
                //   .WithParsed<Options>(o =>
                //   {
                //       if (!o.Servidor.Equals("--servidor"))
                //       {
                //           Console.WriteLine($"IP do servidor.");
                //           Console.WriteLine("Deve ter entre 7 e 15 caracteres");
                //       }
                //       else if (!o.Porta.Equals(""))
                //       {
                //           Console.WriteLine($"Porta do servidor.");
                //           Console.WriteLine("Deve ter entre 0 e 4 digitos");
                //       }
                //       else if (!o.TipoDeServico.Equals(""))
                //       {
                //           Console.WriteLine($"Tipo de serviço.");
                //           Console.WriteLine("Digite 'f' ou 'F' para formatação ou 'm' ou 'M' para manutenção");
                //       }
                //       else if (!o.Patrimonio.Equals(""))
                //       {
                //           Console.WriteLine($"Patrimônio do equipamento.");
                //           Console.WriteLine("Deve ter entre 1 e 6 dígitos");
                //       }
                //       else if (!o.Lacre.Equals(""))
                //       {
                //           Console.WriteLine($"Lacre do equipamento (se houver).");
                //           Console.WriteLine("Deve ter entre 0 e 10 dígitos");
                //       }
                //       else if (!o.Sala.Equals(""))
                //       {
                //           Console.WriteLine($"Sala onde equipamento será instalado.");
                //           Console.WriteLine("Deve ter entre 1 e 4 dígitos");
                //       }
                //       else if (!o.Predio.Equals(""))
                //       {
                //           Console.WriteLine($"Prédio onde equipamento será instalado.");
                //           Console.WriteLine("Valores aceitos: 21, 67, 74A, 74B, 74C, 74D, AR");
                //       }
                //       else if (!o.AD.Equals(""))
                //       {
                //           Console.WriteLine($"Equipamento está cadastrado no Active Directory?.");
                //           Console.WriteLine("Valores aceitos: Sim, Não");
                //       }
                //       else if (!o.Padrao.Equals(""))
                //       {
                //           Console.WriteLine($"Padrão da imagem instalada no computador.");
                //           Console.WriteLine("Valores aceitos: Aluno, Funcionario");
                //       }
                //       else if (!o.Data.Equals(""))
                //       {
                //           Console.WriteLine($"Data do serviço.");
                //           Console.WriteLine("Valores aceitos: hoje, DD/MM/YYYY");
                //       }
                //       else if (!o.Pilha.Equals(""))
                //       {
                //           Console.WriteLine($"Houve troca de pilha?");
                //           Console.WriteLine("Valores aceitos: \"C/ troca de pilha\", \"S/ troca de pilha\"");
                //       }
                //       else if (!o.Ticket.Equals(""))
                //       {
                //           Console.WriteLine($"Número do chamado aberto.");
                //           Console.WriteLine("Deve ter entre 1 e 6 dígitos");
                //       }
                //       else if (!o.Uso.Equals(""))
                //       {
                //           Console.WriteLine($"Computador está em uso atualmente?");
                //           Console.WriteLine("Valores aceitos: Sim, Não");
                //       }
                //       else if (!o.Etiqueta.Equals(""))
                //       {
                //           Console.WriteLine($"Computador possui etiqueta?");
                //           Console.WriteLine("Valores aceitos: Sim, Não");
                //       }
                //       else if (!o.TipoHardware.Equals(""))
                //       {
                //           Console.WriteLine($"Formato do computador");
                //           Console.WriteLine("Valores aceitos: Desktop, Notebook, Tablet");
                //       }
                //       else if (!o.Usuario.Equals(""))
                //       {
                //           Console.WriteLine($"Nome de usuário para login");
                //           Console.WriteLine("Deve ser uma cadeia de caracteres");
                //       }
                //       else if (!o.Senha.Equals(""))
                //       {
                //           Console.WriteLine($"Senha do usuário para login");
                //           Console.WriteLine("Deve ser uma cadeia de caracteres");
                //       }
                //       else
                //       {
                //           Console.WriteLine($"Opção desconhecida");
                //       }
                //   });
            }
        }
    }
}
