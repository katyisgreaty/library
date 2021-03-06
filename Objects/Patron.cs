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

        public string GetPhone()
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
            cmd.Parameters.Add(new SqlParameter("@PatronNumber", this.GetPhone()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(conn, rdr);
        }
        public List<Checkout> GetCheckouts()
        {
            List<Checkout> allCheckouts = new List<Checkout>{};
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT checkouts.* FROM checkouts JOIN patrons ON (checkouts.patron_id = patrons.id) WHERE patrons.id = @PatronId ORDER BY due_date;", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId()));

            SqlDataReader rdr= cmd.ExecuteReader();

            while(rdr.Read())
            {
                int foundId = rdr.GetInt32(0);
                string foundDueDate = rdr.GetString(1);
                int foundPatronId = rdr.GetInt32(2);
                int foundBookId = rdr.GetInt32(3);
                Checkout foundCheckout = new Checkout(foundDueDate, foundPatronId, foundBookId, foundId);
                allCheckouts.Add(foundCheckout);
            }

            DB.CloseSqlConnection(conn, rdr);

            return allCheckouts;
        }

        public static List<Patron> GetAll()
        {
            List<Patron> allPatrons = new List<Patron> {};
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patrons ORDER BY name;", conn);
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
                bool numEquality = this.GetPhone() == newPatron.GetPhone();
                return (idEquality && nameEquality && numEquality);
            }
        }

        public void Update(string newName, string newPhone)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE patrons SET name = @NewName, phone = @NewPhone WHERE id = @PatronId;", conn);
            cmd.Parameters.Add(new SqlParameter("@NewName", newName));
            cmd.Parameters.Add(new SqlParameter("@NewPhone", newPhone));
            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId()));
            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(conn);
        }
    }
}
