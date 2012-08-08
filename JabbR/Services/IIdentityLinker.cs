using System.Web;

namespace JabbR.Services
{
    public interface IIdentityLinker
    {
        void LinkIdentity(HttpContextBase httpContext, string userIdentity, string username, string email);
    }
}
