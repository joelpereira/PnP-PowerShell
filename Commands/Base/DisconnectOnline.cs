﻿using SharePointPnP.PowerShell.CmdletHelpAttributes;
using System;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using SharePointPnP.PowerShell.Commands.Provider;

namespace SharePointPnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommunications.Disconnect, "PnPOnline")]
    [CmdletHelp("Disconnects the context",
        "Disconnects the current context and requires you to build up a new connection in order to use the Cmdlets again. Using Connect-PnPOnline to connect to a different site has the same effect.",
        Category = CmdletHelpCategory.Base)]
    [CmdletExample(
        Code = @"PS:> Disconnect-PnPOnline",
        Remarks = @"This will clear out all active tokens",
        SortOrder = 1)]
    public class DisconnectOnline : PSCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Connection to be used by cmdlet")]
        public PnPConnection Connection = null;

        protected override void ProcessRecord()
        {
#if !ONPREMISES
            if(PnPConnection.CurrentConnection?.Certificate != null)
            {
#if !NETSTANDARD2_1
                PnPConnectionHelper.CleanupCryptoMachineKey(PnPConnection.CurrentConnection.Certificate);
#endif
                PnPConnection.CurrentConnection.Certificate = null;
            }
#endif
                var success = false;
            if (Connection != null)
            {
                success = DisconnectProvidedService(Connection);
            }
            else
            {
                success = DisconnectCurrentService();
            }
            if (!success)
            {
                throw new InvalidOperationException(Properties.Resources.NoConnectionToDisconnect);
            }

            var provider = SessionState.Provider.GetAll().FirstOrDefault(p => p.Name.Equals(SPOProvider.PSProviderName, StringComparison.InvariantCultureIgnoreCase));
            if (provider != null)
            {
                //ImplementingAssembly was introduced in Windows PowerShell 5.0.
#if !NETSTANDARD2_1
                var drives = Host.Version.Major >= 5 ? provider.Drives.Where(d => d.Provider.Module.ImplementingAssembly.FullName == Assembly.GetExecutingAssembly().FullName) : provider.Drives;
#else
                var drives = Host.Version.Major >= 5 ? provider.Drives.Where(d => d.Provider.Module.Name == Assembly.GetExecutingAssembly().FullName) : provider.Drives;
#endif
                foreach (var drive in drives)
                {
                    SessionState.Drive.Remove(drive.Name, true, "Global");
                }
            }
        }

        internal static bool DisconnectProvidedService(PnPConnection connection)
        {
            connection.ClearTokens();
            Environment.SetEnvironmentVariable("PNPPSHOST", string.Empty);
            Environment.SetEnvironmentVariable("PNPPSSITE", string.Empty);
            if (connection == null)
            {
                return false;
            }

            connection.Context = null;
            connection = null;
            return true;
        }

        internal static bool DisconnectCurrentService()
        {            
            Environment.SetEnvironmentVariable("PNPPSHOST", string.Empty);
            Environment.SetEnvironmentVariable("PNPPSSITE", string.Empty);

            if (PnPConnection.CurrentConnection == null)
            {
                return false;
            }
            else
            {
                PnPConnection.CurrentConnection.ClearTokens();
                PnPConnection.CurrentConnection.Context = null;
                PnPConnection.CurrentConnection = null;
                return true;
            }            
        }
    }
}
