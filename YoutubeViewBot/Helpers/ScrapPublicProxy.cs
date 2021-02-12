using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using YoutubeViewBot.Models;

namespace YoutubeViewBot.Helpers
{
    public class ScrapPublicProxy
    {
        ChromeOptions options = new ChromeOptions();
        private IWebDriver _webDriver;
        List<string> _videoLinkList = new List<string>();
        IJavaScriptExecutor js;
        static public string ProxyAddress { get; set; }

        public ScrapPublicProxy()
        {
            
            
            options.AddArgument("ignore-certificate-errors");
            options.AddArgument("--mute-audio");
            options.AddArgument("--autoplay-policy=no-user-gesture-required");
            options.AddArgument("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            //options.AddArgument("--disable-gpu");
            //options.AddArgument("--single-process");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--headless");
            
            new DriverManager().SetUpDriver(new ChromeConfig());
            _webDriver = new ChromeDriver(options);
            _webDriver.Manage().Window.Size = new System.Drawing.Size(1920, 1050);
            //_webDriver = new ChromeDriver(options);
        }
        public void TearDown()
        {
            try
            {
                _webDriver.Close();
                _webDriver.Quit();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error:" + ex.Message);
            }

        }
        public void ScrapHTTPProxy()
        {
            string URL = "http://www.freeproxylists.net/";
            
            _webDriver.Navigate().GoToUrl(URL);


            


            var TableElement=_webDriver.FindElement(By.ClassName("DataGrid"));

            var Rows = TableElement.FindElements(By.TagName("tr"));
            foreach (var row in Rows)
            {
                try
                {
                    var Columns = row.FindElements(By.TagName("td"));
                    var Protocol = Columns[2].Text;
                    if (Protocol.Contains("HTTP"))
                    {
                        string IPAddress = Columns[0].Text;
                        var PortNumber = Columns[1].Text;
                        IPAddress = $"{IPAddress}:{PortNumber}";

                        ProxyDetail proxyDetail = new ProxyDetail();
                        proxyDetail.IpAddress = IPAddress;
                        using (ProxyDBContext _context = new ProxyDBContext())
                        {
                            _context.ProxyDetails.Add(proxyDetail);

                            _context.SaveChanges();
                        }


                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                



                
            }





        }


    }
}
