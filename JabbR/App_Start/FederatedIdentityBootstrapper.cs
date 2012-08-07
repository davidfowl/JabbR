using JabbR.FederatedIdentity.Infrastructure;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivator.PreApplicationStartMethod(typeof(JabbR.App_Start.FederatedIdentityBootstrapper), "PreAppStart", Order = 1)]

namespace JabbR.App_Start
{
    public static partial class FederatedIdentityBootstrapper
    {
        public static void PreAppStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(CustomWSFederationAuthenticationModule));
            DynamicModuleUtility.RegisterModule(typeof(CustomSessionAuthenticationModule));
        }
    }
}