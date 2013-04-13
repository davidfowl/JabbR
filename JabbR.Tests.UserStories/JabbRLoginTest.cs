using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace JabbR.Tests.UserStories
{
    public class JabbRLoginTest : IDisposable
    {
        private FirefoxDriver _driver;
        
        private const string _jabbrBaseUrl = "http://localhost:16207/";
        private const string _testuser1 = "testuser1";
        private const string _testuser1pwd = "testuser1";
        
        public JabbRLoginTest()
        {
            _driver = new FirefoxDriver();
            _driver.Navigate().GoToUrl(_jabbrBaseUrl);
        }

        [Fact]
        public void LoginFailedWithoutUserRegistration()
        {
            var element = _driver.FindElement(By.Name("username"));
            element.SendKeys(_testuser1);

            var pwd = _driver.FindElement(By.Name("password"));
            pwd.SendKeys(_testuser1pwd);

            IWebElement submitBtn = _driver.FindElement(By.ClassName("btn"));
            submitBtn.Click();

            var result = _driver.ExecuteScript("return $('li',$('.validation-summary-errors')).text();");


            Assert.Equal("Login failed. Check your username/password.", result);

        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
