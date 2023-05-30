# Asset Information and Registration (AIR)

<img align="right" width="150" height="150" src=https://github.com/Kevin64/asset-information-and-registration/assets/1903028/bb30c6cc-a0f0-4732-bae9-d1208fd77394)/>

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

After the scan, AIR checks with the <a href=https://github.com/Kevin64/asset-and-personnel-control-system>APCS</a> server, that supplies a JSON file with the stardard configuration set by the IT administrator in the database, for each hardware model. The program then compares the JSON file to the current scanned data, alerting the agent for any incorrect settings or irregularities with the PC specs.

## Screens

### Main window in dark mode (light mode available):

![AIR-v4 0 0 2306_HtGbuHjpNz](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/0259c905-ba92-40cb-81b9-4ac23439f3f7)

### When some specifications are not compliant:

![AIR_AGkqyPkVKi](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/d19f9776-c3cb-4b18-994f-658cb3533c03)

### CLI switches

![cmd_W9coG7aNjA](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/e91d057f-82f0-4e4e-b4ca-2249e6a818ab)

## Offline mode

This software has an 'offline mode', which is used for test the scanning process (mostly for debugging purposes), when the <a href=https://github.com/Kevin64/asset-and-personnel-control-system>APCS</a> server is not available. On this mode, all but one alert triggers are disabled, because there is nothing to compare to (RAM alert will still trigger by default). AIR running in CLI do not support 'offline mode'.

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
![firefox_FPGod3qbhq](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/583c4d25-ce7b-4716-939c-9410598423fd)

### Asset details 
![Web capture_30-5-2023_15910_localhost](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/33abcd34-b94f-4822-a736-8504e7ca6c05)
