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

            Post["/add-book"] = _ => {
                Book newBook = new Book(Request.Form["book-name"], Request.Form["copies"]);
                newBook.Save();

                Dictionary<string, object> model = ModelMaker();
                model.Add("Book", newBook);

                return View["book.cshtml", model];
            };

            Post["/add-author"] = _ => {
                Author newAuthor = new Author(Request.Form["author-name"]);
                newAuthor.Save();

                Dictionary<string, object> model = ModelMaker();
                model.Add("Author", newAuthor);

                return View["author.cshtml", model];
            };


            Get["/author/{id}"] = parameters => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Author", Author.Find(parameters.id));

                return View["author.cshtml", model];
            };

            Post["/author/{id}"] = parameters => {
                Author.Find(parameters.id).AddBook(Request.Form["book-id"]);

                Dictionary<string, object> model = ModelMaker();
                model.Add("Author", Author.Find(parameters.id));

                return View["author.cshtml", model];
            };

            Get["/book/{id}"] = parameters => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Book", Book.Find(parameters.id));

                return View["book.cshtml", model];
            };

            Post["/book/{id}"] = parameters => {
                Book.Find(parameters.id).AddAuthor(Request.Form["author-id"]);

                Dictionary<string, object> model = ModelMaker();
                model.Add("Book", Book.Find(parameters.id));

                return View["book.cshtml", model];
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
