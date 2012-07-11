using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Models;

namespace JabbR.Services
{
    public static class ApplicationSettingsExtensions
    {
        public static bool AllowNullIdentity(this IApplicationSettings settings, ChatUser user)
        {
            return String.IsNullOrEmpty(user.Identity) &&
                (!String.IsNullOrEmpty(settings.AuthApiKey) || !String.IsNullOrEmpty(settings.FedAuthIdentityProviderUrl));
        }
    }
}