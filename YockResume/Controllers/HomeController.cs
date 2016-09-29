using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace YockResume.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public JsonResult CCC()
        { 
            List<string> CCClist = new List<string>();
            CCClist.Add("咕嚕嚕嚕嚕嚕...");
            CCClist.Add("窩顆顆顆顆顆...");
            CCClist.Add("屋窩窩窩窩窩...");
            CCClist.Add("嚕啦啦啦啦啦...");
            CCClist.Add("嚕啦啦啦啦啦...");
            return Json(CCClist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadJson()
        {
            using (StreamReader r = new StreamReader("../json/content.json"))
            {
                string json = r.ReadToEnd();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult LoadJson2()
        {
            using (StreamReader r = new StreamReader("D:/160917_Resume_Yock/YockResume/json/contentTest.json"))
            {
                string json = r.ReadToEnd();
                Item item = JsonConvert.DeserializeObject<Item>(json);
                return Json(item, JsonRequestBehavior.AllowGet);
            }

        }

        public class Item
        {
            public string question;
            public string reply;
            public List<string> test;
        }
    }
}