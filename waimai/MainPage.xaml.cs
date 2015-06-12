using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using waimai.BingdingClass;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace waimai
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpResponseMessage msg;
        public double latitude = 0;
        public double longitude = 0;
        public string responseText;
        HttpClient a = new HttpClient();
        string time = "";
        ordinaryRest myNRest;
        ObservableCollection<ordinaryRest> nRest = new ObservableCollection<ordinaryRest>();
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            //HttpClient a = new HttpClient();
            var httpHeader = a.DefaultRequestHeaders;
            httpHeader.UserAgent.ParseAdd("ie");
            httpHeader.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            time = Convert.ToInt64(now.TotalMilliseconds).ToString();
            
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
            if (e.NavigationMode == NavigationMode.New)
            {
                Geolocator mylocator = new Geolocator();
                mylocator.DesiredAccuracyInMeters = 1;
                try
                {
                    Geoposition geopostion = await mylocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
                    latitude = geopostion.Coordinate.Point.Position.Latitude;
                    longitude = geopostion.Coordinate.Point.Position.Longitude;
                }
                catch (UnauthorizedAccessException)
                {
                    //tb1.Text = "Get position infor failed!";
                }
            }
        }
        JsonObject jsonObject;
        JsonArray myArray;
        JsonArray restArray;
        string geoh;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            connect.Background = null;
            Debug.WriteLine(latitude + "/" + longitude);
            // HttpResponseMessage x = await a.GetAsync(new Uri("http://m.ele.me/restaurants"));
            //  tb1.Text = x.Content.ToString();
            geoh = Geohash.Encode(latitude,longitude);
            //geoh = "wtw37tkct0fw";
            msg = new HttpResponseMessage();
            string address = "http://restapi.ele.me/batch"; //"http://api.ele.me/1/home?banner_width=640&consumer_key=7284397383&geohash="+ geoh+"&session_id=066b2f78e5b6a28eba862842e911d19c&sig=0&timestamp=" + time + "&track_id=1431963253%7C_561daf6c-fd73-11e4-bf65-549f3515da4c%7Cdffd7577b1ef7a401665a87e8bdda416"; //geohash=wtw37tkct0fw    "http://v2.openapi.ele.me/restaurants?geo=" + longitude+","+latitude;sig=b13cf07a2fcce597ef70c6d15c46a50e
            string location = "http://restapi.ele.me/v1/pois/"+geoh;
            try
            {
                msg = await a.GetAsync(new Uri(location));
                msg.EnsureSuccessStatusCode();
                responseText = await msg.Content.ReadAsStringAsync();
                Debug.WriteLine(responseText);
                jsonObject = JsonObject.Parse(responseText);
                LocationTb.Text = jsonObject["name"].GetString();
                //next things need to be modified by command Post
                msg = await a.GetAsync(new Uri(address));
                msg.EnsureSuccessStatusCode();
                responseText = await msg.Content.ReadAsStringAsync();
                Debug.WriteLine(responseText);
                //jsonObject = JsonObject.Parse(responseText);
                // Type x = jsonObject["home"].GetType();
                // string x = jsonObject.GetNamedValue("");
                // myArray = jsonObject.GetArray();
                // JsonObject item = myArray.GetObject();
                //tb1.Text = "finish";
            }
            catch (Exception ex)
            {
                //tb1.Text = "Network request failed" + ex.Message;
            }
            connect.Background = new SolidColorBrush(Windows.UI.Colors.Orange);

        }
        ///http://restapi.ele.me/v1/restaurants/gwygre478/menu?full_image_path=1
        private void bt1_Click(object sender, RoutedEventArgs e)
        {
            myArray = JsonArray.Parse(responseText);
            jsonObject = myArray[4].GetObject();
            restArray = jsonObject["body"].GetArray();

            //jsonObject = JsonObject.Parse(responseText);
            // myArray = jsonObject["promotions"].GetArray();
            JsonObject temp;// = jsonObject["home"].GetObject();
           // myArray = temp["restaurants"].GetArray();
            // int nnnn = myArray.Count; 30
            foreach (var item in restArray)
            {
                myNRest = new ordinaryRest();
                //next is to resolve the data
                temp = item.GetObject();
                JsonArray icon = temp["supports"].GetArray();
               // int count = 0;
                for (int i = 0; i < icon.Count; i++)
                {
                    JsonObject otmp = icon[i].GetObject();
                    // Oh,i will keep on going when i come back from home 
                    string nameee = otmp["icon_name"].GetString();
                    if (nameee == "付")
                    {
                        myNRest.iconPay = otmp["icon_name"].GetString() + "·";
                        myNRest.iconPayText = otmp["description"].GetString();
                    }
                    else if (nameee == "票")
                    {
                        myNRest.iconCheck = otmp["icon_name"].GetString() + "·";
                        myNRest.iconCheckText = otmp["description"].GetString();
                    }
                    else if (nameee == "配")
                    {
                        myNRest.iconDeliver = otmp["icon_name"].GetString() + "·";
                        myNRest.iconDeliverText = otmp["description"].GetString();
                    }

                    //    temp = item.GetObject();
                    //    tb1.Text += temp["subtitle"].GetString()+"\n"+temp["title"].GetString()+"\n"+temp["url"].GetString()+"\n"+temp["image_url"].GetString();
                }
                icon = temp["restaurant_activity"].GetArray();               
                for (int i = 0; i < icon.Count; i++)
                {
                    JsonObject otmp = icon[i].GetObject();
                    string nameee = otmp["icon_name"].GetString();
                    if (nameee == "减")
                    {
                        myNRest.iconMinus = otmp["icon_name"].GetString() + "·";
                        myNRest.iconMinusText = otmp["description"].GetString();
                    }
                    else if (nameee == "首")
                    {
                        myNRest.iconFirst = otmp["icon_name"].GetString() + "·";
                        myNRest.iconFirstText = otmp["description"].GetString();
                    }
                    else
                    {
                        myNRest.iconFirst = otmp["icon_name"].GetString() + "·";
                        myNRest.iconFirstText = otmp["description"].GetString();
                    }
                }
                icon = null;
                myNRest.restName = temp["name"].GetString();
               double dis = temp["distance"].GetNumber();
                if (dis < 1000)
                    myNRest.Distance = Convert.ToString(dis)+"米";
                else
                    myNRest.Distance = Convert.ToString(dis/1000)+"千米";
                myNRest.deliverSpent = Convert.ToString(temp["order_lead_time"].GetNumber()) + "分钟";
                myNRest.imageSource = new BitmapImage(new Uri(temp["image_path"].GetString()));
                //Regex mod = new Regex(@"(月售\d+份)[\s]+(\d+)元起送*");
                //Match x = mod.Match(temp["tips"].GetString());
                //myNRest.leastMoneyTips = "￥" + x.Groups[2].Value.ToString();
                //myNRest.monthSellTips = x.Groups[1].Value.ToString();
                myNRest.leastMoneyTips = temp["minimum_order_amount"].GetString();
                myNRest.monthSellTips = temp["month_sales"].GetString();
                myNRest.name_for_url = temp["name_for_url"].GetString();
                //double customer = temp["num_rating_1"].GetNumber() + temp["num_rating_2"].GetNumber() + temp["num_rating_3"].GetNumber() + temp["num_rating_4"].GetNumber() + temp["num_rating_5"].GetNumber();
                //myNRest.Total = "(" + Convert.ToString(customer) + ")";
                myNRest.Total = temp["rating_count"].GetString();
                double r = temp["rating"].GetNumber(); //(temp["num_rating_1"].GetNumber() + temp["num_rating_2"].GetNumber() * 2 + temp["num_rating_3"].GetNumber() * 3 + temp["num_rating_4"].GetNumber() * 4 + temp["num_rating_5"].GetNumber() * 5) / customer;
                if (r > 4)
                    myNRest.Rate = "★★★★★";
                else if (r > 3)
                    myNRest.Rate = "★★★★☆";
                else if (r > 2)
                    myNRest.Rate = "★★★☆☆";
                else if (r > 1)
                    myNRest.Rate = "★★☆☆☆";
                else
                    myNRest.Rate = "★☆☆☆☆";
                JsonObject jtmp = temp["delivery_mode"].GetObject();
                myNRest.deliverMode = jtmp["text"].GetString();
                nRest.Add(myNRest);
                //Debug.WriteLine(temp["name_for_url"].GetString());
                jtmp = null;
            }
            NRestList.ItemsSource = nRest;

            // temp = myArray[0].GetObject();
            // tb1.Text = temp["subtitle"].GetString();
        }

        private void NRestList_ItemClick(object sender, ItemClickEventArgs e)
        {
            string[] p = new string[2];
            p[0]= ((ordinaryRest)e.ClickedItem).name_for_url;
            p[1] = ((ordinaryRest)e.ClickedItem).restName;
            this.Frame.Navigate(typeof(DetailRest), p);
        }
    }
}
