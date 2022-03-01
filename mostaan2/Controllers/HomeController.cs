using mostaan2.Classes;
using mostaan2.Model;
using mostaan2.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mostaan2.Controllers
{
    public class HomeController : Controller
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        Model.Context dbcontext = new Model.Context();
        public ActionResult Index(string error)
        {
            
            ViewBag.error = String.IsNullOrEmpty(error)? "": error;
            return View();

        }
        public ActionResult login(string password,string name)
        {
           
            user userIem = dbcontext.users.SingleOrDefault(x => x.pasdari_Code == name && x.password == password);
            if (userIem != null)
            {
                List<Model.permission> lst = dbcontext.permissions.Where(x => x.userID == userIem.ID).ToList();
                List<Model.permission> kk = dbcontext.permissions.Where(x => x.serctionID == "1" || x.serctionID == "2" || x.serctionID == "3" || x.serctionID == "4" || x.serctionID == "5" || x.serctionID == "6" || x.serctionID == "7" || x.serctionID == "8" || x.serctionID == "9" || x.serctionID == "10").ToList();


                foreach(var item in kk)
                {
                    dbcontext.permissions.Remove(item);
                }


                dbcontext.SaveChanges();
                Response.Cookies["name"].Value = userIem.name;
                Response.Cookies["jaygah"].Value = userIem.jaygah;
                string srt = "";
                string action = "";
                foreach (var item in lst)
                {
                    if (lst.IndexOf(item) == 0 )
                    {
                        switch (item.serctionID)
                        {
                            case "11":
                                action = "income_List";
                                break;
                            case "21":
                                action = "outcome_List";
                                break;
                            case "31":
                                action = "tamin_List";
                                break;
                            case "41":
                                action = "report_List";
                                break;
                            case "51":
                                action = "User_List";
                                break;
                            case "61":
                                action = "Markaz_List";
                                break;
                            case "71":
                                action = "Komite_List";
                                break;
                            case "81":
                                action = "bank_List";
                                break;
                            case "91":
                                action = "check_List";
                                break;
                        }
                    }
                    srt += item.serctionID + ",";
                }
                Session["permission"] = srt;
                return RedirectToAction(action);



            }

            return RedirectToAction("index","Home", new { error = "1" });
        }
        public ActionResult daryaftiReport(daryaftiReportVM model)
        {
            if (model == null)
            {
                model = new daryaftiReportVM();
            }
            return View(model);
        }



        public ActionResult Bakhsh_List()
        {

            string permisson = Session["permission"] as string;

            string see = permisson.Contains("61,") ? "1" : "";
            string edit = permisson.Contains("62,") ? "1" : "";
            string add = permisson.Contains("63,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            List<user> userlist = dbcontext.users.ToList();
            List<Model.bakhsh> lst = new List<bakhsh>();
            var q = dbcontext.bakhshes.Where(x => x.ID == x.parent || x.final == 0).OrderByDescending(x => x.ID);// && x.isDone != true
            if (q != null)
            {
                lst = q.ToList();
            }
            bakhshListVM model = new bakhshListVM()
            {
                bakhshList = lst,
                userList = userlist,
                add = add,
                edit = edit
            };

            return View(model);
        }
        public void addBakhshForm(string title, string masool, string janeshin, string bakhshID)
        {

            if (String.IsNullOrEmpty(bakhshID))
            {
                string id = RandomString(10);
                using (Context dbcontext = new Context())
                {
                    DateTime nowdatetime = DateTime.Now;
                    bakhsh model = new bakhsh();

                    model.master = "1";
                    model.ID = id;
                    model.parent = id;
                    model.date = nowdatetime;
                    model.time = nowdatetime.TimeOfDay;
                    model.title = title;
                    model.masoul = masool;
                    model.janeshin = janeshin;
                    model.changer = "admin";
                    dbcontext.bakhshes.Add(model);
                    dbcontext.SaveChanges();

                }
            }
            else
            {
                bakhsh model = dbcontext.bakhshes.SingleOrDefault(x => x.ID == bakhshID);
                model.masoul = masool;
                model.janeshin = janeshin;
                model.title = title;
                dbcontext.SaveChanges();
            }



        }

        public void deletBakhsh(string id)
        {
            bakhsh model = dbcontext.bakhshes.SingleOrDefault(x => x.ID == id);
            dbcontext.bakhshes.Remove(model);
            dbcontext.SaveChanges();
        }
        public ContentResult getBakhshDetail(string id)
        {
            bakhsh model = dbcontext.bakhshes.SingleOrDefault(x => x.ID == id);
            return Content(JsonConvert.SerializeObject(model));
        }


        public ActionResult endReport(string id)
        {
            shenasname delitem = dbcontext.shenasnames.SingleOrDefault(x => x.ID == id);
            delitem.isEnded = 1;
            dbcontext.SaveChanges();
            return Content("");
        }
        public ContentResult deleteReport(string id)
        {
            try
            {
                shenasname delitem = dbcontext.shenasnames.SingleOrDefault(x => x.ID == id);
                if (delitem.final == 1)
                    return Content("error");


                List<shenasnameGam> gamList = dbcontext.shenasnameGams.Where(x => x.shenasnameID == id).ToList();
                List<shenasnameFounder> founderList = dbcontext.shenasnameFounders.Where(x => x.shenasnameID == id).ToList();
                List<ejraeiat> ejraeatList = dbcontext.ejraeiats.Where(x => x.shenasnameID == id).ToList();
                List<sarmaye> sarmayeslist = dbcontext.sarmayes.Where(x => x.shenasnameID == id).ToList();
                List<masrafi> masrafislist = dbcontext.masrafis.Where(x => x.shenasnameID == id).ToList();
                List<edari> edarislist = dbcontext.edaris.Where(x => x.shenasnameID == id).ToList();
                List<omrani> omranilist = dbcontext.omranis.Where(x => x.shenasnameID == id).ToList();
                List<gharardad> gharardadslist = dbcontext.gharardads.Where(x => x.shenasnameID == id).ToList();
                List<sayer> sayerslist = dbcontext.sayers.Where(x => x.shenasnameID == id).ToList();
                List<tashvighi> tashvighislist = dbcontext.tashvighis.Where(x => x.shenasnameID == id).ToList();

                foreach (var item in gamList)
                {
                    dbcontext.shenasnameGams.Remove(item);
                }
                foreach (var item in founderList)
                {
                    dbcontext.shenasnameFounders.Remove(item);
                }
                foreach (var item in ejraeatList)
                {
                    dbcontext.ejraeiats.Remove(item);
                }
                foreach (var item in sarmayeslist)
                {
                    dbcontext.sarmayes.Remove(item);
                }
                foreach (var item in masrafislist)
                {
                    dbcontext.masrafis.Remove(item);
                }
                foreach (var item in edarislist)
                {
                    dbcontext.edaris.Remove(item);
                }
                foreach (var item in omranilist)
                {
                    dbcontext.omranis.Remove(item);
                }
                foreach (var item in gharardadslist)
                {
                    dbcontext.gharardads.Remove(item);
                }
                foreach (var item in sayerslist)
                {
                    dbcontext.sayers.Remove(item);
                }
                foreach (var item in tashvighislist)
                {
                    dbcontext.tashvighis.Remove(item);
                }







                dbcontext.Entry(delitem).State = EntityState.Deleted;
                dbcontext.SaveChanges();

               

            }
            catch (Exception errror)
            {
                // message.Text = errror.InnerException.Message;
            }
            return Content("");
        }

        public ActionResult showCopies(string shenasnameID)
        {
            ViewBag.itemID = shenasnameID;

            shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == shenasnameID);
            string parent = shen.parent;
            ViewBag.isEnd = shen.isEnded;
            using (var dbcontext = new Model.Context())
            {
                List<Model.shenasname> lstdd = dbcontext.shenasnames.ToList();
                List<shenasname> lst = (from p in lstdd where (p.parent == parent) && (p.final == 1) select p).OrderByDescending(x => x.date).ThenByDescending(x => x.time).ToList();
                List<ViewModel.showCopiesVM> list = new List<ViewModel.showCopiesVM>();
                foreach (var item in lst)
                {
                    int index = lst.IndexOf(item);

                    string count = index == 0 ? "نسخه نهایی" : "نسخه " + (lst.Count() - (index)).ToString();
                    ViewModel.showCopiesVM vmitem = new ViewModel.showCopiesVM()
                    {
                        ID = item.ID,
                        count = count,
                        date = item.date.ToPersianDateString(),
                        changer = item.changer,
                    };
                    list.Add(vmitem);

                }

                return View(list);

            }


        }
        public ActionResult createCopy(string rowID)
        {


            string permisson = Session["permission"] as string;
            string see = permisson.Contains("41,") ? "1" : "";
            string edit = permisson.Contains("42,") ? "1" : "";
            string add = permisson.Contains("43,") ? "1" : "";
            if (edit == "")
                return RedirectToAction("login");
            string finalID = RandomString(10);
            using (Context dbcontext = new Context())
            {
                shenasname selecteditem = dbcontext.shenasnames.SingleOrDefault(x => x.ID == rowID && x.master == "1");
                if (selecteditem.master != "1")
                {

                }

                string sID = rowID;// RandomString(10);


                List<shenasnameGam> gamList = dbcontext.shenasnameGams.Where(x => x.shenasnameID == rowID).ToList();
                if (gamList != null)
                {

                    foreach (shenasnameGam item in gamList)
                    {
                        shenasnameGam newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.shenasnameGams.Add(newItem);
                    }
                }

                List<shenasnameFounder> founderList = dbcontext.shenasnameFounders.Where(x => x.shenasnameID == rowID).ToList();
                if (founderList != null)
                {

                    foreach (shenasnameFounder item in founderList)
                    {
                        shenasnameFounder newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.shenasnameFounders.Add(newItem);
                    }
                }

                List<ejraeiat> ejraeatList = dbcontext.ejraeiats.Where(x => x.shenasnameID == rowID).ToList();
                if (ejraeatList != null)
                {

                    foreach (ejraeiat item in ejraeatList)
                    {
                        ejraeiat newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.ejraeiats.Add(newItem);
                    }
                }
                List<sarmaye> sarmayeslist = dbcontext.sarmayes.Where(x => x.shenasnameID == rowID).ToList();
                if (sarmayeslist != null)
                {
                    foreach (sarmaye item in sarmayeslist)
                    {
                        sarmaye newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.sarmayes.Add(newItem);
                    }
                }
                List<masrafi> masrafislist = dbcontext.masrafis.Where(x => x.shenasnameID == rowID).ToList();
                if (masrafislist != null)
                {
                    foreach (masrafi item in masrafislist)
                    {
                        masrafi newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.masrafis.Add(newItem);
                    }
                }
                List<edari> edarislist = dbcontext.edaris.Where(x => x.shenasnameID == rowID).ToList();
                if (masrafislist != null)
                {
                    foreach (edari item in edarislist)
                    {
                        edari newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.edaris.Add(newItem);
                    }
                }
                List<omrani> omranilist = dbcontext.omranis.Where(x => x.shenasnameID == rowID).ToList();
                if (masrafislist != null)
                {
                    foreach (omrani item in omranilist)
                    {
                        omrani newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.omranis.Add(newItem);
                    }
                }
                List<gharardad> gharardadslist = dbcontext.gharardads.Where(x => x.shenasnameID == rowID).ToList();
                if (masrafislist != null)
                {
                    foreach (gharardad item in gharardadslist)
                    {
                        gharardad newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.gharardads.Add(newItem);
                    }
                }
                List<sayer> sayerslist = dbcontext.sayers.Where(x => x.shenasnameID == rowID).ToList();
                if (masrafislist != null)
                {
                    foreach (sayer item in sayerslist)
                    {
                        sayer newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.sayers.Add(newItem);
                    }
                }
                List<tashvighi> tashvighislist = dbcontext.tashvighis.Where(x => x.shenasnameID == rowID).ToList();
                if (masrafislist != null)
                {
                    foreach (tashvighi item in tashvighislist)
                    {
                        tashvighi newItem = item;
                        newItem.shenasnameID = sID;
                        dbcontext.tashvighis.Add(newItem);
                    }
                }

                shenasname model = new shenasname()
                {

                    comment = selecteditem.comment,
                    date = DateTime.Now,
                    time = DateTime.Now.TimeOfDay,
                    changer = selecteditem.changer,
                    dastgah = selecteditem.dastgah,
                    dateFrom = DateTime.Now,
                    title = selecteditem.title,
                    token = selecteditem.token,
                    parent = selecteditem.parent,
                    ID = finalID,
                    noshke = selecteditem.noshke,
                    dateTo = DateTime.Now,
                    master = selecteditem.master,
                    LvazifeT = selecteditem.LvazifeT,
                    markaz = selecteditem.markaz,
                    LvazifeHPR = selecteditem.LvazifeHPR,
                    LvazifeHPD = selecteditem.LvazifeHPD,
                    LrasmiT = selecteditem.LrasmiT,
                    LrasmiHPR = selecteditem.LrasmiHPR,
                    LrasmiHPD = selecteditem.LrasmiHPD,
                    LgharartamamT = selecteditem.LgharartamamT,
                    LgharartamamHPR = selecteditem.LgharartamamHPR,
                    DghararsaatHPD = selecteditem.DghararsaatHPD,
                    DghararsaatHPR = selecteditem.DghararsaatHPR,
                    DghararsaatT = selecteditem.DghararsaatT,
                    DgharartamamHPD = selecteditem.DgharartamamHPD,
                    DgharartamamHPR = selecteditem.DgharartamamHPR,
                    DgharartamamT = selecteditem.DgharartamamT,
                    DighararsaatHPD = selecteditem.DighararsaatHPD,
                    DighararsaatHPR = selecteditem.DighararsaatHPR,
                    DighararsaatT = selecteditem.DighararsaatT,
                    DigharartamamHPD = selecteditem.DigharartamamHPD,
                    DigharartamamHPR = selecteditem.DigharartamamHPR,
                    DigharartamamT = selecteditem.DigharartamamT,
                    DirasmiHPD = selecteditem.DirasmiHPD,
                    DirasmiHPR = selecteditem.DirasmiHPR,
                    DirasmiT = selecteditem.DirasmiT,
                    DivazifeHPD = selecteditem.DivazifeHPD,
                    DivazifeHPR = selecteditem.DivazifeHPR,
                    DivazifeT = selecteditem.DivazifeT,
                    DrasmiHPD = selecteditem.DrasmiHPD,
                    DrasmiHPR = selecteditem.DrasmiHPR,
                    DrasmiT = selecteditem.DrasmiT,
                    DvazifeHPD = selecteditem.DvazifeHPD,
                    DvazifeHPR = selecteditem.DvazifeHPR,
                    DvazifeT = selecteditem.DvazifeT,
                    FDghararsaatHPD = selecteditem.FDghararsaatHPD,
                    FDghararsaatHPR = selecteditem.FDghararsaatHPR,
                    FDghararsaatT = selecteditem.FDghararsaatT,
                    FDgharartamamHPD = selecteditem.FDgharartamamHPD,
                    FDgharartamamHPR = selecteditem.FDgharartamamHPR,
                    FDgharartamamT = selecteditem.FDgharartamamT,
                    FDrasmiHPD = selecteditem.FDrasmiHPD,
                    FDrasmiHPR = selecteditem.FDrasmiHPR,
                    FDrasmiT = selecteditem.FDrasmiT,
                    FDvazifeHPD = selecteditem.FDvazifeHPD,
                    FDvazifeHPR = selecteditem.FDvazifeHPR,
                    FDvazifeT = selecteditem.FDvazifeT,
                    final = 0,
                    FLghararsaatHPD = selecteditem.FLghararsaatHPD,
                    FLgharartamamT = selecteditem.FLgharartamamT,
                    FLghararsaatHPR = selecteditem.FLghararsaatHPR,
                    FLghararsaatT = selecteditem.FLghararsaatT,
                    FLgharartamamHPD = selecteditem.FLgharartamamHPD,
                    FLgharartamamHPR = selecteditem.FLgharartamamHPR,
                    FLrasmiHPD = selecteditem.FLrasmiHPD,
                    FLrasmiHPR = selecteditem.FLrasmiHPR,
                    FLrasmiT = selecteditem.FLrasmiT,
                    FLvazifeHPD = selecteditem.FLvazifeHPD,
                    FLvazifeHPR = selecteditem.FLvazifeHPR,
                    FLvazifeT = selecteditem.FLvazifeT,
                    hadaf = selecteditem.hadaf,
                    isDone = false,
                    LghararsaatHPD = selecteditem.LghararsaatHPD,
                    LghararsaatHPR = selecteditem.LghararsaatHPR,
                    LghararsaatT = selecteditem.LghararsaatT,
                    LgharartamamHPD = selecteditem.LgharartamamHPD,
                    ZDighararsaatHPD = selecteditem.ZDighararsaatHPD,
                    ZDighararsaatHPR = selecteditem.ZDighararsaatHPR,
                    ZDighararsaatT = selecteditem.ZDighararsaatT,
                    ZDigharartamamHPD = selecteditem.ZDigharartamamHPD,
                    ZDigharartamamHPR = selecteditem.ZDigharartamamHPR,
                    ZDigharartamamT = selecteditem.ZDigharartamamT,
                    ZDirasmiHPD = selecteditem.ZDirasmiHPD,
                    ZDirasmiHPR = selecteditem.ZDirasmiHPR,
                    ZDirasmiT = selecteditem.ZDirasmiT,
                    ZDivazifeHPD = selecteditem.ZDivazifeHPD,
                    ZDivazifeHPR = selecteditem.ZDivazifeHPR,
                    ZDivazifeT = selecteditem.ZDivazifeT,
                    bayganiFile = selecteditem.bayganiFile,
                    gantFile = selecteditem.gantFile,
                    gharardadFile = selecteditem.gharardadFile,
                    listmavadFile = selecteditem.listmavadFile,
                    mojavezFile = selecteditem.mojavezFile,
                    motamamFile = selecteditem.motamamFile,
                    peyvastFile = selecteditem.peyvastFile,
                    pishraftFile = selecteditem.pishraftFile,








                };

                dbcontext.shenasnames.Add(model);
                dbcontext.SaveChanges();

            }
            return RedirectToAction("showReport", new { id = finalID });

        }
        
        public ActionResult Markaz_List()
        {


            List<user> userlist = dbcontext.users.ToList();
            List<Model.markaz> mlst = dbcontext.markazs.ToList();// new List<markaz>();
            List<Model.bakhsh> bakhshList = dbcontext.bakhshes.ToList();
            //var q = dbcontext.markazs.Where(x => x.ID == x.parent || x.final == 0).OrderByDescending(x => x.ID);// && x.isDone != true
            //if (q != null)
            //{
            //    mlst = q.ToList();
            //}
            markazListVM model = new markazListVM()
            {
                markazList = mlst,
                userList = userlist,
                bakhshList = bakhshList
            };

            return View(model);
        }
        public void addMarkazForm(string title, string masool, string janeshin, string bakhsh, string ID)
        {

            if (String.IsNullOrEmpty(ID))
            {
                string id = RandomString(10);
                using (Context dbcontext = new Context())
                {

                    markaz model = new markaz();
                    DateTime nowdatetime = DateTime.Now;
                    model.master = "1";
                    model.ID = id;
                    model.parent = id;
                    model.date = nowdatetime;
                    model.time = nowdatetime.TimeOfDay;
                    model.changer = "admin";
                    model.title = title;
                    model.masoul = masool;
                    model.janeshin = janeshin;
                    model.BakhshID = bakhsh;

                    dbcontext.markazs.Add(model);
                    dbcontext.SaveChanges();

                }
            }
            else
            {
                markaz model = dbcontext.markazs.SingleOrDefault(x => x.ID == ID);
                model.masoul = masool;
                model.janeshin = janeshin;
                model.title = title;
                model.BakhshID = bakhsh;
                dbcontext.SaveChanges();
            }



        }

        public void deletMarkaz(string id)
        {
            markaz model = dbcontext.markazs.SingleOrDefault(x => x.ID == id);
            dbcontext.markazs.Remove(model);
            dbcontext.SaveChanges();
        }
        public ContentResult getMarkazDetail(string id)
        {

            markaz model = dbcontext.markazs.SingleOrDefault(x => x.ID == id);
            return Content(JsonConvert.SerializeObject(model));
        }


        public ActionResult Komite_List()
        {

            string permisson = Session["permission"] as string;
            string see = permisson.Contains("81,") ? "1" : "";
            string edit = permisson.Contains("82,") ? "1" : "";
            string add = permisson.Contains("83,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            List<user> userlist = dbcontext.users.ToList();
            List<Model.komite> mlst = dbcontext.komites.ToList();// new List<Komite>();
            List<Model.markaz> markazList = dbcontext.markazs.ToList();
            //var q = dbcontext.Komites.Where(x => x.ID == x.parent || x.final == 0).OrderByDescending(x => x.ID);// && x.isDone != true
            //if (q != null)
            //{
            //    mlst = q.ToList();
            //}
            KomiteListVM model = new KomiteListVM()
            {
                komiteList = mlst,
                userList = userlist,
                markazList = markazList,
                 add = add,
                  edit = edit
            };

            return View(model);
        }
        public void addKomiteForm(string title, string masool, string janeshin, string markaz, string ID)
        {

            if (String.IsNullOrEmpty(ID))
            {
                string id = RandomString(10);
                using (Context dbcontext = new Context())
                {

                    komite model = new komite();
                    DateTime nowdatetime = DateTime.Now;
                    model.master = "1";
                    model.ID = id;
                    model.parent = id;
                    model.date = nowdatetime;
                    model.time = nowdatetime.TimeOfDay;
                    model.changer = "admin";
                    model.title = title;
                    model.masoul = masool;
                    model.janeshin = janeshin;
                    model.markazID = markaz;

                    dbcontext.komites.Add(model);
                    dbcontext.SaveChanges();

                }
            }
            else
            {
                komite model = dbcontext.komites.SingleOrDefault(x => x.ID == ID);
                model.masoul = masool;
                model.janeshin = janeshin;
                model.title = title;
                model.markazID = markaz;
                dbcontext.SaveChanges();
            }



        }

        public void deletKomite(string id)
        {
            komite model = dbcontext.komites.SingleOrDefault(x => x.ID == id);
            dbcontext.komites.Remove(model);
            dbcontext.SaveChanges();
        }
        public ContentResult getKomiteDetail(string id)
        {
            komite model = dbcontext.komites.SingleOrDefault(x => x.ID == id);
            return Content(JsonConvert.SerializeObject(model));
        }
        public ActionResult User_List(List<user> lst)
        {
            if (lst == null)
            {
                lst = (from p in dbcontext.users  select p).ToList(); //where p.pasdari_Code != "admin"
            }

            return View(lst);
        }

        public ActionResult getUserPermission(int id)
        {

            List<permission> lst = dbcontext.permissions.Where(x=>x.userID == id).ToList();
           
            permissionVM model = new permissionVM()
            {
                lst = lst,
                id = id,
            };
            return PartialView("/Views/Shared/_userPermission.cshtml", model);

        }
        public ActionResult setPermision(string id, int userID)
        {
            Session["permission"] = id;
            List<permission> ssd = dbcontext.permissions.Where(x => x.userID == userID).ToList();
            foreach(var permis in ssd)
            {
                dbcontext.permissions.Remove(permis);
            }
            dbcontext.SaveChanges();
            List<string> lst = id.Trim(',').Split(',').ToList();
            foreach(var item in lst)
            {
                permission model = dbcontext.permissions.SingleOrDefault(x => x.serctionID == item && x.userID == userID);
                if (model == null){
                    Model.permission mdl = new permission()
                    {
                        userID = userID,
                        serctionID = item
                    };
                    dbcontext.permissions.Add(mdl);
                }
               
            }
            dbcontext.SaveChanges();
            return Content("200");
        }
        public void addUserForm(string password, string fullname, string pasdariCode, string masooliat, int UserID)
        {
            using (Model.Context dbcontext = new Model.Context())
            {
                if (UserID != 0)
                {
                    user model = dbcontext.users.SingleOrDefault(x => x.ID == UserID);
                    if (model != null)
                    {
                        model.name = fullname;
                        model.shomareHesab = masooliat;
                        model.pasdari_Code = pasdariCode;
                        model.password = password;
                    }

                }
                else
                {
                    user newUser = new user()
                    {
                        //family_Number = family_Number,
                        //grade = grade,
                        //madrak = madrak,
                        //madrak_Title = madrak,
                        //maskan = maskan,
                        //meli_Code = meli_Code,
                        //mobile = mobile,
                        name = fullname,
                        pasdari_Code = pasdariCode,
                        shomareHesab = masooliat,
                        password = password,
                        //phone = phone,
                        sazman_Enterdate = DateTime.Now, // sazman_Enterdate.GetSelectedDateInPersianDateTime().ToShortDateString().ToGeorgianDateTime(),
                        Sepah_Enterdate = DateTime.Now, //Sepah_Enterdate.GetSelectedDateInPersianDateTime().ToShortDateString().ToGeorgianDateTime(),
                                                        //tahol = tahol,
                                                        //univercity = univercity,
                                                        //univercity_Score = shomareHesab,
                                                        //jaygah = jaygah
                    };
                    dbcontext.users.Add(newUser);
                }

                dbcontext.SaveChanges();
            }
        }
        public void deletUser(int id)
        {
            user model = dbcontext.users.SingleOrDefault(x => x.ID == id);
            dbcontext.users.Remove(model);
            dbcontext.SaveChanges();
        }
        public ContentResult getUserDetail(int id)
        {
            user model = dbcontext.users.SingleOrDefault(x => x.ID == id);
            return Content(JsonConvert.SerializeObject(model));
        }


        public ActionResult bank_List(List<bank> lst)
        {
            string permisson = Session["permission"] as string;
            string see = permisson.Contains("91,") ? "1" : "";
            string edit = permisson.Contains("92,") ? "1" : "";
            string add = permisson.Contains("93,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            if (lst == null)
            {
                lst = (from p in dbcontext.banks select p).ToList();
            }
            ViewBag.edit = edit;
            ViewBag.add = add;
            return View(lst);
        }
        public void addbankForm(string shomareHesab, string name, string bankType, int bankID)
        {
            using (Model.Context dbcontext = new Model.Context())
            {
                if (bankID != 0)
                {
                    bank model = dbcontext.banks.SingleOrDefault(x => x.ID == bankID);
                    if (model != null)
                    {

                        model.number = shomareHesab;
                        model.title = name;
                        model.type = bankType;

                    }

                }
                else
                {
                    string id = RandomString(10);
                    bank newBank = new bank()
                    {
                        number = shomareHesab,
                        title = name,
                        type = bankType,


                    };
                    dbcontext.banks.Add(newBank);
                    dbcontext.SaveChanges();

                }

                dbcontext.SaveChanges();
            }
        }
        public void deletbank(int id)
        {
            bank model = dbcontext.banks.SingleOrDefault(x => x.ID == id);
            dbcontext.banks.Remove(model);
            dbcontext.SaveChanges();
        }
        public ContentResult getbankDetail(int id)
        {
            bank model = dbcontext.banks.SingleOrDefault(x => x.ID == id);
            return Content(JsonConvert.SerializeObject(model));
        }


        public ActionResult check_List()
        {

            string permisson = Session["permission"] as string;
            string see = permisson.Contains("101,") ? "1" : "";
            string edit = permisson.Contains("102,") ? "1" : "";
            string add = permisson.Contains("103,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            List<checkLVM> lstt = new List<checkLVM>();
            using (Context dbcontext = new Context())
            {
                lstt = (from p in dbcontext.checks
                        join b in dbcontext.banks on p.bankID equals b.ID
                        join f in  dbcontext.Archives on p.checkNumber equals f.checkNumber  into checkArchive
                        from bf in checkArchive.DefaultIfEmpty()
                        orderby p.ID descending
                        select  new checkLVM {
                            price = bf == null ? 0: bf.mablagh, ID = b.ID, checkNumber = p.checkNumber, banktitle = b.title, isused = p.isUsed, banknumber = b.number }
                                         ).Distinct().ToList();

                //

            }

           

            List<bank> bankList = dbcontext.banks.ToList();
            List<check> checkList = dbcontext.checks.ToList();
            checkListVM model = new checkListVM();
            model.checklst = lstt;
            model.banklst = bankList;
            model.edit = edit;
            model.add = add;

            return View(model);
        }
        public ContentResult addcheckForm(string numberFrom, string numberTo, string bankCombo, string pasvand)
        {
            string message = "";
            using (Context dbcontext = new Context())
            {

                if (numberFrom != "" && numberTo != "" && bankCombo != null && pasvand != "")
                {
                    int from = int.Parse(numberFrom);
                    int tedad = int.Parse(numberTo);
                    if (tedad > 0)
                    {
                        string ischeck = "";
                        for (int i = 0; i < tedad; i++)
                        {

                            string checknumber = pasvand + "/" + (from + i).ToString();
                            int selectedBank = int.Parse(bankCombo);
                            List<check> isexist = dbcontext.checks.Where(x => x.checkNumber == checknumber && x.bankID == selectedBank).ToList();
                            if (isexist.Count() > 0)
                            {
                                ischeck = checknumber;
                            }

                        }
                        if (ischeck == "")
                        {
                            for (int i = 0; i < tedad; i++)
                            {
                                string checknumber = pasvand + "/" + (from + i).ToString();
                                check isexist = dbcontext.checks.SingleOrDefault(x => x.checkNumber == checknumber);
                                if (isexist == null)
                                {
                                    check model = new check()
                                    {
                                        bankID = int.Parse(bankCombo),
                                        isUsed = false,
                                        checkNumber = checknumber,
                                    };
                                    dbcontext.checks.Add(model);

                                }

                            }
                            dbcontext.SaveChanges();


                        }
                        else
                        {
                            message = "چک با شماره  " + ischeck + " قبلا ثبت شده است ";
                        }

                    }
                }


            }
            return Content(message);
        }
        public void deletcheck(int id)
        {
            check model = dbcontext.checks.SingleOrDefault(x => x.ID == id);
            dbcontext.checks.Remove(model);
            dbcontext.SaveChanges();
        }



     



        public ActionResult reportList()
        {
            List<Model.shenasname> lst = new List<shenasname>();
            var q = dbcontext.shenasnames.Where(x => (x.master == "1" && x.final == 1) || x.final == 0).OrderByDescending(x => x.ID);// && x.isDone != true

            if (q != null)
            {
                lst = q.ToList();
                foreach (var item in lst)
                {
                    item.datestring = item.date.ToPersianDateString();
                }
            }

            return View(lst);
        }
        public ActionResult addReport()
        {
            string permisson = Session["permission"] as string;

            string see = permisson.Contains("41,") ? "1" : "";
            string edit = permisson.Contains("42,") ? "1" : "";
            string add = permisson.Contains("43,") ? "1" : "";
            if (add == "")
                return RedirectToAction("login");
            string id = RandomString(10);
            shenasname model = new shenasname();
            DateTime nowdatetime = DateTime.Now;
            model.token = "123456789";
            model.master = "1";
            model.ID = id;
            model.parent = id;
            model.date = nowdatetime;
            model.dateFrom = DateTime.Now;
            model.dateTo = DateTime.Now;
            model.time = nowdatetime.TimeOfDay;
            dbcontext.shenasnames.Add(model);
            dbcontext.SaveChanges();
            return Content(id);


        }
        
        
        public ActionResult showReport(string id,string notif)
        {
            string permisson = Session["permission"] as string;

            string see = permisson.Contains("41,") ? "1" : "";
            string edit = permisson.Contains("42,") ? "1" : "";
            string add = permisson.Contains("43,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");


            ViewBag.message = !String.IsNullOrEmpty(notif) ? "1" : "0";
            List<Model.markaz> markazList = dbcontext.markazs.Where(x => x.master == "1").ToList();
            //List<Model.shenasnameFounder> lstFounder = new List<Model.shenasnameFounder>();
            List<Model.user> userlist = dbcontext.users.ToList();
            List<Model.shenasnameGam> shenasnameGamList = (from p in dbcontext.shenasnameGams where p.shenasnameID == id select p).ToList();
            List<Model.ejraeiat> ejraeeatlst = (from p in dbcontext.ejraeiats where p.shenasnameID == id select p).ToList();
            List<Model.sarmaye> sarmayelst = (from p in dbcontext.sarmayes where p.shenasnameID == id select p).ToList();
            List<Model.masrafi> masrafilst = (from p in dbcontext.masrafis where p.shenasnameID == id select p).ToList();
            List<Model.edari> edarilst = (from p in dbcontext.edaris where p.shenasnameID == id select p).ToList();
            List<Model.omrani> omranilst = (from p in dbcontext.omranis where p.shenasnameID == id select p).ToList();
            List<Model.gharardad> gharardadlst = (from p in dbcontext.gharardads where p.shenasnameID == id select p).ToList();
            List<Model.sayer> sayerlst = (from p in dbcontext.sayers where p.shenasnameID == id select p).ToList();
            List<Model.mavad> mavadlist = (from p in dbcontext.mavads where p.shenasnameID == id select p).ToList();
            List<Model.tashvighi> tashvighilst = (from p in dbcontext.tashvighis where p.shenasnameID == id select p).ToList();
            List<Model.product> productlst = (from p in dbcontext.products where p.shenasnameID == id select p).ToList();
            

            functions fns = new functions();
            Model.shenasname item = dbcontext.shenasnames.Where(x => x.ID == id).FirstOrDefault();
            List<string> taminIDKist = dbcontext.Archives.Where(x => x.hesab == "0").Select(x => x.shomareTamin).ToList();
            List<Model.archive> arch = (from a in dbcontext.Archives
                                        where a.project == id && a.hesab == "1"
                                        select a).ToList();

            List<mavad> lstnew = dbcontext.mavads.ToList();

        
            showReportVM model = new showReportVM();
            model.bayganiFile = item.bayganiFile;
            model.gharardadFile = item.gharardadFile;
            model.motamamFile = item.motamamFile;
            model.peyvastFile = item.peyvastFile;
            model.listmavadFile = item.listmavadFile;
            model.gantFile = item.gantFile;
            model.mojavezFile = item.mojavezFile;
            model.pishraftFile = item.pishraftFile;
            model.add = add;
            model.edit = edit;
            model.see = see;
            model.markazList = markazList;
            model.userlist = userlist;
            model.ejraiatlist = ejraeeatlst;
            model.sarmayelist = sarmayelst;
            model.masrafilist = masrafilst;
            model.mavadlist = mavadlist;
            model.omranilist = omranilst;
            model.tashvighilist = tashvighilst;
            model.sayerlist = sayerlst;
            model.gharardadlist = gharardadlst;
            model.edarilist = edarilst;
            model.comment = item.comment;

            // model.lstFounder = lstFounder;
            model.itemID = item.ID;
            model.isFianl = item.isDone;
            model.shenasnameGamList = shenasnameGamList;
            model.shenasnameproductList = productlst;
            if (arch.Count() > 0)
            {
                model.startDate = arch.First().tarikh.ToPersianDateString();
                int pishbini = fns.IsDigitsOnly(item.datePishbini) == true ? Int32.Parse(item.datePishbini) : 0;
                model.endDate = arch.First().tarikh.AddMonths(pishbini).ToPersianDateString();
            }


            List<Model.ejraeiat> PejraRially = dbcontext.ejraeiats.Where(x => x.shenasnameID == id && x.riallyP != 0).ToList();
            List<Model.ejraeiat> pejraDollary = dbcontext.ejraeiats.Where(x => x.shenasnameID == id && x.dollaryP != 0).ToList();

            model.prejraeeyat = PejraRially.Count() > 0 ? string.Format("{0:n0}", PejraRially.Sum(x => x.riallyP)) : "0";
            model.pdejraiat = pejraDollary.Count() > 0 ? string.Format("{0:n0}", pejraDollary.Sum(x => x.dollaryP)) : "0";



            List<Model.tamin> ejraRquery = dbcontext.tamins.Where(x => x.subject == "اجراییات" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long ejraRially = ejraRquery != null ? ejraRquery.ToList().Sum(x => x.mablagh) : 0;
            List<Model.tamin> ejraDquery = dbcontext.tamins.Where(x => x.subject == "اجراییات" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long ejrDorllay = ejraDquery != null ? ejraDquery.ToList().Sum(x => x.mablagh) : 0;

            model.hrejrayiat = string.Format("{0:n0}", ejraRially.ToString());
            model.hdejraeiat = string.Format("{0:n0}", ejrDorllay.ToString());


            List<Model.sarmaye> PsarRially = dbcontext.sarmayes.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.sarmaye> PsarDollary = dbcontext.sarmayes.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.prsarmaye = PsarRially.Count() > 0 ? string.Format("{0:n0}", PsarRially.Sum(x => x.kollPR)) : "0";
            model.pdsarmaye = PsarDollary.Count() > 0 ? string.Format("{0:n0}", PsarDollary.Sum(x => x.kollPD)) : "0";

            List<Model.tamin> sarmaRquery = dbcontext.tamins.Where(x => x.subject == "سرمایه" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> sarmaDquery = dbcontext.tamins.Where(x => x.subject == "سرمایه" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long sarmRially = sarmaRquery != null ? sarmaRquery.ToList().Sum(x => x.mablagh) : 0;
            long sarmDorllay = sarmaDquery != null ? sarmaDquery.ToList().Sum(x => x.mablagh) : 0;

            model.hrsarmaye = string.Format("{0:n0}", sarmRially.ToString());
            model.hdsarmaye = string.Format("{0:n0}", sarmDorllay.ToString());


            List<Model.masrafi> PmasRially = dbcontext.masrafis.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.masrafi> PmasDollary = dbcontext.masrafis.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.prmasrafi = PmasRially.Count() > 0 ? string.Format("{0:n0}", PmasRially.Sum(x => x.kollPR)) : "0";
            model.pdmasrafi = PmasDollary.Count() > 0 ? string.Format("{0:n0}", PmasDollary.Sum(x => x.kollPD)) : "0";

            List<Model.tamin> masrafiRquery = dbcontext.tamins.Where(x => x.subject == "مصرفی" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> masrafiDquery = dbcontext.tamins.Where(x => x.subject == "مصرفی" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long masrafRially = masrafiRquery != null ? masrafiRquery.ToList().Sum(x => x.mablagh) : 0;
            long masrafDorllay = masrafiDquery != null ? masrafiDquery.ToList().Sum(x => x.mablagh) : 0;

            model.hrmasrafi = string.Format("{0:n0}", masrafRially.ToString());
            model.hdmasrafi = string.Format("{0:n0}", masrafDorllay.ToString());

            List<Model.edari> PedaRially = dbcontext.edaris.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.edari> PedaDollary = dbcontext.edaris.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.predari = PedaRially.Count() > 0 ? string.Format("{0:n0}", PedaRially.Sum(x => x.kollPR)) : "0";
            model.pdedari = PedaDollary.Count() > 0 ? string.Format("{0:n0}", PedaDollary.Sum(x => x.kollPD)) : "0";

            List<Model.tamin> edariRquery = dbcontext.tamins.Where(x => x.subject == "اداری" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> edariDquery = dbcontext.tamins.Where(x => x.subject == "اداری" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();

            long edariRially = edariRquery != null ? edariRquery.Sum(x => x.mablagh) : 0;
            long edariDorllay = edariDquery != null ? edariDquery.Sum(x => x.mablagh) : 0;

            model.hredari = string.Format("{0:n0}", edariRially.ToString());
            model.hdedari = string.Format("{0:n0}", edariDorllay.ToString());




            List<Model.omrani> PomrRially = dbcontext.omranis.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.omrani> PomrDollary = dbcontext.omranis.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.promrani = PomrRially.Count() > 0 ? string.Format("{0:n0}", PomrRially.Sum(x => x.kollPR)) : "0";
            model.pdomrani = PomrDollary.Count() > 0 ? string.Format("{0:n0}", PomrDollary.Sum(x => x.kollPD)) : "0";


            List<Model.tamin> omraniRquery = dbcontext.tamins.Where(x => x.subject == "عمرانی" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> omraniDquery = dbcontext.tamins.Where(x => x.subject == "عمرانی" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long omraniRially = omraniRquery != null ? omraniRquery.Sum(x => x.mablagh) : 0;
            long omraniDorllay = omraniDquery != null ? omraniDquery.Sum(x => x.mablagh) : 0;

            model.hromrani = string.Format("{0:n0}", omraniRially.ToString());
            model.hdomrani = string.Format("{0:n0}", omraniDorllay.ToString());


            List<Model.gharardad> PgharRially = dbcontext.gharardads.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.gharardad> PgharDollary = dbcontext.gharardads.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.prgharardad = PgharRially.Count() > 0 ? string.Format("{0:n0}", PgharRially.Sum(x => x.kollPR)) : "0";
            model.pdgharardad = PgharDollary.Count() > 0 ? string.Format("{0:n0}", PgharDollary.Sum(x => x.kollPD)) : "0";


            List<Model.tamin> ghararRquery = dbcontext.tamins.Where(x => x.subject == "قرارداد" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> ghararDquery = dbcontext.tamins.Where(x => x.subject == "قرارداد" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long ghararRially = ghararRquery != null ? ghararRquery.Sum(x => x.mablagh) : 0;
            long ghararDorllay = ghararDquery != null ? ghararDquery.Sum(x => x.mablagh) : 0;

            model.hrgharardad = string.Format("{0:n0}", ghararRially.ToString());
            model.hdgharardad = string.Format("{0:n0}", ghararDorllay.ToString());


            List<Model.sayer> PsayerRially = dbcontext.sayers.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.sayer> PsayerDollary = dbcontext.sayers.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.prsayer = PsayerRially.Count() > 0 ? string.Format("{0:n0}", PsayerRially.Sum(x => x.kollPR)) : "0";
            model.pdsayer = PsayerDollary.Count() > 0 ? string.Format("{0:n0}", PsayerDollary.Sum(x => x.kollPD)) : "0";


            List<Model.mavad> PmavadRially = dbcontext.mavads.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.mavad> PmavadDollary = dbcontext.mavads.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.pravalie = PsayerRially.Count() > 0 ? string.Format("{0:n0}", PsayerRially.Sum(x => x.kollPR)) : "0";
            model.pdavalie = PsayerDollary.Count() > 0 ? string.Format("{0:n0}", PsayerDollary.Sum(x => x.kollPD)) : "0";


            List<Model.tamin> avalieRquery = dbcontext.tamins.Where(x => x.subject == "مواد" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> avalieDquery = dbcontext.tamins.Where(x => x.subject == "مواد" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long avalieRially = avalieRquery != null ? avalieRquery.Sum(x => x.mablagh) : 0;
            long avalieDorllay = avalieDquery != null ? avalieDquery.Sum(x => x.mablagh) : 0;


            model.hravalie = string.Format("{0:n0}", avalieRially.ToString());
            model.hdavalie = string.Format("{0:n0}", avalieDorllay.ToString());


           

            List<Model.tamin> sayerRquery = dbcontext.tamins.Where(x => x.subject == "سایر" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> sayerDquery = dbcontext.tamins.Where(x => x.subject == "سایر" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long sayerRially = sayerRquery != null ? sayerRquery.Sum(x => x.mablagh) : 0;
            long sayerDorllay = sayerDquery != null ? sayerDquery.Sum(x => x.mablagh) : 0;

            model.hrsayer = string.Format("{0:n0}", sayerRially.ToString());
            model.hdsayer = string.Format("{0:n0}", sayerDorllay.ToString());


            List<Model.tashvighi> PtashRially = dbcontext.tashvighis.Where(x => x.shenasnameID == id && x.kollPR != 0).ToList();
            List<Model.tashvighi> PtashDollary = dbcontext.tashvighis.Where(x => x.shenasnameID == id && x.kollPD != 0).ToList();

            model.prtashvighi = PtashRially.Count() > 0 ? string.Format("{0:n0}", PtashRially.Sum(x => x.kollPR)) : "0";
            model.pdtashvighi = PtashDollary.Count() > 0 ? string.Format("{0:n0}", PtashDollary.Sum(x => x.kollPD)) : "0";


            List<Model.tamin> tashvighiRquery = dbcontext.tamins.Where(x => x.subject == "تشویقی" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> tashvighiDquery = dbcontext.tamins.Where(x => x.subject == "تشویقی" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long tashvighiRially = tashvighiRquery != null ? tashvighiRquery.Sum(x => x.mablagh) : 0;
            long tashvighiDorllay = tashvighiDquery != null ? tashvighiDquery.Sum(x => x.mablagh) : 0;

            model.hrtashvighi = string.Format("{0:n0}", tashvighiRially.ToString());
            model.hdtashvighi = string.Format("{0:n0}", tashvighiDorllay.ToString());



            List<Model.tamin> sayerhazineRquery = dbcontext.tamins.Where(x => x.subject == "سایر هزینه ها" && x.type == "ریال" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            List<Model.tamin> sayerhazineDquery = dbcontext.tamins.Where(x => x.subject == "سایر هزینه ها" && x.type == "دلار" && x.hesab == "0" && x.project == id && taminIDKist.Contains(x.shomareSanad)).ToList();
            long sayerhazineRially = sayerhazineRquery != null ? sayerhazineRquery.Sum(x => x.mablagh) : 0;
            long sayerhazineDorllay = sayerhazineDquery != null ? sayerhazineDquery.Sum(x => x.mablagh) : 0;

            model.hrsayerhazine = string.Format("{0:n0}", sayerhazineRially.ToString());
            model.hdsayerhazine = string.Format("{0:n0}", sayerhazineDorllay.ToString());

            model.hrkoll = string.Format("{0:n0}", (avalieRially + edariRially + ejraRially + ghararRially + masrafRially + omraniRially + sarmRially + sayerhazineRially + tashvighiRially).ToString()); ;
            model.hdkoll = string.Format("{0:n0}", (avalieDorllay + edariDorllay + ejrDorllay + ghararDorllay + masrafDorllay + omraniDorllay + sarmDorllay + sayerhazineDorllay + tashvighiDorllay).ToString());
            model.prkoll = string.Format("{0:n0}", (decimal.Parse(model.prejraeeyat) + decimal.Parse(model.prsarmaye) + decimal.Parse(model.predari) + decimal.Parse(model.prmasrafi) + decimal.Parse(model.promrani) + decimal.Parse(model.pravalie) + decimal.Parse(model.prtashvighi)).ToString());
            model.pdkoll = string.Format("{0:n0}", (decimal.Parse(model.pdejraiat) + decimal.Parse(model.pdsarmaye) + decimal.Parse(model.pdedari) + decimal.Parse(model.pdmasrafi) + decimal.Parse(model.pdomrani) + decimal.Parse(model.pdavalie) + decimal.Parse(model.pdtashvighi)).ToString());





            model.hadaf = item.hadaf;
            model.title = item.title;
            model.datePishbini = item.datePishbini;
            model.dastgah = item.dastgah;
            model.markaz = item.markaz;
            model.tarah = item.tarah;

            model.DrasmiT = item.DrasmiT;
            model.DrasmiHPR = item.DrasmiHPR;
            model.DrasmiHPD = item.DrasmiHPD;
            model.DgharartamamT = item.DgharartamamT;
            model.DgharartamamHPR = item.DgharartamamHPR;
            model.DgharartamamHPD = item.DgharartamamHPD;
            model.DghararsaatT = item.DghararsaatT;
            model.DghararsaatHPR = item.DghararsaatHPR;
            model.DghararsaatHPD = item.DghararsaatHPD;
            model.DvazifeT = item.DvazifeT;
            model.DvazifeHPR = item.DvazifeHPR;
            model.DvazifeHPD = item.DvazifeHPD;

            model.FLrasmiT = item.FLrasmiT;
            model.FLrasmiHPR = item.FLrasmiHPR;
            model.FLrasmiHPD = item.FLrasmiHPD;
            model.FLgharartamamT = item.FLgharartamamT;
            model.FLgharartamamHPR = item.FLgharartamamHPR;
            model.FLgharartamamHPD = item.FLgharartamamHPD;
            model.FLghararsaatT = item.FLghararsaatT;
            model.FLghararsaatHPR = item.FLghararsaatHPR;
            model.FLghararsaatHPD = item.FLghararsaatHPD;
            model.FLvazifeT = item.FLvazifeT;
            model.FLvazifeHPR = item.FLvazifeHPR;
            model.FLvazifeHPD = item.FLvazifeHPD;


            model.LrasmiT = item.LrasmiT;
            model.LrasmiHPR = item.LrasmiHPR;
            model.LrasmiHPD = item.LrasmiHPD;
            model.LgharartamamT = item.LgharartamamT;
            model.LgharartamamHPR = item.LgharartamamHPR;
            model.LgharartamamHPD = item.LgharartamamHPD;
            model.LghararsaatT = item.LghararsaatT;
            model.LghararsaatHPR = item.LghararsaatHPR;
            model.LghararsaatHPD = item.LghararsaatHPD;
            model.LvazifeT = item.LvazifeT;
            model.LvazifeHPR = item.LvazifeHPR;
            model.LvazifeHPD = item.LvazifeHPD;


            model.FDrasmiT = item.FDrasmiT;
            model.FDrasmiHPR = item.FDrasmiHPR;
            model.FDrasmiHPD = item.FDrasmiHPD;
            model.FDgharartamamT = item.FDgharartamamT;
            model.FDgharartamamHPR = item.FDgharartamamHPR;
            model.FDgharartamamHPD = item.FDgharartamamHPD;
            model.FDghararsaatT = item.FDghararsaatT;
            model.FDghararsaatHPR = item.FDghararsaatHPR;
            model.FDghararsaatHPD = item.FDghararsaatHPD;
            model.FDvazifeT = item.FDvazifeT;
            model.FDvazifeHPR = item.FDvazifeHPR;
            model.FDvazifeHPD = item.FDvazifeHPD;


            model.DirasmiT = item.DirasmiT;
            model.DirasmiHPR = item.DirasmiHPR;
            model.DirasmiHPD = item.DirasmiHPD;
            model.DigharartamamT = item.DigharartamamT;
            model.DigharartamamHPR = item.DigharartamamHPR;
            model.DigharartamamHPD = item.DigharartamamHPD;
            model.DighararsaatT = item.DighararsaatT;
            model.DighararsaatHPR = item.DighararsaatHPR;
            model.DighararsaatHPD = item.DighararsaatHPD;
            model.DivazifeT = item.DivazifeT;
            model.DivazifeHPR = item.DivazifeHPR;
            model.DivazifeHPD = item.DivazifeHPD;


            model.ZDirasmiT = item.ZDirasmiT;
            model.ZDirasmiHPR = item.ZDirasmiHPR;
            model.ZDirasmiHPD = item.ZDirasmiHPD;
            model.ZDigharartamamT = item.ZDigharartamamT;
            model.ZDigharartamamHPR = item.ZDigharartamamHPR;
            model.ZDigharartamamHPD = item.ZDigharartamamHPD;
            model.ZDighararsaatT = item.ZDighararsaatT;
            model.ZDighararsaatHPR = item.ZDighararsaatHPR;
            model.ZDighararsaatHPD = item.ZDighararsaatHPD;
            model.ZDivazifeT = item.ZDivazifeT;
            model.ZDivazifeHPR = item.ZDivazifeHPR;
            model.ZDivazifeHPD = item.ZDivazifeHPD;


            

            return View(model);
        }


        public ActionResult addComment(string itemID, string comment)
        {
            shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == itemID);

            if (shen.final != 1)
            {
                shen.comment = comment;
                string parentID = shen.parent;

                List<shenasname> shenList = dbcontext.shenasnames.Where(x => x.parent == parentID).ToList();
                foreach (shenasname item in shenList)
                {
                    item.master = "0";
                };
                shen.isDone = true;
                shen.master = "1";
                shen.final = 1;

                dbcontext.SaveChanges();

            }
            return Content("200");
        }

        public ActionResult finalShen(string shenasnameID)
        {
            using (var dbcontext = new Model.Context())
            {

                shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == shenasnameID);

                if (shen.final != 1)
                {
                    string parentID = shen.parent;

                    List<shenasname> shenList = dbcontext.shenasnames.Where(x => x.parent == parentID).ToList();
                    foreach (shenasname item in shenList)
                    {
                        item.master = "0";
                    };
                    shen.isDone = true;
                    shen.master = "1";
                    shen.final = 1;

                    dbcontext.SaveChanges();
                }


            }
            return RedirectToAction("reportList");

        }
        public ActionResult setPersonnelMoney(showReportVM model)
        {

            Model.shenasname item = dbcontext.shenasnames.FirstOrDefault(x => x.ID == model.itemID);
            if (item.final != 1)
            {
                try
                {
                    item.DrasmiT = model.DrasmiT;
                    item.DrasmiHPR = model.DrasmiHPR;
                    item.DrasmiHPD = model.DrasmiHPD;
                    item.DgharartamamT = model.DgharartamamT;
                    item.DgharartamamHPR = model.DgharartamamHPR;
                    item.DgharartamamHPD = model.DgharartamamHPD;
                    item.DghararsaatT = model.DghararsaatT;
                    item.DghararsaatHPR = model.DghararsaatHPR;
                    item.DghararsaatHPD = model.DghararsaatHPD;
                    item.DvazifeT = model.DvazifeT;
                    item.DvazifeHPR = model.DvazifeHPR;
                    item.DvazifeHPD = model.DvazifeHPD;

                    item.FDrasmiT = model.FDrasmiT;
                    item.FDrasmiHPR = model.FDrasmiHPR;
                    item.FDrasmiHPD = model.FDrasmiHPD;
                    item.FDgharartamamT = model.FDgharartamamT;
                    item.FDgharartamamHPR = model.FDgharartamamHPR;
                    item.FDgharartamamHPD = model.FDgharartamamHPD;
                    item.FDghararsaatT = model.FDghararsaatT;
                    item.FDghararsaatHPR = model.FDghararsaatHPR;
                    item.FDghararsaatHPD = model.FDghararsaatHPD;
                    item.FDvazifeT = model.FDvazifeT;
                    item.FDvazifeHPR = model.FDvazifeHPR;
                    item.FDvazifeHPD = model.FDvazifeHPD;

                    item.ZDirasmiT = model.ZDirasmiT;
                    item.ZDirasmiHPR = model.ZDirasmiHPR;
                    item.ZDirasmiHPD = model.ZDirasmiHPD;
                    item.ZDigharartamamT = model.ZDigharartamamT;
                    item.ZDigharartamamHPR = model.ZDigharartamamHPR;
                    item.ZDigharartamamHPD = model.ZDigharartamamHPD;
                    item.ZDighararsaatT = model.ZDighararsaatT;
                    item.ZDighararsaatHPR = model.ZDighararsaatHPR;
                    item.ZDighararsaatHPD = model.ZDighararsaatHPD;
                    item.ZDivazifeT = model.ZDivazifeT;
                    item.ZDivazifeHPR = model.ZDivazifeHPR;
                    item.ZDivazifeHPD = model.ZDivazifeHPD;

                    item.LrasmiT = model.LrasmiT;
                    item.LrasmiHPR = model.LrasmiHPR;
                    item.LrasmiHPD = model.LrasmiHPD;
                    item.LgharartamamT = model.LgharartamamT;
                    item.LgharartamamHPR = model.LgharartamamHPR;
                    item.LgharartamamHPD = model.LgharartamamHPD;
                    item.LghararsaatT = model.LghararsaatT;
                    item.LghararsaatHPR = model.LghararsaatHPR;
                    item.LghararsaatHPD = model.LghararsaatHPD;
                    item.LvazifeT = model.LvazifeT;
                    item.LvazifeHPR = model.LvazifeHPR;
                    item.LvazifeHPD = model.LvazifeHPD;

                    item.DirasmiT = model.DirasmiT;
                    item.DirasmiHPR = model.DirasmiHPR;
                    item.DirasmiHPD = model.DirasmiHPD;
                    item.DigharartamamT = model.DigharartamamT;
                    item.DigharartamamHPR = model.DigharartamamHPR;
                    item.DigharartamamHPD = model.DigharartamamHPD;
                    item.DighararsaatT = model.DighararsaatT;
                    item.DighararsaatHPR = model.DighararsaatHPR;
                    item.DighararsaatHPD = model.DighararsaatHPD;
                    item.DivazifeT = model.DivazifeT;
                    item.DivazifeHPR = model.DivazifeHPR;
                    item.DivazifeHPD = model.DivazifeHPD;

                    item.FLrasmiT = model.FLrasmiT;
                    item.FLrasmiHPR = model.FLrasmiHPR;
                    item.FLrasmiHPD = model.FLrasmiHPD;
                    item.FLgharartamamT = model.FLgharartamamT;
                    item.FLgharartamamHPR = model.FLgharartamamHPR;
                    item.FLgharartamamHPD = model.FLgharartamamHPD;
                    item.FLghararsaatT = model.FLghararsaatT;
                    item.FLghararsaatHPR = model.FLghararsaatHPR;
                    item.FLghararsaatHPD = model.FLghararsaatHPD;
                    item.FLvazifeT = model.FLvazifeT;
                    item.FLvazifeHPR = model.FLvazifeHPR;
                    item.FLvazifeHPD = model.FLvazifeHPD;

                    dbcontext.SaveChanges();


                }
                catch
                {

                }
            }
            return RedirectToAction("showReport", new { id = model.itemID, notif = "1" });
           
        }
        public ActionResult SetNewInfo(string ID, string hadaf, string title, string datePishbini, string markazcombo, string dastgah, string tarah, string startDate, string endDate)
        {
            Model.shenasname item = dbcontext.shenasnames.Where(x => x.ID == ID).FirstOrDefault();
            if (item.final != 1)
            {
                try
                {
                    item.hadaf = hadaf;
                    item.title = title;

                    item.date = DateTime.Now;
                    item.datePishbini = datePishbini;
                    //item.dateFrom = startDate.ToGeorgianDateTime();
                    //item.dateTo = endDate.ToGeorgianDateTime();
                    item.markaz = markazcombo;
                    item.dastgah = dastgah;
                    item.tarah = tarah;
                    dbcontext.SaveChanges();

                }
                catch (Exception error)
                {
                    //throw;
                }
            }

            return RedirectToAction("showReport", new { id = ID,notif = "1" });
        }










        public PartialViewResult addGam(string ID, string onvan, string gamasli, string sharh, string modat, string darsad, string dastavard, string hazine)
        {
            shenasnameGam model = new shenasnameGam()
            {
                achivement = dastavard,
                darsadeVazni = Int32.Parse(darsad),
                description = sharh,
                duration = modat,
                title = onvan,
                shenasnameID = ID,
                gamAsli = gamasli,
                hazine = hazine.Replace(",", "")
            };
            dbcontext.shenasnameGams.Add(model);
            dbcontext.SaveChanges();

            List<shenasnameGam> final = dbcontext.shenasnameGams.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_gam.cshtml", final);


        }
        public PartialViewResult deletGam(int id)
        {
            shenasnameGam model = dbcontext.shenasnameGams.SingleOrDefault(x => x.ID == id);
            string shenID = model.shenasnameID;
            dbcontext.shenasnameGams.Remove(model);
            dbcontext.SaveChanges();

            List<shenasnameGam> final = dbcontext.shenasnameGams.Where(x => x.shenasnameID == shenID).ToList();
            return PartialView("/Views/Shared/_gam.cshtml", final);
        }



        public PartialViewResult addProduct(string ID, string title, string count, string price)
        {
            product model = new product()
            {
                  title = title,
                 count = count,
                 isDeliverd = 0,
                 price = price.Replace(",", ""),
                  shenasnameID = ID


            };
            dbcontext.products.Add(model);
            dbcontext.SaveChanges();

            List<product> final = dbcontext.products.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_Product.cshtml", final);


        }
        public PartialViewResult deletProduct(int id)
        {
            product model = dbcontext.products.SingleOrDefault(x => x.ID == id);
            string shenID = model.shenasnameID;
            dbcontext.products.Remove(model);
            dbcontext.SaveChanges();

            List<product> final = dbcontext.products.Where(x => x.shenasnameID == shenID).ToList();
            return PartialView("/Views/Shared/_Product.cshtml", final);
        }


        public PartialViewResult addejraeeat(string dollaryP, string riallyP, string ejratitle, string ID)
        {

            riallyP = riallyP == "" ? "0" : riallyP.Replace(",", ""); 
            dollaryP = dollaryP == "" ? "0" : dollaryP.Replace(",", "");
            Model.ejraeiat model = new Model.ejraeiat()
            {
                dollaryP = Int32.Parse(dollaryP),
                riallyP = Int32.Parse(riallyP),
                title = ejratitle,
                shenasnameID = ID


            };
            dbcontext.ejraeiats.Add(model);
            dbcontext.SaveChanges();
            List<ejraeiat> final = dbcontext.ejraeiats.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_Ejra.cshtml", final);

        }
        public PartialViewResult deletEjra(int id)
        {
            ejraeiat model = dbcontext.ejraeiats.SingleOrDefault(x => x.ID == id);
            String shenID = model.shenasnameID;
            dbcontext.ejraeiats.Remove(model);
            dbcontext.SaveChanges();


            List<ejraeiat> final = dbcontext.ejraeiats.Where(x => x.shenasnameID == shenID).ToList();
            return PartialView("/Views/Shared/_Ejra.cshtml", final);
        }


        public void addpersoneli(personeliVM inputModel)
        {

            Model.shenasname item = dbcontext.shenasnames.FirstOrDefault(x => x.ID == inputModel.ID);
            if (item.final != 1)
            {
                try
                {
                    item.DrasmiT = Int32.Parse(inputModel.DrasmiT);
                    item.DrasmiHPR = Int32.Parse(inputModel.DrasmiHPR);
                    item.DrasmiHPD = Int32.Parse(inputModel.DrasmiHPD);
                    item.DgharartamamT = Int32.Parse(inputModel.DgharartamamT);
                    item.DgharartamamHPR = Int32.Parse(inputModel.DgharartamamHPR);
                    item.DgharartamamHPD = Int32.Parse(inputModel.DgharartamamHPD);
                    item.DghararsaatT = Int32.Parse(inputModel.DghararsaatT);
                    item.DghararsaatHPR = Int32.Parse(inputModel.DghararsaatHPR);
                    item.DghararsaatHPD = Int32.Parse(inputModel.DghararsaatHPD);
                    item.DvazifeT = Int32.Parse(inputModel.DvazifeT);
                    item.DvazifeHPR = Int32.Parse(inputModel.DvazifeHPR);
                    item.DvazifeHPD = Int32.Parse(inputModel.DvazifeHPD);

                    item.ZDirasmiT = Int32.Parse(inputModel.ZDirasmiT);
                    item.ZDirasmiHPR = Int32.Parse(inputModel.ZDirasmiTHPR != "" ? inputModel.ZDirasmiTHPR : "0");
                    item.ZDirasmiHPD = Int32.Parse(inputModel.ZDirasmiHPD);
                    item.ZDigharartamamT = Int32.Parse(inputModel.ZDigharartamamT);
                    item.ZDigharartamamHPR = Int32.Parse(inputModel.ZDigharartamamHPR);
                    item.ZDigharartamamHPD = Int32.Parse(inputModel.ZDigharartamamHPD);
                    item.ZDighararsaatT = Int32.Parse(inputModel.ZDighararsaatT);
                    item.ZDighararsaatHPR = Int32.Parse(inputModel.ZDighararsaatHPR);
                    item.ZDighararsaatHPD = Int32.Parse(inputModel.ZDighararsaatHPD);
                    item.ZDivazifeT = Int32.Parse(inputModel.ZDivazifeT);
                    item.ZDivazifeHPR = Int32.Parse(inputModel.ZDivazifeHPR);
                    item.ZDivazifeHPD = Int32.Parse(inputModel.ZDivazifeHPD);

                    item.LrasmiT = Int32.Parse(inputModel.LrasmiT);
                    item.LrasmiHPR = Int32.Parse(inputModel.LrasmiHPR);
                    item.LrasmiHPD = Int32.Parse(inputModel.LrasmiHPD);
                    item.LgharartamamT = Int32.Parse(inputModel.LgharartamamT);
                    item.LgharartamamHPR = Int32.Parse(inputModel.LgharartamamHPR);
                    item.LgharartamamHPD = Int32.Parse(inputModel.LgharartamamHPD);
                    item.LghararsaatT = Int32.Parse(inputModel.LghararsaatT);
                    item.LghararsaatHPR = Int32.Parse(inputModel.LghararsaatHPR);
                    item.LghararsaatHPD = Int32.Parse(inputModel.LghararsaatHPD);
                    item.LvazifeT = Int32.Parse(inputModel.LvazifeT);
                    item.LvazifeHPR = Int32.Parse(inputModel.LvazifeHPR);
                    item.LvazifeHPD = Int32.Parse(inputModel.LvazifeHPD);

                    item.DirasmiT = Int32.Parse(inputModel.DirasmiT);
                    item.DirasmiHPR = Int32.Parse(inputModel.DirasmiHPR);
                    item.DirasmiHPD = Int32.Parse(inputModel.DirasmiHPD);
                    item.DigharartamamT = Int32.Parse(inputModel.DigharartamamT);
                    item.DigharartamamHPR = Int32.Parse(inputModel.DigharartamamHPR);
                    item.DigharartamamHPD = Int32.Parse(inputModel.DigharartamamHPD);
                    item.DighararsaatT = Int32.Parse(inputModel.DighararsaatT);
                    item.DighararsaatHPR = Int32.Parse(inputModel.DighararsaatHPR);
                    item.DighararsaatHPD = Int32.Parse(inputModel.DighararsaatHPD);
                    item.DivazifeT = Int32.Parse(inputModel.DivazifeT);
                    item.DivazifeHPR = Int32.Parse(inputModel.DivazifeHPR);
                    item.DivazifeHPD = Int32.Parse(inputModel.DivazifeHPD);

                    item.FLrasmiT = Int32.Parse(inputModel.FLrasmiT);
                    item.FLrasmiHPR = Int32.Parse(inputModel.FLrasmiHPR);
                    item.FLrasmiHPD = Int32.Parse(inputModel.FLrasmiHPD);
                    item.FLgharartamamT = Int32.Parse(inputModel.FLgharartamamT);
                    item.FLgharartamamHPR = Int32.Parse(inputModel.FLgharartamamHPR);
                    item.FLgharartamamHPD = Int32.Parse(inputModel.FLgharartamamHPD);
                    item.FLghararsaatT = Int32.Parse(inputModel.FLghararsaatT);
                    item.FLghararsaatHPR = Int32.Parse(inputModel.FLghararsaatHPR);
                    item.FLghararsaatHPD = Int32.Parse(inputModel.FLghararsaatHPD);
                    item.FLvazifeT = Int32.Parse(inputModel.FLvazifeT);
                    item.FLvazifeHPR = Int32.Parse(inputModel.FLvazifeHPR);
                    item.FLvazifeHPD = Int32.Parse(inputModel.FLvazifeHPD);

                    dbcontext.SaveChanges();


                }
                catch
                {
                }
            }



        }


        public PartialViewResult addSarmaye(string count, string Ctitle, string title, string koldollary, string kolrially, string vaheddollary, string vahedrially, string ID)
        {
            vaheddollary = vaheddollary == "" ? "0" : vaheddollary.Replace(",", "");
            vahedrially = vahedrially == "" ? "0" : vahedrially.Replace(",", "");
            Model.sarmaye model = new Model.sarmaye()
            {
                count = Int32.Parse(count),
                creatoreCo = Ctitle,
                title = title,
                kollPD = Int32.Parse(vaheddollary) * Int32.Parse(count),
                kollPR = Int32.Parse(vahedrially) * Int32.Parse(count),
                vahedPD = Int32.Parse(vaheddollary),
                vahedPR = Int32.Parse(vahedrially),
                shenasnameID = ID,


            };
            dbcontext.sarmayes.Add(model);
            dbcontext.SaveChanges();
            List<sarmaye> final = dbcontext.sarmayes.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_sarmaye.cshtml", final);


        }
        public PartialViewResult deletSarmaye(int id)
        {
            sarmaye model = dbcontext.sarmayes.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.sarmayes.Remove(model);
            dbcontext.SaveChanges();

            List<sarmaye> final = dbcontext.sarmayes.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_sarmaye.cshtml", final);
        }


        public PartialViewResult addMasrafi(string count, string Ctitle, string title, string koldollary, string kolrially, string vaheddollary, string vahedrially, string ID)
        {
            vaheddollary = vaheddollary == "" ? "0" : vaheddollary.Replace(",", "");
            vahedrially = vahedrially == "" ? "0" : vahedrially.Replace(",", "");
            Model.masrafi model = new Model.masrafi()
            {
                count = Int32.Parse(count),
                creatoreCo = Ctitle,
                title = title,
                kollPR = Int32.Parse(vahedrially) * Int32.Parse(count),
                kollPD = Int32.Parse(vaheddollary) * Int32.Parse(count),
                vahedPD = Int32.Parse(vaheddollary),
                vahedPR = Int32.Parse(vahedrially),
                shenasnameID = ID


            };

            dbcontext.masrafis.Add(model);
            dbcontext.SaveChanges();
            List<masrafi> final = dbcontext.masrafis.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_masrafi.cshtml", final);

        }
        public PartialViewResult deletMasrafi(int id)
        {

            masrafi model = dbcontext.masrafis.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.masrafis.Remove(model);
            dbcontext.SaveChanges();

            List<masrafi> final = dbcontext.masrafis.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_masrafi.cshtml", final);
        }

        public PartialViewResult addedari(string ID, string dollari, string rially, string title)
        {
            dollari = dollari == "" ? "0" : dollari.Replace(",", "");
            rially = rially == "" ? "0" : rially.Replace(",", "");
            Model.edari model = new Model.edari()
            {
                kollPD = Int32.Parse(dollari),
                kollPR = Int32.Parse(rially),
                title = title,
                shenasnameID = ID,
            };
            dbcontext.edaris.Add(model);
            dbcontext.SaveChanges();

            List<edari> final = dbcontext.edaris.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_edari.cshtml", final);

        }
        public PartialViewResult deletedari(int id)
        {
            edari model = dbcontext.edaris.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.edaris.Remove(model);
            dbcontext.SaveChanges();


            List<edari> final = dbcontext.edaris.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_edari.cshtml", final);
        }

        public PartialViewResult addomrani(string ID, string dollari, string rially, string title, string zirbana)
        {
            dollari = dollari == "" ? "0" : dollari.Replace(",", "");
            rially = rially == "" ? "0" : rially.Replace(",", "");
            Model.omrani model = new Model.omrani()
            {
                kollPD = Int32.Parse(dollari),
                kollPR = Int32.Parse(rially),
                title = title,
                zirbana = Int32.Parse(zirbana),
                shenasnameID = ID,


            };
            dbcontext.omranis.Add(model);
            dbcontext.SaveChanges();
            List<omrani> final = dbcontext.omranis.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_omrani.cshtml", final);

        }
        public PartialViewResult deletomrani(int id)
        {
            omrani model = dbcontext.omranis.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.omranis.Remove(model);
            dbcontext.SaveChanges();

            List<omrani> final = dbcontext.omranis.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_omrani.cshtml", final);
        }

        public PartialViewResult addghrardad(string ID, string dollari, string rially, string title, string level)
        {
            dollari = dollari == "" ? "0" : dollari.Replace(",", "");
            rially = rially == "" ? "0" : rially.Replace(",", "");
            Model.gharardad model = new Model.gharardad()
            {
                kollPD = Int32.Parse(dollari),
                kollPR = Int32.Parse(rially),
                title = title,
                level = Int32.Parse(level),
                shenasnameID = ID,


            };
            dbcontext.gharardads.Add(model);
            dbcontext.SaveChanges();
            List<gharardad> final = dbcontext.gharardads.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_gharardad.cshtml", final);

        }
        public PartialViewResult deletgharardad(int id)
        {
            gharardad model = dbcontext.gharardads.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.gharardads.Remove(model);
            dbcontext.SaveChanges();
            List<gharardad> final = dbcontext.gharardads.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_gharardad.cshtml", final);
        }


        public PartialViewResult addmavad(string count, string Ctitle, string title, string koldollary, string kolrially, string vaheddollary, string vahedrially, string ID)
        {
            vaheddollary = vaheddollary == "" ? "0" : vaheddollary.Replace(",","");
            vahedrially = vahedrially == "" ? "0" : vahedrially.Replace(",", ""); ;
            Model.mavad model = new Model.mavad();
            model.creatoreCo = Ctitle;
            model.title = title;
            model.kollPD = Int32.Parse(vaheddollary) * Int32.Parse(count);
            model.kollPR = Int32.Parse(vahedrially) * Int32.Parse(count);
            model.vahedPD = Int32.Parse(vaheddollary);
            model.vahedPR = Int32.Parse(vahedrially);
            model.shenasnameID = ID;
            model.count =String.IsNullOrEmpty(count)? 0 : Int32.Parse(count);
            dbcontext.mavads.Add(model);
            dbcontext.SaveChanges();
            List<mavad> final = dbcontext.mavads.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_mavad.cshtml", final);

        }
        public PartialViewResult deletMavad(int id)
        {

            List<mavad> lst = dbcontext.mavads.ToList();
            mavad model = dbcontext.mavads.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.mavads.Remove(model);
            dbcontext.SaveChanges();
            List<mavad> final = dbcontext.mavads.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_mavad.cshtml", final);
        }

        public PartialViewResult addsayer(string count, string Ctitle, string title, string koldollary, string kolrially, string vaheddollary, string vahedrially, string ID)
        {
            vaheddollary = vaheddollary == "" ? "0" : vaheddollary.Replace(",", "");
            vahedrially = vahedrially == "" ? "0" : vahedrially.Replace(",", "");
            Model.sayer model = new Model.sayer()
            {
                count = Int32.Parse(count),
                creatoreCo = Ctitle,
                title = title,
                kollPD = Int32.Parse(vaheddollary) * Int32.Parse(count),
                kollPR = Int32.Parse(vahedrially) * Int32.Parse(count),
                vahedPD = Int32.Parse(vaheddollary),
                vahedPR = Int32.Parse(vahedrially),
                shenasnameID = ID,


            };
            dbcontext.sayers.Add(model);
            dbcontext.SaveChanges();
            List<sayer> final = dbcontext.sayers.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_sayer.cshtml", final);

        }
        public PartialViewResult deletsayer(int id)
        {
            sayer model = dbcontext.sayers.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.sayers.Remove(model);
            dbcontext.SaveChanges();
            List<sayer> final = dbcontext.sayers.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_sayer.cshtml", final);
        }

        public PartialViewResult addtashvighi(string ID, string dollari, string rially, string title)
        {
            dollari = dollari == "" ? "0" : dollari.Replace(",", "");
            rially = rially == "" ? "0" : rially.Replace(",", "");
            Model.tashvighi model = new Model.tashvighi()
            {
                kollPD = Int32.Parse(dollari),
                kollPR = Int32.Parse(rially),
                title = title,
                shenasnameID = ID,

            };
            dbcontext.tashvighis.Add(model);
            dbcontext.SaveChanges();
            List<tashvighi> final = dbcontext.tashvighis.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_tashvighi.cshtml", final);

        }
        public PartialViewResult delettashvighi(int id)
        {
            tashvighi model = dbcontext.tashvighis.SingleOrDefault(x => x.ID == id);
            string ID = model.shenasnameID;
            dbcontext.tashvighis.Remove(model);
            dbcontext.SaveChanges();

            List<tashvighi> final = dbcontext.tashvighis.Where(x => x.shenasnameID == ID).ToList();
            return PartialView("/Views/Shared/_tashvighi.cshtml", final);
        }

        [HttpPost]
        public ActionResult SetFileInfo(string catName, string itemID)
        {
            catName = catName.Trim();
            string imagename = "";
            string pathString = "~/files";
            int type = 1;
            for (int i = 0; i < Request.Files.Count; i++)
            {

                HttpPostedFileBase hpf = Request.Files[i];
                string extension = Path.GetExtension(hpf.FileName);
                switch (extension)
                {
                    case ".pdf":
                        type = 0;
                        break;

                }

                if (hpf.ContentLength == 0)
                    continue;
                imagename = hpf.FileName;// RandomString(7) + ".jpg";
                string savedFileName = Path.Combine(Server.MapPath(pathString), hpf.FileName);
                hpf.SaveAs(savedFileName);
                shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == itemID);
                switch (catName)
                {
                    case "بایگانی موارد":
                        shen.bayganiFile = imagename;
                        break;
                    case "قرارداد":
                        shen.gharardadFile = imagename;
                        break;
                    case "متمم":
                        shen.motamamFile = imagename;
                        break;
                    case "پیوست متنی":
                        shen.peyvastFile = imagename;
                        break;
                    case "لیست مواد":
                        shen.listmavadFile = imagename;
                        break;
                    case "گانت چارت":
                        shen.gantFile = imagename;
                        break;
                    case "مجوز ستاد کل":
                        shen.mojavezFile = imagename;
                        break;
                    case "گزارش پیشرفت":
                        shen.pishraftFile = imagename;
                        break;
                }

            }
            dbcontext.SaveChanges();
            return RedirectToAction("showReport", new { id = itemID, notif = "1" });
        }

        public ActionResult showItem(string itemID, string catName)
        {
            catName = catName.Trim();
            string ITEMNAME = "";
            string filename = "";
            shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == itemID);
            switch (catName)
            {
                case "بایگانی موارد":
                    filename = shen.bayganiFile;
                    break;
                case "قرارداد":
                    filename = shen.gharardadFile;
                    break;
                case "متمم":
                    filename = shen.motamamFile;
                    break;
                case "پیوست متنی":
                    filename = shen.peyvastFile;
                    break;
                case "لیست مواد":
                    filename = shen.listmavadFile;
                    break;
                case "گانت چارت":
                    filename = shen.gantFile;
                    break;
                case "مجوز ستاد کل":
                    filename = shen.mojavezFile;
                    break;
                case "گزارش پیشرفت":
                    filename = shen.pishraftFile;
                    break;
            }
            string extension = Path.GetExtension(filename);
            int type = 1;
            switch (extension)
            {
                case ".pdf":
                    type = 0;
                    break;

            }
            viewFileVM model = new viewFileVM()
            {
                filename = filename,
                type = type
            };
            return PartialView("/Views/Shared/_viewFile.cshtml", model);
        }


        public ActionResult getFactor(int id,string tamin)
        {
            string extension = "";
            string filename = "";
            if (String.IsNullOrEmpty(tamin))
            {
                archive factor = dbcontext.Archives.SingleOrDefault(x => x.ID == id);
                filename = factor.imageName;
                extension = Path.GetExtension(factor.imageName);
            }
            else
            {
                tamin factor = dbcontext.tamins.SingleOrDefault(x => x.ID == id);
                filename = factor.imageName;
                extension = Path.GetExtension(factor.imageName);
            }
           
            int type = 1;
            switch (extension)
            {
                case ".pdf":
                    type = 0;
                    break;

            }
            viewFileVM model = new viewFileVM()
            {
                filename = filename,
                type = type
            };
            return PartialView("/Views/Shared/_viewFile.cshtml", model);
        }

        public ActionResult income_List(List<Model.archive> lst)
        {
            if (lst == null)
            {
                lst = (from p in dbcontext.Archives where p.hesab == "1" select p).OrderBy(x => x.ID).ToList();
            }

            string permisson = Session["permission"] as string;

            string see = permisson.Contains("11,") ? "1" : "";
            string edit = permisson.Contains("12,") ? "1" : "";
            string add = permisson.Contains("13,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            incomeListVM model = new incomeListVM()
            {
                incomeList = lst,
                bankList = dbcontext.banks.ToList(),
                shenList = dbcontext.shenasnames.Where(x => x.master == "1" && x.final == 1).ToList(),
                 add = add,
                   edit = edit
            };
            return View(model);
        }
        public ActionResult addincomeForm(string onvanVarizi, string shomareSanad, string shenNumber, string inputType, string vahed, string bankCombo, string date, string price)
        {
            string permisson = Session["permission"] as string;

            string see = permisson.Contains("11,") ? "1" : "";
            string edit = permisson.Contains("12,") ? "1" : "";
            string add = permisson.Contains("13,") ? "1" : "";
            if (edit == "")
                return RedirectToAction("income_List");
            string pathString = "/files";
            string imagename = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase hpf = Request.Files[i];
                imagename = hpf.FileName;
                if (hpf.ContentLength == 0)
                    continue;

                string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
                hpf.SaveAs(savedFileName);
            }
            using (Model.Context dbcontext = new Model.Context())
            {
                DateTime trk = date.ToGeorgianDateTime();
                string typ = vahed;

                shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == shenNumber);
                bool isreferbish = false;
                long mbl = long.Parse(price.Replace(",", ""));
                archive newITem = new archive()
                {


                    mablagh = mbl,
                    markaz = "",
                    project = shenNumber,
                    shnesnameTitle = shen.title,// shenasnameTitle,
                    shomareSanad = shomareSanad,
                    tarikh = trk,
                    type = typ,
                    hesab = "1",
                    imageName = imagename,
                    bankName = bankCombo,
                    checkNumber = shomareSanad,
                    referbish = isreferbish,
                    subject = onvanVarizi


                };
                dbcontext.Archives.Add(newITem);
                dbcontext.SaveChanges();
            }

            return RedirectToAction("income_List");
        }


       


        public ActionResult checkTaminNumner(string data)
        {

            tamin model = dbcontext.tamins.SingleOrDefault(x => x.shomareSanad == data);
            if (model!= null)
            {
                return Content(model.mablagh.ToString());
            }
            else
            {
                return Content("error");
            }
        }
        public ActionResult getTaminPartial()
        {
           
                List<Model.tamin> lst = (from p in dbcontext.tamins
                                         where p.hesab == "0"
                                         select p).OrderBy(x => x.ID).ToList();

           


            return PartialView("/Views/Shared/_taminPartial.cshtml",lst);
        }



        public ActionResult addTaminForm(int onvanNumber, string gamNumber, string shenNumber, string exType, string vahed, string date, string price, string shomareName)
        {
            shenasname shen = dbcontext.shenasnames.SingleOrDefault(x => x.ID == shenNumber);
            var lstTamin = dbcontext.tamins.Where(x => x.hesab == "1" && x.shnesnameTitle == shen.title).ToList();

            long final = lstTamin.Count() != 0 ? lstTamin.Sum(x => x.mablagh) : 0;
            final += long.Parse(price.Replace(",",""));

            List<archive> lst = dbcontext.Archives.ToList();
            var DaramadLst = dbcontext.Archives.Where(x => x.hesab == "1" && x.shnesnameTitle == shen.title && x.type == vahed).ToList() ;
            long finalDaramad = DaramadLst.Count() != 0 ? DaramadLst.Sum(x => x.mablagh) : 0;
            if (final > finalDaramad)
            {
                return RedirectToAction("tamin_List",new { error  ="1"});
            }

            string pathString = "/files";
            string imagename = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase hpf = Request.Files[i];
                imagename = hpf.FileName;
                if (hpf.ContentLength == 0)
                    continue;

                string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
                hpf.SaveAs(savedFileName);
            }
            using (Model.Context dbcontext = new Model.Context())
            {
                DateTime trk = date.ToGeorgianDateTime();
                string typ = vahed;
                string rdTitle = "";

                switch (exType)
                {

                    case "اجراییات":
                        rdTitle = dbcontext.ejraeiats.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "سرمایه":
                        rdTitle = dbcontext.sarmayes.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "قرارداد":
                        rdTitle = dbcontext.gharardads.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "عمرانی":
                        rdTitle = dbcontext.omranis.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "اداری":
                        rdTitle = dbcontext.edaris.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "مصرفی":
                        rdTitle = dbcontext.masrafis.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "مواد":
                        rdTitle = dbcontext.sayers.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                    case "تشویقی":
                        rdTitle = dbcontext.tashvighis.SingleOrDefault(x => x.ID == onvanNumber).title;
                        break;
                }

                tamin newITem = new tamin()
                {

                    radif = onvanNumber.ToString(),
                    mablagh = long.Parse(price.Replace(",","")),
                    markaz = "",
                    project = shenNumber,
                    shnesnameTitle = shen.title,
                    gam = gamNumber,
                    radifTitle = rdTitle,
                    shomareSanad = shomareName,
                    subject = exType,
                    tarikh = trk,
                    type = typ,
                    hesab = "0",
                    imageName = imagename,
                    //bankName = bank.Text,
                    //checkNumber = checkNumber.Text,
                    //referbish = isreferbish

                };
                dbcontext.tamins.Add(newITem);
                dbcontext.SaveChanges();

            }

            return RedirectToAction("tamin_List");
        }


        public ActionResult tamin_List(string hesab,string error)
        {
            string permisson = Session["permission"] as string;
            string see = permisson.Contains("31,") ? "1" : "";
            string edit = permisson.Contains("32,") ? "1" : "";
            string add = permisson.Contains("33,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            if (error == "1")
            {
                ViewBag.message = "درآمد حاصله از پروژه کافی نمی باشد";
            }

            ViewBag.hesab = hesab;
            var lst = dbcontext.tamins.AsQueryable();
            if (!String.IsNullOrEmpty(hesab))
            {
                ViewBag.hesab = hesab;
                lst = lst.Where(x => x.hesab == hesab);
            }

            taminVM model = new taminVM()
            {
                taminList = lst.ToList(),
                shenList = dbcontext.shenasnames.Where(x => x.master == "1" && x.final == 1).ToList(),
                 add = add,
                 edit = edit
            };

            List<shenasname> ls = dbcontext.shenasnames.ToList();


            return View(model);
        }
        public ActionResult deletTamin(int shomareSanad, int id)
        {

            tamin model = dbcontext.tamins.SingleOrDefault(x => x.ID == id);
            List<archive> rmodel = dbcontext.Archives.Where(x => x.shomareTamin == model.shomareSanad).ToList();
            if (rmodel.Count() == 0)
            {
                dbcontext.tamins.Remove(model);
                dbcontext.SaveChanges();

                return Content("200");
            }
            else
            {
                return Content("300");
            }

        }


        public ActionResult getGam(string id)
        {

            List<shenasnameGam> lst = dbcontext.shenasnameGams.Where(x => x.shenasnameID == id).ToList();
            return PartialView("/Views/Shared/_returnGam.cshtml", lst);
        }
        public ActionResult getDetail(string id, string type)
        {
            List<dropDownVM> lst = new List<dropDownVM>();

            List<ejraeiat> kkk = dbcontext.ejraeiats.ToList();
            switch (type)
            {
                case "اجراییات":
                    lst = (from p in dbcontext.ejraeiats
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "سرمایه":
                    lst = (from p in dbcontext.sarmayes
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "قرارداد":
                    lst = (from p in dbcontext.gharardads
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "عمرانی":
                    lst = (from p in dbcontext.omranis
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "اداری":
                    lst = (from p in dbcontext.edaris
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "مصرفی":
                    lst = (from p in dbcontext.masrafis
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "مواد اولیه":
                    lst = (from p in dbcontext.sayers
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                case "تشویقی":
                    lst = (from p in dbcontext.tashvighis
                           where p.shenasnameID == id
                           select (new dropDownVM { title = p.title, ID = p.ID })).ToList();
                    break;
                    //case "سایر هزینه ها":
                    //    List<sayer> lst = dbcontext.shenasnameGams.Where(x => x.shenasnameID == id).ToList();
                    //    return PartialView("/Views/Shared/_returnDetail.cshtml", lst);
                    //    break;


            }

            return PartialView("/Views/Shared/_returnDropDown.cshtml", lst);
        }





        public ActionResult outcome_List(List<Model.archive> lst)
        {

            string permisson = Session["permission"] as string;
            string see = permisson.Contains("41,") ? "1" : "";
            string edit = permisson.Contains("42,") ? "1" : "";
            string add = permisson.Contains("43,") ? "1" : "";
            if (see == "")
                return RedirectToAction("login");
            if (lst == null)
            {
                var lst0 = (from p in dbcontext.Archives
                            join t in dbcontext.tamins on p.shomareTamin equals t.shomareSanad
                           
                            where p.hesab == "0"
                            orderby p.ID
                            select new { shomareTamin = t.shomareSanad, shnesnameTitle = t.shnesnameTitle, radifTitle = t.radifTitle, ID = p.ID, tarikh = p.tarikh, hesab = p.hesab, bankName = p.bankName, checkNumber = p.checkNumber, mablagh = t.mablagh, radif = t.radif, subject = t.subject, project = t.project, markaz = t.markaz, shomareSanad = p.shomareSanad, type = t.type }).ToList();
                //dataGridView1.DataSource = lst;
                lst = new List<Model.archive>();
                foreach (var item in lst0)
                {
                    lst.Add(new Model.archive
                    {
                        bankName = item.bankName,
                        checkNumber = item.checkNumber,
                        hesab = item.hesab,
                        ID = item.ID,
                        imageName = "",
                        mablagh = item.mablagh,
                        markaz = item.markaz,
                        project = item.project,
                        radif = item.radif,
                        radifTitle = item.radifTitle,
                        shnesnameTitle = item.shnesnameTitle,
                        shomareSanad = item.shomareSanad,
                        shomareTamin = item.shomareTamin,
                        subject = item.subject,
                        tarikh = item.tarikh,
                        type = item.type
                    });
                }
            }

            outcomVM model = new outcomVM()
            {
                outcomList = lst,
                shenList = dbcontext.shenasnames.Where(x => x.master == "1" && x.final == 1).ToList(),
                 edit = edit,
                  add = add
            };

            return View(model);
        }

        public ActionResult getBank(string type)
        {
            
            List<dropDownVM> lst = new List<dropDownVM>();
            using (Context dbcontext = new Context())
            {


                if (type == "منابع غیر تولید")
                {

                    lst = (from p in dbcontext.banks
                           where p.type == "منابع غیر تولید"
                           select new dropDownVM { title = p.title, ID = p.ID }).ToList();
                }
                else
                {
                    lst = (from p in dbcontext.banks
                           where p.type != "منابع غیر تولید" || p.type == null
                           select new dropDownVM { title = p.title, ID = p.ID, classname = "bank" }).ToList();
                }



            }
            return PartialView("/Views/Shared/_returnBank.cshtml", lst);
        }
        public ActionResult getCheck(int id)
        {
            List<dropDownVM> lst = new List<dropDownVM>();
            lst = (from p in dbcontext.checks
                   where p.bankID == id && p.isUsed == false
                   select new dropDownVM { title = p.checkNumber, ID = p.ID }).ToList();


            return PartialView("/Views/Shared/_returnCheck.cshtml", lst);
        }
        public ActionResult addExenceForm(string taminText, string bankType, string shomareSanad, int bankList, string checkList, string date, string price)
        {
            string pathString = "/files";
            string imagename = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase hpf = Request.Files[i];
                imagename = hpf.FileName;
                if (hpf.ContentLength == 0)
                    continue;

                string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
                hpf.SaveAs(savedFileName);
            }

            bank bnk = dbcontext.banks.SingleOrDefault(x => x.ID == bankList);
            using (Model.Context dbcontext = new Model.Context())
            {
                DateTime trk = date.ToGeorgianDateTime();


                bool isreferbish = false;

                archive newITem = new archive()
                {

                    radif = "",
                    mablagh = long.Parse(price.Replace(",","")),
                    markaz = "",
                    project = "",
                    shnesnameTitle = "",
                    radifTitle = "",
                    shomareSanad = shomareSanad,
                    shomareTamin = taminText,
                    subject = "",
                    tarikh = trk,
                    type = "",
                    hesab = "0",
                    imageName = imagename,
                    bankName = bnk.title,
                    checkNumber = checkList,
                    referbish = isreferbish

                };

                check checkitem = dbcontext.checks.SingleOrDefault(x => x.checkNumber == checkList);
                checkitem.isUsed = true;


                tamin taminitem = dbcontext.tamins.SingleOrDefault(x => x.shomareSanad == taminText);
                taminitem.hesab = "1";

                dbcontext.Archives.Add(newITem);
                dbcontext.SaveChanges();
            }

            return RedirectToAction("outcome_List");
        }

        public ActionResult menu()
        {
            string srt = Session["permission"] as string;
            string name = Request.Cookies["name"].Value as string;
            string jaygah = Request.Cookies["jaygah"].Value as string;

            menuVM model = new menuVM()
            {
                 jaygah = jaygah,
                  name = name,
                   lst = srt
            };

            if (model.lst == null)
            {
                return Content("");
            }

            return PartialView("/Views/Shared/_menu.cshtml", model);
        }


    }
}