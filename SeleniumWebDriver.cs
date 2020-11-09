using ClickFarm;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Opera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Farmer
{

    // this class contains the logic for setting up the web driver

    class SeleniumWebDriver
    {
        public static ChromeDriver SetUpChromeDriver()
        {
            try
            {
                // several options are added to the chrome driver, including muting the audio, starting maximized, and not displaying the window if the config is set that way
                var chromeOptions = new ChromeOptions();
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddArgument("--start-maximized");
                chromeOptions.AddArgument("mute-audio");

                string test = ClickFarmer.getConfigValue(ObjectRepo.ShowChromeWindow);

                if (ClickFarmer.getConfigValue(ObjectRepo.ShowChromeWindow).Equals("false"))
                {
                       // this argument stops the window from being displayed
                    chromeOptions.AddArgument("Headless");                    
                }

                return new ChromeDriver(service, chromeOptions);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ChromeDriver Error");
                return null;
            }
        }

        public static OperaDriver SetUpOperaDriver()
        {
            try
            {
                var operaOptions = new OperaOptions();
                OperaDriverService service = OperaDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                operaOptions.AddUserProfilePreference("download.prompt_for_download", false);
                operaOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                operaOptions.AddArgument("--start-maximized");
                operaOptions.AddArgument("mute-audio");
                return new OperaDriver(service, operaOptions);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "OperaDriver Error");
                return null;
            }
        }

        public static EdgeDriver SetUpEdgeDriver()
        {
            try
            {
                var edgeOptions = new EdgeOptions();
                EdgeDriverService service = EdgeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                //edgeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                //edgeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                //edgeOptions.AddArgument("--start-maximized");
                //edgeOptions.AddArgument("mute-audio");
                //chromeOptions.BinaryLocation = @"C:\Users\Will\source\repos\ClickFarm\chrome-win\chrome.exe";
                return new EdgeDriver(service, edgeOptions);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "EdgeDriver Error");
                return null;
            }
        }

    }
}
