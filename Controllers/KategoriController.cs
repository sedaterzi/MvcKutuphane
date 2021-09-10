using System.Linq;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;
using PagedList;
using PagedList.Mvc;

namespace MvcKutuphane.Controllers
{
    public class KategoriController : Controller
    {
        // GET: Kategori
        DBKUTUPHANEEntities db = new DBKUTUPHANEEntities();
        public ActionResult Index()
        {
            var degerler = db.TBLKATEGORI.Where(X=>X.DURUM==true).OrderBy(X=>X.AD).ToList();
            return View(degerler);
        }
        [HttpGet]// Ekle sayfası geldiğinde dbye otomatik ekleme yapmasını engellemek için boş ActionResult çalışssın
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]//Sayfada ekle dediğimizde yani bir işlem yaptığımızda çalışmasını istediğimiz Action Result bloğu
            public ActionResult KategoriEkle(TBLKATEGORI p)
        {
            if (!ModelState.IsValid)
            {
                return View("KategoriEkle");
            }
            db.TBLKATEGORI.Add(p);//p değerini TBLKATEGORİ'ye ekle
            db.SaveChanges();//Değişiklikleri kaydet
            return RedirectToAction("Index") ;
        }

        public ActionResult KategoriSil(int id)
        {
            var kategori = db.TBLKATEGORI.Find(id);
           //db.TBLKATEGORI.Remove(kategori);//ilişkili tabloda bu yanlış
           kategori.DURUM = false;

            db.SaveChanges();//Değişiklikleri kaydet
            return RedirectToAction("Index");
        }

        public ActionResult KategoriGetir(int id)
        {
            var ktg = db.TBLKATEGORI.Find(id);
            return View("KategoriGetir", ktg);
        }
        public ActionResult KategoriGuncelle(TBLKATEGORI p)
        {
            var k = db.TBLKATEGORI.Find(p.ID);
            k.AD = p.AD;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}