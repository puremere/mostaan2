using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace mostaan2.Classes
{
    public static class problemC
    {
        public static string signe = "";
        public static void switchSigne()
        {
            switch (signe) {
                case "#":
                    signe = ".";
                    break;
                case ".":
                    signe = "#";
                    break;
            }
        }

        public static void readInputAndcompare()
        {

            int counter = 0;
            List<string> inputLines = System.IO.File.ReadLines(@"g:\sample.in").ToList();
            List<string> outputLines = System.IO.File.ReadLines(@"g:\sample.ans").ToList();
            List<string> newOutPut = new List<string>();
            int Number = 0;
            int inputCount = inputLines.Count - 1;
            for (int i=0; i< inputCount; i++ )
            {
                string line = inputLines[i].ToString();
                string noSpaceline = line.Replace(" ", "");
                try
                {
                    Number += Int32.Parse(noSpaceline);
                }
                catch (Exception)
                {
                    bool ismatch = true;
                    char[] charlist = noSpaceline.ToCharArray();

                    signe = charlist[0].ToString();
                    string finalOutPut = "";
                    for (int j = 1; j <= charlist.Count() - 1; j++)
                    {
                        int charNum = Int32.Parse(charlist[j].ToString());
                        for (int k = 1; k <= charNum; k++)
                        {
                            finalOutPut += signe;
                        }
                        switchSigne();
                    }

                    if (outputLines[i - 1] != finalOutPut)
                    {
                        ismatch = false;
                        i = Number+1 ; // زمانی که در هر خطی از عکس یک عدم تطابق پیدا شود به اول عکس بعدی میرویم
                        newOutPut = outputLines;
                        newOutPut[i] = "error decoding image";
                        System.IO.File.WriteAllLines(@"g:\sampleFinal.ans", newOutPut);
                    }

                        
                    
                    finalOutPut = "";
                }
                System.Console.WriteLine(line);
                counter++;
            }
        }
    }

}