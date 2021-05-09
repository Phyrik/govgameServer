using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace govgameSharedClasses.Helpers.MySQL
{
    public class MySQLHelper
    {
        static string mySQLConnectionString = "server=localhost;user=root;database=govgame;port=3306;password='A`T7fYQ!tP6;N[K$';";
        static MySqlConnection mySQLConnection = new MySqlConnection(mySQLConnectionString);

        public static void SampleMethod()
        {
            mySQLConnection.Open();

            string mySQLQuery = "SELECT * FROM users";
            MySqlCommand mySqlCommand = new MySqlCommand(mySQLQuery, mySQLConnection);
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

            foreach (DbColumn column in mySqlDataReader.GetColumnSchemaAsync().Result)
            {
                Console.WriteLine(column.DataType);
            }
        }
    }
}
