using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.Messaging;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Supervisor.Messaging
{
    public partial class SendMessageView : BaseForm
    {
        #region Members

        private List<int> m_IDs;
        private readonly IUserSurveyListRepository _userSurveyListRepository;
        private readonly ISendMessageManager _messageManager;

        #endregion

        public SendMessageView()
        {
            _userSurveyListRepository = ServiceLocator.Resolve<IUserSurveyListRepository>();
            _messageManager = ServiceLocator.Resolve<ISendMessageManager>();
        }

        #region Properties

        /// <summary>
        /// Selected call ids
        /// </summary>
        protected List<int> IDs
        {
            //TODO: should always return some List<int>
            get
            {
                if (m_IDs == null)
                {
                    string requestIDS = (String)ViewState["IDS"];
                    string[] ids = requestIDS.Split(',');
                    m_IDs = ids.Select(x => Int32.Parse(x)).ToList();
                }
                return m_IDs;
            }
        }

        /// <summary>
        /// Returns recipient type 
        /// </summary>
        public MessageRecipientType RecipientType
        {
            get
            {
                return (MessageRecipientType)Enum.Parse(typeof(MessageRecipientType), (String)ViewState["MessageRecipientType"], true);
            }
        }

        /// <summary>
        /// True if it is possible to send message to offline interviewer
        /// otherwise false
        /// </summary>
        public bool EnableSendToOffline
        {
            get
            {
                bool enableSendToOffline = (bool)(ViewState["EnableOffline"]);
                if (RecipientType == MessageRecipientType.Survey)
                {
                    enableSendToOffline = false;
                }

                return enableSendToOffline;
            }
        }

        #endregion

        #region Life Cycle

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["IDS"] = Request.Params["IDS"];
                ViewState["MessageRecipientType"] = Request.Params["MessageRecipientType"];
                ViewState["EnableOffline"] = Request.Params["DisableOffline"] != "true";

                if (RecipientType == MessageRecipientType.Survey)
                {
                    SurveyPermissionVerifier.CheckPermission(Request.Params["IDS"], ",", User.Name);
                }

                foreach (var surveyId in IDs)
                {
                    _userSurveyListRepository.Insert(UserSurveyListType.Recent, surveyId);
                }
            }

            InitView();
        }

        /// <summary>
        /// Replace intitialization here because 
        /// in recipient role can be interviewers/interviewer groups/surveys  
        /// </summary>
        private void InitView()
        {
            if (IDs.Count == 1)
            {
                tbSendTo.Rows = 1;
            }

            cbDeliverToUserNotOnline.Enabled = EnableSendToOffline;
            tbSendTo.Text = string.Empty;

            switch (RecipientType)
            {
                case MessageRecipientType.Interviewer:
                    foreach (int sid in IDs)
                    {
                        BvPersonEntity user = PersonRepository.GetById(sid);
                        tbSendTo.Text += string.Format("{0}; ", user.Name);
                    }
                    break;
                case MessageRecipientType.InterviewerGroup:
                    foreach (int sid in IDs)
                    {
                        BvPersonGroupEntity group = PersonGroupRepository.GetById(sid);
                        tbSendTo.Text += string.Format("{0}; ", group.Name);
                    }
                    break;
                case MessageRecipientType.Survey:
                    foreach (int sid in IDs)
                    {
                        tbSendTo.Text += string.Format("{0}; ", SurveyManager.GetProjectID(sid));
                    }
                    break;
                default:
                    throw new NotSupportedException(RecipientType.ToString());
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Executing message sending
        /// </summary>
        protected void SendClick(object sender, EventArgs e)
        {
            try
            {
                bool onlineOnly = !cbDeliverToUserNotOnline.Checked;

                _messageManager.SendMessage(User.Name, tbMessageBody.Text, RecipientType, IDs, onlineOnly);

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
        #endregion
    }
}
