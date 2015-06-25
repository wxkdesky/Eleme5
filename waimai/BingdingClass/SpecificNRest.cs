using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace waimai.BingdingClass
{
   public class SpecificNRest
    {
        public List<string> itemName { set; get; }//eg 热销榜
        public List<string> itemDescription { set; get; }//eg 大家喜欢吃，才叫真好吃
       // public List<nFood> allfoods { set; get; }//
    }
    public class nFood
    {
        public string foodName { set; get; }
        public string foodActivity { set; get; }
        public string foodAttributes1 { set; get; }
        public string foodAttributes2 { set; get; }
        public string rateStar { set; get; }
        public string Evaluate { set; get; }
        public string monthSale { set; get; }
        public string foodPrice { set; get; }
        public BitmapImage foodImage { set; get; }
        public string foodLimitation { set; get; }
        public string foodDescription { set; get; }
    }
    public class topDescription
    {
        public string itemName { set; get; }
        public string itemDescription { set; get; }
    }
}
