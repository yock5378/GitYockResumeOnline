using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YockResume.Models;

namespace YockResume.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            if ((string)TempData["MessageSend"] == "Success")
                ViewBag.MessageSend = "Success";
            else
                ViewBag.MessageSend = null;
            return View();
        }

        public ActionResult CreateData(UserContact contact_form)
        {
            var db = new SampleContext();
            //新增一筆資料
            var addData = new UserContact();
            var createDate = DateTime.Now;

            addData.Name = contact_form.Name;
            addData.Email = contact_form.Email;
            addData.Phone = contact_form.Phone;
            addData.Message = contact_form.Message;
            addData.CreateOn = createDate;
            db.UserContacts.Add(addData);
            db.SaveChanges();  
            TempData["MessageSend"] = "Success";
            return RedirectToAction("Index");
        }
    }
}