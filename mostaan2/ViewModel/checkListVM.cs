using mostaan2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class checkLVM
    {
        public int ID { get; set; }
        public string checkNumber { get; set; }
        public string banktitle { get; set; }
        public bool isused { get; set; }
        public string banknumber { get; set; }
        public long price { get; set; }
    }
    public class checkListVM
    {
        
       public List<checkLVM> checklst { get; set; }
        public List<bank> banklst { get; set; }
    }
}