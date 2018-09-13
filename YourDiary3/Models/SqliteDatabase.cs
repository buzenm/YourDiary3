using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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

        public static void InsertTable2(string DBName, string TableName)
        {
            using (SqliteConnection conn = new SqliteConnection("Filename=" + DBName))
            {
                //DateTime.Now.ToShortDateString();
                conn.Open();

                string tableCommand = "CREATE TABLE " + "IF NOT EXISTS " + TableName +
                    "(CSY_ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "CSY_DATE TEXT," +
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
                    "(CSY_DATE,CSY_WEATHER,CSY_CONTENT) VALUES ('" + item.Date + "','" + item.Weather + "','" +
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
                "(CSY_DATE,CSY_WEATHER,CSY_CONTENT) VALUES ('" + diary.Date + "','" + diary.Weather + "','" +
                diary.Content + "')";
                insertCommand.ExecuteReader();
                db.Close();
            }
        }

        public static void InsertData(Diary diary, SqliteConnection db, string TableName)
        {
            SqliteCommand insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            //diary.Date = DateTime.ParseExact(diary.Date.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            insertCommand.CommandText = "INSERT INTO " + TableName +
                "(CSY_DATE,CSY_WEATHER,CSY_CONTENT) VALUES ('" + diary.Date + "','" + diary.Weather + "','" +
                diary.Content + "')";
            insertCommand.ExecuteReader();
        }

        public static void InsertData(Remind remind, string DBName, string TableName)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=" + DBName))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;
                //diary.Date = DateTime.ParseExact(diary.Date.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                insertCommand.CommandText = "INSERT INTO " + TableName +
                "(CSY_DATE,CSY_CONTENT) VALUES ('" + remind.Date + "','" +
                remind.Content + "')";
                insertCommand.ExecuteReader();
                db.Close();
            }
        }

        public static void InsertData(Remind remind, SqliteConnection db, string TableName)
        {
            SqliteCommand insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            //diary.Date = DateTime.ParseExact(diary.Date.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            insertCommand.CommandText = "INSERT INTO " + TableName +
                "(CSY_DATE,CSY_CONTENT) VALUES ('" + remind.Date + "','" +
                remind.Content + "')";
            insertCommand.ExecuteReader();
        }

        public static ObservableCollection<Diary> LoadFromDatabase(string DBName,string TableName)
        {
            ObservableCollection<Diary> diaries = new ObservableCollection<Diary>();
            string path = ApplicationData.Current.LocalFolder.Path + "\\" + DBName;
            if (File.Exists(path))
            {
                using (SqliteConnection db = new SqliteConnection("Filename=" + DBName))
                {
                    db.Open();
                    SqliteCommand selectCommand = new SqliteCommand("SELECT CSY_DATE,CSY_WEATHER,CSY_CONTENT FROM " + TableName, db);


                    SqliteDataReader query = selectCommand.ExecuteReader();
                    while (query.Read())
                    {
                        Diary diary = new Diary();
                        diary.Date = query.GetString(0);
                        diary.Weather = query.GetString(1);
                        diary.Content = query.GetString(2);
                        diaries.Add(diary);
                    }
                    db.Close();
                }
            }
            return diaries;
            
        }

        public static ObservableCollection<Remind> LoadFromDatabase2(string DBName, string TableName)
        {
            ObservableCollection<Remind> reminds = new ObservableCollection<Remind>();
            string path = ApplicationData.Current.LocalFolder.Path + "\\" + DBName;
            if (File.Exists(path))
            {
                using (SqliteConnection db = new SqliteConnection("Filename=" + DBName))
                {
                    db.Open();
                    SqliteCommand selectCommand = new SqliteCommand("SELECT CSY_DATE,CSY_CONTENT FROM " + TableName, db);


                    SqliteDataReader query = selectCommand.ExecuteReader();
                    while (query.Read())
                    {
                        Remind remind = new Remind();
                        remind.Date = query.GetString(0);
                        remind.Content = query.GetString(1);
                        reminds.Add(remind);
                    }
                    db.Close();
                }
            }
            return reminds;

        }

        public static void UpdateData(string DBName,string sql)
        {
            using(SqliteConnection db=new SqliteConnection(DBName))
            {
                db.Open();
                SqliteCommand comm = new SqliteCommand(sql,db);
                comm.ExecuteReader();
                db.Close();
            }
        }

        
    }
}
