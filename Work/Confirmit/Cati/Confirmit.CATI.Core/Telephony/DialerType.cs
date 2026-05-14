using System;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerType : IDialerType
    {
        private readonly IDialerSettings _dialerSettings;

        public DialerType(IDialerSettings dialerSettings)
        {
            _dialerSettings = dialerSettings;
        }

        public T CreateInstance<T>()
        {
            var o = (T)CreateInstance(_dialerSettings.DialerType);

            if (o == null)
            {
                throw new DialerIsNotConfiguredException(string.Format("Dialer object creation failed for type [{0}]", _dialerSettings.DialerType));
            }

            return o;
        }

        private object CreateInstance(string typeAsString)
        {
            var type = FromString(typeAsString);

            return CreateInstance(type);
        }

        private Type FromString(string typeAsString)
        {
            DiallerType typeAsEnum;

            try
            {
                typeAsEnum = (DiallerType)Enum.Parse(typeof(DiallerType), typeAsString);
            }
            catch (Exception ex)
            {
                throw new DialerIsNotConfiguredException(
                    string.Format("Dialer type string [{0}] parsing error: {1}",
                    typeAsString, ex));
            }

            // Load dialer type from the corresponding assembly

            string dialerAssembly;

            switch (typeAsEnum)
            {
                case DiallerType.BvTCI:
                    dialerAssembly = "Confirmit.CATI.Telephony.BvTciLibrary, BvTciLibrary";
                    break;

                case DiallerType.PROTS:
                    dialerAssembly = "Confirmit.CATI.Telephony.PROTSLibrary, PROTSLibrary";
                    break;

                case DiallerType.Generic:
                    dialerAssembly = "Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary, DialerLibrary";
                    break;

                case DiallerType.NoDialler:
                default:
                    throw new DialerIsNotConfiguredException(
                        string.Format("Attempt to initialize dialer with wrong type [string: {0}, enum: {1}]",
                        typeAsString, typeAsEnum));
            }

            Type dialerType = Type.GetType(dialerAssembly);

            if (dialerType == null)
            {
                throw new DialerIsNotConfiguredException(
                    string.Format("Dialer type can't be loaded [string: {0}, enum: {1}, assembly: {2}]",
                    typeAsEnum, typeAsEnum, dialerAssembly));
            }

            return dialerType;
        }


        private object CreateInstance(Type type)
        {
            var o = Activator.CreateInstance(type);

            return o;
        }
    }
}
