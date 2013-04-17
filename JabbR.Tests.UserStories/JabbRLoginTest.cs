using System;
using SimpleBrowser;
using SimpleBrowser.WebDriver;
using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;
using Xunit;

namespace JabbR.Tests.UserStories
{
    public class JabbRLoginTest : IDisposable
    {
        private SimpleBrowserDriver _driver;

        private const string _jabbrLoginUrl = "http://localhost:16207/account/login";
        private const string _testuser1 = "testuser1";
        private const string _testuser1pwd = "testuser1";
        
        public JabbRLoginTest()
        {
            _driver = new SimpleBrowserDriver();
            _driver.Navigate( ).GoToUrl( _jabbrLoginUrl );
        }

        [Fact]
        public void LoginFailedWithoutUserRegistration()
        {
            var element = _driver.FindElement(By.Id("username"));
            element.SendKeys(_testuser1);

            var pwd = _driver.FindElement(By.Name("password"));
            pwd.SendKeys(_testuser1pwd);

            var submitBtn = _driver.FindElement(By.ClassName("btn"));
            submitBtn.Click();

            var result = _driver.FindElement(By.CssSelector(".validation-summary-errors > li")).Text;

            Assert.Equal("Login failed. Check your username/password.", result);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
