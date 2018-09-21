using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class Connection
    {
        private static IDbConnection _conn;



        private Connection()
        {

        }



        public void ChangeConnection(IDbConnection connection)
        {
            _conn = connection;
        }

        public static IDbConnection GetConnection(string connection)
        {
            if (_conn == null)
            {
                BuildConnection(connection);
            }


            if (string.IsNullOrEmpty(_conn.ConnectionString))
            {
                _conn.ConnectionString = connection;
            }

            return _conn;
        }

    private static void BuildConnection(string connection)
    {

        _conn = new SqlConnection(connection);
    }

}
}
