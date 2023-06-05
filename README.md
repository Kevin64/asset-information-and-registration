# Asset Information and Registration (AIR)

<img align="right" width="150" height="150" src=https://github.com/Kevin64/asset-information-and-registration/assets/1903028/44bdf179-9c9a-410f-9be4-7808627053c3 />


AIR is a program designed to scan a PC configuration and then registering into a database on a remote server, while enforcing some settings to ensure a specific level of standardization inside the organization. It's ideal for scenarios where an IT administrator delegates a service to a subordinate agent (empoloyee or intern) and cannot check for compliance on each asset individually.
The software scans various aspects of a computer hardware and configuration, such:
- **Brand**
- **Model**
- **Serial Number**
- **Processor (CPU)**
- **RAM** - enforced by default (alerts if RAM amount is too high ou too low for x86 or x64 systems respectively)
- **Total storage size**
- **S.M.A.R.T. status** - enforced by default (alerts if the storage device is not healty, according to the OS built-in S.M.A.R.T. reporting)
- **Storage type**
- **SATA/M.2 operation mode** - enforced by default (alerts if the storage devices are not operating in the correct mode for a given hardware model)
- **Video card (GPU)**
- **Hostname** - enforced by default (alerts if hostname is not different from a name template used in custom system images)
- **Mac address**
- **IP address**
- **Firmware type** - enforced by default (alerts if the OS is not running on the correct firmware type for a given hardware model)
- **Firmware version** - enforced by default (alerts if the firmware is not updated for a given hardware model)
- **Secure Boot status** - enforced by default (alerts if the secure boot is disabled on UEFI capable systems)
- **Virtualization Technology status** - enforced by default (alerts if the virtualization technology is disabled on UEFI capable systems)
- **TPM version** - enforced by default (alerts if the trusted platform module is not on the right version for a given hardware model)

