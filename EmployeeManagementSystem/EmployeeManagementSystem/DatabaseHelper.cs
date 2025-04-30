using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem
{
    public class DatabaseHelper
    {
        private static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=EmployeeDB;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
