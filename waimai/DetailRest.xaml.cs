﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace waimai
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailRest : Page
    {
        private HttpResponseMessage msg;
        public string responseText;
        HttpClient a = new HttpClient();
        JsonObject jsonObject=new JsonObject();
        JsonArray myArray=new JsonArray();
        public DetailRest()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            var httpHeader = a.DefaultRequestHeaders;
            httpHeader.UserAgent.ParseAdd("ie");
            httpHeader.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        string[] nameToCopy = new string[2] { "",""};
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            string[]nameTo = (string[])e.Parameter;
            if (nameTo[0] != nameToCopy[0])
            {
                restNameTb.Text = nameTo[1];
                msg = new HttpResponseMessage();
                string add = "http://restapi.ele.me/v1/restaurants/" + nameTo[0] + "/menu?full_image_path=1";
                try
                {
                    msg = await a.GetAsync(new Uri(add));
                    msg.EnsureSuccessStatusCode();
                    responseText = await msg.Content.ReadAsStringAsync();
                    Debug.WriteLine(responseText);
                }
                catch (Exception ex)
                {
                    // Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                    XmlNodeList element = toastXml.GetElementsByTagName("text");
                    element[0].AppendChild(toastXml.CreateTextNode("获取远程数据时发生意外，请检查您的网络"));
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
                Popup p = new Popup();
                try
                {
                    // responseText = "{" + responseText + "}";
                    myArray = JsonArray.Parse(responseText);
                    //myArray = jsonObject.GetArray();
                }
                catch
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = "数据解析发生未知错误";
                    tb.FontSize = 20;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    StackPanel Panel = new StackPanel();
                    Panel.Background = new SolidColorBrush(Windows.UI.Colors.Cyan);
                    Panel.HorizontalAlignment = HorizontalAlignment.Center;
                    Panel.VerticalAlignment = VerticalAlignment.Center;
                    Panel.Height = 40;
                    Panel.Width = 400;
                    Panel.Children.Add(tb);
                    p.Child = Panel;
                    p.IsOpen = true;
                }
                await Task.Delay(2000);
                p.IsOpen = false;
                foreach (var item in myArray)
                {
                    JsonObject temp = item.GetObject();
                    //tb1.Text += temp["description"].GetString();
                }
                nameToCopy = nameTo;
            }
            else
                return;
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            //tb1.Text = "";
            if(Frame.CanGoBack)
            this.Frame.GoBack();//.Navigate(typeof(MainPage),1);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                this.Frame.GoBack();//Navigate(typeof(MainPage), 1);
        }
    }
}