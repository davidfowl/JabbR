using JabbR.Models;
using JabbR.Services;

namespace JabbR.Tests.UserStories {
    
    public class JabbRUserStoryHelper
    {
        private   IMembershipService membershipService;
        private IJabbrRepository repository;
        public ICryptoService criptoService;

        public JabbRUserStoryHelper()
        {
                Setup();
        }

        public void Setup()
        {
            repository = new PersistedRepository(new JabbrContext());
            criptoService = new CryptoService(new FileBasedKeyProvider());

            membershipService = new MembershipService(repository, criptoService);
        }

        
        public void SetupNewUser(string userName, string password, string email)
        {
            membershipService.AddUser(userName, email, password);
        }
    }
}