After the scan, AIR checks with the [APCS](https://github.com/Kevin64/asset-and-personnel-control-system) server, that supplies a JSON file with the stardard configuration set by the IT administrator in the database, for each hardware model. The program then compares the JSON file to the current scanned data, alerting the agent for any incorrect settings or irregularities with the PC specs.

## Screens

### Main window in dark mode (light mode available):

![AIR-v4 0 0 2306_HtGbuHjpNz](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/9945a21b-4e83-4cfe-8957-5a962302fa7d)

### When some specifications are not compliant:

![AIR_AGkqyPkVKi](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/a823e28b-8334-4c69-b077-33137591d770)

### CLI switches

![cmd_cYxsfZnoZH](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/e7c7bd6f-815f-4567-bcbb-78c3343b1834)

## Offline mode

This software has an 'offline mode', which is used for test the scanning process (mostly for debugging purposes), when the [APCS](https://github.com/Kevin64/asset-and-personnel-control-system) server is not available. On this mode, all but one alert triggers are disabled, because there is nothing to compare to (RAM alert will still trigger by default). AIR running in CLI do not support 'offline mode'.

## Customization

AIR supports some customization, allowing changing the program organization banners and its names, and all the iconography used. This allows organizations to tailor the program visuals to their specific needs. To accomplish that, you have to navigate to the directory 'resources\header\' to change the banners, 'resources\icons\' to change the iconography, and, inside the 'definitions.ini' file, edit the contents of `[Parameters]` to change some AIR settings, edit the contents of `[Enforcement]` to choose what settings you want to standardize, and edit the contents of `[OrgData]` section to change the organization names/acronyms.

### Pictures aspect ratio

Required aspect ratio for proper image showing, without stretching:

- Login window banner - 2:1
- Main window banner - 12:1
- Iconography - 1:1

### Set default switches and settings

Modifying the contents of `[Parameters]` section allows to set some settings as default, like setting the log file location, theming and default APCS server IP/Port access. The first IP and Port set will be used as the default switches if the agent omits `--serverIP` and/or `--serverPort` on the command line.

```ini
[Parameters]
LogLocation=C:\AppLog\
ServerIP=192.168.1.1,localhost
ServerPort=8080,80
ThemeUI=Auto
```

### Flexible enforcement

AIR lets the agent choose which specifications and settings will be enforced when registering a computer. To change this parameters, modify the `[Enforcement]` section. Keys are self explanatory.

```ini
[Enforcement]
RamLimitEnforcement=true
SmartStatusEnforcement=true
MediaOperationModeEnforcement=true
HostnameEnforcement=true
FirmwareTypeEnforcement=true
FirmwareVersionEnforcement=true
SecureBootEnforcement=true
VirtualizationTechnologyEnforcement=true
TpmEnforcement=true
```

## Output in APCS

After finishing, your asset will be recorded on the APCS database and will show on queries.

### General asset list

![firefox_FPGod3qbhq](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/57a52e2f-96cc-4927-887e-f0a7f9ce3da7)

### Asset details 

![Untitled](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/af30b7ac-4c05-4f08-aeff-4ec7e79245c0)

## Third-party code and assets

This software and its libraries (DLLs) use arts, Open Source libraries and loose codes from third parties. All credits go to their respective creators and maintainers:

#### Configurable Quality PictureBox (credits to Jason D)
   - [URL](https://stackoverflow.com/a/1774592/16838132)

#### Custom Flat ComboBox (credits to Reza Aghaei)
   - [URL](https://stackoverflow.com/a/65976649/16838132)

#### CommandLineParser
   - Copyright © 2005 - 2015 Giacomo Stelluti Scala & Contributors
   - [Repository (GitHub)](https://github.com/commandlineparser/commandline)
   - [License: (MIT)](https://github.com/commandlineparser/commandline/blob/master/License.md)

#### ini parser
   - Copyright © 2008 Ricardo Amores Hernández
   - [Repository (GitHub)](https://github.com/rickyah/ini-parser)
   - [License: (MIT)](https://github.com/rickyah/ini-parser/blob/master/LICENSE)

#### Microsoft.Web.WebView2
   - Copyright © Microsoft Corporation. All rights reserved.
   - [URL](https://learn.microsoft.com/pt-br/microsoft-edge/webview2/)
   - [License](https://www.nuget.org/packages/Microsoft.Web.WebView2/1.0.1722.45/License)

#### System.Buffers, System.Memory, System.Numerics.Vectors, System.Runtime.CompilerServices.Unsafe
   - Copyright (c) .NET Foundation and Contributors
   - [URL](https://dot.net/)
   - [License: (MIT)](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### WindowsAPICodePack
   - Copyright © Microsoft Corporation. All rights reserved.
   - [Repository (GitHub)](https://github.com/aybe/Windows-API-Code-Pack-1.1)
   - [License](https://github.com/aybe/Windows-API-Code-Pack-1.1/blob/master/LICENCE)

#### PowerShellStandard.Library
   - Copyright © Microsoft Corporation.
   - [Repository (GitHub)](https://github.com/PowerShell/PowerShellStandard)
   - [License: (MIT)](https://github.com/PowerShell/PowerShell/blob/master/LICENSE.txt)

#### LoadingCircle (credits to Martin Gagne)
   - [URL](https://www.codeproject.com/Articles/14841/How-to-write-a-loading-circle-animation-in-NET)
   - [License: (CPOL)](https://www.codeproject.com/info/cpol10.aspx)

#### Newtonsoft.Json
   - Copyright © 2007 James Newton-King
   - [Repository (GitHub)](https://github.com/JamesNK/Newtonsoft.Json)
   - [License: (MIT)](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)

#### BCrypt.Net-Next
   - Copyright © 2006 Damien Miller djm@mindrot.org (jBCrypt)
   - Copyright © 2013 Ryan D. Emerle (.Net port)
   - Copyright © 2016/2021 Chris McKee (.Net-core port / patches)
   - [Repository (GitHub)](https://github.com/BcryptNet/bcrypt.net)
   - [License: (MIT)](https://github.com/BcryptNet/bcrypt.net/blob/main/licence.txt)

#### DarkNet (credits to Ben Hutchison)
   - [Repository (GitHub)](https://github.com/Aldaviva/DarkNet)
   - [License: (Apache-2.0)](https://github.com/Aldaviva/DarkNet/blob/master/License.txt)

#### fluentui-system-icons
   - Copyright © 2020 Microsoft Corporation.
   - [Repository (GitHub)](https://github.com/microsoft/fluentui-system-icons)
   - [License: (MIT)](https://github.com/microsoft/fluentui-system-icons/blob/main/LICENSE)

#### Octokit
   - Copyright (c) 2017 GitHub, Inc.
   - [Repository (GitHub)](https://github.com/octokit/octokit.net)
   - [License: (MIT)](https://github.com/octokit/octokit.net/blob/main/LICENSE.txt)
