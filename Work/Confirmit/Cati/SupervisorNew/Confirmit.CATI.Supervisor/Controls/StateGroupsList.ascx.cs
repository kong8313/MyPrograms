using System;
using System.Linq;
using System.Xml.Serialization;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class StateGroupsList : BaseWUC
    {
        private const string ExportFileName = "ExtendedStatusList.xml";
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            grid.InitializeRow += OnInitializeRow;

            var defaultStateGroup = StateGroupRepository.GetDefault();
            grid.GetCommand("CopyToDefaultGroup").Confirmation = string.Format(Strings.CopyToDefaultStatusGroupConfirmation, defaultStateGroup.Name);

            grid.GetPage += delegate(out int totalCount)
            {
                List<BvStateGroupEntity> list = StateGroupRepository.GetAll();

                return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
            };
        }

        private void OnInitializeRow(object sender, RowEventArgs args)
        {
            var defaultStateGroup = StateGroupRepository.GetDefault();

            if (args.Row.Items.FindItemByKey("ID").Text == defaultStateGroup.ID.ToString())
            {
                args.Row.CssClass = "highlighted-in-bold";
            }

        }

        protected void DeleteStateGroup(object sender, EventArgs e)
        {
            try
            {
                using (
                    var transaction = new DatabaseTransactionScope("Supervisor.DeleteStateGroup",
                        DeadlockPriority.Supervisor))
                {
                    foreach (string groupId in grid.SelectedKeys)
                    {
                        StateGroupRepository.Delete(Int32.Parse(groupId));
                    }

                    grid.BindData();
                    Page.CloseInfoFrame();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void CopyCustomGroupToDefault(object sender, EventArgs e)
        {
            try
            {
                if (!SupervisorPrincipal.Current.IsCatiAdministratorOrPros)
                {
                    throw new UserMessageException(Strings.Error_NoPermissionForCopyToDefaultAction);
                }

                StateGroupsManager.CopyToDefaultGroup(grid.SelectedKeysInt[0], _timezoneProvider.GetCurrentLocalTime());
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ExportStateGroup(object sender, EventArgs e)
        {
            try
            {
                var groupId = grid.SelectedKeys.First();

                var stateGroup = StateGroupRepository.GetById(Int32.Parse(groupId));
                var states = StateRepository.GetAll(Int32.Parse(groupId));

                var list = new ExtendedStatusList(states, stateGroup.Name);

                Page.FileToClientSender.Send(list, ExportFileName);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ImportStateGroup(object sender, EventArgs e)
        {
            try
            {
                if (FileLoad.HasFile)
                {
                    var serializer = new XmlSerializer(typeof(ExtendedStatusList));
                    using (
                        var transactionScope = new DatabaseTransactionScope("Supervisor.ImportStateGroup",
                            DeadlockPriority.Supervisor))
                    {
                        var list = (ExtendedStatusList)serializer.Deserialize(FileLoad.PostedFile.InputStream);

                        if (ValidateExtendedStatusList(list) == false)
                            return;

                        var stateGroupId = StateGroupsManager.AddStateGroup(list.StateGroupName);

                        foreach (var state in list.States)
                        {
                            state.StateGroupID = stateGroupId;
                            StateRepository.Update(state);
                        }

                        transactionScope.Commit();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Page.AddUserMessage(Strings.ErrorFileIncorrect, ex);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Returns true if no errors found
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool ValidateExtendedStatusList(ExtendedStatusList list)
        {
            var errorList = new List<string>();
            var groupName = list.StateGroupName;

            if (string.IsNullOrEmpty(groupName))
                errorList.Add(Strings.StateGroupNameIsNotSpecified);

            if (list.States == null || !list.States.Any())
                errorList.Add(Strings.StatusesAreNotSpecified);

            if (errorList.Any())
            {
                Page.AddUserMessage(string.Join("\n", errorList));
                return false;
            }

            if (StateGroupsManager.CheckGroupNameExists(groupName))
            {
                Page.AddUserMessage(string.Format(Strings.GroupNameExistsMessage, groupName));
                return false;
            }

            return true;
        }
    }
}