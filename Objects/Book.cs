using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Library
{
    public class Book
    {
        private string _name;
        private string _copies;
        private int _id;

        public Book(string Name, string Copies, int Id = 0)
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

        public string GetCopies()
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
            string foundBookCopies = null;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
                foundBookCopies = rdr.GetString(2);
            }

            DB.CloseSqlConnection(conn, rdr);

            return new Book(foundName, foundBookCopies, foundId);
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books (name, number) OUTPUT INSERTED.id VALUES (@BookName, @BookCopies);", conn);
            cmd.Parameters.Add(new SqlParameter("@BookName", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@BookCopies", this.GetCopies()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(conn, rdr);
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
                allBooks.Add(new Book(rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(0)));
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
