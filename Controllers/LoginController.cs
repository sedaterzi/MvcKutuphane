using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;
using System.Web.Security;//üye paneli yetkilendirmesi için
using System.Net.Mail;
using System.Net;

namespace MvcKutuphane.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        DBKUTUPHANEEntities db = new DBKUTUPHANEEntities();
        private string code = null;

        public ActionResult GirisYap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GirisYap(TBLUYELER p)
        {
            var bilgiler = db.TBLUYELER
                .FirstOrDefault(x => x.MAIL == p.MAIL && x.SIFRE == p.SIFRE);
            if (bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.MAIL, false);
                Session["Mail"] = bilgiler.MAIL.ToString();
                return RedirectToAction("Index", "Panelim");
            }
            else
            {
                return View();
            }
        }

        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SifreSifirlama(string code, string yenisifre)
        {
            return View();
        }
        public ActionResult SifreSifirlamaKod(string code, string yenisifre)
        {
            var sifrekodu = db.TBLSIFREKOD.Where(y => y.KOD == code)
                .FirstOrDefault();
            if (sifrekodu != null)
            {
                var uye = db.TBLUYELER.Find(sifrekodu.UYE);
                uye.SIFRE = yenisifre;
               // db.Update(uye);
                sifrekodu = null;
                db.SaveChanges();

                return RedirectToAction("GirisYap");

            }

            return RedirectToAction("GirisYap");
        }

        public string KodUret()
        {
            if (code == null)
            {
                Random rand = new Random();
                code = "";
                for (int i = 0; i < 6; i++)
                {
                    /*48: 0--58:9-->ASCII karşılıkları oluyor. 
                     0-9 arası 6 haneli sayı ürettir*/
                    char tmp = Convert.ToChar(rand.Next(48, 58));
                    code += tmp;
                }
            }

            return code;
        }
        public ActionResult KodGonder(string email)
        {

            var uye = db.TBLUYELER.Where(x => x.MAIL == email).FirstOrDefault();
            if (uye != null)
            {
                db.TBLSIFREKOD.Add(new TBLSIFREKOD { UYE = uye.ID, KOD = KodUret() });
                db.SaveChanges();

                string icerik = "MVC Kütüphane Üye Paneli Şifre Sıfırlama Aktivasyon Kodunuz:  " + KodUret() + "";
                string konu = "Parola Sıfırlama";
                //parametreleri: (gönderici, gönderilecek mail-parametre,konu,içerik)
                MailMessage msj = new MailMessage("mvckutuphane@gmail.com", email, konu, icerik);
                msj.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                NetworkCredential cre = new NetworkCredential("mvckutuphane@gmail.com", "KutuphaneYonetimi");
                smtp.Credentials = cre;
                smtp.EnableSsl = true;
                smtp.Send(msj);

                return RedirectToAction("SifreSifirlama");

            }

            return RedirectToAction("SifremiUnuttum");
        }
    }
}