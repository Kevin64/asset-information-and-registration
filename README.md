# Asset Information and Registration (AIR)

<img align="right" width="50" height="50" src=https://github.com/Kevin64/asset-information-and-registration/assets/1903028/bb30c6cc-a0f0-4732-bae9-d1208fd77394)/>

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

![AIR-v4 0 0 2305_4CZVnzcUka](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/edf72a6e-d0b9-4cb1-9e0d-637163a6b5d7)
![AIR-v4 0 0 2305_4vj9v8PRr5](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/e35ab7ee-c449-4055-8ab6-1785016a9402)

### Main window in light and dark modes, respectively:

![AIR-v4 0 0 2305_w8rt5f8bIR](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/fc5dbf4c-af28-4771-a25c-1c64a627e173)
![AIR-v4 0 0 2305_OIwWJJc1iK](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/0a338266-4248-42b2-a2ef-5c6ffadeafcb)

### When some specifications are not compliant:

![AIR-v4 0 0 2305_0ahVutDPlE](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/ed0a8127-eb83-41d7-8478-617ab063bc16)


### CLI switches

![cmd_vudLBXaV4X](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/1cd649a8-fefd-4c9a-a893-65b5186dcf12)

## Offline mode

This software has an 'offline mode', which is used for test the scanning process (mostly for debugging purposes), when the <a href=https://github.com/Kevin64/asset-and-personnel-control-system>APCS</a> server is not available. On this mode, all alert triggers are disabled, because there is nothing to compare to.

AIR running in CLI do not support 'offline mode'.

## Flexible enforcement

AIR lets the agent choose which specifications and settings will be enforced when registering a computer. To change this parameters, open the 'definitions.ini' file and modify the [Enforcement] section. Keys are self explanatory.

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
## Customization

AIR supports some customization, allowing changing the program organization banners and its names, and all the iconography used. This allows organizations to tailor the program visuals to their specific needs. To accomplish that, you have to navigate to the directory 'resources\header\' to change the banners, 'resources\icons\' to change the iconography, and, inside the 'definitions.ini' file, edit the contents of [OrgData] section to change the organization names/acronyms, and the contents of [Parameters] to change some AIR settings. The program supports Light/Dark theme enforcement too.

Required aspect ratio for proper image showing, without stretching:

- Login window banner - 2:1

- Main window banner - 12:1

- Iconography - 1:1
