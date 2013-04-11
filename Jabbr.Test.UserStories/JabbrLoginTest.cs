using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Selenium;
using Xunit;

namespace Jabbr.Test.UserStories
{
    public class JabbrLoginTest : IDisposable
    {
        private OpenQA.Selenium.Firefox.FirefoxDriver driver;
        private JabbrUserStoryHelper helper = new JabbrUserStoryHelper();

        private const string jabbrBaseUrl = "http://localhost:16207/";

        private const string testuser1 = "testuser1";
        private const string testuser1pwd = "testuser1";
        private const string testuser1email = "abc@xyz.com";

        public JabbrLoginTest()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(jabbrBaseUrl);
        }

        [Fact]
        public void LoginFailedWithoutUserRegistration()
        {
            var element = driver.FindElement(By.Name("username"));
            element.SendKeys(testuser1);

            var pwd = driver.FindElement(By.Name("password"));
            pwd.SendKeys(testuser1pwd);

            IWebElement submitBtn = driver.FindElement(By.ClassName("btn"));
            submitBtn.Click();

            var result = driver.ExecuteScript("return $('li',$('.validation-summary-errors')).text();");


            Assert.Equal("Login failed. Check your username/password.", result);

        }


        public void Dispose()
        {
            driver.Dispose();
        }
    }
}
