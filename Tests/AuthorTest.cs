using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace Library
{
    public class AuthorTest : IDisposable
    {
        public AuthorTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Author_EmptyOnLoad_0()
        {
            Assert.Equal(0, Author.GetAll().Count);
        }
        [Fact]
        public void Author_EqualityTest_1()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            Author secondAuthor = new Author("Ernest Hemingway");

            Assert.Equal(newAuthor, secondAuthor);
        }

        [Fact]
        public void Save_SavesAuthorToDatabase_2()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();

            List<Author> expectedList = new List<Author> {newAuthor};
            List<Author> actualList = Author.GetAll();

            Assert.Equal(expectedList, actualList);
        }

        [Fact]
        public void Author_Find_3()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();

            Author foundAuthor = Author.Find(newAuthor.GetId());

            Assert.Equal(newAuthor, foundAuthor);
        }

        [Fact]
        public void Author_Delete_RemoveFromDatabase_4()
        {
            Author newAuthor = new Author("Ernest Hemingway");
            newAuthor.Save();

            Author.Delete(newAuthor.GetId());

            Assert.Equal(0, Author.GetAll().Count);
        }

        public void Dispose()
        {
            Author.DeleteAll();

        }
    }
}
