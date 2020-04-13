using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameAndHang.Controllers
{
    public class CustomErrorController : Controller
    {
        // GET: CustomError
        [HandleError]
        public ActionResult Index()
        {
            return View("Error");
        }
        [HandleError]
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }
    }
}