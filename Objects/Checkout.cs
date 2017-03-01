using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Library
{
    public class Checkout
    {
        private string _dueDate;
        private int _id;
        private int _bookId;
        private int _patronId;

        public Checkout(string DueDate, int PatronId, int BookId, int Id = 0)
        {
            _dueDate = DueDate;
            _id = Id;
            _bookId = BookId;
            _patronId = PatronId;
        }

        public int GetId()
        {
            return _id;
        }

        public int GetPatronId()
        {
            return _patronId;
        }

        public int GetBookId()
        {
            return _bookId;
        }

        public string GetDueDate()
        {
            return _dueDate;
        }

        public void SetDueDate(string DueDate)
        {
            _dueDate = DueDate;
        }


        public static Checkout Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM checkouts WHERE id = @CheckoutId;", conn);
            cmd.Parameters.Add(new SqlParameter("@CheckoutId", id));

            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            string foundCheckoutDueDate = null;
            int foundPatronId = 0;
            int foundBookId = 0;


            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundCheckoutDueDate = rdr.GetString(1);
                foundPatronId = rdr.GetInt32(2);
                foundBookId = rdr.GetInt32(3);
            }

            DB.CloseSqlConnection(conn, rdr);

            return new Checkout(foundCheckoutDueDate, foundPatronId, foundBookId, foundId);
        }

        public void Save(Book checkoutBook)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO checkouts (due_date, patron_id, book_id) OUTPUT INSERTED.id VALUES (@CheckoutDueDate, @PatronId, @BookId);", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetPatronId()));
            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetBookId()));
            cmd.Parameters.Add(new SqlParameter("@CheckoutDueDate", this.GetDueDate()));


            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            if(rdr != null)
            {
                rdr.Close();
            }

            SqlCommand updateCmd = new SqlCommand("UPDATE books SET copies = @CopiesParameter OUTPUT INSERTED.copies WHERE id = @BookId;", conn);
            updateCmd.Parameters.Add(new SqlParameter("@BookId", this.GetBookId()));
            updateCmd.Parameters.Add(new SqlParameter("@CopiesParameter", (checkoutBook.GetCopies() - 1)));


            SqlDataReader updateRdr = updateCmd.ExecuteReader();

            while(updateRdr.Read())
            {
                checkoutBook.SetCopies(updateRdr.GetInt32(0));
            }


            DB.CloseSqlConnection(conn, updateRdr);
        }

        public static List<Checkout> GetAll()
        {
            List<Checkout> allCheckouts = new List<Checkout> {};
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM checkouts;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                allCheckouts.Add(new Checkout(rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(3), rdr.GetInt32(0)));
            }

            DB.CloseSqlConnection(conn, rdr);
            return allCheckouts;
        }

        public static void Return(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE checkouts SET due_date = 'returned' WHERE id=@CheckoutId;", conn);
            cmd.Parameters.Add(new SqlParameter("@CheckoutId", id));

            SqlCommand copyCmd = new SqlCommand("UPDATE books SET copies = @BookCopies WHERE id = @BookId;", conn);
            copyCmd.Parameters.Add(new SqlParameter("@BookCopies", (Book.Find(Checkout.Find(id).GetBookId()).GetCopies() + 1)));
            copyCmd.Parameters.Add(new SqlParameter("@BookId", Checkout.Find(id).GetBookId()));

            cmd.ExecuteNonQuery();
            copyCmd.ExecuteNonQuery();
            DB.CloseSqlConnection(conn);
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM checkouts;", conn);
            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(conn);
        }

        public override bool Equals(System.Object otherCheckout)
        {
            if(!(otherCheckout is Checkout))
            {
                return false;
            }
            else
            {
                Checkout newCheckout = (Checkout) otherCheckout;
                bool idEquality = this.GetId() == newCheckout.GetId();
                bool dueDateEquality = this.GetDueDate() == newCheckout.GetDueDate();
                bool patronIdEquality = this.GetPatronId() == newCheckout.GetPatronId();
                bool bookIdEquality = this.GetBookId() == newCheckout.GetBookId();
                return (idEquality && dueDateEquality && patronIdEquality && bookIdEquality);
            }
        }

        public void Update(string dueDate)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE checkouts SET due_date = @CheckoutDueDate OUTPUT INSERTED.due_date WHERE id = @CheckoutId;", conn);
            // Next line represents following 4 lines of code, put into one
            // cmd.Parameters.Add(new SqlParameter("@CheckoutId", this.GetId()));

            SqlParameter checkoutIdParameter = new SqlParameter();
            checkoutIdParameter.ParameterName = "@CheckoutId";
            checkoutIdParameter.Value = this.GetId();
            cmd.Parameters.Add(checkoutIdParameter);

            cmd.Parameters.Add(new SqlParameter("@CheckoutDueDate", dueDate));

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                this.SetDueDate(rdr.GetString(0));
            }

            DB.CloseSqlConnection(conn, rdr);
        }
    }
}
