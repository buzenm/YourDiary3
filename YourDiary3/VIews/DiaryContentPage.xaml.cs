using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
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
                if (MainPage.current.RightFrame.BackStackDepth==1)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += DiaryContentPage_BackRequested;
                    
                }



            }
            else if (e.Parameter.GetType() == typeof(Diary))
            {
                SaveAppBarButton.IsEnabled = true;
                Diary diary = (Diary)e.Parameter;
                TitleTextBlock.Text = diary.Date;
                //diary.Content= Regex.Replace(diary.Content, "''", "'");
                ContentTextBox.Text = Regex.Replace(diary.Content, "''", "'");
                WeatherComboBox.SelectedItem = diary.Weather;
                if (MainPage.current.RightFrame.BackStackDepth == 1)
                {
                    
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += DiaryContentPage_BackRequested;
                    
                }



            }
            else if (e.Parameter.GetType() == typeof(string))
            {

                TitleTextBlock.Text = DateTime.Now.ToLongDateString();
                SaveAppBarButton.IsEnabled = false;
                ContentTextBox.IsEnabled = false;
                //ContentTextBox.Background = new AcrylicBrush();
                MainPage.current.RightFrame.BackStack.Clear();
                Functions.SetCanvasZ("10");
            }

            
            
        }

        public static void DiaryContentPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainPage.current.RightFrame.CanGoBack)
            {
                e.Handled = true;

                if (current.ContentTextBox.Text != ((Diary)ListViewPage.current.DiaryListView.SelectedItem).Content)
                {
                    ContentDialog saveDialog = new ContentDialog()
                    {
                        Title = "YourDiary",
                        Content = "是否保存已编辑的内容",
                        IsPrimaryButtonEnabled = true,
                        IsSecondaryButtonEnabled = true,
                        PrimaryButtonText = "是",
                        SecondaryButtonText = "否"
                    };
                    saveDialog.PrimaryButtonClick += SaveDialog_PrimaryButtonClick;
                    saveDialog.SecondaryButtonClick += SaveDialog_SecondaryButtonClick;
                    saveDialog.ShowAsync();
                    
                }
                else
                {
                    //MainPage.current.RightFrame.GoBack();
                    MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
                    MainPage.current.RightFrame.BackStack.Clear();
                    ListViewPage.current.DiaryListView.SelectedIndex = -1;
                    ListViewPage.current.BeiWangLuListView.SelectedIndex = -1;
                    Functions.SetCanvasZ("10");
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    SystemNavigationManager.GetForCurrentView().BackRequested -= DiaryContentPage_BackRequested;
                }
            }
            

        }

        private static void SaveDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
            MainPage.current.RightFrame.BackStack.Clear();
            ListViewPage.current.DiaryListView.SelectedIndex = -1;
            ListViewPage.current.BeiWangLuListView.SelectedIndex = -1;
            Functions.SetCanvasZ("10");
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= DiaryContentPage_BackRequested;
        }

        private static void SaveDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            current.SavetoCollection();
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
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "YourDiary",
                        Content = "请选择天气",
                        PrimaryButtonText = "确定"
                    };
                    //dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
                    //MessageDialog dialog = new MessageDialog("请选择天气");
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

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
            throw new NotImplementedException();
        }

        public void SavetoCollection()
        {
            foreach (var item in ListViewPage.current.diaries)
            {
                if (TitleTextBlock.Text == item.Date)
                {
                    item.Weather = WeatherComboBox.SelectedItem.ToString();
                    item.Content = ContentTextBox.Text;
                    item.Content = Regex.Replace(item.Content, "'", "''");
                    string sql = "UPDATE " + DiaryTableName + " SET CSY_CONTENT='" + item.Content + "',CSY_WEATHER='" +
                        item.Weather + "' WHERE CSY_DATE='" + item.Date + "'";
                    
                    SqliteDatabase.UpdateData(sql);
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
            diary.Content = Regex.Replace(diary.Content, "'", "''");
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
