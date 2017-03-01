using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Library
{
    public class Author
    {
        private string _name;
        private int _id;

        public Author(string Name, int Id = 0)
        {
            _name = Name;
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



        public static Author Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM authors WHERE id = @AuthorId;", conn);
            cmd.Parameters.Add(new SqlParameter("@AuthorId", id));

            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }

            DB.CloseSqlConnection(conn, rdr);

            return new Author(foundName, foundId);
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO authors (name) OUTPUT INSERTED.id VALUES (@AuthorName);", conn);
            cmd.Parameters.Add(new SqlParameter("@AuthorName", this.GetName()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(conn, rdr);
        }

        public static List<Author> GetAll()
        {
            List<Author> allAuthors = new List<Author> {};
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM authors;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                allAuthors.Add(new Author(rdr.GetString(1), rdr.GetInt32(0)));
            }

            DB.CloseSqlConnection(conn, rdr);
            return allAuthors;
        }

        public static void Delete(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM authors WHERE id=@AuthorId;", conn);
            cmd.Parameters.Add(new SqlParameter("@AuthorId", id));

            cmd.ExecuteNonQuery();
            DB.CloseSqlConnection(conn);
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM authors;", conn);
            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(conn);
        }

        public override bool Equals(System.Object otherAuthor)
        {
            if(!(otherAuthor is Author))
            {
                return false;
            }
            else
            {
                Author newAuthor = (Author) otherAuthor;
                bool idEquality = this.GetId() == newAuthor.GetId();
                bool nameEquality = this.GetName() == newAuthor.GetName();
                return (idEquality && nameEquality);
            }
        }
    }
}
