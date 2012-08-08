using System;
using System.IdentityModel.Selectors;
using System.Web;
using Microsoft.IdentityModel.Web;

namespace JabbR.FederatedIdentity.Infrastructure
{
    public partial class CustomWSFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        private static bool initialized = false;
        private static object locker = new object();

        protected override void InitializeModule(HttpApplication context)
        {
            // shortcircuit registration of the module WSFederationAuthenticationModule events if fed auth is not configured
            var settings = new FederatedIdentityConfiguration();
            if (!string.IsNullOrEmpty(settings.IdentityProviderUrl))
            {
                base.InitializeModule(context);
            }
        }

        protected override void InitializePropertiesFromConfiguration(string serviceName)
        {
            var settings = new FederatedIdentityConfiguration();

            // do this once since FederatedAuthentication is singleton
            if (!initialized)
            {
                lock (locker)
                {
                    if (!initialized)
                    {
                        FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.Configuration.IssuerNameRegistry = new CustomIssuerNameRegistry(settings.CertificateThumbprint);
                        FederatedAuthentication.ServiceConfiguration.AudienceRestriction.AllowedAudienceUris.Add(new Uri(settings.Realm));
                        FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.Configuration.CertificateValidator = X509CertificateValidator.None;

                        // farm friendly session token handler
                        FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(new MachineKeySessionSecurityTokenHandler());
                        initialized = true;
                    }
                }
            }
            
            // do this every time the module is used
            this.Realm = settings.Realm;
            this.Issuer = settings.IdentityProviderUrl;
            this.PassiveRedirectEnabled = !settings.ManualRedirectEnabled;
            this.RequireHttps = settings.RequiresSsl;
            this.Reply = settings.ReplyUrl;
        }
    }
}