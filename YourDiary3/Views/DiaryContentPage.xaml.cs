using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YourDiary3.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace YourDiary3.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiaryContentPage : Page
    {
        private static readonly string DBName = "YourDiary.db3";
        private static readonly string DiaryTableName = "CSY_DIARY";
        
        public static DiaryContentPage current;
        
        public DiaryContentPage()
        {
            this.InitializeComponent();

            current = this;
        }

        private void WeatherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            base.OnNavigatedTo(e);
            if (e.Parameter.GetType() == typeof(int))
            {
                TitleTextBlock.Text = DateTime.Now.ToLongDateString();
            }
            else if (e.Parameter.GetType() == typeof(Diary))
            {
                Diary diary = (Diary)e.Parameter;
                TitleTextBlock.Text = diary.Date;
                ContentTextBox.Text = diary.Content;
                WeatherComboBox.SelectedItem = diary.Weather;
            }
        }

        private void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SavetoCollection();
            Functions.SetCanvasZ("10");
        }

        public void SavetoCollection()
        {
            foreach (var item in ListViewPage.current.diaries)
            {
                if (TitleTextBlock.Text == item.Date)
                {
                    item.Weather = WeatherComboBox.SelectedItem.ToString();
                    item.Content = ContentTextBox.Text;
                    string sql = "UPDATE " + DiaryTableName + " SET CSY_CONTENT='" + item.Content + "',CSY_WEATHER='" +
                        item.Weather + "' WHERE CSY_DATE='" + item.Date + "'";
                    string conn = "Filename=" + ApplicationData.Current.LocalFolder.Path + "\\" + DBName;
                    SqliteDatabase.UpdateData(conn, sql);
                    return;
                }
            }
            Diary diary = new Diary();
            diary.Date = TitleTextBlock.Text;
            diary.Content = ContentTextBox.Text;
            diary.Weather = WeatherComboBox.SelectionBoxItem.ToString();
            ListViewPage.current.diaries.Add(diary);

            SqliteDatabase.InsertData(diary, DBName, DiaryTableName);
            
        }
    }
}
