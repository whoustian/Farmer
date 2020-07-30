using ClickFarm;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(drv => drv.FindElement(locator));
                IWebElement element = driver.FindElement(locator);
                element.Click();
            }
            catch (Exception e)
            {
                ClickFarmer.Log("Click exception: " + e.Message);
            }
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
                string content = "Waited " + secondsToWait + " seconds for visiblity of element: " + locator
                        + ". Element was not visible.";
                Console.WriteLine(content);
                File.AppendAllText(".\\log.txt", DateTime.Now + ": " + content + Environment.NewLine);
                driver.Close();
                Thread.Sleep(10000);
                ClickFarmer.FarmSpotify("");
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

        public void scrollTo(IWebDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(drv => drv.FindElement(locator));
                IWebElement element = driver.FindElement(locator);
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
            }
            catch (Exception e)
            {
                ClickFarmer.Log("ScrollTo exception: " + e.Message);
            }
        }



    }
}
