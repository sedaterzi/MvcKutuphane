using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;
using MvcKutuphane.Models.Siniflarim;
using System.Net;

namespace MvcKutuphane.Controllers
{
    public class vitrinController : Controller
    {
        // GET: vitrin
        DBKUTUPHANEEntities db = new DBKUTUPHANEEntities();
        [HttpGet]
        public ActionResult Index()
        {
            Class1 cs = new Class1();
            cs.Deger1 = db.TBLKITAP.ToList();
            cs.Deger2 = db.TBLHAKKIMIZDA.ToList();
            var degerler = db.TBLKITAP.ToList();
       
            

            return View(cs);
        }
        [HttpPost]
        public ActionResult Index(TBLILETISIM t)
        {
            db.TBLILETISIM.Add(t);
            db.SaveChanges();
            MailMessage mailim = new MailMessage();
            mailim.To.Add("mvckutuphane@gmail.com");
            mailim.From = new MailAddress("mvckutuphane@gmail.com");
            mailim.Subject = "MVC Kütüphane Yönetimine:  " + t.KONU;
            mailim.Body = "Sayın Yetkili, " + t.AD + " kişisinden gelen 1 adet mesajınız var.. <br>  Mesaj İçeriği: <br>  " + t.MESAJ;
            mailim.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential("mvckutuphane@gmail.com", "KutuphaneYonetimi");
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(mailim);
                TempData["Message"] = "Mesajınız gönderilmiştir. En kısa zamanda size geri dönüş sağlanacaktır. ";

            }
            catch (Exception ex)
            {
                TempData["Message"] = "Mesajınız gönderilemedi.Hata nedeni: " + ex.Message;
                 
            }
            return RedirectToAction("Index");

        }
       
    }
}