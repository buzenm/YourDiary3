﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
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
        private static readonly string RemindTableName = "CSY_REMIND";
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
            reminds = SqliteDatabase.LoadFromDatabase2(DBName, RemindTableName);
        }

        private async void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {

            Functions.SetCanvasZ("01");
            #region 脑残式新建功能实现，不完整功能
            //if (MyPivot.SelectedItem == BeiWangLuPivotItem)
            //{
            //    try
            //    {
            //        if (MainPage.current.RightFrame.SourcePageType.Name != "RemindContentPage")
            //        {
            //            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage),"1");
            //        }
            //        else
            //        {
            //            if (RemindContentPage.current.ContentTextBox.Text == string.Empty)
            //            {
            //                MainPage.current.RightFrame.Navigate(typeof(RemindContentPage),"1");
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        MainPage.current.RightFrame.Navigate(typeof(RemindContentPage),"1");
            //    }
            //    //AddItem(DateTime.Now)


            //}
            //else
            //{
            //    try
            //    {
            //        if (MainPage.current.RightFrame.SourcePageType.Name != "DiaryContentPage")
            //        {
            //            MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage),1);
            //        }

            //    }
            //    catch
            //    {
            //        MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage),1);
            //    }
            //}
            #endregion

            try
            {
                if (MyPivot.SelectedItem == BeiWangLuPivotItem)
                {
                    if (RemindContentPage.current.ContentTextBox.Text != ((Remind)BeiWangLuListView.SelectedItem).Content)
                    {
                        //MessageDialog md = new MessageDialog("保存编辑");

                        //await md.ShowAsync();
                        
                        ContentDialog cd = new ContentDialog();
                        cd.Title = "YourDiary";
                        cd.Content = "是否保存编辑内容";
                        cd.PrimaryButtonText = "是";
                        cd.SecondaryButtonText = "否";
                        cd.PrimaryButtonClick += Cd_PrimaryButtonClick;
                        cd.SecondaryButtonClick += Cd_SecondaryButtonClick;
                        await cd.ShowAsync();


                    }
                    else
                    {
                        MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
                    }
                    
                }
                else
                {
                    foreach (var item in diaries)
                    {
                        if (item.Date == DateTime.Now.ToLongDateString())
                        {
                            MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), item);
                            return;
                        }

                    }
                    MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), 1);
                }


            }
            catch { }
            
        }

        private void Cd_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
        }

        private void Cd_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
            foreach (var item in reminds)
            {
                if (RemindContentPage.current.TitleTextblock.Text == item.Date)
                {
                    item.Content = RemindContentPage.current.ContentTextBox.Text;
                    string sql = "UPDATE " + RemindTableName + " SET CSY_CONTENT='" + item.Content + "' WHERE CSY_DATE='" + item.Date + "'";
                    string conn = "Filename=" + ApplicationData.Current.LocalFolder.Path + "\\" + DBName;
                    SqliteDatabase.UpdateData(conn, sql);
                    MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
                    return;

                }
            }

            Remind remind = new Remind();
            remind.Date = RemindContentPage.current.TitleTextblock.Text;
            remind.Content = RemindContentPage.current.ContentTextBox.Text;
            reminds.Add(remind);

            SqliteDatabase.InsertData(remind, DBName, RemindTableName);
            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
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

        private void BeiWangLuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Functions.SetCanvasZ("01");
            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), e.ClickedItem);
            
        }

        private void DiaryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Functions.SetCanvasZ("01");
            MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), e.ClickedItem);
        }

        private void MyPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyPivot.SelectedItem == DiaryPivotItem)
            {
                MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), 1);

            }
            else
            {
                MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
            }
        }
    }
}