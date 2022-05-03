using HardwareInformation;
using System;
using System.Windows.Forms;

public class Program : Form
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			//Runs Form2 (login)
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form2());
		}
		else
        {
			Application.Run(new Form2(args));
        }
	}
}