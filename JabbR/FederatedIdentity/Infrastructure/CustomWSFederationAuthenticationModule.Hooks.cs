using System;
using System.Linq;
using System.Web;
using JabbR.Services;
using Microsoft.IdentityModel.Claims;
using Ninject;
using JabbR.App_Start;

namespace JabbR.FederatedIdentity.Infrastructure
{
    public partial class CustomWSFederationAuthenticationModule 
    {
        protected override void OnSignedIn(EventArgs args)
        {
            this.ProcessRequest(HttpContext.Current);

            base.OnSignedIn(args);
        }

        public void ProcessRequest(HttpContext context)
        {
            IClaimsIdentity identity = context.User.Identity as IClaimsIdentity;

            if (identity == null)
            {
                throw new InvalidOperationException("IClaimsIdentity is null.");
            }

            Claim userIdentityClaim = identity.Claims.SingleOrDefault(c => c.ClaimType == ClaimTypes.NameIdentifier);
            if (userIdentityClaim == null)
            {
                throw new InvalidOperationException("NameIdentifier claim not found.");
            }

            if (identity.Name == null)
            {
                throw new InvalidOperationException("Name claim not found.");
            }

            string userIdentity = userIdentityClaim.Value;
            string username = identity.Name;
            string email = String.Empty;
            Claim emailClaim = identity.Claims.SingleOrDefault(c => c.ClaimType == ClaimTypes.Email);
            if (emailClaim != null)
            {
                email = emailClaim.Value.ToString();
            }

            var identityLinker = Bootstrapper.Kernel.Get<IIdentityLinker>();
            identityLinker.LinkIdentity(new HttpContextWrapper(context), userIdentity, username, email);

            string hash = context.Request.Form["wctx"];
            context.Response.Redirect(GetUrl(hash), false);
            context.ApplicationInstance.CompleteRequest();
        }

        private string GetUrl(string hash)
        {
            return HttpRuntime.AppDomainAppVirtualPath + hash;
        }
    }
}