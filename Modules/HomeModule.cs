using Nancy;
using System.Collections.Generic;
using System;

namespace Library
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            string thisDay = "1";

            Get["/"] = _ => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Overdue", Checkout.GetAllOverdue(thisDay));
                return View["index.cshtml", model];
            };

            Post["/update-date"] = _ => {
                thisDay = Request.Form["current-day"];
                Dictionary<string, object> model = ModelMaker();
                model.Add("Overdue", Checkout.GetAllOverdue(thisDay));
                return View["index.cshtml", model];
            };

            Post["/add-book"] = _ => {
                Book newBook = new Book(Request.Form["book-name"], Request.Form["copies"]);
                newBook.Save();

                if(Request.Form["author-name"] != null)
                {
                    Author newAuthor = new Author(Request.Form["author-name"]);
                    newAuthor.Save();
                    newBook.AddAuthor(newAuthor.GetId());
                }

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

            Post["/patron/{patronId}/return/{id}"] = parameters => {
                Checkout.Return(parameters.id);

                Dictionary<string, object> model = ModelMaker();
                model.Add("Patron", Patron.Find(parameters.patronId));
                model.Add("Patron Checkouts", Patron.Find(parameters.patronId).GetCheckouts());

                return View["patron.cshtml", model];
            };

            Post["/add-patron"] = _ => {
                Patron newPatron = new Patron(Request.Form["patron-name"], Request.Form["patron-phone"]);
                newPatron.Save();

                Dictionary<string, object> model = ModelMaker();
                model.Add("Patron", newPatron);
                model.Add("Patron Checkouts", newPatron.GetCheckouts());

                return View["patron.cshtml", model];
            };

            Get["/patron/{id}"] = parameters => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Patron", Patron.Find(parameters.id));
                model.Add("Patron Checkouts", Patron.Find(parameters.id).GetCheckouts());

                return View["patron.cshtml", model];
            };

            Post["/patron/{id}"] = parameters => {
                Checkout newCheckout = new Checkout(Request.Form["due-date"], parameters.id, Request.Form["booklist"]);
                newCheckout.Save(Book.Find(Request.Form["booklist"]));
                Dictionary<string, object> model = ModelMaker();
                model.Add("Patron", Patron.Find(parameters.id));
                model.Add("Patron Checkouts", Patron.Find(parameters.id).GetCheckouts());

                return View["patron.cshtml", model];
            };

            Patch["/patron/{id}"] = parameters => {
                Patron.Find(parameters.id).Update(Request.Form["new-name"], Request.Form["new-phone"]);

                Dictionary<string, object> model = ModelMaker();
                model.Add("Patron", Patron.Find(parameters.id));
                model.Add("Patron Checkouts", Patron.Find(parameters.id).GetCheckouts());

                return View["patron.cshtml", model];
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

            Patch["/author/{id}"] = parameters => {
                Author.Find(parameters.id).Update(Request.Form["new-name"]);
                Dictionary<string, object> model = ModelMaker();
                model.Add("Author", Author.Find(parameters.id));

                return View["author.cshtml", model];
            };

            Get["/book/{id}"] = parameters => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Book", Book.Find(parameters.id));

                return View["book.cshtml", model];
            };

            Delete["/book/{id}"] = parameters => {
                Dictionary<string, object> model = ModelMaker();
                model.Add("Overdue", Checkout.GetAllOverdue(thisDay));
                Book.Delete(parameters.id);

                return View["index.cshtml", model];
            };

            Post["/book/{id}"] = parameters => {
                Book.Find(parameters.id).AddAuthor(Request.Form["author-id"]);

                Dictionary<string, object> model = ModelMaker();
                model.Add("Book", Book.Find(parameters.id));

                return View["book.cshtml", model];
            };

            Post["/book/{id}/remove_author"] = parameters => {
                Book.Find(parameters.id).RemoveAuthor(Request.Form["author-id"]);

                Dictionary<string, object> model = ModelMaker();
                model.Add("Book", Book.Find(parameters.id));

                return View["book.cshtml", model];
            };

            Patch["/book/{id}"] = parameters => {
                Book.Find(parameters.id).Update(Request.Form["new-name"], Request.Form["new-copies"]);
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
