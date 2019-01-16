using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BigNumManager
{
    static List<string> unit = new List<string>();

    static BigNumManager()
    {
        unit.Add("");
        unit.Add("K");
        unit.Add("M");
        unit.Add("B");
        unit.Add("T");
        unit.Add("aa");
        unit.Add("ab");
        unit.Add("ac");
        unit.Add("ad");
        unit.Add("ae");
        unit.Add("af");
        unit.Add("ag");
        unit.Add("ah");
        unit.Add("ai");
        unit.Add("aj");
        unit.Add("ba");
        unit.Add("bb");
        unit.Add("bc");
        unit.Add("bd");
        unit.Add("be");
        unit.Add("bf");
        unit.Add("bg");
        unit.Add("bh");
        unit.Add("bi");
        unit.Add("bj");
        unit.Add("ca");
        unit.Add("cb");
        unit.Add("cc");
        unit.Add("cd");
        unit.Add("ce");
        unit.Add("cf");
        unit.Add("cg");
        unit.Add("ch");
        unit.Add("ci");
        unit.Add("cj");
        unit.Add("da");
        unit.Add("db");
        unit.Add("dc");
        unit.Add("dd");
        unit.Add("de");   
        unit.Add("df");
        unit.Add("dg");
        unit.Add("dh");
        unit.Add("di");
        unit.Add("dj");
        unit.Add("ea");
        unit.Add("eb");
        unit.Add("ec");
        unit.Add("ed");
        unit.Add("ee");
        unit.Add("ef");
        unit.Add("eg");
        unit.Add("eh");
        unit.Add("ei");
        unit.Add("ej");
        unit.Add("fa");
        unit.Add("fb");
        unit.Add("fc");
        unit.Add("fd");
        unit.Add("fe");
        unit.Add("ff");        
    }
    
    public static string BigNumString(float numParam)
    {
        double num = numParam;

        int unitToUse = 0;
        while (num >= 1000d)
        {
            unitToUse++;
            num /= 1000d;
        }

        if (numParam != 0)
            return string.Format("{0}{1}", num.ToString("#.00"), unit[unitToUse]);
        else
            return "0";
    }
}
