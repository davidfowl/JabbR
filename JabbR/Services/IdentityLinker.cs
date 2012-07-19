using System;
using System.Web;
using JabbR.Infrastructure;
using JabbR.Models;
using Newtonsoft.Json;

namespace JabbR.Services
{
    public class IdentityLinker : IIdentityLinker
    {
        private readonly IJabbrRepository _repository;
        private readonly IChatService _service;
        
        public IdentityLinker(IJabbrRepository repository, IChatService chatService)
        {
            _repository = repository;
            _service = chatService;
        }

        public void LinkIdentity(HttpContextBase httpContext, string userIdentity, string username, string email)
        {
            // Try to get the user by identity
            ChatUser user = _repository.GetUserByIdentity(userIdentity);

            // No user with this identity
            if (user == null)
            {
                // See if the user is already logged in (via cookie)
                var clientState = GetClientState(httpContext);
                user = _repository.GetUserById(clientState.UserId);

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
                    _repository.CommitChanges();
                }
                else
                {
                    // There's no logged in user so create a new user with the associated credentials
                    // but first, let's clean up that username!
                    username = FixUserName(username);
                    user = _service.AddUser(username, userIdentity, email);

                    SaveClientState(httpContext, user);
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
                _repository.CommitChanges();

                SaveClientState(httpContext, user);
            }
        }

        private static void SaveClientState(HttpContextBase context, ChatUser user)
        {
            // Save the cokie state
            var state = JsonConvert.SerializeObject(new { userId = user.Id });
            var cookie = new HttpCookie("jabbr.state", state);
            cookie.Expires = DateTime.Now.AddDays(30);
            context.Response.Cookies.Add(cookie);
        }

        private string FixUserName(string username)
        {
            // simple for now, translate spaces to underscores
            return username.Replace(' ', '_');
        }

        private ClientState GetClientState(HttpContextBase context)
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

        private string GetCookieValue(HttpContextBase context, string key)
        {
            HttpCookie cookie = context.Request.Cookies[key];
            return cookie != null ? HttpUtility.UrlDecode(cookie.Value) : null;
        }
    }
}