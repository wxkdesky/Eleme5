using System;
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
using waimai.BingdingClass;
using Windows.UI.Xaml.Media.Imaging;

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
        //SpecificNRest wholeRest;
        List<topDescription> td;
        List<topItem> ti;
        List<nFood> foodCollection;
        List<List<nFood>> foodCategory;
        nFood foodOne;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //name for url and restname
            string[]nameTo = (string[])e.Parameter;
            if (nameTo[0] != nameToCopy[0])
            {
                //initialize
                td = new List<topDescription>();
                foodCollection = new List<nFood>();
                foodCategory = new List<List<nFood>>();
                ti = new List<topItem>();
                td = new List<topDescription>();
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
                    nameToCopy = new string[2] { "", "" };
                    return;
                }
                int tabIndex = 0;
                    myArray = JsonArray.Parse(responseText);
                    foreach (var item in myArray)
                    {
                        tabIndex++;
                        JsonObject temp = item.GetObject();
                        topDescription tempTd = new topDescription();
                        topItem tempTi = new topItem();
                        tempTi.itemName = temp["name"].GetString();
                        tempTi.count = tabIndex-1;
                        tempTd.itemDescription = temp["description"].GetString();
                        td.Add(tempTd);
                        ti.Add(tempTi);
                        var foodArray = temp["foods"].GetArray();
                        foreach (var one in foodArray)
                        {
                            foodOne = new nFood();
                            JsonObject jso = one.GetObject();
                            foodOne.foodDescription = jso["description"].GetString();
                            foodOne.foodName = jso["name"].GetString();
                            if (jso["image_path"].GetString()=="")
                                foodOne.foodImage = new BitmapImage(new Uri("ms-appx:///Assets/defaultPic.jpg"));
                            else
                                foodOne.foodImage = new BitmapImage(new Uri(jso["image_path"].GetString()));
                            foodOne.monthSale ="月售"+Convert.ToString( jso["month_sales"].GetNumber())+"份";
                            foodOne.foodPrice ="￥"+ Convert.ToString(jso["price"].GetNumber());
                            foodOne.Evaluate = Convert.ToString(jso["rating_count"].GetNumber()) + "评价";
                            double rate = jso["rating"].GetNumber();
                            if (rate > 4)
                                foodOne.rateStar = "★★★★★";
                            else if (rate > 3)
                                foodOne.rateStar = "★★★★☆";
                            else if (rate > 2)
                                foodOne.rateStar = "★★★☆☆";
                            else if (rate > 1)
                                foodOne.rateStar = "★★☆☆☆";
                            else
                                foodOne.rateStar = "★☆☆☆☆";
                           // foodOne.foodActivity = jso["activity"].GetString();
                            var attribute = jso["attributes"].GetArray();
                            for(int i=0;i<attribute.Count;i++)
                            {
                                JsonObject jso2 = attribute[i].GetObject();
                                if (i == 0)
                                {
                                    foodOne.foodAttributes1 = (jso2["icon_name"].GetString()==null)?"":jso2["icon_name"].GetString();
                                }
                                else
                                    foodOne.foodAttributes2 = (jso2["icon_name"].GetString() == null) ? "" : jso2["icon_name"].GetString();                                
                            }
                            //try
                            //{
                            //    var limit = jso["limitation"].GetObject();
                            //    foodOne.foodLimitation = limit["text"].GetString();
                            //}
                            //catch
                            //{
                                foodOne.foodLimitation = "某些项目会有购买限制，在下单时请注意提示";
                            //}
                            foodCollection.Add(foodOne);
                        }
                        foodCategory.Add(foodCollection);
                        foodCollection = new List<nFood>();
                    }
                   // wholeRest.allfoods = foodCollection;
                    //默认绑定第一个条目
                    itemList.ItemsSource = ti;
                   // topDescription td = new topDescription() { itemDescription = wholeRest.itemDescription[0], itemName = wholeRest.itemName[0] };                    
                    tbItemName.Text = ti[0].itemName;
                    tbItemDecription.Text = td[0].itemDescription;
                    foodList.ItemsSource =(List<nFood>)foodCategory[0];
                    setColor(cd, zp, ct);                                        
            }
            nameToCopy = nameTo;
        }
        /// <summary>
        /// 蓝和灰颜色的toggle
        /// </summary>
        /// <param name="a">a是突出显示</param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void setColor(Grid a,Grid b,Grid c)
        {
            a.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 92, 172, 238));
            b.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 247, 247, 247));
            c.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 247, 247, 247));
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
        int gridCount = 0;
        private void itemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Border t = (Border)itemList.SelectedItem;
            //int x = itemList.SelectedIndex;
           //Debug.WriteLine(((topItem)e.ClickedItem));
            gridCount = ((topItem)e.ClickedItem).count;
            foodList.ItemsSource = null;
            foodList.ItemsSource = foodCategory[((topItem)e.ClickedItem).count];
            tbItemName.Text = ((topItem)e.ClickedItem).itemName;
            tbItemDecription.Text = td[((topItem)e.ClickedItem).count].itemDescription;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Border t = (Border)itemList.SelectedItem;
            //int x = itemList.SelectedIndex;
            //for (int i = 0; i < x; i++)
            //{
                //x = (Grid)itemList.ContainerFromIndex(i);
               // ((Grid)((Border)ic[i]).Child).Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
               // x.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
            //}
            ti[gridCount].itemGrid = (Grid)sender;
            for (int i = 0; i < ti.Count;i++ )
            {
                if (ti[i].itemGrid != null)
                    ti[i].itemGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,245,245,245));
            }
            ((Grid)sender).Background = new SolidColorBrush(Windows.UI.Color.FromArgb(80, 92, 172, 238));
            gridCount = 0;
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
