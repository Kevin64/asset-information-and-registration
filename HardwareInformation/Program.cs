using CommandLine;
using System;
using System.Windows.Forms;

namespace HardwareInformation
{
	public class Program
    {
		//public class Options
		//{
		//	[Option('s', "servidor", Required = false, HelpText = "Servidor do sistema de patrimônio")]
		//	public bool Servidor { get; set; }
  //          [Option('p', "porta", Required = false, HelpText = "Porta do sistema de patrimônio")]
  //          public bool Porta { get; set; }
  //          [Option('m', "modo", Required = false, HelpText = "Tipo de serviço")]
  //          public bool TipoDeServico { get; set; }
  //          [Option('a', "patrimonio", Required = false, HelpText = "Patrimônio")]
  //          public bool Patrimonio { get; set; }
  //          [Option('l', "lacre", Required = false, HelpText = "Lacre (se houver)")]
  //          public bool Lacre { get; set; }
  //          [Option('w', "sala", Required = false, HelpText = "Sala")]
  //          public bool Sala { get; set; }
  //          [Option('r', "predio", Required = false, HelpText = "Prédio")]
  //          public bool Predio { get; set; }
  //          [Option('c', "ad", Required = false, HelpText = "Cadastrado no Active Directory")]
  //          public bool AD { get; set; }
  //          [Option('f', "padrao", Required = false, HelpText = "Padrão da imagem")]
  //          public bool Padrao { get; set; }
  //          [Option('d', "data", Required = false, HelpText = "Data do serviço")]
  //          public bool Data { get; set; }
  //          [Option('i', "pilha", Required = false, HelpText = "Troca de pilha?")]
  //          public bool Pilha { get; set; }
  //          [Option('t', "ticket", Required = false, HelpText = "Número do chamado")]
  //          public bool Ticket { get; set; }
  //          [Option('o', "uso", Required = false, HelpText = "Em uso?")]
  //          public bool Uso { get; set; }
  //          [Option('e', "etiqueta", Required = false, HelpText = "Possui etiqueta")]
  //          public bool Etiqueta { get; set; }
  //          [Option('k', "tipo", Required = false, HelpText = "Categoria do equipamento")]
  //          public bool TipoHardware { get; set; }
  //          [Option('u', "usuario", Required = false, HelpText = "Usuário de login")]
  //          public bool Usuario { get; set; }
  //          [Option('q', "senha", Required = false, HelpText = "Senha de login")]
  //          public bool Senha { get; set; }
  //      }
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
				//Runs Form2 (login)				
				Application.Run(new Form2());
			}
			//else
			//{
   //             Parser.Default.ParseArguments<Options>(args)
   //                .WithParsed<Options>(o =>
   //                {
   //                    if (o.Servidor)
   //                    {
   //                        Console.WriteLine($"IP do servidor.");
   //                        Console.WriteLine("Deve ter entre 7 e 15 caracteres");
   //                    }
   //                    else if (o.Porta)
   //                    {
   //                        Console.WriteLine($"Porta do servidor.");
   //                        Console.WriteLine("Deve ter entre 0 e 4 digitos");
   //                    }
   //                    else if (o.TipoDeServico)
   //                    {
   //                        Console.WriteLine($"Tipo de serviço.");
   //                        Console.WriteLine("Digite 'f' ou 'F' para formatação ou 'm' ou 'M' para manutenção");
   //                    }
   //                    else if (o.Patrimonio)
   //                    {
   //                        Console.WriteLine($"Patrimônio do equipamento.");
   //                        Console.WriteLine("Deve ter entre 1 e 6 dígitos");
   //                    }
   //                    else if (o.Lacre)
   //                    {
   //                        Console.WriteLine($"Lacre do equipamento (se houver).");
   //                        Console.WriteLine("Deve ter entre 0 e 10 dígitos");
   //                    }
   //                    else if (o.Sala)
   //                    {
   //                        Console.WriteLine($"Sala onde equipamento será instalado.");
   //                        Console.WriteLine("Deve ter entre 1 e 4 dígitos");
   //                    }
   //                    else if (o.Predio)
   //                    {
   //                        Console.WriteLine($"Prédio onde equipamento será instalado.");
   //                        Console.WriteLine("Valores aceitos: 21, 67, 74A, 74B, 74C, 74D, AR");
   //                    }
   //                    else if (o.AD)
   //                    {
   //                        Console.WriteLine($"Equipamento está cadastrado no Active Directory?.");
   //                        Console.WriteLine("Valores aceitos: Sim, Não");
   //                    }
   //                    else if (o.Padrao)
   //                    {
   //                        Console.WriteLine($"Padrão da imagem instalada no computador.");
   //                        Console.WriteLine("Valores aceitos: Aluno, Funcionario");
   //                    }
   //                    else if (o.Data)
   //                    {
   //                        Console.WriteLine($"Data do serviço.");
   //                        Console.WriteLine("Valores aceitos: hoje, DD/MM/YYYY");
   //                    }
   //                    else if (o.Pilha)
   //                    {
   //                        Console.WriteLine($"Houve troca de pilha?");
   //                        Console.WriteLine("Valores aceitos: \"C/ troca de pilha\", \"S/ troca de pilha\"");
   //                    }
   //                    else if (o.Ticket)
   //                    {
   //                        Console.WriteLine($"Número do chamado aberto.");
   //                        Console.WriteLine("Deve ter entre 1 e 6 dígitos");
   //                    }
   //                    else if (o.Uso)
   //                    {
   //                        Console.WriteLine($"Computador está em uso atualmente?");
   //                        Console.WriteLine("Valores aceitos: Sim, Não");
   //                    }
   //                    else if (o.Etiqueta)
   //                    {
   //                        Console.WriteLine($"Computador possui etiqueta?");
   //                        Console.WriteLine("Valores aceitos: Sim, Não");
   //                    }
   //                    else if (o.TipoHardware)
   //                    {
   //                        Console.WriteLine($"Formato do computador");
   //                        Console.WriteLine("Valores aceitos: Desktop, Notebook, Tablet");
   //                    }
   //                    else if (o.Usuario)
   //                    {
   //                        Console.WriteLine($"Nome de usuário para login");
   //                        Console.WriteLine("Deve ser uma cadeia de caracteres");
   //                    }
   //                    else if (o.Senha)
   //                    {
   //                        Console.WriteLine($"Senha do usuário para login");
   //                        Console.WriteLine("Deve ser uma cadeia de caracteres");
   //                    }
   //                    else
   //                    {
   //                        Console.WriteLine($"Opção desconhecida");
   //                    }
   //                });

   //             if (MiscMethods.Authenticate(args[15], args[16]))
   //                 Application.Run(new CLIRegister(args));
   //             else
   //                 Console.WriteLine("erro de autenticação! saindo do programa...");
            //}
		}
    }
}
