using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using System.IO;

using Google.Apis.Customsearch.v1;


namespace TestSolution.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }


        //-----------Solution 1-------------\\
        [HttpGet]
        public ActionResult Solution_1()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Solution_1(string Query)
        {   
            

            string key = "AIzaSyCIQtWuCbPHDZYtGw5vqPp1TOPIv5Iup0w";
            string searchEngineId = "004596914479420657830:aayargcrndm";
            
            CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = key });
            Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(Query);
            listRequest.Cx = searchEngineId;
            var search = listRequest.Execute();
            foreach (var item in search.Items)
            {
                ViewBag.GoogleResulTitle = item.Title;
                ViewBag.GoogleResultURL = item.FormattedUrl;
                break;
            }

            try
            {
                string Accounkey = "4aXH2shYEVqKMEUzrXsKT3gasno16p37EaQN9gK3L6o=";
                string rootUrl = "https://api.datamarket.azure.com/Bing/SearchWeb/";

                var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));
                string market = "en-us";
                bingContainer.Credentials = new NetworkCredential(Accounkey, Accounkey);

                var webQuery = bingContainer.Web(Query, null, null, market, null, null, null, null);
                webQuery = webQuery.AddQueryOption("$top", 1);

                var webResults = webQuery.Execute();
                foreach (var result in webResults)
                {
                    ViewBag.BingResulTitle = result.Title;
                    ViewBag.BingResultURL = result.Url;
                }
            }
            catch (Exception ex)
            {

                string innerMessage =  (ex.InnerException != null) ? ex.InnerException.Message : String.Empty;
                ViewBag.BingResulTitle = ex.Message;
                ViewBag.BingResultURL = innerMessage;
            }
            
            return View();
        }

        //-----------Solution 2-------------\\    
      
        [HttpGet]
        public ActionResult Solution_2()
        {
            string currDir = Environment.CurrentDirectory.ToString();

            ViewBag.inputText = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/App_Data/input.txt"));
            ViewBag.patternsText = System.IO.File.ReadAllLines(HttpContext.Server.MapPath("~/App_Data/patterns.txt"));
            ViewBag.Result = "";
            return View();
        }

        [HttpPost]
        public ActionResult Solution_2(string modes)
        {
            string[] seporators = {". ","! ","? ","... ","."};
            string valuetext= System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/App_Data/input.txt")) ;
            string[] input = valuetext.Split(seporators, StringSplitOptions.RemoveEmptyEntries);

            string[] patterns = System.IO.File.ReadAllLines(HttpContext.Server.MapPath("~/App_Data/patterns.txt"));
            List<string> result = new List<string>();
            int index = 0;

            switch( modes)
            { 
                case null:                
                    result.Add("Вы не выбрали режим поиска");
                    break;

                case "Exactly":                
                    foreach (var pattern in patterns)
                    {
                        foreach(var line in input)
                        {
                            if (pattern == line) result.Add(line);
                        }
                    }
                    break;

                case "Somewhere" :                
                    foreach (var line in input)               
                    {
                        foreach (var pattern in patterns)
                        {
                            if (line.Contains(pattern)) result.Add(line);
                        }
                    }
                    break;

                case "WithEditDistance":
                    foreach (var pattern in patterns)
                    {
                        foreach (var line in input)
                        {
                            if (pattern.Count() == line.Count())
                            {
                                index = 0;
                                for(int i=0;i<line.Count();++i)
                                {
                                    if (line[i] != pattern[i]) ++index;
                                    if (index > 1) break;
                                }

                                if (index <= 1) result.Add(line);
                            }
                        }
                    }
                    break;
            }

            result = result.Distinct().ToList();

            ViewBag.inputText = valuetext;
            ViewBag.patternsText = patterns;
            ViewBag.Result = result;

            return View();
        }
    }
}