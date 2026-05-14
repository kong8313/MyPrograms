using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Filters;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.NavigationControls;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class VariablesTreeControl : BaseWUC
    {
        [StoreInViewState]
        public int SurveySid;
        
        [StoreInViewState]
        public int? FilterId;

        public string TreeClientId { get { return tree.ClientID; } }

        private readonly IFilterVariablesProvider _filterVariablesProvider = ServiceLocator.Resolve<FilterVariablesProvider>();

        private List<VariableInfo> _variableInfos;

        public List<VariableInfo> TreeItems
        {
            get { return _variableInfos ?? (_variableInfos = _filterVariablesProvider.GetVariables(SurveySid, FilterId)); }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            DataBind();

            RegisterClientScripts();

            foreach (var node in tree.AllNodes)
            {
                if (!string.IsNullOrEmpty(node.ImageUrl))
                {
                    node.ImageUrl = BaseRelativePath("svgimages/" + node.ImageUrl);
                }
            }
        }

        #region Methods

        public override void DataBind()
        {
            InitTree();
            base.DataBind();
        }
       
        private void InitTree()
        {
            var data = TreeItems;
            
            tree.Nodes.Clear();

            var callNode = new DataTreeNode
            {
                Text = Strings.CallFields,
                ImageUrl = "call.svg",
            };
            tree.Nodes.Add(callNode);

            var interviewNode = new DataTreeNode
            {
                Text = Strings.InterviewFields,
                ImageUrl = "receipt.svg",
            };
            tree.Nodes.Add(interviewNode);

            var surveyQuestionsNode = new DataTreeNode
            {
                Text = Strings.SurveyQuestions,
                ImageUrl = "question_answer.svg",
            };
            tree.Nodes.Add(surveyQuestionsNode);

            var appointmentNode = new DataTreeNode
            {
                Text = Strings.AppointmentFields,
                ImageUrl = "time.svg",
            };
            tree.Nodes.Add(appointmentNode);

            var filtersNode = new DataTreeNode
            {
                Text = Strings.Filters,
                ImageUrl = "filter_list.svg",
            };
            tree.Nodes.Add(filtersNode);

            foreach (var variableInfo in data)
            {
                DataTreeNode treeItem = null;
                switch (variableInfo.TableType)
                {
                    case TableTypes.Subfilter:
                        treeItem = new DataTreeNode
                        {
                            Text = variableInfo.Name,
                            ImageUrl = filtersNode.ImageUrl
                        };
                        filtersNode.Nodes.Add(treeItem);
                        break;
                    case TableTypes.Person:
                    case TableTypes.Interview:
                        treeItem = new DataTreeNode
                        {
                            Text = variableInfo.Name,
                            ImageUrl = interviewNode.ImageUrl
                        };
                        interviewNode.Nodes.Add(treeItem);
                        break;
                    case TableTypes.Call:
                    case TableTypes.ShiftType:
                    case TableTypes.Resource:
                        treeItem = new DataTreeNode
                        {
                            Text = variableInfo.Name,
                            ImageUrl = callNode.ImageUrl
                        };
                        callNode.Nodes.Add(treeItem);
                        break;
                    case TableTypes.Appointment:
                        treeItem = new DataTreeNode
                        {
                            Text = variableInfo.Name,
                            ImageUrl = appointmentNode.ImageUrl
                        };
                        appointmentNode.Nodes.Add(treeItem);
                        break;
                    case TableTypes.QSLVariables:
                        break;
                    case TableTypes.Quotas:
                        break;
                    case TableTypes.Container:
                        break;
                    case TableTypes.Web:
                        break;
                    case TableTypes.CFVariables:
                        treeItem = new DataTreeNode
                        {
                            Text = variableInfo.ConfirmitVariableType == ConfirmitVariableType.NotSet
                                       ? variableInfo.Name
                                       : String.Format("{0} ({1})", variableInfo.Name,
                                                       variableInfo.ConfirmitVariableTypeLocalizedString),
                            ImageUrl = surveyQuestionsNode.ImageUrl
                        };
                        surveyQuestionsNode.Nodes.Add(treeItem);
                        break;
                    case TableTypes.Expression:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (treeItem != null)
                {
                    treeItem.Value = new JavaScriptSerializer().Serialize(new
                        {
                            TableType = (int) variableInfo.TableType,
                            VarType = (int) variableInfo.VariableType,
                            Column = variableInfo.Column,
                            Value = variableInfo.Value,
                            IsBackground = variableInfo.IsBackground
                        });
                }
            }
        }


        /// <summary>
        /// Register client scripts.
        /// </summary>
        private void RegisterClientScripts()
        {
        }

        #endregion
    }
}