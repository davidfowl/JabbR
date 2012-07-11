using System;
using System.Web;
using System.Web.Util;
using JabbR.App_Start;
using JabbR.Services;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Ninject;

namespace JabbR.Auth
{
    public class AllowTokenPostRequestValidator : RequestValidator
    {
        protected override bool IsValidRequestString(HttpContext context, string value,
                                                     RequestValidationSource requestValidationSource,
                                                     string collectionKey, out int validationFailureIndex)
        {
            var settings = Bootstrapper.Kernel.Get<IApplicationSettings>();
            if (string.IsNullOrEmpty(settings.FedAuthIdentityProviderUrl))
            {
                return base.IsValidRequestString(context, value, requestValidationSource, collectionKey,
                                             out validationFailureIndex);
            }

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