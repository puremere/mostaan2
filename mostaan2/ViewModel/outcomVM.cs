using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class outcomVM
    {
        public List<Model.archive> outcomList { get; set; }
        public List<Model.shenasname> shenList { get; set; }
    }
    public class taminVM
    {
        public List<Model.tamin> taminList { get; set; }
        public List<Model.shenasname> shenList { get; set; }
    }
}