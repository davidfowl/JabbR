using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;

namespace JabbR.FederatedIdentity.Infrastructure
{
    public class MachineKeySessionSecurityTokenHandler : SessionSecurityTokenHandler
    {
        public MachineKeySessionSecurityTokenHandler()
            : base(CreateTransforms())
        { }

        public MachineKeySessionSecurityTokenHandler(SecurityTokenCache cache, TimeSpan tokenLifetime)
            : base(CreateTransforms(), cache, tokenLifetime)
        { }

        private static ReadOnlyCollection<CookieTransform> CreateTransforms()
        {
            return new List<CookieTransform>
                {
                    new DeflateCookieTransform(),
                    new MachineKeyCookieTransform()
                }.AsReadOnly();
        }

        private class MachineKeyCookieTransform : CookieTransform
        {
            public override byte[] Decode(byte[] encoded)
            {
                return MachineKey.Decode(Encoding.UTF8.GetString(encoded), MachineKeyProtection.All);
            }

            public override byte[] Encode(byte[] value)
            {
                return Encoding.UTF8.GetBytes(MachineKey.Encode(value, MachineKeyProtection.All));
            }
        }
    }
}