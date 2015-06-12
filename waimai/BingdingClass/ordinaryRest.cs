using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace waimai.BingdingClass
{
    class ordinaryRest
    {
        /// <summary>
        /// 付，票，配在第三列；减，预，首在第四行
        /// </summary>
       public BitmapImage imageSource { set; get; }
        public string restName { set; get; }
        public string monthSellTips { set; get; }//月售
        public string deliverSpent { set; get; }//送餐时间
        public string Distance { set; get; }
        public string leastMoneyTips { set; get; }//起送
        public string iconPay { set; get; }//付
        public string iconPayText { set; get; }
        public string iconCheck { set; get; }//票
        public string iconCheckText { set; get; }
        public string iconFirst { set; get; }//首
        public string iconFirstText { set; get; }
        public string iconMinus { set; get; }//减
        public string iconMinusText { set; get; }
        public string iconBook { set; get; }//预
        public string iconBookText { set; get; }
        public string iconDeliver { set; get; }//配
        public string iconDeliverText { set; get; }
        public string name_for_url { set; get; }
        public string Rate { set; get; }
        public string Total { set; get; }
    }
}
