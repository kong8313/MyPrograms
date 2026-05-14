using System;
using Confirmit.SurveyVoiceXml.Service.Client;
using Newtonsoft.Json;
using Microsoft.Rest;

namespace Confirmit.SurveyVoiceXml.Service.Client.Fakes
{
    public class StubIInternalSurveyVoiceXmlAPI : IInternalSurveyVoiceXmlAPI 
    {
        private IInternalSurveyVoiceXmlAPI _inner;

        public StubIInternalSurveyVoiceXmlAPI()
        {
            _inner = null;
        }

        public IInternalSurveyVoiceXmlAPI Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        private Uri _BaseUri;
        public Func<Uri> BaseUriGet;
        public Action<Uri> BaseUriSetUri;

        Uri IInternalSurveyVoiceXmlAPI.BaseUri
        {
            get
            {
                if (BaseUriGet != null)
                {
                    return BaseUriGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).BaseUri;
                }

                if (BaseUriSetUri == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BaseUri;
                }

                return default(Uri);
            }

            set
            {
                if (BaseUriSetUri != null)
                {
                    BaseUriSetUri(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInternalSurveyVoiceXmlAPI)_inner).BaseUri = value;
                    return;
                }

                if (BaseUriGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BaseUri = value;
                }

            }
        }

        private JsonSerializerSettings _SerializationSettings;
        public Func<JsonSerializerSettings> SerializationSettingsGet;
        public Action<JsonSerializerSettings> SerializationSettingsSetJsonSerializerSettings;

        JsonSerializerSettings IInternalSurveyVoiceXmlAPI.SerializationSettings
        {
            get
            {
                if (SerializationSettingsGet != null)
                {
                    return SerializationSettingsGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).SerializationSettings;
                }

                if (SerializationSettingsSetJsonSerializerSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SerializationSettings;
                }

                return default(JsonSerializerSettings);
            }

        }

        private JsonSerializerSettings _DeserializationSettings;
        public Func<JsonSerializerSettings> DeserializationSettingsGet;
        public Action<JsonSerializerSettings> DeserializationSettingsSetJsonSerializerSettings;

        JsonSerializerSettings IInternalSurveyVoiceXmlAPI.DeserializationSettings
        {
            get
            {
                if (DeserializationSettingsGet != null)
                {
                    return DeserializationSettingsGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).DeserializationSettings;
                }

                if (DeserializationSettingsSetJsonSerializerSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DeserializationSettings;
                }

                return default(JsonSerializerSettings);
            }

        }

        private ServiceClientCredentials _Credentials;
        public Func<ServiceClientCredentials> CredentialsGet;
        public Action<ServiceClientCredentials> CredentialsSetServiceClientCredentials;

        ServiceClientCredentials IInternalSurveyVoiceXmlAPI.Credentials
        {
            get
            {
                if (CredentialsGet != null)
                {
                    return CredentialsGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).Credentials;
                }

                if (CredentialsSetServiceClientCredentials == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Credentials;
                }

                return default(ServiceClientCredentials);
            }

        }

        private IAbout _About;
        public Func<IAbout> AboutGet;
        public Action<IAbout> AboutSetIAbout;

        IAbout IInternalSurveyVoiceXmlAPI.About
        {
            get
            {
                if (AboutGet != null)
                {
                    return AboutGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).About;
                }

                if (AboutSetIAbout == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _About;
                }

                return default(IAbout);
            }

        }

        private IMain _Main;
        public Func<IMain> MainGet;
        public Action<IMain> MainSetIMain;

        IMain IInternalSurveyVoiceXmlAPI.Main
        {
            get
            {
                if (MainGet != null)
                {
                    return MainGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).Main;
                }

                if (MainSetIMain == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Main;
                }

                return default(IMain);
            }

        }

        private IRoot _Root;
        public Func<IRoot> RootGet;
        public Action<IRoot> RootSetIRoot;

        IRoot IInternalSurveyVoiceXmlAPI.Root
        {
            get
            {
                if (RootGet != null)
                {
                    return RootGet();
                } else if (_inner != null)
                {
                    return ((IInternalSurveyVoiceXmlAPI)_inner).Root;
                }

                if (RootSetIRoot == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Root;
                }

                return default(IRoot);
            }

        }

    }
}