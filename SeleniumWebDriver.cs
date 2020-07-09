using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Farmer
{
    class SeleniumWebDriver
    {
        public static ChromeDriver SetUpChromeDriver()
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddArgument("--start-maximized");
                //chromeOptions.AddArgument("mute-audio");
                //chromeOptions.BinaryLocation = @"C:\Users\Will\source\repos\ClickFarm\chrome-win\chrome.exe";
                return new ChromeDriver(service, chromeOptions);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ChromeDriver Error");
                return null;
            }
        }

    }
}
