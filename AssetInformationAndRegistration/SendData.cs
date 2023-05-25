using AssetInformationAndRegistration.Properties;
using LogGeneratorDLL;
using Microsoft.Web.WebView2.WinForms;
using System;

namespace AssetInformationAndRegistration
{
    internal static class SendData
    {
        /** !!Labels!!
          * serverArgs[0](serverIP)
          * serverArgs[1](serverPort)
          * serverArgs[2](assetNumber)
          * serverArgs[3](building)
          * serverArgs[4](roomNumber)
          * serverArgs[5](serviceDate)
          * serverArgs[6](serviceType)
          * serverArgs[7](batteryChange)
          * serverArgs[8](ticketNumber)
          * serverArgs[9](agent)
          * serverArgs[10](standard)
          * serverArgs[11](adRegistered)
          * serverArgs[12](brand)
          * serverArgs[13](model)
          * serverArgs[14](serialNumber)
          * serverArgs[15](processor)
          * serverArgs[16](ram)
          * serverArgs[17](storageSize)
          * serverArgs[18](storageType)
          * serverArgs[19](mediaOperationMode)
          * serverArgs[20](videoCard)
          * serverArgs[21](operatingSystem)
          * serverArgs[22](hostname)
          * serverArgs[23](fwType)
          * serverArgs[24](fwVersion)
          * serverArgs[25](secureBoot)
          * serverArgs[26](virtualizationTechnology)
          * serverArgs[27](tpmVersion)
          * serverArgs[28](macAddress)
          * serverArgs[29](ipAddress)
          * serverArgs[30](inUse)
          * serverArgs[31](sealNumber)
          * serverArgs[32](tag)
          * serverArgs[33](hwType)
          */

        ///<summary>Sends hardware info to the specified server</summary>
        ///<param name="serverArgs">Array containing asset information, which will be sent to server via GET method</param>
        internal static void ServerSendInfo(string[] serverArgs, LogGenerator log, bool consoleOut, WebView2 webView2Control)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING, string.Empty, consoleOut);
            webView2Control.CoreWebView2.Navigate(ConstantsDLL.Properties.Resources.HTTP + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[6] + ".php"
                + ConstantsDLL.Properties.Resources.PHP_ASSET_NUMBER + serverArgs[2]
                + ConstantsDLL.Properties.Resources.PHP_BUILDING + serverArgs[3]
                + ConstantsDLL.Properties.Resources.PHP_ROOM + serverArgs[4]
                + ConstantsDLL.Properties.Resources.PHP_SERVICE_DATE + serverArgs[5]
                + ConstantsDLL.Properties.Resources.PHP_PREVIOUS_SERVICE_DATES + serverArgs[5]
                + ConstantsDLL.Properties.Resources.PHP_BATTERY_CHANGE + serverArgs[7]
                + ConstantsDLL.Properties.Resources.PHP_TICKET_NUMBER + serverArgs[8]
                + ConstantsDLL.Properties.Resources.PHP_AGENT + serverArgs[9]
                + ConstantsDLL.Properties.Resources.PHP_STANDARD + serverArgs[10]
                + ConstantsDLL.Properties.Resources.PHP_AD_REGISTERED + serverArgs[11]
                + ConstantsDLL.Properties.Resources.PHP_BRAND + serverArgs[12]
                + ConstantsDLL.Properties.Resources.PHP_MODEL + serverArgs[13]
                + ConstantsDLL.Properties.Resources.PHP_SERIAL_NUMBER + serverArgs[14]
                + ConstantsDLL.Properties.Resources.PHP_PROCESSOR + serverArgs[15]
                + ConstantsDLL.Properties.Resources.PHP_RAM + serverArgs[16]
                + ConstantsDLL.Properties.Resources.PHP_STORAGE_SIZE + serverArgs[17]
                + ConstantsDLL.Properties.Resources.PHP_STORAGE_TYPE + serverArgs[18]
                + ConstantsDLL.Properties.Resources.PHP_MEDIA_OPERATION_MODE + serverArgs[19]
                + ConstantsDLL.Properties.Resources.PHP_VIDEO_CARD + serverArgs[20]
                + ConstantsDLL.Properties.Resources.PHP_OPERATING_SYSTEM + serverArgs[21]
                + ConstantsDLL.Properties.Resources.PHP_HOSTNAME + serverArgs[22]
                + ConstantsDLL.Properties.Resources.PHP_FW_TYPE + serverArgs[23]
                + ConstantsDLL.Properties.Resources.PHP_FW_VERSION + serverArgs[24]
                + ConstantsDLL.Properties.Resources.PHP_SECURE_BOOT + serverArgs[25]
                + ConstantsDLL.Properties.Resources.PHP_VIRTUALIZATION_TECHNOLOGY + serverArgs[26]
                + ConstantsDLL.Properties.Resources.PHP_TPM_VERSION + serverArgs[27]
                + ConstantsDLL.Properties.Resources.PHP_MAC_ADDRESS + serverArgs[28]
                + ConstantsDLL.Properties.Resources.PHP_IP_ADDRESS + serverArgs[29]
                + ConstantsDLL.Properties.Resources.PHP_IN_USE + serverArgs[30]
                + ConstantsDLL.Properties.Resources.PHP_SEAL_NUMBER + serverArgs[31]
                + ConstantsDLL.Properties.Resources.PHP_TAG + serverArgs[32]
                + ConstantsDLL.Properties.Resources.PHP_HW_TYPE + serverArgs[33]);
        }
    }
}
