using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Tasks;

namespace Confirmit.CATI.Core.Tasks.Fakes
{
    public class StubITaskExtension : ITaskExtension 
    {
        private ITaskExtension _inner;

        public StubITaskExtension()
        {
            _inner = null;
        }

        public ITaskExtension Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void UpdateOnCallConnectedBvTasksEntityBvInterviewEntityBvCallEntityDelegate(BvTasksEntity task, BvInterviewEntity interview, BvCallEntity call);
        public UpdateOnCallConnectedBvTasksEntityBvInterviewEntityBvCallEntityDelegate UpdateOnCallConnectedBvTasksEntityBvInterviewEntityBvCallEntity;

        void ITaskExtension.UpdateOnCallConnected(BvTasksEntity task, BvInterviewEntity interview, BvCallEntity call)
        {

            if (UpdateOnCallConnectedBvTasksEntityBvInterviewEntityBvCallEntity != null)
            {
                UpdateOnCallConnectedBvTasksEntityBvInterviewEntityBvCallEntity(task, interview, call);
            } else if (_inner != null)
            {
                ((ITaskExtension)_inner).UpdateOnCallConnected(task, interview, call);
            }
        }

        public delegate void ProcessLinkedChainBvTasksEntityBvTasksEntityDelegate(BvTasksEntity task, BvTasksEntity originalTask);
        public ProcessLinkedChainBvTasksEntityBvTasksEntityDelegate ProcessLinkedChainBvTasksEntityBvTasksEntity;

        void ITaskExtension.ProcessLinkedChain(BvTasksEntity task, BvTasksEntity originalTask)
        {

            if (ProcessLinkedChainBvTasksEntityBvTasksEntity != null)
            {
                ProcessLinkedChainBvTasksEntityBvTasksEntity(task, originalTask);
            } else if (_inner != null)
            {
                ((ITaskExtension)_inner).ProcessLinkedChain(task, originalTask);
            }
        }

        public delegate int GetFirstCampaignFromLinkedChainBvTasksEntityDelegate(BvTasksEntity task);
        public GetFirstCampaignFromLinkedChainBvTasksEntityDelegate GetFirstCampaignFromLinkedChainBvTasksEntity;

        int ITaskExtension.GetFirstCampaignFromLinkedChain(BvTasksEntity task)
        {


            if (GetFirstCampaignFromLinkedChainBvTasksEntity != null)
            {
                return GetFirstCampaignFromLinkedChainBvTasksEntity(task);
            } else if (_inner != null)
            {
                return ((ITaskExtension)_inner).GetFirstCampaignFromLinkedChain(task);
            }

            return default(int);
        }

        public delegate int? SetLinkedInterviewSessionIdBvTasksEntityDelegate(BvTasksEntity task);
        public SetLinkedInterviewSessionIdBvTasksEntityDelegate SetLinkedInterviewSessionIdBvTasksEntity;

        int? ITaskExtension.SetLinkedInterviewSessionId(BvTasksEntity task)
        {


            if (SetLinkedInterviewSessionIdBvTasksEntity != null)
            {
                return SetLinkedInterviewSessionIdBvTasksEntity(task);
            } else if (_inner != null)
            {
                return ((ITaskExtension)_inner).SetLinkedInterviewSessionId(task);
            }

            return default(int?);
        }

        public delegate void AssignCallOnTaskBvTasksEntityBvSurveyEntityBvInterviewEntityBvCallEntityBvActiveDialEntityDelegate(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call, BvActiveDialEntity dial);
        public AssignCallOnTaskBvTasksEntityBvSurveyEntityBvInterviewEntityBvCallEntityBvActiveDialEntityDelegate AssignCallOnTaskBvTasksEntityBvSurveyEntityBvInterviewEntityBvCallEntityBvActiveDialEntity;

        void ITaskExtension.AssignCallOnTask(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call, BvActiveDialEntity dial)
        {

            if (AssignCallOnTaskBvTasksEntityBvSurveyEntityBvInterviewEntityBvCallEntityBvActiveDialEntity != null)
            {
                AssignCallOnTaskBvTasksEntityBvSurveyEntityBvInterviewEntityBvCallEntityBvActiveDialEntity(task, survey, interview, call, dial);
            } else if (_inner != null)
            {
                ((ITaskExtension)_inner).AssignCallOnTask(task, survey, interview, call, dial);
            }
        }

        public delegate void SetInterviewingStateBvTasksEntityBvActiveDialEntityDelegate(BvTasksEntity task, BvActiveDialEntity dial);
        public SetInterviewingStateBvTasksEntityBvActiveDialEntityDelegate SetInterviewingStateBvTasksEntityBvActiveDialEntity;

        void ITaskExtension.SetInterviewingState(BvTasksEntity task, BvActiveDialEntity dial)
        {

            if (SetInterviewingStateBvTasksEntityBvActiveDialEntity != null)
            {
                SetInterviewingStateBvTasksEntityBvActiveDialEntity(task, dial);
            } else if (_inner != null)
            {
                ((ITaskExtension)_inner).SetInterviewingState(task, dial);
            }
        }

    }
}