using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Library
{
    public class Book
    {
        private string _name;
        private int _copies;
        private int _id;

        public Book(string Name, int Copies, int Id = 0)
        {
            _name = Name;
            _copies = Copies;
            _id = Id;
        }

        public int GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public int GetCopies()
        {
            return _copies;
        }


        public static Book Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);
            cmd.Parameters.Add(new SqlParameter("@BookId", id));

            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string foundName = null;
            int foundBookCopies = 0;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
                foundBookCopies = rdr.GetInt32(2);
            }

            DB.CloseSqlConnection(conn, rdr);

            return new Book(foundName, foundBookCopies, foundId);
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books (title, copies) OUTPUT INSERTED.id VALUES (@BookName, @BookCopies);", conn);
            cmd.Parameters.Add(new SqlParameter("@BookName", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@BookCopies", this.GetCopies()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(conn, rdr);
        }

        public void AddAuthor(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books_authors(book_id, author_id) VALUES (@BookId, @AuthorId);", conn);
            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetId()));
            cmd.Parameters.Add(new SqlParameter("@AuthorId", id));

            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(conn);
        }

        public static List<Book> GetAll()
        {
            List<Book> allBooks = new List<Book> {};
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM books;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                allBooks.Add(new Book(rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(0)));
            }

            DB.CloseSqlConnection(conn, rdr);
            return allBooks;
        }

        public static void Delete(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id=@BookId;", conn);
            cmd.Parameters.Add(new SqlParameter("@BookId", id));

            cmd.ExecuteNonQuery();
            DB.CloseSqlConnection(conn);
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM books;", conn);
            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(conn);
        }

        public override bool Equals(System.Object otherBook)
        {
            if(!(otherBook is Book))
            {
                return false;
            }
            else
            {
                Book newBook = (Book) otherBook;
                bool idEquality = this.GetId() == newBook.GetId();
                bool nameEquality = this.GetName() == newBook.GetName();
                bool copiesEquality = this.GetCopies() == newBook.GetCopies();
                return (idEquality && nameEquality && copiesEquality);
            }
        }
    }
}
