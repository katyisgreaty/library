using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Library
{
    public class Patron
    {
        private string _name;
        private string _number;
        private int _id;

        public Patron(string Name, string Number, int Id = 0)
        {
            _name = Name;
            _number = Number;
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

        public string GetNumber()
        {
            return _number;
        }


        public static Patron Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patrons WHERE id = @PatronId;", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronId", id));

            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string foundName = null;
            string foundPatronNumber = null;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
                foundPatronNumber = rdr.GetString(2);
            }

            DB.CloseSqlConnection(conn, rdr);

            return new Patron(foundName, foundPatronNumber, foundId);
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO patrons (name, phone) OUTPUT INSERTED.id VALUES (@PatronName, @PatronNumber);", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronName", this.GetName()));
            cmd.Parameters.Add(new SqlParameter("@PatronNumber", this.GetNumber()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(conn, rdr);
        }

        public static List<Patron> GetAll()
        {
            List<Patron> allPatrons = new List<Patron> {};
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patrons;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                allPatrons.Add(new Patron(rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(0)));
            }

            DB.CloseSqlConnection(conn, rdr);
            return allPatrons;
        }

        public static void Delete(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM patrons WHERE id=@PatronId;", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronId", id));

            cmd.ExecuteNonQuery();
            DB.CloseSqlConnection(conn);
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM patrons;", conn);
            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(conn);
        }

        public override bool Equals(System.Object otherPatron)
        {
            if(!(otherPatron is Patron))
            {
                return false;
            }
            else
            {
                Patron newPatron = (Patron) otherPatron;
                bool idEquality = this.GetId() == newPatron.GetId();
                bool nameEquality = this.GetName() == newPatron.GetName();
                bool numEquality = this.GetNumber() == newPatron.GetNumber();
                return (idEquality && nameEquality && numEquality);
            }
        }
    }
}
