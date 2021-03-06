﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using Size = Windows.Foundation.Size;
using YourDiary3.Views;
using YourDiary3.Models;
using Windows.Storage;



// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace YourDiary3
{

    
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static readonly string DBName = "YourDiary.db3";
        public static MainPage current;
        public int DeviceWidth = 0;
        //public int LeftFrameWidth = 0;
        public MainPage()
        {
            Size size = GetDeviceResolution();
            DeviceWidth = (int)size.Width / 2;
            //LeftFrameWidth = (int)size.Width / 4;
            this.InitializeComponent();
            LeftFrame.Navigate(typeof(ListViewPage));
            //RightFrame.Navigate(typeof(DiaryContentPage));
            current = this;
            RightFrame.Navigate(typeof(RemindContentPage), "1");
            

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Functions.SetCanvasZ("10");

            ApplicationData.Current.LocalSettings.CreateContainer("signStateContainer", ApplicationDataCreateDisposition.Always);

            if (!(ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] is bool))

            {

                ApplicationData.Current.LocalSettings.Containers["signStateContainer"].Values["signState"] = false;

            }
            ApplicationData.Current.LocalSettings.CreateContainer("signStateContent", ApplicationDataCreateDisposition.Always);
            if (ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"]==null)
            {
                ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"] = "登录";
                ListViewPage.current.LoginContent = ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"].ToString();
            }
            else
            {
                ListViewPage.current.LoginContent = ApplicationData.Current.LocalSettings.Containers["signStateContent"].Values["signState"].ToString();
            }
            


        }

        /// <summary>
        /// 获取设备屏幕长宽
        /// </summary>
        /// <returns></returns>
        public static Size GetDeviceResolution()
        {
            //using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            //{
            //    float dpiX = graphics.DpiX;
            //    float dpiY = graphics.DpiY;
            //}

            //using (ManagementClass mc = new ManagementClass("Win32_DesktopMonitor"))
            //{
            //    using (ManagementObjectCollection moc = mc.GetInstances())
            //    {

            //        int PixelsPerXLogicalInch = 0; // dpi for x
            //        int PixelsPerYLogicalInch = 0; // dpi for y

            //        foreach (ManagementObject each in moc)
            //        {
            //            PixelsPerXLogicalInch = int.Parse((each.Properties["PixelsPerXLogicalInch"].Value.ToString()));
            //            PixelsPerYLogicalInch = int.Parse((each.Properties["PixelsPerYLogicalInch"].Value.ToString()));
            //        }

            //        Console.WriteLine("PixelsPerXLogicalInch:" + PixelsPerXLogicalInch.ToString());
            //        Console.WriteLine("PixelsPerYLogicalInch:" + PixelsPerYLogicalInch.ToString());
            //        Console.Read();
            //    }
            //}
            Size resolution = Size.Empty;
            //var rawPixelsPerViewPixel = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            //var DPI = DisplayInformation.GetForCurrentView().RawDpiX;

            foreach (var item in PointerDevice.GetPointerDevices())
            {
                resolution.Width = item.ScreenRect.Width;
                resolution.Height = item.ScreenRect.Height;
                break;
            }
            return resolution;
        }

        

        
    }
}
