using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;
using PagedList;
using PagedList.Mvc;


namespace MvcKutuphane.Controllers
{
    public class OduncController : Controller
    {
        // GET: Odunc
        DBKUTUPHANEEntities db = new DBKUTUPHANEEntities();

        public ActionResult Index()
        {
            var degerler = db.TBLHAREKET.Where(x => x.ISLEMDURUM == false).OrderBy(x => x.ALISTARIH).ToList();
            return View(degerler);
        }
        [HttpGet]
        public ActionResult OduncVer()
        {
            List<SelectListItem> deger1 = (from x in db.TBLUYELER.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.AD + " " + x.SOYAD,
                                               Value = x.ID.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from y in db.TBLKITAP.Where(x => x.DURUM == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = y.AD,
                                               Value = y.ID.ToString()
                                           }).ToList();

            List<SelectListItem> deger3 = (from z in db.TBLPERSONEL.ToList()
                                           select new SelectListItem
                                           {
                                               Text = z.PERSONEL,
                                               Value = z.ID.ToString()
                                           }).ToList();

            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            return View();
        }
        [HttpPost]//Sayfada ekle dediğimizde yani bir işlem yaptığımızda çalışmasını istediğimiz Action Result bloğu
        public ActionResult OduncVer(TBLHAREKET p)
        {
            if (!ModelState.IsValid)
            {
                return View("OduncVer");
            }
            var d1 = db.TBLUYELER.Where(x => x.ID == p.TBLUYELER.ID).FirstOrDefault();
            var d2 = db.TBLKITAP.Where(y => y.ID == p.TBLKITAP.ID).FirstOrDefault();
            var d3 = db.TBLPERSONEL.Where(z => z.ID == p.TBLPERSONEL.ID).FirstOrDefault();
            p.TBLUYELER = d1;
            p.TBLKITAP = d2;
            p.TBLPERSONEL = d3;
            p.ISLEMDURUM = false;
            db.TBLHAREKET.Add(p);
            db.SaveChanges();
            return View("IslemBasarili");
        }
        public ActionResult Odunciade(TBLHAREKET p)
        {
            var odn = db.TBLHAREKET.Find(p.ID);
            DateTime d1 = DateTime.Parse(odn.IADETARIH.ToString());
            DateTime d2 = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            TimeSpan d3 = d2 - d1;

            ViewBag.dgr = d3.TotalDays;
            return View("Odunciade", odn);
        }

        public ActionResult OduncGuncelle(TBLHAREKET p)
        {
            var hrk = db.TBLHAREKET.Find(p.ID);
            hrk.UYEGETIRTARIH = p.UYEGETIRTARIH;
            hrk.ISLEMDURUM = true;
            hrk.TBLKITAP.DURUM = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult OduncDetay(TBLHAREKET p)
        {

            var odn = db.TBLHAREKET.Find(p.ID);

            DateTime d1 = DateTime.Parse(odn.IADETARIH.ToString());
            DateTime d2 = DateTime.Parse(odn.UYEGETIRTARIH.ToString());
            TimeSpan d3 = d2 - d1;

            if (d3.TotalDays < 0)
            {
                d3 = d1 - d2;
                ViewBag.dgr = (d3.TotalDays) + " Gün Erken Getirdi";

            }
            else
            {

                ViewBag.dgr = (d3.TotalDays) + " Gün Geç Getirdi!";

            }
            ViewBag.dgr1 = odn.ID;
            return View("OduncDetay", odn);
        }




        public ActionResult MailAt()
        {
            return View();
        }
        public ActionResult MailGonder(string email)
        {
            var uye = db.TBLUYELER.Where(x => x.MAIL == email).FirstOrDefault();
            if (uye != null)
            {
                string isim = uye.AD;
                string soyad = uye.SOYAD;
                string icerik = " Sayın " + isim + " " + soyad + ", Kütüphanemizden almış olduğunuz kitabın teslim tarihi yaklaşmaktardır veya geçmiştir. <br> Başkalarının da kitaplarımızdan faydalanabilmesi için lütfen zamanında teslim ediniz.<br> Geç getirdiğiniz her gün için '1 TL' ceza uygulanmaktadır.<br><br><br> MVC Kütüphane Yönetimi";
                string konu = "Ödünç Hatırlatma";

                MailMessage msj = new MailMessage("mvckutuphane@gmail.com", email, konu, icerik);//parametreleri: (gönderici, gönderilecek mail-parametre,konu,içerik)
                msj.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                NetworkCredential cre = new NetworkCredential("mvckutuphane@gmail.com", "KutuphaneYonetimi");
                smtp.Credentials = cre;
                smtp.EnableSsl = true;
                smtp.Send(msj);

                return View("MailBasarili");

            }
            else
            {
                return View("MailBasarisiz");

            }

        }
    }
}