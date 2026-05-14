using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class ConfigureDdiNumbers : BaseForm
    {
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IDialersRepository _dialersRepository;
        private readonly ISurveyRepository _surveyRepository;

        public override string TopTitle
        {
            get
            {
                return Strings.ConfigureDDINumbers;
            }
        }

        public ConfigureDdiNumbers()
        {
            _inboundTelephoneNumberRepository = ServiceLocator.Resolve<IInboundTelephoneNumberRepository>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GridName = TopTitle;
            m_grid.GetPage += GetPage;
            m_grid.InitializeRow += Grid_InitializeRow;
        }

        void Grid_InitializeRow(object sender, RowEventArgs rowEventArgs)
        {
            var gridDdiNumberModel = (GridDdiNumberModel) rowEventArgs.Row.DataItem;

            var surveyDeleted = gridDdiNumberModel.SurveyDeleted;
            if (surveyDeleted)
            {
                var surveyNameCell = rowEventArgs.Row.Items.FindItemByKey("SurveyName");
                surveyNameCell.Text = Strings.SurveyIsDeleted;
                surveyNameCell.CssClass += " warning";

                var surveyIdCell = rowEventArgs.Row.Items.FindItemByKey("SurveyId");
                surveyIdCell.CssClass += " warning";
                surveyIdCell.Text = Strings.SurveyIsDeleted;
            }

            var dialerDeleted = gridDdiNumberModel.DialerDeleted;
            if (dialerDeleted)
            {
                var dialerNameCell = rowEventArgs.Row.Items.FindItemByKey("DialerName");
                dialerNameCell.Text = Strings.DialerIsDeleted;
                dialerNameCell.CssClass += " warning";

                var dialerIdCell = rowEventArgs.Row.Items.FindItemByKey("DialerId");
                dialerIdCell.Text = Strings.DialerIsDeleted;
                dialerIdCell.CssClass += " warning";
            }
        }

        private object GetPage(out int totalCount)
        {            
            List<BvInboundTelephoneNumberEntity> bvInboundTelephoneNumberEntities = BvInboundTelephoneNumberCache.Instance.GetAll();
            var dialers = _dialersRepository.GetAll();

            var models = bvInboundTelephoneNumberEntities.Select(entity =>
            {
                var survey = entity.SurveyId.HasValue ? _surveyRepository.GetById(entity.SurveyId.Value) : null;
                var dialer = dialers.FirstOrDefault(x => x.Id == entity.DialerId);
                bool surveyDeleted = survey == null || survey.State == (int) SurveyState.SoftDeleted;

                return new GridDdiNumberModel
                {
                    TelephoneNumber = entity.TelephoneNumber,
                    SurveyName = survey != null ? survey.Description : "",
                    SurveyId = survey != null ? survey.Name : "",
                    DialerName = dialer != null ? dialer.Name : "",
                    DialerId = entity.DialerId,
                    SurveyDeleted = surveyDeleted,
                    DialerDeleted = dialer == null,
                    HasOverridingMessages = entity.InboundAudioMessagesDictionary.Count() > 0 ? "Yes" : "No"
                };
            });

            return BaseMethods.GetPage(models, m_grid.PageArguments, out totalCount);
        }

        protected void DeleteDdiNumbers(object sender, EventArgs e)
        {
            if (m_grid.SelectedKeys.Length == 0) return;

            var keys = m_grid.SelectedKeys;
            var dialerIds = _inboundTelephoneNumberRepository.GetByTelephoneNumbers(keys).Select(x => x.DialerId).Distinct();

            var evt = new DeleteDdiNumberEvent(keys.Length);
            _inboundTelephoneNumberRepository.Delete(keys);

            string wrongDialers = string.Empty;
            foreach (var dialerId in dialerIds)
            {
                try
                {
                    ConfigureDialersWithDdiNumbers(dialerId);
                }
                catch (Exception ex)
                {
                    wrongDialers += ", " + _dialersRepository.GetById(dialerId).Name;
                    System.Diagnostics.Trace.TraceWarning(ex.ToString());
                }
            }

            evt.Finish();

            m_grid.ClearSelectedKeys();

            if(!string.IsNullOrEmpty(wrongDialers))
            {
                Context.AddError(new UserMessageException(Strings.WarningDuringDdiNumberRemoval + wrongDialers.TrimStart(',')));
            }
        }

        private void ConfigureDialersWithDdiNumbers(int dialerId)
        {
            _supervisorServiceClient.ConfigureInboundDdiNumbers(dialerId);
        }

        protected class GridDdiNumberModel
        {
            public string TelephoneNumber { get; set; }
            public string DialerName { get; set; }
            public string SurveyName { get; set; }
            public string SurveyId { get; set; }
            public int DialerId { get; set; }
            public bool SurveyDeleted { get; set; }
            public bool DialerDeleted { get; set; }
            public string HasOverridingMessages { get; set; }
        }
    }
}