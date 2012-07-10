using System;
using System.Linq;
using System.Web;
using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Services;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using Ninject;

namespace JabbR.App_Start
{
    public static partial class FederatedLogin
    {
        private partial class NoConfigWSFederationAuthenticationModule 
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
                if (identity.Claims.SingleOrDefault(c => c.ClaimType == ClaimTypes.Email) != null)
                {
                    email = identity.Claims.SingleOrDefault(c => c.ClaimType == ClaimTypes.Email).Value.ToString();
                }

                var repository = Bootstrapper.Kernel.Get<IJabbrRepository>();
                var chatService = Bootstrapper.Kernel.Get<IChatService>();

                // Try to get the user by identity
                ChatUser user = repository.GetUserByIdentity(userIdentity);

                string wctx = context.Request.Form["wctx"];
                string hash = wctx;

                // No user with this identity
                if (user == null)
                {
                    // See if the user is already logged in (via cookie)
                    var clientState = GetClientState(context);
                    user = repository.GetUserById(clientState.UserId);

                    if (user != null)
                    {
                        // If they are logged in then assocate the identity
                        user.Identity = userIdentity;
                        user.Email = email;
                        if (!String.IsNullOrEmpty(email) &&
                            String.IsNullOrEmpty(user.Hash))
                        {
                            user.Hash = email.ToMD5();
                        }
                        repository.CommitChanges();
                        context.Response.Redirect(GetUrl(hash), false);
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    else
                    {
                        // There's no logged in user so create a new user with the associated credentials
                        // but first, let's clean up that username!
                        username = FixUserName(username);
                        user = chatService.AddUser(username, userIdentity, email);
                    }
                }
                else
                {
                    // Update email and gravatar
                    user.Email = email;
                    if (!String.IsNullOrEmpty(email) &&
                        String.IsNullOrEmpty(user.Hash))
                    {
                        user.Hash = email.ToMD5();
                    }
                    repository.CommitChanges();
                }

                // Save the cokie state
                var state = JsonConvert.SerializeObject(new { userId = user.Id });
                var cookie = new HttpCookie("jabbr.state", state);
                cookie.Expires = DateTime.Now.AddDays(30);
                context.Response.Cookies.Add(cookie);
                context.Response.Redirect(GetUrl(hash), false);
                context.ApplicationInstance.CompleteRequest();
            }

            private string GetUrl(string hash)
            {
                return HttpRuntime.AppDomainAppVirtualPath + hash;
            }

            private string FixUserName(string username)
            {
                // simple for now, translate spaces to underscores
                return username.Replace(' ', '_');
            }

            private ClientState GetClientState(HttpContext context)
            {
                // New client state
                var jabbrState = GetCookieValue(context, "jabbr.state");

                ClientState clientState = null;

                if (String.IsNullOrEmpty(jabbrState))
                {
                    clientState = new ClientState();
                }
                else
                {
                    clientState = JsonConvert.DeserializeObject<ClientState>(jabbrState);
                }

                return clientState;
            }

            private string GetCookieValue(HttpContext context, string key)
            {
                HttpCookie cookie = context.Request.Cookies[key];
                return cookie != null ? HttpUtility.UrlDecode(cookie.Value) : null;
            }
        }
    }
}