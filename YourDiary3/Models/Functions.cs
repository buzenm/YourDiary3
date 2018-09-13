using System;
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
using Microsoft.Toolkit.Services.OneDrive;
using Windows.Storage.Streams;

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

        public async static Task SaveToOnedrive()
        {
            var onedriveAppFolder = await OneDriveService.Instance.AppRootFolderAsync();

            // Creating or uploading files less than 4MB
            // Open the local file or create a local file if brand new
            var selectedFile = await ApplicationData.Current.LocalFolder.GetFileAsync("YourDiary.db3");
            if (selectedFile != null)
            {
                using (var localStream = await selectedFile.OpenReadAsync())
                {
                    var fileCreated = await onedriveAppFolder.StorageFolderPlatformService.CreateFileAsync(
                        selectedFile.Name, CreationCollisionOption.ReplaceExisting, localStream);
                }
            }

        }
        
        public async static Task LoadFromOnedrive()
        {
            var oneDriveAppFolder = await OneDriveService.Instance.AppRootFolderAsync();

            // Downloading files
            // Download a file and save the content in a local file
            // Convert the storage item to a storage file
            var oneDriveFile = await oneDriveAppFolder.GetFileAsync("YourDiary.db3");
            
            using (var remoteStream = (await oneDriveFile.StorageFilePlatformService.OpenAsync()) as IRandomAccessStream)
            {
                // Use a helper method to open local filestream and write to it 
                await SaveToLocalFolder(remoteStream, oneDriveFile.Name);
            }

        }

        public static async Task SaveToLocalFolder(IRandomAccessStream remoteStream,string name)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile localFile = await folder.CreateFileAsync("YourDiary1.db3", CreationCollisionOption.ReplaceExisting);

            using(FileStream fs = File.Open(localFile.Path, FileMode.OpenOrCreate))
            {
                Stream stream = remoteStream.AsStream();
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                using(BinaryWriter bw=new BinaryWriter(fs))
                {
                    bw.Write(buffer);
                }
            }
            
        }

        public async static Task AndDatabaseAsync()
        {
            string conn = "Filename="+ ApplicationData.Current.LocalFolder.Path + "\\YourDiary1.db3";
            using(SqliteConnection db1=new SqliteConnection(conn))
            {
                db1.Open();
                string sql = "";
                //SqliteCommand comm1 = new SqliteCommand(sql, db1);
                //SqliteDataReader dataReader1=await comm1.ExecuteReaderAsync();

                conn = "Filename=" + ApplicationData.Current.LocalFolder.Path + "\\YourDiary.db3";
                using(SqliteConnection db=new SqliteConnection(conn))
                {
                    db.Open();
                    sql = "select * from CSY_DIARY";
                    SqliteCommand comm = new SqliteCommand(sql, db);
                    SqliteDataReader dataReader = await comm.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        sql = "select * from CSY_DIARY where CSY_DATE='" +
                        dataReader.GetString(0) + "'";
                        SqliteCommand comm1 = new SqliteCommand(sql, db1);
                        SqliteDataReader dataReader1 = await comm1.ExecuteReaderAsync();
                        if (!dataReader1.HasRows)
                        {
                            Diary diary = new Diary() { Date = dataReader.GetString(0),
                                Weather = dataReader.GetString(1), Content = dataReader.GetString(2) };
                            
                            SqliteDatabase.InsertData(diary, 
                                db, DiaryTableName);
                            
                            
                        }
                    }
                    //dataReader.Close();
                    sql = "select * from CSY_REMIND";
                    SqliteCommand comm2 = new SqliteCommand(sql, db);
                    SqliteDataReader dataReader2 = await comm.ExecuteReaderAsync();
                    while (dataReader2.Read())
                    {
                        sql = "select * from CSY_REMIND where CSY_DATE='" +
                        dataReader2.GetString(0) + "'";
                        SqliteCommand comm1 = new SqliteCommand(sql, db1);
                        SqliteDataReader dataReader1 = await comm1.ExecuteReaderAsync();
                        if (!dataReader1.HasRows)
                        {
                            Remind remind = new Remind()
                            {
                                Date = dataReader2.GetString(0),
                                Content = dataReader2.GetString(1)
                            };
                            SqliteDatabase.InsertData(remind,
                                db, RemindTableName);
                        }
                    }
                    dataReader2.Close();
                    db.Close();

                }
                db1.Close();
                
            }
        }
    }
}
