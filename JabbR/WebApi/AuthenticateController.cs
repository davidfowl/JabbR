using JabbR.Models;
using JabbR.Resources;
using JabbR.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace JabbR.WebApi
{
    public class AuthenticateController : ApiController
    {
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationTokenService _tokenService;

        public AuthenticateController(IMembershipService membershipService, IAuthenticationTokenService tokenService)
        {
            _membershipService = membershipService;
            _tokenService = tokenService;
        }

        // POST  { username:, password: }
        public async Task<HttpResponseMessage> Post()
        {
            JObject body = null;

            try
            {
                body = await Request.Content.ReadAsAsync<JObject>();

                if (body == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            string username = body.Value<string>("username");
            string password = body.Value<string>("password");

            if (String.IsNullOrEmpty(username))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LanguageResources.MissingUsername);
            }

            if (String.IsNullOrEmpty(password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LanguageResources.MissingPassword);
            }

            ChatUser user = null;

            try
            {
                user = _membershipService.AuthenticateUser(username, password);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }

            string token = _tokenService.GetAuthenticationToken(user);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }
    }
}