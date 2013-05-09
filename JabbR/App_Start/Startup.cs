﻿using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.Net.Http.Formatting;
using System.ServiceModel.Security;
using System.Web.Http;
using JabbR.Infrastructure;
using JabbR.Middleware;
using JabbR.Nancy;
using JabbR.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.SystemWeb.Infrastructure;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Federation;
using Microsoft.Owin.Security.Forms;
using Newtonsoft.Json.Serialization;
using Ninject;
using Owin;

namespace JabbR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var settings = new ApplicationSettings();

            if (settings.MigrateDatabase)
            {
                // Perform the required migrations
                DoMigrations();
            }

            var kernel = SetupNinject(settings);

            app.Use(typeof(DetectSchemeHandler));

            if (settings.RequireHttps)
            {
                app.Use(typeof(RequireHttpsHandler));
            }

            app.UseShowExceptions();

            SetupAuth(app, kernel);
            SetupSignalR(kernel, app);
            SetupWebApi(kernel, app);
            SetupMiddleware(kernel, app, settings);
            SetupNancy(kernel, app);

            SetupErrorHandling();

            SetupDefaultAdmin(settings, kernel.Get<IJabbrRepository>(), kernel.Get<IMembershipService>());
        }

        private static void SetupAuth(IAppBuilder app, KernelBase kernel)
        {
            app.UseFormsAuthentication(new FormsAuthenticationOptions
            {
                LoginPath = "/account/login",
                LogoutPath = "/account/logout",
                CookieHttpOnly = true,
                AuthenticationType = Constants.JabbRAuthType,
                CookieName = "jabbr.id",
                ExpireTimeSpan = TimeSpan.FromDays(30),
                DataProtection = kernel.Get<IDataProtection>(),
                Provider = kernel.Get<IFormsAuthenticationProvider>()
            });

            //var config = new FederationConfiguration(loadConfig: false);
            //config.WsFederationConfiguration.Issuer = "";
            //config.WsFederationConfiguration.Realm = "http://localhost:16207/";
            //config.WsFederationConfiguration.Reply = "http://localhost:16207/wsfederation";
            //var cbi = new ConfigurationBasedIssuerNameRegistry();
            //cbi.AddTrustedIssuer("", "");
            //config.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(new Uri("http://localhost:16207/"));
            //config.IdentityConfiguration.IssuerNameRegistry = cbi;
            //config.IdentityConfiguration.CertificateValidationMode = X509CertificateValidationMode.None;
            //config.IdentityConfiguration.CertificateValidator = X509CertificateValidator.None;

            //app.UseFederationAuthentication(new FederationAuthenticationOptions
            //{
            //    ReturnPath = "/wsfederation",
            //    SigninAsAuthenticationType = Constants.JabbRAuthType,
            //    FederationConfiguration = config,
            //    Provider = new FederationAuthenticationProvider()
            //});

            app.Use(typeof(WindowsPrincipalHandler));
        }

        private static void SetupNancy(IKernel kernel, IAppBuilder app)
        {
            var bootstrapper = new JabbRNinjectNancyBootstrapper(kernel);
            app.UseNancy(bootstrapper);
        }

        private static void SetupMiddleware(IKernel kernel, IAppBuilder app, IApplicationSettings settings)
        {
            if (settings.ProxyImages)
            {
                app.MapPath("/proxy", subApp => subApp.Use(typeof(ImageProxyHandler), settings));
            }

            app.UseStaticFiles();
            app.Use(typeof(LoggingHandler), kernel);
        }

        private static void SetupSignalR(IKernel kernel, IAppBuilder app)
        {
            var resolver = new NinjectSignalRDependencyResolver(kernel);
            var connectionManager = resolver.Resolve<IConnectionManager>();

            // Ah well loading system web.
            kernel.Bind<IProtectedData>()
                  .To<MachineKeyProtectedData>();

            kernel.Bind<IConnectionManager>()
                  .ToConstant(connectionManager);

            var config = new HubConfiguration
            {
                Resolver = resolver,
                EnableDetailedErrors = true
            };

            app.MapHubs(config);

            StartBackgroundWork(kernel, resolver);
        }

        private static void SetupWebApi(IKernel kernel, IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var jsonFormatter = new JsonMediaTypeFormatter();

            config.Formatters.Clear();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Add(jsonFormatter);
            config.DependencyResolver = new NinjectWebApiDependencyResolver(kernel);

            config.Routes.MapHttpRoute(
                name: "MessagesV1",
                routeTemplate: "api/v1/{controller}/{room}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api",
                defaults: new { controller = "ApiFrontPage" }
            );

            app.UseWebApi(config);
        }

        private static void SetupDefaultAdmin(IApplicationSettings settings, IJabbrRepository repository, IMembershipService membershipService)
        {
            if (String.IsNullOrEmpty(settings.DefaultAdminUserName))
                return;

            if (repository.GetUserByName(settings.DefaultAdminUserName) == null)
            {
                var defaultAdmin = membershipService.AddUser(settings.DefaultAdminUserName, "admin@myjabbrsite.com", settings.DefaultAdminPassword);

                defaultAdmin.IsAdmin = true;

                repository.CommitChanges();
            }
        }
    }
}