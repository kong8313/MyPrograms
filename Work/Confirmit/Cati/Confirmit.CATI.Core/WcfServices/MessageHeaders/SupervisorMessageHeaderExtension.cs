using System;
using System.ServiceModel.Configuration;

namespace Confirmit.CATI.Core.WcfServices.MessageHeaders
{
    public class SupervisorMessageHeaderExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new SupervisorMessageHeaderBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(SupervisorMessageHeaderBehavior);
            }
        }
    }
}