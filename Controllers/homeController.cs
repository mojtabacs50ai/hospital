using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hospital.Models;
using System.Web.Mvc;
using System.Globalization;
using System.Web.Security;
using System.Text;
using System.Threading;


namespace hospital.Controllers
{
    public class homeController : Controller
    {

        datap context = new datap();


        // GET: home
        public ActionResult Index()
        {
            int docs = context.tbl_Doctors.Count();

            int dep = context.tbl_skills.Count();

            int awards = context.tbl_Awards.Count();

            int research = context.tbl_Research.Count();

            int skills = context.tbl_skills.Count();

            ViewBag.docs = docs;

            ViewBag.dep = dep;

            ViewBag.awards = awards;

            ViewBag.research = research;

            ViewBag.skills = skills;

            var res = context.View_Comm.ToList();

            string lang = dl();


            ViewBag.title = res.Where(x => x.Name == "تیتر اصلی" && x.Language==lang).Select(x => x.valuee).SingleOrDefault();

            ViewBag.bannerimg= res.Where(x => x.Name == "عکس اصلی").Select(x => x.valuee).SingleOrDefault();

            return View();
        }

        [HttpPost]
        public ActionResult getdep()
        {

            var dep = context.tbl_skills.Where(x=> x.pkID!=1005).Select(x=> new {x.pkID,x.skill}).ToList();
            return Json(dep, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]


        public ActionResult getdocs(int dep)
        {

            var docs = context.tbl_Doctors.Where(x => x.fkSkill == dep).Select(x => new { x.pkID, x.Name, x.Family }).ToList();

            return Json(docs, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult getvisittype(int dep)
        {

            var visittype = context.View_VisitPerDoctors.Where(x => x.fkDocID == dep).Select(x => new { x.fkVisitID, x.Type }).ToList();
            return Json(visittype, JsonRequestBehavior.AllowGet);

        }

        public ActionResult datetimepicker()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]


        public ActionResult addvisit(int docid, int visitid, string visitdatetime, string pid)
        {
            int intpid = 0;


            
            if (pid != "")
            {
                var p = context.tbl_Patient.Where(x => x.NationalCode == pid).SingleOrDefault();
                if (p == null)
                {
                    return Json(new { status = 2, nv = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    intpid = p.pkID;
                }
            }

            string[] vdatetime = visitdatetime.Split(' ');
            string[] vdate = vdatetime[0].Split('/');
            string[] vtime = vdatetime[1].Split(':');

            string s = vdate[0];



            int y = MyExtensions.PersianToEnglish(vdate[0]);
            int m = MyExtensions.PersianToEnglish(vdate[1]);
            int d = MyExtensions.PersianToEnglish(vdate[2]);
            int h = MyExtensions.PersianToEnglish(vtime[0]);
            int min = MyExtensions.PersianToEnglish(vtime[1]);

            PersianCalendar pc = new PersianCalendar();

            DateTime a = pc.ToDateTime(y, m, d, h, min, 0, 0);

            double duration = context.View_VisitPerDoctors.Where(x => x.fkDocID == docid && x.fkVisitID == visitid).Select(x => x.Duration).Single();

            var overlap1 = context.tbl_Visit.Where(x => x.fkDocID == docid && x.SDate <= a && x.EDate > a).ToList();
            if (overlap1.Count != 0)
            {
                return Json(new { status = 3, nv = 0 }, JsonRequestBehavior.AllowGet);
            }
            if (intpid > 0)
            {
                var overlap2 = context.tbl_Visit.Where(x => x.fkPID == intpid && x.SDate <= a && x.EDate > a).ToList();
                if (overlap2.Count != 0)
                {
                    return Json(new { status = 4, nv = 0 }, JsonRequestBehavior.AllowGet);
                }

            }
            tbl_Visit nv = new tbl_Visit();

            nv.fkDocID = docid;
            nv.SDate = a;
            nv.EDate = a.AddMinutes(duration);
            if (pid == "")
            {
                nv.fkPID = null;
            }
            else
            {
                nv.fkPID = intpid;
            }
            nv.fkVTID = visitid;
            nv.fkVisitStatus = 1;

            context.tbl_Visit.Add(nv);
            context.SaveChanges();

            var thisvisit = context.View_Visit.Where(x => x.fkDocID == docid && x.SDate == nv.SDate && x.EDate == nv.EDate).SingleOrDefault();

            var idtext = Encoding.UTF8.GetBytes(thisvisit.pkID.ToString());
            var encryptedID = Convert.ToBase64String(MachineKey.Protect(idtext, "mojtaba"));
            thisvisit.hashid = encryptedID;

            return Json(new { status = 1, nv = thisvisit }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getvisit(int doc)
        {

            var visit = context.View_Visit.Where(x => x.fkDocID == doc && x.fkPID == null).Select(X => new { X.pkID, X.PDate, X.PTime }).ToList();


            return Json(visit, JsonRequestBehavior.AllowGet);



        }

        public ActionResult setvisit(int vn, string namee, string phone, string family)
        {

            var c = context.tbl_Visit.Where(x => x.pkID == vn).SingleOrDefault();

            int state = 0;

            if (c.fkPID == null) 
            {

                int pid = 0;
                var p = context.tbl_Patient.Where(x => x.Mobile == phone).SingleOrDefault();

                if (p == null)
                {

                    tbl_Patient newp = new tbl_Patient();

                    newp.Name = namee;
                    newp.Family = family;
                    newp.Mobile = phone;

                    context.tbl_Patient.Add(newp);
                    context.SaveChanges();
                    var np = context.tbl_Patient.Where(x => x.Mobile == phone).SingleOrDefault();
                    pid = np.pkID;
                }

                else
                {
                    pid = p.pkID;
                }

                var v = context.tbl_Visit.Where(x => x.pkID == vn).SingleOrDefault();
                v.fkPID = pid;
                v.fkVTID = 1;
                v.EDate = v.SDate.AddMinutes(20);
                context.SaveChanges();
                state = 2;//نوبت دهی انجام شد
        }

            else
            {
                state = 1;//نوبت پر است

            }
            return Json(state, JsonRequestBehavior.AllowGet);

        }

        public ActionResult recept()
        {
            ViewBag.title = hospital.res.res1.m_dashboard;
            return View();
        }

        public ActionResult visit_management()
        {
            ViewBag.title = hospital.res.res1.m_visit;
            return View();

        }

        public ActionResult visit_type()
        {
            ViewBag.title = hospital.res.res1.m_visit_type;
            return View();

        }

        public ActionResult login()
        {

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login_check(int pn,string pass)
        {

            int status = 0;

            var user = context.tbl_Doctors.Where(x => x.PersonalNum == pn).SingleOrDefault();
            if (user != null)
            {
                if (user.password == pass)
                {
                    // Session["userid"] = user.pkID;

                    var cookieText = Encoding.UTF8.GetBytes(user.pkID.ToString());
                    var encryptedValue = Convert.ToBase64String(MachineKey.Protect(cookieText, "mojtaba"));


                    Response.Cookies["iiiid"].Value = encryptedValue;

                    Response.Cookies["iiiid"].Expires = DateTime.Now.AddDays(500);
                    status = 1; //login ok

                }
                else
                {
                    status = 2; //wrong pass
                }
            }

            else
            {
                status = 3; //dosnt match pn
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }


        public ActionResult chlang(string lang)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture =new CultureInfo(lang);
            Response.Cookies["lang"].Value = lang;
            Response.Cookies["lang"].Expires = DateTime.Now.AddDays(500);
            return RedirectToAction("index");
        }

        public ActionResult getvisits()
        {
            var visits = context.View_Visit.OrderBy(x=> x.SDate).ToList();

            foreach(var item in visits)
            {

                var idText = Encoding.UTF8.GetBytes(item.pkID.ToString());
                var encryptedid = Convert.ToBase64String(MachineKey.Protect(idText, "mojtaba"));
                item.hashid = encryptedid;
            }

            return Json(visits, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult removevisit(string vid)
        {
            var bytes = Convert.FromBase64String(vid);
            var output = MachineKey.Unprotect(bytes, "mojtaba");
            string result = Encoding.UTF8.GetString(output);

            int id = int.Parse(result);

            var visit = context.tbl_Visit.Where(x => x.pkID == id).SingleOrDefault();

            context.tbl_Visit.Remove(visit);
            context.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public ActionResult getstatus() {

            var status = context.tbl_VisitStatus.Select(x => new { x.pkID, x.VisitStatus }).ToList();

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult setstatus(int state, string sid)
        {

            var bytes = Convert.FromBase64String(sid);
            var output = MachineKey.Unprotect(bytes, "mojtaba");
            string result = Encoding.UTF8.GetString(output);

            int id = int.Parse(result);

            int status = 0;

            string sname = "";

            var visit = context.tbl_Visit.Where(x => x.pkID == id).SingleOrDefault();

            if (visit != null)
            {
                visit.fkVisitStatus = state;

                context.SaveChanges();

                status = 1; //ok

                sname = context.tbl_VisitStatus.Where(x => x.pkID == state).Select(x => x.VisitStatus).SingleOrDefault();

            }
            return Json(new {status= status, sname= sname }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult get_VPD (int doc)
        {
            var vt = context.tbl_VisitType.Select(x => new { x.pkID, x.Type }).ToList();
            var vpd = context.View_VisitPerDoctors.Where(x => x.fkDocID == doc).ToList();
            return Json(new { vt = vt, vpd = vpd }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult chg_vpd(int vpdid ,bool state , int doc)
        {
            if(state)
            {
                tbl_VisitPerDoctors ntype = new tbl_VisitPerDoctors();
                ntype.fkDocID = doc;
                ntype.fkVisitID= vpdid;
                ntype.Duration = 20;
                context.tbl_VisitPerDoctors.Add(ntype);
                context.SaveChanges();
            }
            else
            {
                var vpd = context.tbl_VisitPerDoctors.Where(x => x.fkDocID== doc && x.fkVisitID == vpdid).Single();
                context.tbl_VisitPerDoctors.Remove(vpd);
                context.SaveChanges();
            }
            var res=context.tbl_VisitPerDoctors.Where(x=> x.fkDocID==doc && x.fkVisitID==vpdid).Select(x=>new {x.fkDocID,x.fkVisitID,x.Duration }).SingleOrDefault();
            if(res == null)
            {
                return Json("null", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addVisitType(string vt)
        {
            var res = context.tbl_VisitType.Where(x => x.Type == vt).SingleOrDefault();
            if (res != null) { return Json(new { state = false, vt = "نوبت تکراری است" }, JsonRequestBehavior.AllowGet); }
            tbl_VisitType nvt = new tbl_VisitType();
            nvt.Type = vt;
            context.tbl_VisitType.Add(nvt);

            return Json(new { state = true, vt = vt }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult deleteVisitType(string vt)
        {
            try
            {
                var res = context.tbl_VisitType.Where(x => x.Type == vt).Single();
                context.tbl_VisitType.Remove(res);
                context.SaveChanges();
                return Json(new { state = true, vt = vt }, JsonRequestBehavior.AllowGet);
            }
            
            catch(Exception e)
            {
                return Json(new { state = false, vt =e.HResult}, JsonRequestBehavior.AllowGet);
            }

        }


        public void logout()
        {

            Response.Cookies["iiiid"].Expires = DateTime.Now.AddDays(-1);

            Session.Abandon();

            Response.Redirect("/Home/login");

        }

        public void setname()
        {
            var bytes = Convert.FromBase64String(Request.Cookies["iiiid"].Value);
            var output = MachineKey.Unprotect(bytes, "mojtaba");
            string result = Encoding.UTF8.GetString(output);



            var userid = int.Parse(result);

            var user = context.tbl_Doctors.Where(x => x.pkID == userid).SingleOrDefault();

            Session["username"] = user.Name + " " + user.Family;
        }
       
        public string dl()
        {
            if (Request.Cookies["lang"] != null)
            {
                if (Request.Cookies["lang"].Value != null)
                {
                    return Request.Cookies["lang"].Value;
                }
                else
                {
                    return "fa";
                }
            }
            else
            {
                return "fa";
            }

        }
                   

    }

    public static class MyExtensions
    {
        public static int PersianToEnglish(this string persianStr)
        {
            Dictionary<char, char> LettersDictionary = new Dictionary<char, char>
            {
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9',
                ['0'] = '0',
                ['1'] = '1',
                ['2'] = '2',
                ['3'] = '3',
                ['4'] = '4',
                ['5'] = '5',
                ['6'] = '6',
                ['7'] = '7',
                ['8'] = '8',
                ['9'] = '9'

            };
            foreach (var item in persianStr)
            {
                persianStr = persianStr.Replace(item, LettersDictionary[item]);
            }
            return int.Parse(persianStr);
        }
    }


}


