using System;
using System.Collections.Generic;

using System.Web.Script.Serialization;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources.Controls
{
    public partial class CallGroupsList : BaseWUC
    {
        private readonly ICallGroupRepository _callGroupRepository = ServiceLocator.Resolve<ICallGroupRepository>();

        public string ClientControllerName { get { return "CallGroupController" + ClientID; } }

        protected void Page_Load(object sender, EventArgs e)
        {            
            grid.GetPage += delegate(out int totalCount)
                                {
                                    List<BvCallGroupEntity> list = _callGroupRepository.GetAllGroups();

                                    return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
                                };
            grid.GetCommand("Delete").OnClientClick = String.Format("{0}.DeleteSelectedCallGroup()", ClientControllerName);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {            
            Page.RegisterStartupScript(string.Format("var {0} = new CallGroupController({1});", 
                                     ClientControllerName, 
                                     GetClientSettings()), 
                                     "Controller" + ClientID, 
                                     GetType());
                
        }

        protected void DeleteGroup(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.DeletePriorityGroup", DeadlockPriority.Supervisor))
                {
                    foreach (string groupId in grid.SelectedKeys)
                    {
                        _callGroupRepository.Delete(Int32.Parse(groupId));
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

        private object GetClientSettings()
        {
            var settings = new
            {
                ClientGridId = grid.ClientID,
                DeleteCallGroupPostBackReference = Page.ClientScript.GetPostBackEventReference(grid, "Delete"),             
            };

            return new JavaScriptSerializer().Serialize(settings);
        }
    }
}