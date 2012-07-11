using System;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Web;
using System.Web.Util;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivator.PostApplicationStartMethod(typeof(JabbR.App_Start.FederatedLogin), "PostAppStart")]
[assembly: WebActivator.PreApplicationStartMethod(typeof(JabbR.App_Start.FederatedLogin), "PreAppStart")]

namespace JabbR.App_Start
{
    public static partial class FederatedLogin
    {
        public static string ApplicationRealm
        {
            get
            {
                return ConfigurationManager.AppSettings["fedauth.realm"];
            }
        }

        public static string IssuerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["fedauth.identityProviderUrl"];
            }
        }

        public static string IssuerCertificateThumbprint
        {
            get
            {
                return ConfigurationManager.AppSettings["fedauth.certThumbprint"];
            }
        }

        public static bool RequireSsl
        {
            get
            {
                string requireSsl = ConfigurationManager.AppSettings["fedauth.requireSsl"];
                return (requireSsl != null && bool.Parse(requireSsl));
            }
        }

        public static bool IdentityProviderSelectorEnabled
        {
            get
            {
                string selectorEnabled = ConfigurationManager.AppSettings["fedauth.waad.selectorEnabled"];
                return (selectorEnabled != null && bool.Parse(selectorEnabled));
            }
        }

        public static void PreAppStart()
        {
            if (!string.IsNullOrEmpty(IssuerUrl))
            {
                DynamicModuleUtility.RegisterModule(typeof(NoConfigWSFederationAuthenticationModule));
                DynamicModuleUtility.RegisterModule(typeof(NoConfigSessionAuthenticationModule));
            }
        }

        public static void PostAppStart()
        {
            if (!string.IsNullOrEmpty(IssuerUrl))
            {
                FederatedAuthentication.ServiceConfiguration.AudienceRestriction.AllowedAudienceUris.Add(new Uri(ApplicationRealm));
                FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.Configuration.CertificateValidator = X509CertificateValidator.None;
                FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.Configuration.IssuerNameRegistry = new SimpleIssuerNameRegistry(IssuerCertificateThumbprint);
            }
        }


        private partial class NoConfigWSFederationAuthenticationModule : WSFederationAuthenticationModule
        {
            protected override void InitializePropertiesFromConfiguration(string serviceName)
            {
                this.Realm = ApplicationRealm;
                this.Issuer = IssuerUrl;
                // if not using the idp selector, redirect straight to idp if not authenticated
                this.PassiveRedirectEnabled = !IdentityProviderSelectorEnabled; 
                this.RequireHttps = RequireSsl;
            }
        }

        private class NoConfigSessionAuthenticationModule : SessionAuthenticationModule
        {
            protected override void InitializePropertiesFromConfiguration(string serviceName)
            {
                this.CookieHandler.RequireSsl = RequireSsl;
            }
        }

        private class SimpleIssuerNameRegistry : IssuerNameRegistry
        {
            private readonly string trustedThumbrpint;

            public SimpleIssuerNameRegistry(string trustedThumbprint)
            {
                this.trustedThumbrpint = trustedThumbprint;
            }

            public override string GetIssuerName(System.IdentityModel.Tokens.SecurityToken securityToken)
            {
                var x509 = securityToken as X509SecurityToken;
                if (x509 != null)
                {
                    if (x509.Certificate.Thumbprint.Equals(trustedThumbrpint, StringComparison.OrdinalIgnoreCase))
                    {
                        return x509.Certificate.Subject;
                    }
                }

                return null;
            }
        }

        public class AllowTokenPostRequestValidator : RequestValidator
        {
            protected override bool IsValidRequestString(HttpContext context, string value,
                                                         RequestValidationSource requestValidationSource,
                                                         string collectionKey, out int validationFailureIndex)
            {
                validationFailureIndex = 0;

                if (requestValidationSource == RequestValidationSource.Form &&
                    collectionKey.Equals(WSFederationConstants.Parameters.Result, StringComparison.Ordinal))
                {
                    SignInResponseMessage message =
                        WSFederationMessage.CreateFromFormPost(context.Request) as SignInResponseMessage;

                    if (message != null)
                    {
                        return true;
                    }
                }
                return base.IsValidRequestString(context, value, requestValidationSource, collectionKey,
                                                 out validationFailureIndex);
            }
        }
    }
}