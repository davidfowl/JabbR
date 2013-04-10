using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JabbR.Models;
using JabbR.Services;

namespace Jabbr.Test.UserStories {
    
    public class JabbrUserStoryHelper
    {
        private   IMembershipService membershipService;
        private IJabbrRepository repository;
        public ICryptoService criptoService;

        public JabbrUserStoryHelper()
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
