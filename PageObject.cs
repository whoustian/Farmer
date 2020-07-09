using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmer
{
    class PageObject
    {
        public By locator { get; set; }

        public PageObject(By locator)
        {
            this.locator = locator;
        }

        public void SetValue(IWebDriver driver, String input)
        {
            IWebElement element = driver.FindElement(locator);
            element.SendKeys(input);
        }

        public void click(IWebDriver driver)
        {
            IWebElement element = driver.FindElement(locator);
            element.Click();
        }

        public String getText(IWebDriver driver)
        {
            IWebElement element = driver.FindElement(locator);
            return element.Text;
        }

        public void waitForVisible(IWebDriver driver, int secondsToWait)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait));
                wait.Until(drv => drv.FindElement(locator));
            }
            catch (Exception e)
            {
                Console.WriteLine("Waited " + secondsToWait + " seconds for visiblity of element: " + locator
                        + ". Element was not visible.");
            }
        }

        public bool isVisible(IWebDriver driver)
        {
            try
            {
                driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

    }
}
