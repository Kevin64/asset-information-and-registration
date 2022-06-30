using System.Collections.Generic;
using System.Drawing;

namespace HardwareInformation
{
    internal static class StringsAndConstants
    {
        internal const string cliHelpTextServer = "Servidor do sistema de patrimônio (Ex.: 192.168.76.103, localhost)";
        internal const string cliHelpTextPort = "Porta do sistema de patrimônio (Ex.: 8081)";
        internal const string cliHelpTextMode = "Tipo de serviço realizado (Valores possíveis: F/f para formatação, M/m para manutenção)";
        internal const string cliHelpTextPatrimony = "Patrimônio do equipamento (Ex.: 123456)";
        internal const string cliHelpTextSeal = "Lacre do equipamento (se houver) (Ex.: 12345678)";
        internal const string cliHelpTextRoom = "Sala onde o equipamento estará localizado (Ex.: 1234)";
        internal const string cliHelpTextBuilding = "Prédio onde o equipamento estará localizado (Valores possíveis: 21, 67, 74A, 74B, 74C, 74D, AR)";
        internal const string cliHelpTextActiveDirectory = "Cadastrado no Active Directory (Valores possíveis: Sim, Não)";
        internal const string cliHelpTextStandard = "Padrão da imagem implantado no equipamento (Valores possíveis: F/f para funcionário, A/a para aluno)";
        internal const string cliHelpTextDate = "Data do serviço realizado (Valores possíveis: hoje, ou especificar data, ex.: 12/12/2020)";
        internal const string cliHelpTextBattery = "Realizada troca de pilha? (Valores possíveis: Sim, Não)";
        internal const string cliHelpTextTicket = "Número do chamado aberto (Ex.: 123456)";
        internal const string cliHelpTextInUse = "Equipamento em uso? (Valores possíveis: Sim, Não)";
        internal const string cliHelpTextTag = "Equipamento possui etiqueta? (Valores possíveis: Sim, Não)";
        internal const string cliHelpTextType = "Categoria do equipamento (Valores possíveis: Desktop, Notebook, Tablet)";
        internal const string cliHelpTextUser = "Usuário de login";
        internal const string cliHelpTextPassword = "Senha de login";
        internal const string today = "hoje";

        internal const string ok = "OK", activated = "Ativado", deactivated = "Desativado";
        internal const string unknown = "Desconhecido", notSupported = "Não suportado", notDetermined = "Não determinado", notExistant = "Não existente";
        internal const string tb = "TB", gb = "GB", mb = "MB", predFail = "Pred Fail";
        internal const string ahci = "AHCI", nvme = "NVMe", ide = "IDE/Legacy ou RAID", sata = "SATA", raid = "RAID";
        internal const string frequency = "MHz";
        internal const string ddr2 = "DDR2", ddr3 = "DDR3", ddr3smbios = "24", ddr3memorytype = "24", ddr4 = "DDR4", ddr4smbios = "26";
        internal const string systemRom = "SYSTEM ROM", arch32 = "32", arch64 = "64";
        internal const string windows10 = "10", windows8_1 = "8.1", windows8 = "8", windows7 = "7";
        internal const string bios = "BIOS", uefi = "UEFI";
        internal const string hdd = "HDD", ssd = "SSD";
        internal const string build = "build";

        internal const string offlineModeUser = "test";
        internal const string offlineModePassword = "test";
        
        internal const string employee = "Funcionário";
        internal const string student = "Aluno";
        internal const string replacedBattery = "C/ troca de pilha";
        internal const string sameBattery = "S/ troca de pilha";
        internal static readonly List<string> defaultServerIP = new List<string>() { "192.168.76.103", "localhost" };
        internal static readonly List<string> defaultServerPort = new List<string>() { "8081", "80" };
        internal static readonly List<string> listBuilding = new List<string>() { "21", "67", "74A", "74B", "74C", "74D", "AR" };
        internal static readonly List<string> listMode = new List<string>() { "F", "f", "M", "m" };
        internal static readonly List<string> listActiveDirectory = new List<string>() { "Sim", "Não" };
        internal static readonly List<string> listStandard = new List<string>() { "Funcionário", "Aluno" };
        internal static readonly List<string> listInUse = new List<string>() { "Sim", "Não" };
        internal static readonly List<string> listTag = new List<string>() { "Sim", "Não" };
        internal static readonly List<string> listType = new List<string>() { "Desktop", "Notebook", "Tablet" };
        internal static readonly List<string> listBattery = new List<string>() { "Sim", "Não" };

        internal static string loginPath = System.IO.Path.GetTempPath() + fileLogin;
        internal static string biosPath = System.IO.Path.GetTempPath() + fileBios;
        internal const string THEME_REG_PATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
        internal const string HWINFO_REG_PATH = @"Software\HardwareInformation";
        internal const string THEME_REG_KEY = "AppsUseLightTheme";

        internal const string lastInstall = "LastInstallation", lastMaintenance = "LastMaintenance";
        internal const string fileBios = "bios.json", fileLogin = "login.json";
        internal const string fileShaBios = "bios-checksum.txt", fileShaLogin = "login-checksum.txt";
        internal const string formatURL = "recebeDadosFormatacao", maintenanceURL = "recebeDadosManutencao", supplyLoginData = "forneceDadosUsuario.php", supplyBiosData = "forneceDadosBIOS.php";
        internal const string nonAHCImodel1 = "7057", nonAHCImodel2 = "8814", nonAHCImodel3 = "6078", nonAHCImodel4 = "560s", nonAHCImodel5 = "945GCM-S2C";
        internal const string nvmeModel1 = "A315-56";
        internal const string nonSecBootGPU1 = "210", nonSecBootGPU2 = "430";

