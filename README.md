# Asset Information and Registration (AIR)

<img align="right" width="150" height="150" src=https://github.com/Kevin64/asset-information-and-registration/assets/1903028/44bdf179-9c9a-410f-9be4-7808627053c3 />


AIR is a program designed to scan a computer configuration and then register into an [APCS](https://github.com/Kevin64/asset-and-personnel-control-system) database on a remote server, while enforcing some settings to ensure a specific level of standardization inside the organization. It's ideal for scenarios where an IT administrator delegates a service to a subordinate agent (employee or intern) and cannot check for compliance on each asset individually.

The software scans various aspects of a computer hardware and configuration, such:

- **Brand**
- **Model**
- **Serial Number**
- **Processor (CPU)** - ID, name, frequency, number of cores, number of threads and cache. Supports more than one CPU.
- **RAM** - Slot occupied, amount, type, frequency, manufacturer, serial number and part number. Scans details for each module. (Enforced by default - alerts if the RAM amount is too high or too low for x86 or x64 systems respectively)
- **Storage drives** - ID, type, size, connection, model, serial number and S.M.A.R.T. status. Scans details for each drive. (Enforces S.M.A.R.T. failing status by default (alerts if the storage device is not healthy, according to the OS built-in S.M.A.R.T. reporting))
- **Video card (GPU)** - ID, name and vRAM.
- **Hostname** - enforced by default (alerts if hostname is not following Regex rules defined in the [APCS](https://github.com/Kevin64/asset-and-personnel-control-system)' `Parameters` file)
- **IP address**
- **Storage type**
- **Firmware type** - enforced by default (alerts if the OS is not running on the correct firmware type for a given hardware model. Shows what's the correct option)
- **Firmware version** - enforced by default (alerts if the firmware is not updated for a given hardware model. Shows what's the correct version)
- **Secure Boot status** - enforced by default (alerts if the secure boot is disabled on UEFI capable systems. Shows what's the correct option)
- **Virtualization Technology status** - enforced by default (alerts if the virtualization technology is disabled on UEFI capable systems. Shows what's the correct option)
- **TPM version** - enforced by default (alerts if the trusted platform module is not in the right version for a given hardware model. Shows what's the correct version)
- **SATA/M.2 operation mode** - enforced by default (alerts if the storage devices are not operating in the correct mode for a given hardware model. Shows what's the correct option)
- **Operating System** - Name, version, build and architecture.

After the scan, AIR checks with the [APCS](https://github.com/Kevin64/asset-and-personnel-control-system) server, that supplies data with the standard configuration set by the IT administrator in the database, for that specific hardware model. The program then compares the supplied data to the current scanned data, alerting the agent for any incorrect settings or irregularities with the PC specs, as well as any hardware changes that happended with the asset.

## Screens

### Main window in dark mode (light mode available):

![AIR-main-window-2](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/54749654-a258-4c30-a49b-98fbff8ec059)

### When some specifications are not compliant:

![AIR-main-window-1](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/ac5635c4-3a43-45e3-beec-6e8d1013c4cf)

### CLI switches

![cmd_LaNVnnrYg0](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/554cafa0-3e52-44a4-8063-0cf5bfff63d8)

## Offline mode

This software has an 'offline mode', which is used for testing the scanning process (mostly for debugging purposes), when the [APCS](https://github.com/Kevin64/asset-and-personnel-control-system) server is not available. In this mode, all but one alert triggers are disabled, because there is nothing to compare to (RAM alert will still trigger by default). AIR running in CLI does not support 'offline mode'.

## Customization

AIR supports some customization, allowing changing the program organization banners and its names, and all the iconography used. This allows organizations to tailor the program visuals to their specific needs. To accomplish that, you have to navigate to the directory 'resources\header\' to change the banners, 'resources\icons\' to change the iconography, and, inside the 'config.json' file, edit the contents of `Definitions` to change some AIR settings, edit the contents of `Enforcement` to choose what settings you want to standardize, and edit the contents of `OrgData` section to change the organization names/acronyms.

### Pictures aspect ratio

Required aspect ratio for proper image showing, without stretching:

- Login window banner - 2:1
- Main window banner - 12:1
- Iconography - 1:1

### Set default switches and settings

Modifying the contents of the `Definitions` section allows you to set some settings as default, like setting the log file location, theming and default APCS server IP/Port access. The first IP and Port set will be used as the default switches if the agent omits `--serverIP` and/or `--serverPort` on the command line.

```json
"Definitions": {
        "LogLocation": "C:\\AppLog\\",
        "ServerIP": [
            "192.168.1.1",
            "localhost"
        ],
        "ServerPort": [
            "8080",
            "80"
        ],
        "ThemeUI": "Auto"
    }
```

### Flexible enforcement

AIR lets the agent choose which specifications and settings will be enforced when registering a computer. To change these parameters, modify the `Enforcement` section. The keys are self-explanatory.

```json
"Enforcement": {
        "RamLimit": true,
        "SmartStatus": true,
        "MediaOperationMode": true,
        "Hostname": true,
        "FirmwareType": true,
        "FirmwareVersion": true,
        "SecureBoot": true,
        "VirtualizationTechnology": true,
        "Tpm": true
    }
```

### Automatic update checking

AIR will check for updates when the program is launched, and via 'Check for updates' button in the About window. To disable auto update checks, edit the 'config.json' file and modify the key `CheckForUpdates` inside `Enforcement`.

```json
"Enforcement": {
        "CheckForUpdates": true
    }
```

## Output in APCS

After finishing, your asset will be recorded on the APCS database and will be shown on queries.

### General asset list

![firefox_iQ2fHF99wJ](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/9de3cbfc-9bf8-4e05-acf4-5d644b9dedff)

### Asset details 

![Firefox_Screenshot_2023-12-07T17-34-47 231Z](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/835daa44-ba00-4bab-aef1-40094b5288bc)

### Notice

AIR grabs hardware, firmware and configuration information reported by the operating system (specifically WMI in Windows), and can introduce unintentional errors or fake data when scanning some exotic or old piece of hardware. Some manufacturers won't follow the best practices when hardcoding information into its hardware/firmware. If you witness any bugs or incorrect information collected by the program, create an issue. Collection on PC hardware newer than 2010 should be fine.

## Build from scratch

If you want to build AIR from source code, you'll also need to clone its [Dependencies](https://github.com/Kevin64/Dependencies).

## Third-party code and assets

This software and its libraries (DLLs) use art, Open Source libraries and loose code from third parties. All credits go to their respective creators and maintainers:

#### Configurable Quality PictureBox (credits to Jason D)
   - [URL](https://stackoverflow.com/a/1774592/16838132)

#### Custom Flat ComboBox (credits to Reza Aghaei)
   - [URL](https://stackoverflow.com/a/65976649/16838132)

#### Tree View Sync (credits to Doctor Jones)
   - [URL](https://stackoverflow.com/a/6456738)

#### CommandLineParser
   - Copyright © 2005 - 2015 Giacomo Stelluti Scala & Contributors
   - [Repository (GitHub)](https://github.com/commandlineparser/commandline)
   - [License (MIT)](https://github.com/commandlineparser/commandline/blob/master/License.md)

#### coverlet.collector
   - Copyright (c) 2018 Toni Solarin-Sodara
   - [Repository (GitHub)](https://github.com/coverlet-coverage/coverlet)
   - [License (MIT)](https://github.com/coverlet-coverage/coverlet/blob/master/LICENSE)

#### DarkNet (credits to Ben Hutchison)
   - [Repository (GitHub)](https://github.com/Aldaviva/DarkNet)
   - [License (Apache-2.0)](https://github.com/Aldaviva/DarkNet/blob/master/License.txt)

#### fluentui-system-icons
   - Copyright © 2020 Microsoft Corporation.
   - [Repository (GitHub)](https://github.com/microsoft/fluentui-system-icons)
   - [License (MIT)](https://github.com/microsoft/fluentui-system-icons/blob/main/LICENSE)

#### LoadingCircle (credits to Martin Gagne)
   - [URL](https://www.codeproject.com/Articles/14841/How-to-write-a-loading-circle-animation-in-NET)
   - [License (CPOL)](https://www.codeproject.com/info/cpol10.aspx)

#### Microsoft.AspNet.WebApi.Client
   - Copyright © Microsoft Corporation. All rights reserved.
   - [URL](https://dotnet.microsoft.com/pt-br/apps/aspnet/apis)
   - [License](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Client/6.0.0/license)

#### Microsoft.NET.Test.Sdk
   - Copyright © Microsoft Corporation. All rights reserved.
   - [Repository (GitHub)](https://github.com/microsoft/vstest)
   - [License (MIT)](https://github.com/microsoft/vstest/blob/main/LICENSE)

#### Newtonsoft.Json
   - Copyright © 2007 James Newton-King
   - [Repository (GitHub)](https://github.com/JamesNK/Newtonsoft.Json)
   - [License (MIT)](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)

#### Newtonsoft.Json.Bson
   - Copyright © 2017 James Newton-King
   - [Repository (GitHub)](https://github.com/JamesNK/Newtonsoft.Json.Bson)
   - [License (MIT)](https://github.com/JamesNK/Newtonsoft.Json.Bson/blob/master/LICENSE.md)

#### Octokit
   - Copyright (c) 2017 GitHub, Inc.
   - [Repository (GitHub)](https://github.com/octokit/octokit.net)
   - [License (MIT)](https://github.com/octokit/octokit.net/blob/main/LICENSE.txt)

#### PowerShellStandard.Library
   - Copyright © Microsoft Corporation.
   - [Repository (GitHub)](https://github.com/PowerShell/PowerShellStandard)
   - [License (MIT)](https://github.com/PowerShell/PowerShell/blob/master/LICENSE.txt)

#### System.Buffers, System.Memory, System.Numerics.Vectors, System.Runtime.CompilerServices.Unsafe, System.Threading.Tasks.Extensions
   - Copyright © .NET Foundation and Contributors
   - [URL](https://dot.net/)
   - [License (MIT)](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### WindowsAPICodePack
   - Copyright © Microsoft Corporation. All rights reserved.
   - [Repository (GitHub)](https://github.com/aybe/Windows-API-Code-Pack-1.1)
   - [License](https://github.com/aybe/Windows-API-Code-Pack-1.1/blob/master/LICENCE)

#### xunit
   - Copyright © .NET Foundation
   - [Repository (GitHub)](https://github.com/xunit/xunit)
   - [License (Apache-2.0)](https://licenses.nuget.org/Apache-2.0)

#### xunit.runner.visualstudio
   - Copyright © .NET Foundation
   - [Repository (GitHub)](https://github.com/xunit/visualstudio.xunit)
   - [License (Apache-2.0)](https://licenses.nuget.org/Apache-2.0)
