using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class incomeListVM
    {
        public List<Model.archive> incomeList { get; set; }
        public List<Model.shenasname> shenList { get; set; }
        public List<Model.bank> bankList { get; set; }
        public string edit { get; set; }
        public string add { get; set; }
    }
}