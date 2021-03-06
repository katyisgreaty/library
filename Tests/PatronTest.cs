using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace Library
{
    public class PatronTest : IDisposable
    {
        public PatronTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Patron_EmptyOnLoad_0()
        {
            Assert.Equal(0, Patron.GetAll().Count);
        }
        [Fact]
        public void Patron_EqualityTest_1()
        {
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            Patron secondPatron = new Patron("Johnny English", "555-555-5555");

            Assert.Equal(newPatron, secondPatron);
        }
        [Fact]
        public void Save_SavesPatronToDatabase_2()
        {
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            List<Patron> expectedList = new List<Patron> {newPatron};
            List<Patron> actualList = Patron.GetAll();

            Assert.Equal(expectedList, actualList);
        }

        [Fact]
        public void Patron_Find_3()
        {
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            Patron foundPatron = Patron.Find(newPatron.GetId());

            Assert.Equal(newPatron, foundPatron);
        }

        [Fact]
        public void Patron_Delete_RemoveFromDatabase_4()
        {
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            Patron.Delete(newPatron.GetId());

            Assert.Equal(0, Patron.GetAll().Count);
        }

        [Fact]
        public void Update_UpdateNameAndPhone_UpdatedInfo()
        {
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();
            string updatedName = "Billy English";
            string updatedPhone = "555-444-5555";
            newPatron.Update(updatedName, updatedPhone);

            Assert.Equal(updatedName, Patron.GetAll()[0].GetName());
            Assert.Equal(updatedPhone, Patron.GetAll()[0].GetPhone());
        }

        [Fact]
        public void Patron_ReturnCheckoutItems()
        {
            Patron newPatron = new Patron("Johnny English", "555-555-5555");
            newPatron.Save();

            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();
            Book newBook = new Book("Old Man and the Sea", 5);
            newBook.Save();
            Book otherBook = new Book("Farewell to Arms", 7);
            otherBook.Save();

            Checkout newCheckout = new Checkout("2017/03/30", newPatron.GetId(), newBook.GetId());
            newCheckout.Save(newBook);
            Checkout otherCheckout = new Checkout("2017/01/30", newPatron.GetId(), otherBook.GetId());
            otherCheckout.Save(newBook);

            List<Checkout> actual = newPatron.GetCheckouts();
            List<Checkout> expected = new List<Checkout>{otherCheckout, newCheckout};

            Assert.Equal(expected, actual);
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
