using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourDiary3.Models
{
    public class SqliteDatabase
    {
        public static void InsertTable(string DBName,string TableName)
        {
            using(SqliteConnection conn=new SqliteConnection("Filename=" + DBName))
            {
                DateTime.Now.ToShortDateString();
                conn.Open();
                string tableCommand = "CREATE TABLE " + "IF NOT EXISTS " + TableName +
                    "(CSY_ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "CSY_DATE TEXT," +
                    "CSY_WEATHER TEXT," +
                    "CSY_CONTENT TEXT)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, conn);
                createTable.ExecuteReader();
                conn.Close();
            }
        }
    }
}
