using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.ServerControls.Commands;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class States : BaseForm
    {
        private const string ExportFileName = "ExtendedStatusList.xml";

        private int? GroupID
        {
            get { return (int?)ViewState["GroupID"]; }
            set { ViewState["GroupID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["ID"] != null)
            {
                GroupID = Int32.Parse(Request.Params["ID"]);
            }

            grid.GetPage +=
                delegate(out int totalCount)
                {
                    var list =
                        StateGroupsManager.GetITSList(GroupID.Value).Select(
                            x =>
                            new
                                {
                                    StateID = x.StateID.GetValueOrDefault(),
                                    Name = x.Name,
                                    Priority = x.Priority.GetValueOrDefault(),
                                    DA = x.DA.GetValueOrDefault() != 0,
                                    FcdAction = x.FcdAction,
                                    AaporCode = x.AaporCode
                                }).ToList();
                    totalCount = list.Count;
                    return list;
                };
                        
            grid.BindData();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BvStateGroupEntity group = StateGroupRepository.GetById(GroupID.Value);
            grid.GridName = String.Format(Strings.GroupExtendedStatusCodes, group.Name);

            var command = (OverlayCommand) grid.GetCommand("Edit");
            command.ExternalDynamicParams.Add("GroupId", GroupID.Value.ToString());            
        }

        protected void ExportStateGroup(object sender, EventArgs e)
        {
            try
            {
                var stateGroup = StateGroupRepository.GetById(GroupID.Value);
                var states = StateRepository.GetAll(GroupID.Value);

                var list = new ExtendedStatusList(states, stateGroup.Name);

                FileToClientSender.Send(list, ExportFileName);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}
