using System.Configuration;

namespace JabbR.Services
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string AuthApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["auth.apiKey"];
            }
        }

        public string DefaultAdminUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["defaultAdminUserName"];
            }
        }

        public string DefaultAdminPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["defaultAdminPassword"];
            }
        }


        public string AuthAppId
        {
            get 
            {
                return ConfigurationManager.AppSettings["auth.appId"];
            }
        }


        public string FedAuthIdentityProviderUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["fedauth.identityProviderUrl"];
            }
        }

        public string FedAuthRealm
        {
            get
            {
                return ConfigurationManager.AppSettings["fedauth.realm"];
            }
        }

        public string FedAuthCertificateThumbprint
        {
            get
            {
                return ConfigurationManager.AppSettings["fedauth.certThumbprint"];
            }
        }

        public bool FedAuthRequiresSsl
        {
            get
            {
                string requireSsl = ConfigurationManager.AppSettings["fedauth.requireSsl"];
                return (!string.IsNullOrEmpty(requireSsl) && bool.Parse(requireSsl));
            }
        }

        public bool FedAuthWindowsAzureActiveDirectorySelectorEnabled
        {
            get
            {
                string selectorEnabled = ConfigurationManager.AppSettings["fedauth.waad.selectorEnabled"];
                return (!string.IsNullOrEmpty(selectorEnabled) && bool.Parse(selectorEnabled));
            }
        }
    }
}