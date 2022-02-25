using mostaan2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class showReportVM
    {

        public string comment { get; set; }
        public List<markaz> markazList { get; set; }
        public List<shenasnameFounder> lstFounder { get; set; }
        public List<user> userlist { get; set; }
        public List<ejraeiat> ejraiatlist { get; set; }
        public List<sarmaye> sarmayelist { get; set; }
        public List<masrafi> masrafilist { get; set; }
        public List<edari> edarilist { get; set; }
        public List<omrani> omranilist { get; set; }
        public List<gharardad> gharardadlist { get; set; }
        public List<sayer> sayerlist { get; set; }
        public List<mavad> mavadlist { get; set; }
        public List<tashvighi> tashvighilist { get; set; }
        public List<shenasnameGam> shenasnameGamList { get; set; }
        public bool isFianl { get; set; }
        public string itemID { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string prejraeeyat { get; set; }
        public string pdejraiat { get; set; }
        public string hrejrayiat { get; set; }
        public string hdejraeiat { get; set; }
        public string prsarmaye { get; set; }
        public string pdsarmaye { get; set; }
        public string hrsarmaye { get; set; }
        public string hdsarmaye { get; set; }
        public string prmasrafi { get; set; }
        public string pdmasrafi { get; set; }
        public string hrmasrafi { get; set; }
        public string hdmasrafi { get; set; }

        public string prsayer { get; set; }
        public string pdsayer { get; set; }
        public string hrsayer { get; set; }
        public string hdsayer { get; set; }

        public string predari { get; set; }
        public string pdedari { get; set; }
        public string hredari { get; set; }
        public string hdedari { get; set; }
        public string promrani { get; set; }
        public string pdomrani { get; set; }
        public string hromrani { get; set; }
        public string hdomrani { get; set; }
        public string prgharardad { get; set; }
        public string pdgharardad { get; set; }
        public string hrgharardad { get; set; }
        public string hdgharardad { get; set; }
        public string pravalie { get; set; }
        public string pdavalie { get; set; }
        public string hravalie { get; set; }
        public string hdavalie { get; set; }
        public string prtashvighi { get; set; }
        public string pdtashvighi { get; set; }
        public string hrtashvighi { get; set; }
        public string hdtashvighi { get; set; }
        public string hrsayerhazine { get; set; }
        public string hdsayerhazine { get; set; }
        public string hrkoll { get; set; }
        public string hdkoll { get; set; }
        public string prkoll { get; set; }
        public string pdkoll { get; set; }
        public string hadaf { get; set; }
        public string title { get; set; }
        public string datePishbini { get; set; }
        public string markaz { get; set; }
        public string tarah { get; set; }
        public string dastgah { get; set; }





        public int DrasmiT { get; set; }
        public int DrasmiHPR { get; set; }
        public int DrasmiHPD { get; set; }
        public int DrasmiHNR { get; set; }
        public int DrasmiHND { get; set; }
        public int DgharartamamT { get; set; }
        public int DgharartamamHPR { get; set; }
        public int DgharartamamHPD { get; set; }
        public int DgharartamamHNR { get; set; }
        public int DgharartamamHND { get; set; }
        public int DghararsaatT { get; set; }
        public int DghararsaatHPR { get; set; }
        public int DghararsaatHPD { get; set; }
        public int DghararsaatHNR { get; set; }
        public int DghararsaatHND { get; set; }
        public int DvazifeT { get; set; }
        public int DvazifeHPR { get; set; }
        public int DvazifeHPD { get; set; }
        public int DvazifeHNR { get; set; }
        public int DvazifeHND { get; set; }
        public int FLrasmiT { get; set; }
        public int FLrasmiHPR { get; set; }
        public int FLrasmiHPD { get; set; }
        public int FLrasmiHNR { get; set; }
        public int FLrasmiHND { get; set; }
        public int FLgharartamamT { get; set; }
        public int FLgharartamamHPR { get; set; }
        public int FLgharartamamHPD { get; set; }
        public int FLgharartamamHNR { get; set; }
        public int FLgharartamamHND { get; set; }
        public int FLghararsaatT { get; set; }
        public int FLghararsaatHPR { get; set; }
        public int FLghararsaatHPD { get; set; }
        public int FLghararsaatHNR { get; set; }
        public int FLghararsaatHND { get; set; }
        public int FLvazifeT { get; set; }
        public int FLvazifeHPR { get; set; }
        public int FLvazifeHPD { get; set; }
        public int FLvazifeHNR { get; set; }
        public int FLvazifeHND { get; set; }

        public int LrasmiT { get; set; }
        public int LrasmiHPR { get; set; }
        public int LrasmiHPD { get; set; }
        public int LrasmiHNR { get; set; }
        public int LrasmiHND { get; set; }
        public int LgharartamamT { get; set; }
        public int LgharartamamHPR { get; set; }
        public int LgharartamamHPD { get; set; }
        public int LgharartamamHNR { get; set; }
        public int LgharartamamHND { get; set; }
        public int LghararsaatT { get; set; }
        public int LghararsaatHPR { get; set; }
        public int LghararsaatHPD { get; set; }
        public int LghararsaatHNR { get; set; }
        public int LghararsaatHND { get; set; }
        public int LvazifeT { get; set; }
        public int LvazifeHPR { get; set; }
        public int LvazifeHPD { get; set; }
        public int LvazifeHNR { get; set; }
        public int LvazifeHND { get; set; }

        public int FDrasmiT { get; set; }
        public int FDrasmiHPR { get; set; }
        public int FDrasmiHPD { get; set; }
        public int FDrasmiHNR { get; set; }
        public int FDrasmiHND { get; set; }
        public int FDgharartamamT { get; set; }
        public int FDgharartamamHPR { get; set; }
        public int FDgharartamamHPD { get; set; }
        public int FDgharartamamHNR { get; set; }
        public int FDgharartamamHND { get; set; }
        public int FDghararsaatT { get; set; }
        public int FDghararsaatHPR { get; set; }
        public int FDghararsaatHPD { get; set; }
        public int FDghararsaatHNR { get; set; }
        public int FDghararsaatHND { get; set; }
        public int FDvazifeT { get; set; }
        public int FDvazifeHPR { get; set; }
        public int FDvazifeHPD { get; set; }
        public int FDvazifeHNR { get; set; }
        public int FDvazifeHND { get; set; }

        public int DirasmiT { get; set; }
        public int DirasmiHPR { get; set; }
        public int DirasmiHPD { get; set; }
        public int DirasmiHNR { get; set; }
        public int DirasmiHND { get; set; }
        public int DigharartamamT { get; set; }
        public int DigharartamamHPR { get; set; }
        public int DigharartamamHPD { get; set; }
        public int DigharartamamHNR { get; set; }
        public int DigharartamamHND { get; set; }
        public int DighararsaatT { get; set; }
        public int DighararsaatHPR { get; set; }
        public int DighararsaatHPD { get; set; }
        public int DighararsaatHNR { get; set; }
        public int DighararsaatHND { get; set; }
        public int DivazifeT { get; set; }
        public int DivazifeHPR { get; set; }
        public int DivazifeHPD { get; set; }
        public int DivazifeHNR { get; set; }
        public int DivazifeHND { get; set; }

        public int ZDirasmiT { get; set; }
        public int ZDirasmiHPR { get; set; }
        public int ZDirasmiHPD { get; set; }
        public int ZDirasmiHNR { get; set; }
        public int ZDirasmiHND { get; set; }
        public int ZDigharartamamT { get; set; }
        public int ZDigharartamamHPR { get; set; }
        public int ZDigharartamamHPD { get; set; }
        public int ZDigharartamamHNR { get; set; }
        public int ZDigharartamamHND { get; set; }
        public int ZDighararsaatT { get; set; }
        public int ZDighararsaatHPR { get; set; }
        public int ZDighararsaatHPD { get; set; }
        public int ZDighararsaatHNR { get; set; }
        public int ZDighararsaatHND { get; set; }
        public int ZDivazifeT { get; set; }
        public int ZDivazifeHPR { get; set; }
        public int ZDivazifeHPD { get; set; }
        public int ZDivazifeHNR { get; set; }
        public int ZDivazifeHND { get; set; }




    }
}