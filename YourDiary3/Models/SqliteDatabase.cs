using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                //DateTime.Now.ToShortDateString();
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

        public static void InsertDataCollection(ObservableCollection<Diary> diaries,string DBName,string TableName)
        {
            using (SqliteConnection db =new SqliteConnection("Filename=" + DBName))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;
                foreach (var item in diaries)
                {
                    insertCommand.CommandText = "INSERT INTO " + TableName +
                    " VALUES (NULL," + item.Date + ",'" + item.Weather + "','" +
                    item.Content + "')";
                    insertCommand.ExecuteReader();
                }
                
                db.Close();
            }
        }

        public static void InsertData(Diary diary, string DBName, string TableName)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=" + DBName))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;
                //diary.Date = DateTime.ParseExact(diary.Date.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                insertCommand.CommandText = "INSERT INTO " + TableName +
                    " VALUES (NULL," + diary.Date.ToString("yyyyMMdd") + ",'" + diary.Weather + "','" +
                    diary.Content + "')";
                insertCommand.ExecuteReader();
                db.Close();
            }
        }

        public static ObservableCollection<Diary> LoadFromDatabase(string DBName,string TableName)
        {
            ObservableCollection<Diary> diaries = new ObservableCollection<Diary>();
            using (SqliteConnection db=new SqliteConnection("Filename=" + DBName))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand();
                selectCommand.Connection = db;
                selectCommand.CommandText = "SELECT CSY_DATE,CSY_WEATHER,CSY_CONTENT FROM " + TableName;
                SqliteDataReader query = selectCommand.ExecuteReader();
                Diary diary = new Diary();
                while (query.Read())
                {
                    diary.Date = query.GetDateTime(1);
                    diary.Weather = query.GetString(2);
                    diary.Content = query.GetString(3);
                    diaries.Add(diary);
                }
                return diaries;
            }
        }
    }
}
