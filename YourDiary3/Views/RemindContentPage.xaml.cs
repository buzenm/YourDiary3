﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
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
    public sealed partial class RemindContentPage : Page
    {
        public static RemindContentPage current;
        private static readonly string DBName = "YourDiary.db3";
        private static readonly string RemindTableName = "CSY_REMIND";
        
        public RemindContentPage()
        {
            this.InitializeComponent();

            current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter.GetType() == typeof(string))
            {
                TitleTextblock.Text = DateTime.Now.ToString();
                //if (FirstLoad)
                //{
                //    MainPage.current.RightFrame.BackStack.Clear();
                //    FirstLoad = false;
                //}
                if (MainPage.current.RightFrame.BackStackDepth == 1)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += RemindContentPage_BackRequested;
                    
                }


            }
            else if(e.Parameter.GetType() == typeof(Remind))
            {
                TitleTextblock.Text = ((Remind)e.Parameter).Date;
                //((Remind)e.Parameter).Content= Regex.Replace(((Remind)e.Parameter).Content, "''", "'");
                ContentTextBox.Text = ((Remind)e.Parameter).FixContent;
                if (MainPage.current.RightFrame.BackStackDepth == 1)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += RemindContentPage_BackRequested;
                    
                }

            }
            else if (e.Parameter.GetType() == typeof(int))
            {
                TitleTextblock.Text = DateTime.Now.ToString();
                MainPage.current.RightFrame.BackStack.Clear();
                Functions.SetCanvasZ("10");
            }
        }

        public static void RemindContentPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainPage.current.RightFrame.CanGoBack)
            {
                e.Handled = true;

                if (current.ContentTextBox.Text != ((Remind)ListViewPage.current.BeiWangLuListView.SelectedItem)?.FixContent&& ListViewPage.current.BeiWangLuListView.SelectedItem!=null)
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
                    try
                    {
                        saveDialog.ShowAsync();
                    }
                    catch
                    {

                    }
                    

                }
                else
                {
                    MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), 1);
                    MainPage.current.RightFrame.BackStack.Clear();
                    ListViewPage.current.BeiWangLuListView.SelectedIndex = -1;
                    ListViewPage.current.DiaryListView.SelectedIndex = -1;
                    Functions.SetCanvasZ("10");
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    SystemNavigationManager.GetForCurrentView().BackRequested -= RemindContentPage_BackRequested;
                }
            }
        }

        private static void SaveDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), 1);
            MainPage.current.RightFrame.BackStack.Clear();
            ListViewPage.current.BeiWangLuListView.SelectedIndex = -1;
            ListViewPage.current.DiaryListView.SelectedIndex = -1;
            Functions.SetCanvasZ("10");
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= RemindContentPage_BackRequested;
        }

        private static void SaveDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            current.SaveToCollection();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
            
        }

        

        private void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentTextBox.Text != "")
            {
                SaveToCollection();
            }
            
            Functions.SetCanvasZ("10");
        }

        private void SaveToCollection()
        {
            foreach (var item in ListViewPage.current.reminds)
            {
                if (TitleTextblock.Text == item.Date)
                {

                    item.Content = ContentTextBox.Text;
                    item.FixContent = item.Content;
                    item.Content = Regex.Replace(item.Content, "'", "''");
                    
                    string sql = "UPDATE " + RemindTableName + " SET CSY_CONTENT='" + item.Content + "' WHERE CSY_DATE='" + item.Date + "'";
                    
                    SqliteDatabase.UpdateData(sql);
                    //MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
                    MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), 1);
                    MainPage.current.RightFrame.BackStack.Clear();
                    ListViewPage.current.BeiWangLuListView.SelectedIndex = -1;
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    return;
                }
            }
            Remind remind = new Remind();
            remind.Date = TitleTextblock.Text;
            remind.Content = ContentTextBox.Text;
            remind.FixContent = remind.Content;
            remind.Content = Regex.Replace(remind.Content, @"'", @"''");
            
            ListViewPage.current.reminds.Add(remind);

            SqliteDatabase.InsertData(remind, DBName, RemindTableName);
            MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), 1);
            MainPage.current.RightFrame.BackStack.Clear();
            ListViewPage.current.BeiWangLuListView.SelectedIndex = -1;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
        
        
    }
}
