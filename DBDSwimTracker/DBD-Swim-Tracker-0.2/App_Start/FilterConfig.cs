﻿using System.Web;
using System.Web.Mvc;

namespace DBD_Swim_Tracker_0._2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
