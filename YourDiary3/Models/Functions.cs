﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using Windows.UI.Xaml.Controls;

namespace YourDiary3.Models
{
    public class Functions
    {
        private static readonly string DBName = "YourDiary.db3";
        private static readonly string DiaryTableName = "CSY_DIARY";
        private static readonly string RemindTableName = "CSY_REMIND";

        public async static Task LoadFromDatabase()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var files = await folder.GetFilesAsync();
            if (files.Count != 0)
            {
                foreach (var item in files)
                {
                    if (item.Name == DBName)
                    {

                    }
                }
            }

        }

        public async static Task SaveToDatabase()
        {
            //SQLiteConnection conn = new SQLiteConnection();
            //SQLiteConnection.CreateFile("YourDiary.db");
            //conn = new SQLiteConnection("Data Source=YourDiary.db");
            //SQLiteConnection.CreateFile(ApplicationData.Current.LocalFolder.ToString());
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(DBName, CreationCollisionOption.OpenIfExists);
            //SQLiteConnection.CreateFile(folder+"YourDiary.db3");
            SqliteDatabase.InsertTable(DBName, DiaryTableName);
            SqliteDatabase.InsertTable2(DBName, RemindTableName);

        }

        public interface IRightOnLeft
        {
            
        }

        public static void SetCanvasZ(string zIndex)
        {
             
            if(zIndex == "10")
            {
                Canvas.SetZIndex(MainPage.current.LeftFrame, 1);
                Canvas.SetZIndex(MainPage.current.RightFrame, 0);
            }
            else
            {
                Canvas.SetZIndex(MainPage.current.LeftFrame, 0);
                Canvas.SetZIndex(MainPage.current.RightFrame, 1);
            }

        }

    }
}