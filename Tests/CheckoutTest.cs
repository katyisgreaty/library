using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace Library
{
    public class CheckoutTest : IDisposable
    {
        public CheckoutTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Checkout_EmptyOnLoad_0()
        {
            Assert.Equal(0, Checkout.GetAll().Count);
        }
        [Fact]
        public void Checkout_EqualityTest_1()
        {
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            Checkout secondCheckout = new Checkout("2017-05-1", 1, 1);

            Assert.Equal(newCheckout, secondCheckout);
        }
        [Fact]
        public void Save_SavesCheckoutToDatabase_2()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();
            Book newBook = new Book("Old Man and the Sea", 5);
            newBook.Save();
            newBook.AddAuthor(newAuthor.GetId());
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            Checkout newCheckout = new Checkout("2017-05-1", newPatron.GetId(), newBook.GetId());
            newCheckout.Save(newBook);


            Assert.Equal(4, newBook.GetCopies());
        }

        [Fact]
        public void Checkout_Find_3()
        {
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            Book newBook = new Book("Old Man and the Sea", 4);
            newBook.Save();
            newCheckout.Save(newBook);

            Checkout foundCheckout = Checkout.Find(newCheckout.GetId());

            Assert.Equal(newCheckout, foundCheckout);
        }

        [Fact]
        public void Checkout_Return_DoNotRemoveFromDatabase_4()
        {
            Book newBook = new Book("Old Man and the Sea", 4);
            newBook.Save();
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            newCheckout.Save(newBook);

            Checkout.Return(newCheckout.GetId());

            Assert.Equal("returned", Checkout.GetAll()[0].GetDueDate());
        }

        [Fact]
        public void Update_UpdateInDatabase_true()
        {
            //Arrange
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();
            Book newBook = new Book("Old Man and the Sea", 5);
            newBook.Save();
            newBook.AddAuthor(newAuthor.GetId());
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            Checkout newCheckout = new Checkout("2017-05-1", newPatron.GetId(), newBook.GetId());
            newCheckout.Save(newBook);
            //Act
            newCheckout.Update("2017-06-12");
            string result = newCheckout.GetDueDate();

            //Assert
            Assert.Equal("2017-06-12", result);
            // Assert.Equal(newName, result.GetName());
        }
        [Fact]
        public void Return_SpecificCheckAndAddCopyToBook()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();
            Book newBook = new Book("Old Man and the Sea", 5);
            newBook.Save();
            newBook.AddAuthor(newAuthor.GetId());
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            Checkout newCheckout = new Checkout("2017-05-1", newPatron.GetId(), newBook.GetId());
            newCheckout.Save(newBook);

            Checkout.Return(newCheckout.GetId());

            int actual = Book.GetAll()[0].GetCopies();

            Assert.Equal(5, actual);
        }



        public void Dispose()
        {
            Author.DeleteAll();
            Book.DeleteAll();
            Checkout.DeleteAll();
            Patron.DeleteAll();

        }
    }
}
