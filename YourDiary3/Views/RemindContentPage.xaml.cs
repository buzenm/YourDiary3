﻿using System;
using System.Collections.Generic;
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
            }
            else if(e.Parameter.GetType() == typeof(Remind))
            {
                TitleTextblock.Text = ((Remind)e.Parameter).Date;
                ContentTextBox.Text = ((Remind)e.Parameter).Content;
            }
        }

        private void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToCollection();
            Functions.SetCanvasZ("10");
        }

        private void SaveToCollection()
        {
            foreach (var item in ListViewPage.current.reminds)
            {
                if (TitleTextblock.Text == item.Date)
                {

                    item.Content = ContentTextBox.Text;
                    string sql = "UPDATE " + RemindTableName + " SET CSY_CONTENT='" + item.Content + "' WHERE CSY_DATE='" + item.Date + "'";
                    string conn = "Filename=" + ApplicationData.Current.LocalFolder.Path + "\\" + DBName;
                    SqliteDatabase.UpdateData(conn, sql);
                    //MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1");
                    return;
                }
            }
            Remind remind = new Remind();
            remind.Date = TitleTextblock.Text;
            remind.Content = ContentTextBox.Text;
            ListViewPage.current.reminds.Add(remind);

            SqliteDatabase.InsertData(remind, DBName, RemindTableName);
        }
    }
}