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
    public class JabbrLoginTest
    {

        public void Init()
        {

            OpenQA.Selenium.Firefox.FirefoxDriver driver = new FirefoxDriver();

            driver.Navigate().GoToUrl("http://192.168.1.2/");

            var element =driver.FindElement(By.Name("username"));
            element.SendKeys("sachin_mob");

            var pwd =driver.FindElement(By.Name("password"));
            pwd.SendKeys("Test@123");

            IWebElement submitBtn = driver.FindElement( By.ClassName( "btn" ) );
                submitBtn.Click( );


            //WebDriverBackedSelenium backed = new WebDriverBackedSelenium(driver, "http://192.168.1.2/");

            //backed.


        }
        
        [Fact]
        public void ShouldLoginWithTestUser()
        {

            Init();
        }

    }
}
