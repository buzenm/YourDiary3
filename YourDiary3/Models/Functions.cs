using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;

namespace YourDiary3.Models
{
    public class Functions
    {
        

        public async static void SaveToDatabase()
        {
            //SQLiteConnection conn = new SQLiteConnection();
            //SQLiteConnection.CreateFile("YourDiary.db");
            //conn = new SQLiteConnection("Data Source=YourDiary.db");
            //SQLiteConnection.CreateFile(ApplicationData.Current.LocalFolder.ToString());
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("YourDiary.db", CreationCollisionOption.OpenIfExists);
            using(SQLiteConnection conn=new SQLiteConnection(file.Path))
            {

            }

        }

    }
}
