/****************************************************************************************************************************************
 * © 2016 Daqri International. All Rights Reserved.                                                                                     *
 *                                                                                                                                      *
 *     NOTICE:  All software code and related information contained herein is, and remains the property of DAQRI INTERNATIONAL and its  *
 * suppliers, if any.  The intellectual and technical concepts contained herein are proprietary to DAQRI INTERNATIONAL and its          *
 * suppliers and may be covered by U.S. and Foreign Patents, patents in process, and/or trade secret law, and the expression of         *
 * those concepts is protected by copyright law. Dissemination, reproduction, modification, public display, reverse engineering, or     *
 * decompiling of this material is strictly forbidden unless prior written permission is obtained from DAQRI INTERNATIONAL.             *
 *                                                                                                                                      *
 *                                                                                                                                      *
 *                                                                                                                                      *
 *     File Purpose:       Various commands that are sent to BuildToSmartHelmet.SmartDevicePreferencesWindow.InvokeDaqriBridgeCommand.  *
 *     Guide:              <todo>                                                                                                       *
 *                                                                                                                                      *
 ****************************************************************************************************************************************/
namespace DAQRI.BuildToSmartHelmet {
    internal abstract class DaqriBridge {
        internal readonly string arguments;

        protected DaqriBridge(string arguments) {
            this.arguments = arguments;
        }
    }

    internal sealed class DaqriBridgeInstall : DaqriBridge {
		internal DaqriBridgeInstall(string ipAddress, string applicationName, string buildPath, string companyName)
            : base(string.Format(
                "install {0} \"{1}\" \"{2}\" \"{3}\"",
                ipAddress,
                applicationName,
                buildPath,
				companyName)) {}
    }

    internal sealed class DaqriBridgeList : DaqriBridge {
        internal DaqriBridgeList(string ipAddress) : base(string.Format("list {0}", ipAddress)) {}
    }

    internal sealed class DaqriBridgeRemove : DaqriBridge {
        internal DaqriBridgeRemove(string ipAddress, string appname)
            : base(string.Format("remove {0} \"{1}\"", ipAddress, appname)) {}
    }

	internal sealed class DaqriBridgeApplog : DaqriBridge {
		internal DaqriBridgeApplog(string ipAddress, string appname, string localPath)
			: base(string.Format("applog {0} \"{1}\" \"{2}\"", ipAddress, appname, localPath)) {}
	}
}
