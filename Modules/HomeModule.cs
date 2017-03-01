using Nancy;
using System.Collections.Generic;
using System;

namespace Library
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Overdue", Checkout.GetAllOverdue("2016-01-01"));
                return View["index.cshtml", model];
            };
        }

        public Dictionary<string, object> ModelMaker()
        {
            Dictionary<string, object> model = new Dictionary<string, object>{
                {"Books", Book.GetAll()},
                {"Authors", Author.GetAll()},
                {"Patrons", Patron.GetAll()}
            };
            return model;
        }
    }
}
