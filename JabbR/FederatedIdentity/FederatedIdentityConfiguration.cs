using System;
using System.Configuration;
using JabbR.Services;
using JabbR.App_Start;
using Ninject;

namespace JabbR.FederatedIdentity
{
    public class FederatedIdentityConfiguration
    {
        IApplicationSettings _settings;

        public FederatedIdentityConfiguration()
        {
            this._settings = Bootstrapper.Kernel.Get<IApplicationSettings>();
        }

        public string IdentityProviderUrl
        {
            get
            {
                return this._settings.FedAuthIdentityProviderUrl;
            }
        }

        public string Realm
        {
            get
            {
                return this._settings.FedAuthRealm;
            }
        }
        
        public string ReplyUrl
        {
            get
            {
                return this._settings.FedAuthReplyUrl;
            }
        }

        public string CertificateThumbprint
        {
            get
            {
                return this._settings.FedAuthCertificateThumbprint;
            }
        }

        public bool RequiresSsl
        {
            get
            {
                return this._settings.FedAuthRequiresSsl;
            }
        }

        public bool ManualRedirectEnabled
        {
            get
            {
                return this._settings.FedAuthWindowsAzureActiveDirectorySelectorEnabled;
            }
        }
    }
}