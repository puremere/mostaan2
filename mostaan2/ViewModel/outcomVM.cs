﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class outcomVM
    {
        public List<Model.archive> outcomList { get; set; }
        public List<Model.shenasname> shenList { get; set; }
        public string edit { get; set; }
        public string add { get; set; }
    }
    public class taminVM
    {
        public List<Model.tamin> taminList { get; set; }
        public List<Model.shenasname> shenList { get; set; }
        public string edit { get; set; }
        public string add { get; set; }
    }
    public class graphVM
    {
        public List<Model.shenasname> shenList { get; set; }
        public List<string> names { get; set; }
        public List<long> daramad { get; set; }
        public List<long> hazine { get; set; }
    }

}