using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Coleta de Hardware e Cadastro de Patrimônio")]
[assembly: AssemblyDescription("Software criado, desenvolvido e mantido por Kevin Costa. Programado em C# utilizando .NET Framework 4.8. Sistemas com suporte: Windows 7, 10 e 11.\r\n" +
    "\r\n" +
    "    • Copyright © 2023 Kevin Vinícius Teixeira Costa\r\n" +
    "    • URL do projeto: https://github.com/Kevin64/HardwareInformation\r\n" +
    "    • Licença: (MIT) https://github.com/Kevin64/HardwareInformation/blob/master/LICENCE\r\n" +
    "\r\n" +
    "Este software deve ser usado em conjunto com projeto SCPD para funcionar corretamente.\r\n" +
    "\r\n" +
    "► Sistema de Controle de Patrimônio e Docentes - SCPD\r\n" +
    "    • URL do projeto: https://github.com/Kevin64/Sistema-de-controle-de-patrimonio-e-docentes\r\n" +
    "    • Licença: (MIT) https://github.com/Kevin64/Sistema-de-controle-de-patrimonio-e-docentes/blob/main/LICENCE\r\n" +
    "\r\n" +
    "Este software e suas bibliotecas (DLLs) utilizam artes, bibliotecas Open Source e códigos avulsos de terceiros. Todos os créditos vão para os seus respectivos criadores e mantenedores. Estes componentes estão listados no README na página do projeto no GitHub: https://github.com/Kevin64/Dependencies/blob/main/README.md\r\n")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Coleta de Hardware e Cadastro de Patrimônio")]
[assembly: AssemblyCopyright("Copyright © 2023 Kevin Vinícius Teixeira Costa")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a7afe7a2-f74a-400e-9dfa-02bc6a8e4f9b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.0.0.0")]
[assembly: AssemblyFileVersion("4.0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif