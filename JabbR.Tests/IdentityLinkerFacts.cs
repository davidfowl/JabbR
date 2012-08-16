using System.Web;
using JabbR.Models;
using JabbR.Services;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace JabbR.Tests
{
    public class IdentityLinkerFacts
    {
        public class LinkIdentity
        {
            [Fact]
            public void FirstTimeUserLogin()
            {
                var repository = new InMemoryRepository();
                var service = new IdentityLinker(repository, new ChatService(new Mock<ICache>().Object, repository, new Mock<ICryptoService>().Object));

                var context = FakeHttpContext();
                context.Request.Cookies.Add(new HttpCookie("jabbr.state", null));  // user is not logged in

                // act
                service.LinkIdentity(context, "userid100000", "foo", "foo@mail.com");

                // assert a new user is created with the right props
                var newUser = repository.GetUserByIdentity("userid100000");
                Assert.NotNull(newUser);
                Assert.Equal("userid100000", newUser.Identity);
                Assert.Equal("foo", newUser.Name);
                Assert.Equal("foo@mail.com", newUser.Email);
            }

            [Fact]
            public void ExistingUserLoginUpdatesEmailAndGravatar()
            {
                var repository = new InMemoryRepository();
                var service = new IdentityLinker(repository, new ChatService(new Mock<ICache>().Object, repository, new Mock<ICryptoService>().Object));

                var context = FakeHttpContext();
                // user is not logged in
                context.Request.Cookies.Add(new HttpCookie("jabbr.state", null));  
                // user at least logged in once
                repository.Add(new ChatUser { Identity = "userid100000", Name = "foo", Email = "foo@mail.com" });

                // act (update email)
                service.LinkIdentity(context, "userid100000", "foo", "foo_update@mail.com");

                // assert a new user is created with the right props
                var newUser = repository.GetUserByIdentity("userid100000");
                Assert.NotNull(newUser);
                Assert.Equal("userid100000", newUser.Identity);
                Assert.Equal("foo", newUser.Name);
                Assert.Equal("foo_update@mail.com", newUser.Email);
            }

            [Fact]
            public void ExistingUserWithAnotherIdentityLogin()
            {
                var repository = new InMemoryRepository();
                var service = new IdentityLinker(repository, new ChatService(new Mock<ICache>().Object, repository, new Mock<ICryptoService>().Object));

                var context = FakeHttpContext();

                var state = JsonConvert.SerializeObject(new { userId = "existinguser" });
                // user is logged in
                context.Request.Cookies.Add(new HttpCookie("jabbr.state", state));  
                // user already logged with another login mechanism
                repository.Add(new ChatUser { Id = "existinguser", Identity = "userid100000", Name = "foo", Email = "foo@mail.com" });

                // act
                service.LinkIdentity(context, "userid100000-CHANGED", "foo-CHANGED", "foo-CHANGED@mail.com");

                // assert a new user is created with the right props
                var migratedUSer = repository.GetUserById("existinguser");
                Assert.NotNull(migratedUSer);
                Assert.Equal("userid100000-CHANGED", migratedUSer.Identity);
                Assert.Equal("foo", migratedUSer.Name); // name not updated
                Assert.Equal("foo-CHANGED@mail.com", migratedUSer.Email);
            }

            private static HttpContextBase FakeHttpContext()
            {
                var context = new Mock<HttpContextBase>();
                var request = new Mock<HttpRequestBase>();
                var response = new Mock<HttpResponseBase>();
                request.SetupGet(r => r.Cookies).Returns(new HttpCookieCollection());
                response.SetupGet(r => r.Cookies).Returns(new HttpCookieCollection());
                context.SetupGet(c => c.Request).Returns(request.Object);
                context.SetupGet(c => c.Response).Returns(response.Object);

                return context.Object;
            }
        }
    }
}
