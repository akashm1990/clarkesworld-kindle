using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Concurrent;

namespace ClarkesWorldKindle
{
    class Program
    {
        static string outputFilePath = ConfigurationManager.AppSettings["outputFilePath"];
        static void makefile(string url)
        {
            if(url.Length==0)
            {
                return;
            }
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            string pageHTML = webClient.DownloadString(url);
            htmlDoc.LoadHtml(pageHTML);
            var storyNode = htmlDoc.DocumentNode.Descendants().Where(i => i.GetAttributeValue("class", "").Contains("entry clear single_entry entry-content"));
            string name = url.Replace("http://www.lightspeedmagazine.com/fiction/", "");
            name = name.Remove(name.Length - 1);
            System.IO.StreamWriter outputFile = new System.IO.StreamWriter(outputFilePath + name + ".html", false, Encoding.UTF8);
            outputFile.WriteLine("<head><h3>" + name + "</h3></head><body>");
            foreach (var i in storyNode)
            {
                outputFile.Write(i.InnerHtml.ToString());
            }
            outputFile.WriteLine("</body>");
            outputFile.Close();
        }
        static void Main(string[] args)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(".\\URLlist.txt");
            string url;
            ConcurrentBag<string> URLs = new ConcurrentBag<string>();
            //while ((url = sr.ReadLine()) != null)
            //{
            //    makefile(url);
            //}
            while ((url = sr.ReadLine()) != null)
            {
                URLs.Add(url);
            }
            Parallel.ForEach(URLs, iurl =>
            {
                    makefile(iurl);
            });
        }
    }
}
