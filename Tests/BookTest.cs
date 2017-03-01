using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace Library
{
    public class BookTest : IDisposable
    {
        public BookTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Book_EmptyOnLoad_0()
        {
            Assert.Equal(0, Book.GetAll().Count);
        }
        [Fact]
        public void Book_EqualityTest_1()
        {
            Book newBook = new Book("Old Man and the Sea", 4);
            Book secondBook = new Book("Old Man and the Sea", 4);

            Assert.Equal(newBook, secondBook);
        }
        [Fact]
        public void Save_SavesBookToDatabase_2()
        {
            Book newBook = new Book("Old Man and the Sea", 4);
            newBook.Save();

            List<Book> expectedList = new List<Book> {newBook};
            List<Book> actualList = Book.GetAll();

            Assert.Equal(expectedList, actualList);
        }

        [Fact]
        public void Book_Find_3()
        {
            Book newBook = new Book("Old Man and the Sea", 4);
            newBook.Save();

            Book foundBook = Book.Find(newBook.GetId());

            Assert.Equal(newBook, foundBook);
        }

        [Fact]
        public void Book_Delete_RemoveFromDatabase_4()
        {
            Book newBook = new Book("Old Man and the Sea", 4);
            newBook.Save();

            Book.Delete(newBook.GetId());

            Assert.Equal(0, Book.GetAll().Count);
        }

        [Fact]
        public void Book_AddAuthorToBook()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();
            Book newBook = new Book("Old Man and the Sea", 4);
            newBook.Save();
            List<Author> expected = new List<Author>{newAuthor};

            newBook.AddAuthor(newAuthor.GetId());

            Assert.Equal(expected, newBook.GetAuthors());
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
