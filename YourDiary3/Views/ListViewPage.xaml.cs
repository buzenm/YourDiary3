using Microsoft.Toolkit.Services.OneDrive;
using Microsoft.Toolkit.Services.Services.MicrosoftGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using YourDiary3.Models;
using YourDiary3.ViewModels;
using YourDiary3.Views;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace YourDiary3.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListViewPage : Page, INotifyPropertyChanged
    {
        public Remind deleteRemind = new Remind();
        public Diary deleteDiary = new Diary();
        //public string loginContent = "";

        private string loginContent;
        public string LoginContent
        {
            get
            {
                return loginContent;
            }
            set
            {
                if (loginContent != value)
                {
                    loginContent = value;
                    OnPropertyChanged("LoginContent");
                }
            }
        }
        private static readonly string DBName = "YourDiary.db3";
        private static readonly string DiaryTableName = "CSY_DIARY";
        private static readonly string RemindTableName = "CSY_REMIND";
        public static ListViewPage current;
        public ObservableCollection<Remind> reminds = new ObservableCollection<Remind>();
        public ObservableCollection<Diary> diaries = new ObservableCollection<Diary>();

        public static GroupingViewModel groupingViewModel;
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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
            //Canvas.SetZIndex(FlyoutFrame, -1);
            groupingViewModel = new GroupingViewModel(diaries);
            CSV.Source = groupingViewModel.Groups;
            string AppClientID = "cb8d4295-9fd0-4604-b220-ccfbc7aad516";
            string[] scopes = { MicrosoftGraphScope.FilesReadWriteAppFolder };
            OneDriveService.Instance
                .Initialize(
                AppClientID,
                scopes, null, null
                );
            
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

            if (MyPivot.SelectedItem == BeiWangLuPivotItem)
            {
                try
                {
                    if (RemindContentPage.current.ContentTextBox.Text != ((Remind)BeiWangLuListView.SelectedItem).Content)
                    {
                        //MessageDialog md = new MessageDialog("保存编辑");

                        //await md.ShowAsync();

                        ContentDialog cd = new ContentDialog
                        {
                            Title = "YourDiary",
                            Content = "是否保存编辑内容",
                            PrimaryButtonText = "是",
                            SecondaryButtonText = "否"
                        };
                        cd.PrimaryButtonClick += Cd_PrimaryButtonClick;
                        cd.SecondaryButtonClick += Cd_SecondaryButtonClick;
                        await cd.ShowAsync();


                    }
                    else
                    {
                        MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1", new DrillInNavigationTransitionInfo());

                    }
                    BeiWangLuListView.SelectedIndex = -1;
                }
                catch
                {
                    MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), "1", new DrillInNavigationTransitionInfo());
                }
                

            }
            else if (MyPivot.SelectedItem == DiaryPivotItem)
            {
                foreach (var item in diaries)
                {
                    if (item.Date == DateTime.Now.ToLongDateString())
                    {
                        MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), item);
                        DiaryListView.SelectedIndex = -1;
                        return;
                    }

                }
                MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), 1);
                DiaryListView.SelectedIndex = -1;
            }

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
                    
                    SqliteDatabase.UpdateData(sql);
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

        //private bool FirstLoad = true;
        private async void MyPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (FirstLoad)
            //{
            //    FirstLoad = false;
            //    return;
            //}
            
            if (MyPivot.SelectedItem == DiaryPivotItem)
            {
                if (SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility == AppViewBackButtonVisibility.Visible)
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested -= RemindContentPage.RemindContentPage_BackRequested;
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    
                }
                
                
                try
                {
                    if (RemindContentPage.current.ContentTextBox.Text != ((Remind)BeiWangLuListView.SelectedItem).Content)
                    {
                        //MessageDialog md = new MessageDialog("保存编辑");

                        //await md.ShowAsync();

                        ContentDialog cd = new ContentDialog
                        {
                            Title = "YourDiary",
                            Content = "是否保存编辑内容",
                            PrimaryButtonText = "是",
                            SecondaryButtonText = "否"
                        };
                        cd.PrimaryButtonClick += Cd_PrimaryButtonClick;
                        cd.SecondaryButtonClick += Cd_SecondaryButtonClick;
                        await cd.ShowAsync();


                    }
                    
                    
                    
                }
                catch { }
                BeiWangLuListView.SelectedIndex = -1;
                MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
                
            }
            else
             {
                if (SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility == AppViewBackButtonVisibility.Visible)
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested -= DiaryContentPage.DiaryContentPage_BackRequested;
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    
                }
                
                
                try
                {
                    if (DiaryContentPage.current.ContentTextBox.Text != ((Diary)DiaryListView.SelectedItem).Content)
                    {
                        ContentDialog cd1 = new ContentDialog
                        {
                            Title = "YourDiary",
                            Content = "是否保存编辑内容",
                            PrimaryButtonText = "是",
                            SecondaryButtonText = "否"
                        };
                        cd1.PrimaryButtonClick += Cd1_PrimaryButtonClick;
                        cd1.SecondaryButtonClick += Cd1_SecondaryButtonClick;
                        await cd1.ShowAsync();
                    }
                    
                    
                }
                catch { }
                DiaryListView.SelectedIndex = -1;
                MainPage.current.RightFrame.Navigate(typeof(RemindContentPage), 1);
                

            }
        }

        private void Cd1_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.current.RightFrame.Navigate(typeof(DiaryContentPage), "1");
        }

        private void Cd1_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            foreach (var item in ListViewPage.current.diaries)
            {
                if (DiaryContentPage.current.TitleTextBlock.Text == item.Date)
                {
                    item.Weather = DiaryContentPage.current.WeatherComboBox.SelectedItem.ToString();
                    item.Content = DiaryContentPage.current.ContentTextBox.Text;
                    string sql = "UPDATE " + DiaryTableName + " SET CSY_CONTENT='" + item.Content + "',CSY_WEATHER='" +
                        item.Weather + "' WHERE CSY_DATE='" + item.Date + "'";
                    
                    SqliteDatabase.UpdateData(sql);
                    return;
                }
            }
            Diary diary = new Diary();
            diary.Date = DiaryContentPage.current.TitleTextBlock.Text;
            diary.Content = DiaryContentPage.current.ContentTextBox.Text;
            diary.Weather = DiaryContentPage.current.WeatherComboBox.SelectionBoxItem.ToString();
            diaries.Add(diary);

            SqliteDatabase.InsertData(diary, DBName, DiaryTableName);
        }

        //private void LoginButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void BeiWangLuListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            deleteRemind = (e.OriginalSource as FrameworkElement)?.DataContext as Remind;

            RemindMenuFlyout.ShowAt(BeiWangLuListView, e.GetPosition(BeiWangLuListView));
        }

        private void DiaryListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            deleteDiary = (e.OriginalSource as FrameworkElement)?.DataContext as Diary;
            //DiaryRightTapPop.ShowAt(e.OriginalSource as FrameworkElement);
            DiaryMenuFlyout.ShowAt(DiaryListView, e.GetPosition(DiaryListView));
        }

        

        private void RemindMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in reminds)
            {
                if (deleteRemind.Date == item.Date)
                {
                    reminds.Remove(item);
                    
                    string sql = "delete from " + RemindTableName + " where CSY_DATE='" + item.Date + "'";
                    SqliteDatabase.UpdateData(sql);
                    break;
                }
            }
        }

        private void DiaryMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in diaries)
            {
                if (deleteDiary.Date == item.Date)
                {
                    diaries.Remove(item);
                    
                    string sql = "delete from " + DiaryTableName + " where CSY_DATE='" + item.Date+"'";
                    SqliteDatabase.UpdateData(sql);
                    break;
                }
            }
        }

        

        private async void LoginAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //await OneDriveService.Instance.LogoutAsync();
            //ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] = true;
            //ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"] = "注销";

            
            if ((bool)ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] == false)
            {
                MainPage.current.WaitProgressRing.IsActive = true;
                MainPage.current.WaitProgressTextBlock.Text = "正在登陆";
                MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Visible;
                try
                {
                    await OneDriveService.Instance.LoginAsync();
                    ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] = true;
                    ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"] = "注销";
                    LoginContent = ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"].ToString();
                    MainPage.current.WaitProgressTextBlock.Text = "连接到OneDrive";
                    MainPage.current.WaitProgressTextBlock.Text = "合并数据";
                    await Functions.LoadFromOnedrive();
                }
                catch { }
                
                MainPage.current.WaitProgressRing.IsActive = false;
                MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainPage.current.WaitProgressRing.IsActive = true;
                MainPage.current.WaitProgressTextBlock.Text = "正在注销";
                MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Visible;
                var folder = await OneDriveService.Instance.AppRootFolderAsync();
                await OneDriveService.Instance.LogoutAsync();
                ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] = false;
                ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"] = "登陆";
                LoginContent = ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"].ToString();
                MainPage.current.WaitProgressRing.IsActive = false;
                MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        
        private void SettingAppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SyncAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
            // After initialization the user will need to log in and give permission for the access scopes
            MainPage.current.WaitProgressRing.IsActive = true;
            

            try
            {
                //ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] = false;

                if ((bool)ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] == false)
                {
                    MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Visible;
                    MainPage.current.WaitProgressTextBlock.Text = "正在登录";
                    await OneDriveService.Instance.LoginAsync();
                    ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] = true;
                    ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"] = "注销";
                    LoginContent = ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"].ToString();
                    MainPage.current.WaitProgressTextBlock.Text = "连接到OneDrive";
                    MainPage.current.WaitProgressTextBlock.Text = "合并数据";
                    await Functions.LoadFromOnedrive();
                    
                }
                
                //await Functions.AndDatabaseAsync();
                MainPage.current.WaitProgressTextBlock.Text = "保存到OneDrive";
                await Functions.SaveToOnedrive();
                diaries = SqliteDatabase.LoadFromDatabase(DBName, DiaryTableName);
                reminds = SqliteDatabase.LoadFromDatabase2(DBName, RemindTableName);
                this.Bindings.Update();
            }
            catch(Exception ex)
            {
                if(ex.Message!= "User canceled authentication")
                {
                    MainPage.current.WaitProgressRing.IsActive = false;
                    MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Collapsed;
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "YourDiary",
                        Content = ex.Message,
                        IsSecondaryButtonEnabled = true,
                        SecondaryButtonText = "关闭"
                    };
                    await dialog.ShowAsync();
                }

                    
                
            }



            MainPage.current.WaitProgressTextBlock.Visibility = Visibility.Collapsed;
            MainPage.current.WaitProgressRing.IsActive = false;
        }
    }
}
