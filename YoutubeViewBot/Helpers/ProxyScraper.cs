using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Leaf.xNet;
using static YoutubeViewBot.Helpers.UsedProxyType;

namespace YoutubeViewBot.Helpers
{
    class ProxyScraper
    {

        public static Regex Proxy_re { get; private set; } = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):[0-9]{1,5}\b", RegexOptions.Compiled);
        
        public int Time { get; private set; } = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        public string FileName { get; private set; }

        public List<Proxy> Proxies { get; private set; }
        private Queue<Proxy> proxies;
        private readonly Object locker = new Object();

        public ProxyScraper()
        {
            Scrape();
        }
        public ProxyScraper(string fileName)
        {
            FileName = fileName;
            Scrape();
        }

        public Proxy Next()
        {
            lock (locker)
                if (Program.proxyType == Public && Time < (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - 150)
                    Scrape();

            if (proxies.Count == 0 && Proxies.Count != 0)
                proxies = new Queue<Proxy>(Proxies);

            return proxies.Dequeue();
        }

        public void Scrape()
        {
            if (Program.proxyType != 0)
                FromFile();
            else
                FromUrls();

            proxies = new Queue<Proxy>(Proxies);
        }

        private void FromFile()
        {
            string res = File.ReadAllText(FileName);

            List<string> proxies = GetProxies(res);

            Proxies = Proxy.GetList(proxies);
        }

        private void FromUrls()
        {
            List<string> proxies = new List<string>();
            using (HttpRequest req = new HttpRequest())
            {
                foreach (string url in Program.Urls)
                {
                    try
                    {
                        HttpResponse res = req.Get(url);
                        GetProxies(res.ToString()).ForEach(proxies.Add);
                    }
                    catch (Exception e)
                    {
                        ConsoleColor temp = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{url} - Proxy scraping error: {e.Message}");
                        Console.ForegroundColor = temp;
                    }
                }
            }

            Proxies = Proxy.GetList(proxies);
            Time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private List<string> GetProxies(string str) 
        {
            List<string> res = new List<string>();
        	try
        	{
        		foreach(Match proxy in Proxy_re.Matches(str))
        		    if(!res.Contains(proxy.Value))
        		        res.Add(proxy.Value);
        	}
        	catch { }
        	return res;
        }
    }
}
