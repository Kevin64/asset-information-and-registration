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

### Login window in light and dark modes, respectively:

![AIR-v4 0 0 2306_9voXdXA4wJ](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/1df7689a-0b02-46fa-b642-fdc5b7b00458)
![AIR-v4 0 0 2306_pMXDjd3E9t](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/f725e71c-4764-41fc-bc56-5d8e052ae62f)

### Main window in light and dark modes, respectively:

![AIR-v4 0 0 2306_GNVz5s1vXI](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/799dbcda-e77a-450e-9376-b3204d37e771)
![AIR-v4 0 0 2306_x8wB2wWxGa](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/6f26447f-6c60-4c23-b50e-8e51d09a327c)

### When some specifications are not compliant:

![AIR-v4 0 0 2306_aUoKMgwhcy](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/43e18bdc-14a5-42fc-8e5b-3d8ae780cb62)

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

Modifying the contents of `[Parameters]` section allows to set some switches as default when running AIR in CLI mode, as well as set the log file location and theming. The first IP and Port set will be used as the default switches if the agent omits `--serverIP` and/or `--serverPort` on the command line.

```ini
[Parameters]
LogLocation=C:\AppLog\
ServerIP=192.168.1.1,localhost
ServerPort=8080,80
ThemeUI=Auto
```

### Flexible enforcement

AIR lets the agent choose which specifications and settings will be enforced when registering a computer. To change this parameters, open the 'definitions.ini' file and modify the `[Enforcement]` section. Keys are self explanatory.

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
