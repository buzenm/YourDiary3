using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
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
        private bool Firstload = true;
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
                SaveAppBarButton.IsEnabled = true;
                TitleTextBlock.Text = DateTime.Now.ToLongDateString();
                //if (FirstLoad)
                //{
                //    MainPage.current.RightFrame.BackStack.Clear();
                //    FirstLoad = false;
                //}
                if ((MainPage.current.RightFrame.BackStackDepth==2&&Firstload)||MainPage.current.RightFrame.BackStackDepth==1)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += DiaryContentPage_BackRequested;
                    Firstload = false;
                }



            }
            else if (e.Parameter.GetType() == typeof(Diary))
            {
                SaveAppBarButton.IsEnabled = true;
                Diary diary = (Diary)e.Parameter;
                TitleTextBlock.Text = diary.Date;
                ContentTextBox.Text = diary.Content;
                WeatherComboBox.SelectedItem = diary.Weather;
                if ((MainPage.current.RightFrame.BackStackDepth == 2 && Firstload)|| MainPage.current.RightFrame.BackStackDepth == 1)
                {
                    
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += DiaryContentPage_BackRequested;
                    Firstload = false;
                }



            }
            else if (e.Parameter.GetType() == typeof(string))
            {
                TitleTextBlock.Text = DateTime.Now.ToLongDateString();
                SaveAppBarButton.IsEnabled = false;
            }

            
            
        }

        public static void DiaryContentPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainPage.current.RightFrame.CanGoBack)
            {
                //MainPage.current.RightFrame.GoBack();
                MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
                MainPage.current.RightFrame.BackStack.Clear();
                ListViewPage.current.DiaryListView.SelectedIndex = -1;
                Functions.SetCanvasZ("10");
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= DiaryContentPage_BackRequested;

            }
            

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
            
        }

        private async void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentTextBox.Text != "")
            {
                
                if(!(WeatherComboBox.SelectedItem is string))
                {
                    MessageDialog dialog = new MessageDialog("请选择天气");
                    await dialog.ShowAsync();
                    return;
                }
                else
                {
                    SavetoCollection();
                }
            }
            
            //if(TitleTextBlock.Text==((Diary)(ListViewPage.current.DiaryListView.SelectedItem)).Date)

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
                    MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
                    MainPage.current.RightFrame.BackStack.Clear();
                    ListViewPage.current.DiaryListView.SelectedIndex = -1;
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    return;
                }
            }
            Diary diary = new Diary();
            diary.Date = TitleTextBlock.Text;
            diary.Content = ContentTextBox.Text;
            diary.Weather = WeatherComboBox.SelectionBoxItem.ToString();
            ListViewPage.current.diaries.Add(diary);

            SqliteDatabase.InsertData(diary, DBName, DiaryTableName);
            MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
            MainPage.current.RightFrame.BackStack.Clear();
            ListViewPage.current.DiaryListView.SelectedIndex = -1;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

        }
    }
}
