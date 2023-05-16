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

After the scan, AIR checks with the APCS server, that supplies the stardard configuration set in the database, for each hardware model. The client then alerts the agent for any configurations or irregularities with the PC specs.