        internal const string WEBVIEW2_PATH = "runtimes\\win-x86";
        internal const string SMART_FAIL = " (Drive com falha iminente)";
        internal const string ONLINE = "ONLINE";
        internal const string OFFLINE = "OFFLINE";
        internal const string FETCHING = "Coletando...";
        internal const string REGISTERING = "Cadastrando / Atualizando, aguarde...";
        internal const string FETCH_AGAIN = "Coletar Novamente";
        internal const string REGISTER_AGAIN = "Cadastrar / Atualizar dados";
        internal const string SERVER_PORT_ERROR = "Para acessar, selecione o servidor e a porta!";
        internal const string ERROR_WINDOWTITLE = "Erro";
        internal const string DEFAULT_HOSTNAME = "MUDAR-NOME";
        internal const string CLI_HOSTNAME_ALERT = "Hostname: ";
        internal const string HOSTNAME_ALERT = " (Nome incorreto, alterar)";
        internal const string MEDIA_OPERATION_NVME = "NVMe";
        internal const string MEDIA_OPERATION_IDE_RAID = "IDE/Legacy ou RAID";
        internal const string CLI_MEDIA_OPERATION_ALERT = "Modo de operação SATA/M.2: ";
        internal const string MEDIA_OPERATION_ALERT = " (Modo de operação incorreto, alterar)";
        internal const string CLI_SECURE_BOOT_ALERT = "Secure Boot: ";
        internal const string SECURE_BOOT_ALERT = " (Ativar boot seguro)";
        internal const string CLI_DATABASE_REACH_ERROR = "Conectividade com o banco de dados: ";
        internal const string DATABASE_REACH_ERROR = "Erro ao contatar o banco de dados, verifique a sua conexão com a intranet e se o servidor web está ativo!";
        internal const string CLI_BIOS_VERSION_ALERT = "Versão da BIOS/UEFI: ";
        internal const string BIOS_VERSION_ALERT = " (Atualizar BIOS/UEFI)";
        internal const string CLI_FIRMWARE_TYPE_ALERT = "Tipo de firmware: ";
        internal const string FIRMWARE_TYPE_ALERT = " (PC suporta UEFI, fazer a conversão do sistema)";
        internal const string CLI_NETWORK_IP_ERROR = "Endereço IP: ";
        internal const string CLI_NETWORK_MAC_ERROR = "Endereço MAC: ";
        internal const string NETWORK_ERROR = "Computador sem conexão com a Intranet";
        internal const string CLI_VT_ALERT = "Tecnologia de Virtualização: ";
        internal const string VT_ALERT = " (Ativar Tecnologia de Virtualização na BIOS/UEFI)";
        internal const string SERVER_NOT_FOUND_ERROR = "Servidor não encontrado. Selecione um servidor válido!";
        internal const string PENDENCY_ERROR = "Resolva as pendencias exibidas em vermelho!";
        internal const string MANDATORY_FIELD = "Preencha os campos obrigatórios";
        internal const string DAYS_PASSED_TEXT = " dias desde a última ";
        internal const string FORMAT_TEXT = "formatação";
        internal const string MAINTENANCE_TEXT = "manutenção";
        internal const string SINCE_UNKNOWN = "(Não foi possível determinar a data do último serviço)";
        internal const string ALREADY_REGISTERED_TODAY = "Serviço já registrado para esta dia. Caso seja necessário outro registro, escolha outra data.";
        internal const string OFFLINE_MODE_ACTIVATED = "Modo OFFLINE!";
        internal const string FIX_PROBLEMS = "Corrija o problemas a seguir antes de prosseguir:";
        internal const string ARGS_ERROR = "Um ou mais argumentos contém erros! Saindo do programa...";
        internal const string AUTH_ERROR = "Erro de autenticação! Saindo do programa...";
        internal const string AUTH_INVALID = "Credenciais inválidas. Tente novamente.";
        internal const string INTRANET_REQUIRED = "É necessário conexão com a intranet.";
        internal const string NO_AUTH = "Preencha suas credenciais.";        

        internal const int TIMER_INTERVAL = 1000;
        internal const int MAX_SIZE = 100;

        internal static Color LIGHT_FORECOLOR = SystemColors.ControlText;
        internal static Color LIGHT_BACKCOLOR = SystemColors.ControlLight;
        internal static Color LIGHT_ASTERISKCOLOR = Color.Red;
        internal static Color LIGHT_AGENT = Color.DarkCyan;
        internal static Color ALERT_COLOR = Color.Red;
        internal static Color OFFLINE_ALERT = Color.Red;
        internal static Color ONLINE_ALERT = Color.Lime;
        internal static Color DARK_FORECOLOR = SystemColors.ControlLightLight;
        internal static Color DARK_BACKCOLOR = Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
        internal static Color DARK_ASTERISKCOLOR = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
        internal static Color DARK_AGENT = Color.DarkCyan;
        internal static Color LIGHT_BACKGROUND = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        internal static Color DARK_BACKGROUND = Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
        internal static Color BLUE_FOREGROUND = SystemColors.Highlight;
    }
}