# Asset Information and Registration (AIR)

AIR is a program designed to scan a PC configuration and enforce some settings to ensure a specific level of standardization inside the organization.
The software scans various aspects of a computer hardware, listed below:
- **Brand**
- **Model**
- **Serial Number**
- **Processor (CPU)**
- **RAM** - enforced by default (alerts if RAM amount is too high ou too low for x86 or x64 systems respectively)
- **Total storage size**
- **S.M.A.R.T. status** - enforced by default (alerts if the storage device is not healty, according to the OS built-in S.M.A.R.T. reporting)
- **Storage type**
- **SATA/M.2 operation mode** - enforced by default (alerts if the storage devices are operating in the correct mode for a given hardware model)
- **Video card (GPU)**
- **Hostname** - enforced by default (alerts if hostname is different from a name template used in custom system images)
- **Mac address**
- **IP address**
- **Firmware type** - enforced by default (alerts if the OS is running on the correct firmware type for a given hardware model)
- **Firmware version** - enforced by default (alerts if the firmware is updated for a given hardware model)
- **Secure Boot status** - enforced by default (alerts if the secure boot is enabled on UEFI capable systems)
- **Virtualization Technology status** - enforced by default (alerts if the virtualization technology is enabled on UEFI capable systems)
- **TPM version** - enforced by default (alerts if the trusted platform module is on the right version for a given hardware model)

After the scan, AIR checks with the <a href=https://github.com/Kevin64/asset-and-personnel-control-system>APCS</a> server, that supplies a JSON file with the stardard configuration set by the agent in the database, for each hardware model. The client then compares the JSON file to the current scanned config, alerting the agent for any incorrect settings or irregularities with the PC specs.

## Screens

### Login window in light and dark modes, respectively:

![AIR-v4 0 0 2305_4CZVnzcUka](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/f9bbfd6f-8e60-42b4-a6fa-fcb414be1916)
![AIR-v4 0 0 2305_4vj9v8PRr5](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/a7177f98-1ebd-42c7-8462-7e768d1e9533)

### Main window in light and dark modes, respectively:

![AIR-v4 0 0 2305_sqi2MZ9X35](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/d5c0d3ec-5707-47ee-a530-3251f1fe1162)
![AIR-v4 0 0 2305_sZ49AI7yVv](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/3a0be826-8dc8-44e0-8feb-37ca51a73eac)

### When some specifications are not compliant:

![AIR-v4 0 0 2305_AWPkUd9sJn](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/8811c7d1-b449-4262-8332-e31b51a74eb3)

### CLI switches

![cmd_vudLBXaV4X](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/1cd649a8-fefd-4c9a-a893-65b5186dcf12)

## Offline mode

This software has an 'offline mode', which is used for test the scanning process (mostly for debugging purposes), when the <a href=https://github.com/Kevin64/asset-and-personnel-control-system>APCS</a> server is not available. On this mode, all alert triggers are disabled, because there is nothing to compare to.

## Customization

AIR supports some customization, allowing changing the program organization banners and its names, and all the iconography used. To accomplish that, you have to navigate to the directory 'resources\header\' to change the banners, 'resources\icons\' to change the iconography, and edit the definitions.ini file to change the organization names/acronyms.

Required aspect ratio:

- Login window banner - 2:1

- Main window banner - 12:1

- Iconography - 1:1

This capability allows organizations to tailor the program to their specific needs.
