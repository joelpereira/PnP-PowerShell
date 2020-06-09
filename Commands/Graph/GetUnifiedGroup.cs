﻿#if !ONPREMISES
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Graph;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;
using System.Collections.Generic;
using System.Management.Automation;

namespace SharePointPnP.PowerShell.Commands.Graph
{
    [Cmdlet(VerbsCommon.Get, "PnPUnifiedGroup")]
    [CmdletHelp("Gets one Microsoft 365 Group (aka Unified Group) or a list of Microsoft 365 Groups. Requires the Azure Active Directory application permission 'Group.Read.All'.",
        Category = CmdletHelpCategory.Graph,
        OutputTypeLink = "https://docs.microsoft.com/graph/api/group-list",
        SupportedPlatform = CmdletSupportedPlatform.Online)]
    [CmdletExample(
       Code = "PS:> Get-PnPUnifiedGroup",
       Remarks = "Retrieves all the Microsoft 365 Groups",
       SortOrder = 1)]
    [CmdletExample(
       Code = "PS:> Get-PnPUnifiedGroup -Identity $groupId",
       Remarks = "Retrieves a specific Microsoft 365 Group based on its ID",
       SortOrder = 2)]
    [CmdletExample(
       Code = "PS:> Get-PnPUnifiedGroup -Identity $groupDisplayName",
       Remarks = "Retrieves a specific or list of Microsoft 365 Groups that start with the given DisplayName",
       SortOrder = 3)]
    [CmdletExample(
       Code = "PS:> Get-PnPUnifiedGroup -Identity $groupSiteMailNickName",
       Remarks = "Retrieves a specific or list of Microsoft 365 Groups for which the email starts with the provided mail nickName",
       SortOrder = 4)]
    [CmdletExample(
       Code = "PS:> Get-PnPUnifiedGroup -Identity $group",
       Remarks = "Retrieves a specific Microsoft 365 Group based on its object instance",
       SortOrder = 5)]
    [CmdletExample(
       Code = "PS:> Get-PnPUnifiedGroup -IncludeIfHasTeam",
       Remarks = "Retrieves all the Microsoft 365 Groups and checks for each of them if it has a Microsoft Team provisioned for it",
       SortOrder = 6)]
    [CmdletMicrosoftGraphApiPermission(MicrosoftGraphApiPermission.Group_Read_All | MicrosoftGraphApiPermission.Group_ReadWrite_All | MicrosoftGraphApiPermission.GroupMember_ReadWrite_All | MicrosoftGraphApiPermission.GroupMember_Read_All | MicrosoftGraphApiPermission.Directory_ReadWrite_All | MicrosoftGraphApiPermission.Directory_Read_All)]
    public class GetUnifiedGroup : PnPGraphCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The Identity of the Microsoft 365 Group")]
        public UnifiedGroupPipeBind Identity;

        [Parameter(Mandatory = false, HelpMessage = "Exclude fetching the site URL for Microsoft 365 Groups. This speeds up large listings.")]
        public SwitchParameter ExcludeSiteUrl;

        [Parameter(Mandatory = false, HelpMessage = "Include Classification value of Microsoft 365 Groups")]
        public SwitchParameter IncludeClassification;

        [Parameter(Mandatory = false, HelpMessage = "Include a flag for every Microsoft 365 Group if it has a Microsoft Team provisioned for it. This will slow down the retrieval of Microsoft 365 Groups so only use it if you need it.")]
        public SwitchParameter IncludeHasTeam;

        protected override void ExecuteCmdlet()
        {
            UnifiedGroupEntity group = null;
            List<UnifiedGroupEntity> groups = null;

            if (Identity != null)
            {
                group = Identity.GetGroup(AccessToken);
            }
            else
            {
                // Retrieve all the groups
                groups = UnifiedGroupsUtility.ListUnifiedGroups(AccessToken, includeSite: !ExcludeSiteUrl.IsPresent, includeClassification:IncludeClassification.IsPresent, includeHasTeam: IncludeHasTeam.IsPresent);
            }

            if (group != null)
            {
                WriteObject(group);
            }
            else if (groups != null)
            {
                WriteObject(groups, true);
            }
        }
    }
}
#endif