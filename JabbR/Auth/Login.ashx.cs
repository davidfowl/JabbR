using System;
using System.Net;
using System.Web;
using JabbR.App_Start;
using JabbR.Services;
using Newtonsoft.Json;
using Ninject;

namespace JabbR.Auth
{
    /// <summary>
    /// Summary description for Login
    /// </summary>
    public class Login : IHttpHandler
    {
        private const string VerifyTokenUrl = "https://rpxnow.com/api/v2/auth_info?apiKey={0}&token={1}";

        public void ProcessRequest(HttpContext context)
        {
            var settings = Bootstrapper.Kernel.Get<IApplicationSettings>();
            string apiKey = settings.AuthApiKey;

            if (String.IsNullOrEmpty(apiKey))
            {
                // Do nothing
                context.Response.Redirect("~/", false);
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            string token = context.Request.Form["token"];

            if (String.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Bad response from login provider - could not find login token.");
            }

            var response = new WebClient().DownloadString(String.Format(VerifyTokenUrl, apiKey, token));

            if (String.IsNullOrEmpty(response))
            {
                throw new InvalidOperationException("Bad response from login provider - could not find user.");
            }

            dynamic j = JsonConvert.DeserializeObject(response);

            if (j.stat.ToString() != "ok")
            {
                throw new InvalidOperationException("Bad response from login provider.");
            }


            string userIdentity = j.profile.identifier.ToString();
            string username = j.profile.preferredUsername.ToString();
            string email = String.Empty;
            if (j.profile.email != null)
            {
                email = j.profile.email.ToString();
            }

            var identityLinker = Bootstrapper.Kernel.Get<IIdentityLinker>();
            identityLinker.LinkIdentity(new HttpContextWrapper(context), userIdentity, username, email);

            string hash = context.Request.QueryString["hash"];
            context.Response.Redirect(GetUrl(hash), false);
            context.ApplicationInstance.CompleteRequest();
        }

        private string GetUrl(string hash)
        {
            return HttpRuntime.AppDomainAppVirtualPath + hash;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}