# Asset Information and Registration (AIR)

AIR is a program designed to scan a PC configuration and enforce some settings to ensure a specific level of standardization inside the organization.
The software scans various aspects of a computer hardware, listed below:
- **Brand**
- **Model**
- **Serial Number**
- **Processor (CPU)**
- **RAM** - enforced by default (alerts if RAM amount is too high ou too low for x86 or x64 systems respectively)
- **Total storage size**
- **S.M.A.R.T. status** - enforced by default (alerts if the storage device is not healty, according to the S.M.A.R.T. reporting)
- **Storage type**
- **SATA/M.2 operation mode** - enforced by default (alerts if the storage devices are operating in the correct mode for a given hardware model)
- **Video card (GPU)**
- **Hostname** - enforced by default (alerts if hostname is different from a name template used in system images)
- **Mac address**
- **IP address**
- **Firmware type** - enforced by default (alerts if the OS is running on the correct firmware type for a given hardware model)
- **Firmware version** - enforced by default (alerts if the firmware is updated  for a given hardware model)
- **Secure Boot status** - enforced by default (alerts if the secure boot is enable on UEFI capable systems)
- **Virtualization Technology status** - enforced by default (alerts if the virtualization technology is enabled on UEFI capable systems)
- **TPM version** - enforced by default (alerts if the trusted platform module is the right version for a given hardware model)

After the scan, AIR checks with the <a href=https://github.com/Kevin64/asset-and-personnel-control-system>APCS</a> server, that supplies the stardard configuration set in the database, for each hardware model. The client then alerts the agent for any configurations or irregularities with the PC specs.

## Screens

### Login window in light mode:

![AIR-v4 0 0 2305_4CZVnzcUka](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/f9bbfd6f-8e60-42b4-a6fa-fcb414be1916)

### Login window in dark mode:

![AIR-v4 0 0 2305_4vj9v8PRr5](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/a7177f98-1ebd-42c7-8462-7e768d1e9533)

### Main window in light mode:

![AIR-v4 0 0 2305_sqi2MZ9X35](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/d5c0d3ec-5707-47ee-a530-3251f1fe1162)

### Main window in dark mode:

![AIR-v4 0 0 2305_sZ49AI7yVv](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/3a0be826-8dc8-44e0-8feb-37ca51a73eac)

### When some specifications are not compliant:

![AIR-v4 0 0 2305_AWPkUd9sJn](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/8811c7d1-b449-4262-8332-e31b51a74eb3)

### CLI switches

![cmd_vudLBXaV4X](https://github.com/Kevin64/asset-information-and-registration/assets/1903028/1cd649a8-fefd-4c9a-a893-65b5186dcf12)




