using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class DB
  {
    public static SqlConnection Connection()
    {
      SqlConnection conn = new SqlConnection(DBConfiguration.ConnectionString);
      return conn;
    }

    public static void CloseSqlConnection(SqlConnection conn, SqlDataReader rdr = null)
    {
        if(rdr != null)
        {
            rdr.Close();
        }
        if(conn != null)
        {
            conn.Close();
        }
    }
  }
}
