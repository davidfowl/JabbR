using JabbR.Models;
using JabbR.Services;

namespace JabbR.Tests.UserStories 
{   
    public class JabbRUserStoryHelper
    {
        private   IMembershipService _membershipService;
        private IJabbrRepository _repository;
        public ICryptoService _criptoService;

        public JabbRUserStoryHelper()
        {
                Setup();
        }

        public void Setup()
        {
            _repository = new PersistedRepository(new JabbrContext());
            _criptoService = new CryptoService(new FileBasedKeyProvider());

            _membershipService = new MembershipService(_repository, _criptoService);
        }

        public void SetupNewUser(string userName, string password, string email)
        {
            _membershipService.AddUser(userName, email, password);
        }
    }
}
