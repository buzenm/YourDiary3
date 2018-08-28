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
using YourDiary3.Views;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace YourDiary3.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListViewPage : Page
    {
        private static readonly string DBName = "YourDiary.db3";
        private static readonly string DiaryTableName = "CSY_DIARY";
        public static ListViewPage current;
        public ObservableCollection<Remind> reminds = new ObservableCollection<Remind>();
        public ObservableCollection<Diary> diaries = new ObservableCollection<Diary>();
        public ListViewPage()
        {
            this.InitializeComponent();
            current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            diaries = SqliteDatabase.LoadFromDatabase(DBName, DiaryTableName);
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyPivot.SelectedItem == BeiWangLuPivotItem)
            {
                try
                {
                    if (MainPage.current.RightFrame.SourcePageType.Name != "RemindContentPage")
                    {
                        MainPage.current.RightFrame.Navigate(typeof(RemindContentPage),1);
                    }
                    else
                    {
                        if (RemindContentPage.current.ContentTextBox.Text == string.Empty)
                        {
                            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage),1);
                        }
                    }
                }
                catch
                {
                    MainPage.current.RightFrame.Navigate(typeof(RemindContentPage),1);
                }
                //AddItem(DateTime.Now)
                
                
            }
            else
            {
                try
                {
                    if (MainPage.current.RightFrame.SourcePageType.Name != "DiaryContentPage")
                    {
                        MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage),1);
                    }
                    
                }
                catch
                {
                    MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage),1);
                }
            }
                

        }

        public void AddItem(string date,string weather,string content)
        {
            Diary diary = new Diary() { Date = date, Weather = weather, Content = content };
            diaries.Add(diary);
        }
        public void AddItem(string date,string content)
        {
            Remind remind = new Remind() { Date = date, Content = content };
            reminds.Add(remind);
        }

        
    }
}
