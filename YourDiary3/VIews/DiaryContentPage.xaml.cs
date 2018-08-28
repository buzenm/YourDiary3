using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private static readonly string RemindTableName = "CSY_REMIND";
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
                TitleTextBlock.Text = DateTime.Now.ToShortDateString();
            }
        }

        private void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SavetoCollection();
        }

        public void SavetoCollection()
        {
            Diary diary = new Diary();
            diary.Date = TitleTextBlock.Text;
            diary.Content = ContentTextBox.Text;
            diary.Weather = WeatherComboBox.SelectionBoxItem.ToString();
            ListViewPage.current.diaries.Add(diary);

            SqliteDatabase.InsertData(diary, DBName, DiaryTableName);
            
        }
    }
}
