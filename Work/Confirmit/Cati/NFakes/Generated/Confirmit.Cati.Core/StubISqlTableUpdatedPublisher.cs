using System;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;

namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes
{
    public class StubISqlTableUpdatedPublisher : ISqlTableUpdatedPublisher 
    {
        private ISqlTableUpdatedPublisher _inner;

        public StubISqlTableUpdatedPublisher()
        {
            _inner = null;
        }

        public ISqlTableUpdatedPublisher Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void PublishSystemSettingsUpdatedDelegate();
        public PublishSystemSettingsUpdatedDelegate PublishSystemSettingsUpdated;

        void ISqlTableUpdatedPublisher.PublishSystemSettingsUpdated()
        {

            if (PublishSystemSettingsUpdated != null)
            {
                PublishSystemSettingsUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishSystemSettingsUpdated();
            }
        }

        public delegate void PublishSystemSettingsUpdatedInAllCompaniesDelegate();
        public PublishSystemSettingsUpdatedInAllCompaniesDelegate PublishSystemSettingsUpdatedInAllCompanies;

        void ISqlTableUpdatedPublisher.PublishSystemSettingsUpdatedInAllCompanies()
        {

            if (PublishSystemSettingsUpdatedInAllCompanies != null)
            {
                PublishSystemSettingsUpdatedInAllCompanies();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishSystemSettingsUpdatedInAllCompanies();
            }
        }

        public delegate void PublishShiftsUpdatedDelegate();
        public PublishShiftsUpdatedDelegate PublishShiftsUpdated;

        void ISqlTableUpdatedPublisher.PublishShiftsUpdated()
        {

            if (PublishShiftsUpdated != null)
            {
                PublishShiftsUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishShiftsUpdated();
            }
        }

        public delegate void PublishScheduleUpdatedDelegate();
        public PublishScheduleUpdatedDelegate PublishScheduleUpdated;

        void ISqlTableUpdatedPublisher.PublishScheduleUpdated()
        {

            if (PublishScheduleUpdated != null)
            {
                PublishScheduleUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishScheduleUpdated();
            }
        }

        public delegate void PublishScheduleParamsUpdatedDelegate();
        public PublishScheduleParamsUpdatedDelegate PublishScheduleParamsUpdated;

        void ISqlTableUpdatedPublisher.PublishScheduleParamsUpdated()
        {

            if (PublishScheduleParamsUpdated != null)
            {
                PublishScheduleParamsUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishScheduleParamsUpdated();
            }
        }

        public delegate void PublishBackendInstanceUpdatedDelegate();
        public PublishBackendInstanceUpdatedDelegate PublishBackendInstanceUpdated;

        void ISqlTableUpdatedPublisher.PublishBackendInstanceUpdated()
        {

            if (PublishBackendInstanceUpdated != null)
            {
                PublishBackendInstanceUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishBackendInstanceUpdated();
            }
        }

        public delegate void PublishTimeZoneUpdatedDelegate();
        public PublishTimeZoneUpdatedDelegate PublishTimeZoneUpdated;

        void ISqlTableUpdatedPublisher.PublishTimeZoneUpdated()
        {

            if (PublishTimeZoneUpdated != null)
            {
                PublishTimeZoneUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishTimeZoneUpdated();
            }
        }

        public delegate void PublishDialersUpdatedDelegate();
        public PublishDialersUpdatedDelegate PublishDialersUpdated;

        void ISqlTableUpdatedPublisher.PublishDialersUpdated()
        {

            if (PublishDialersUpdated != null)
            {
                PublishDialersUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishDialersUpdated();
            }
        }

        public delegate void PublishInboundTelephoneNumberUpdatedDelegate();
        public PublishInboundTelephoneNumberUpdatedDelegate PublishInboundTelephoneNumberUpdated;

        void ISqlTableUpdatedPublisher.PublishInboundTelephoneNumberUpdated()
        {

            if (PublishInboundTelephoneNumberUpdated != null)
            {
                PublishInboundTelephoneNumberUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishInboundTelephoneNumberUpdated();
            }
        }

        public delegate void PublishPersonUpdatedDelegate();
        public PublishPersonUpdatedDelegate PublishPersonUpdated;

        void ISqlTableUpdatedPublisher.PublishPersonUpdated()
        {

            if (PublishPersonUpdated != null)
            {
                PublishPersonUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishPersonUpdated();
            }
        }

        public delegate void PublishPersonGroupUpdatedDelegate();
        public PublishPersonGroupUpdatedDelegate PublishPersonGroupUpdated;

        void ISqlTableUpdatedPublisher.PublishPersonGroupUpdated()
        {

            if (PublishPersonGroupUpdated != null)
            {
                PublishPersonGroupUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishPersonGroupUpdated();
            }
        }

        public delegate void PublishBreakTypeUpdatedDelegate();
        public PublishBreakTypeUpdatedDelegate PublishBreakTypeUpdated;

        void ISqlTableUpdatedPublisher.PublishBreakTypeUpdated()
        {

            if (PublishBreakTypeUpdated != null)
            {
                PublishBreakTypeUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishBreakTypeUpdated();
            }
        }

        public delegate void PublishCallCenterUpdatedDelegate();
        public PublishCallCenterUpdatedDelegate PublishCallCenterUpdated;

        void ISqlTableUpdatedPublisher.PublishCallCenterUpdated()
        {

            if (PublishCallCenterUpdated != null)
            {
                PublishCallCenterUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishCallCenterUpdated();
            }
        }

        public delegate void PublishSurveyUpdatedDelegate();
        public PublishSurveyUpdatedDelegate PublishSurveyUpdated;

        void ISqlTableUpdatedPublisher.PublishSurveyUpdated()
        {

            if (PublishSurveyUpdated != null)
            {
                PublishSurveyUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishSurveyUpdated();
            }
        }

        public delegate void PublishStateUpdatedDelegate();
        public PublishStateUpdatedDelegate PublishStateUpdated;

        void ISqlTableUpdatedPublisher.PublishStateUpdated()
        {

            if (PublishStateUpdated != null)
            {
                PublishStateUpdated();
            } else if (_inner != null)
            {
                ((ISqlTableUpdatedPublisher)_inner).PublishStateUpdated();
            }
        }

    }
}