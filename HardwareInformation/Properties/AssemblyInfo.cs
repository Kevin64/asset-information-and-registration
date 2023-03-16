﻿using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Coleta de Hardware e Cadastro de Patrimônio")]
[assembly: AssemblyDescription("Software criado, desenvolvido e mantido por Kevin Costa. Programado em C# utilizando .NET Framework 4.8. Sistemas com suporte: Windows 7, 10 e 11.\r\n" +
    "\r\n" +
    "    • URL do projeto: https://github.com/Kevin64/HardwareInformation\r\n" +
    "    • Licença: (MIT) https://github.com/Kevin64/HardwareInformation/blob/master/LICENCE\r\n" +
    "\r\n" +
    "Este software deve ser usado em conjunto com projeto SCPD para funcionar corretamente.\r\n" +
    "\r\n" +
    "► Sistema de Controle de Patrimônio e Docentes - SCPD\r\n" +
    "    • URL do projeto: https://github.com/Kevin64/Sistema-de-controle-de-patrimonio-e-docentes\r\n" +
    "    • Licença: (MIT) https://github.com/Kevin64/Sistema-de-controle-de-patrimonio-e-docentes/blob/main/LICENCE\r\n" +
    "\r\n" +
    "Este software e suas bibliotecas (DLLs) utilizam artes, bibliotecas Open Source e códigos avulsos de terceiros, listados abaixo. Todos os créditos vão para os seus respectivos criadores e mantenedores:\r\n" +
    "\r\n" +
    "► Configurable Quality PictureBox (créditos a Jason D)\r\n" +
    "    • URL do projeto: https://stackoverflow.com/a/1774592/16838132\r\n" +
    "\r\n" +
    "► Custom Flat ComboBox (créditos a Reza Aghaei)\r\n" +
    "    • URL do projeto: https://stackoverflow.com/a/65976649/16838132\r\n" +
    "\r\n" +
    "► CommandLineParser\r\n" +
    "    • Copyright (c) 2005 - 2015 Giacomo Stelluti Scala & Contributors\r\n" +
    "    • URL do projeto: https://github.com/commandlineparser/commandline\r\n" +
    "    • Licença: (MIT) https://github.com/commandlineparser/commandline/blob/master/License.md\r\n" +
    "\r\n" +
    "► ini-parser\r\n" +
    "    • Copyright (c) 2008 Ricardo Amores Hernández\r\n" +
    "    • URL do projeto: https://github.com/rickyah/ini-parser \r\n" +
    "    • Licença: (MIT) https://github.com/rickyah/ini-parser/blob/master/LICENSE\r\n" +
    "\r\n" +
    "► Microsoft.Web.WebView2\r\n" +
    "    • Copyright (C) Microsoft Corporation. All rights reserved.\r\n" +
    "    • URL do projeto: https://learn.microsoft.com/pt-br/microsoft-edge/webview2/\r\n" +
    "    • Licença: https://www.nuget.org/packages/Microsoft.Web.WebView2/1.0.1518.46/license\r\n" +
    "\r\n" +
    "► WindowsAPICodePack\r\n" +
    "    • Copyright (C) Microsoft Corporation. All rights reserved.\r\n" +
    "    • URL do projeto: https://github.com/aybe/Windows-API-Code-Pack-1.1\r\n" +
    "    • Licença: https://github.com/aybe/Windows-API-Code-Pack-1.1/blob/master/LICENCE\r\n" +
    "\r\n" +
    "► PowerShellStandard.Library\r\n" +
    "    • Copyright (c) Microsoft Corporation.\r\n" +
    "    • URL do projeto: https://github.com/PowerShell/PowerShellStandard\r\n" +
    "    • Licença: (MIT) https://github.com/PowerShell/PowerShell/blob/master/LICENSE.txt\r\n" +
    "\r\n" +
    "► LoadingCircle (créditos a Martin Gagne)\r\n" +
    "    • URL do projeto: https://www.codeproject.com/Articles/14841/How-to-write-a-loading-circle-animation-in-NET\r\n" +
    "    • Licença: (CPOL) https://www.codeproject.com/info/cpol10.aspx\r\n" +
    "\r\n" +
    "► NewtonsoftJson\r\n" +
    "    • Copyright (c) 2007 James Newton-King\r\n" +
    "    • URL do projeto: https://github.com/JamesNK/Newtonsoft.Json\r\n" +
    "    • Licença: (MIT) https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md\r\n" +
    "\r\n" +
    "► BCrypt.Net-Next\r\n" +
    "    • Copyright (c) 2006 Damien Miller djm@mindrot.org (jBCrypt)\r\n" +
    "    • Copyright (c) 2013 Ryan D. Emerle (.Net port)\r\n" +
    "    • Copyright (c) 2016/2021 Chris McKee (.Net-core port / patches)\r\n" +
    "    • URL do projeto: https://github.com/BcryptNet/bcrypt.net\r\n" +
    "    • Licença: (MIT) https://github.com/BcryptNet/bcrypt.net/blob/main/licence.txt\r\n" +
    "\r\n" +
    "► DarkNet (créditos a Ben Hutchison)\r\n" +
    "    • URL do projeto: https://github.com/Aldaviva/DarkNet\r\n" +
    "    • Licença: (Apache-2.0) https://github.com/Aldaviva/DarkNet/blob/master/License.txt\r\n" +
    "\r\n" +
    "► fluentui-system-icons\r\n" +
    "    • Copyright (c) 2020 Microsoft Corporation.\r\n" +
    "    • URL do projeto: https://github.com/microsoft/fluentui-system-icons\r\n" +
    "    • Licença: (MIT) https://github.com/microsoft/fluentui-system-icons/blob/main/LICENSE\r\n")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Coleta de Hardware e Cadastro de Patrimônio")]
[assembly: AssemblyCopyright("")]
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
[assembly: NeutralResourcesLanguage("pt-BR")]
#else
[assembly: AssemblyConfiguration("Release")]
[assembly: NeutralResourcesLanguage("pt-BR")]
#endif