using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

namespace waimai
{
   public static class postJson
    {
        static  string str;
        public static bool buildJson(string geohash,string consumer_key)
        {
            str =String.Format("{\"requests\":[{\"method\":GET,\"url\":/v1/app_banners?full_image_path=1&geohash={0}},{\"method\":GET,\"url\":url=/v1/app_activities?consumer_key={1}&full_image_path=1&geohash={0}},{\"method\":GET,\"url\":url=/v1/restaurants?extras%5B%5D=food_activity&extras%5B%5D=restaurant_activity&full_image_path=1&geohash={0}&is_premium=1&limit=3&type=geohash},{\"method\":GET,\"url\":url=/v1/restaurants/count?geohash={0}&is_premium=1&type=geohash},{\"method\":GET,\"url\":url=/v1/restaurants?extras%5B%5D=food_activity&extras%5B%5D=restaurant_activity&full_image_path=1&geohash={0}&is_premium=0&limit=30&type=geohash},{\"method\":GET,\"url\":url=/v1/restaurants/count?geohash={0}&is_premium=0&type=geohash}]}",geohash,consumer_key);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            str = sb.ToString();
            return true;
        }
        public static async bool goPost(string response,HttpClient a, Uri b)
        {
            HttpResponseMessage msg;
            JsonArray js = JsonArray.Parse(str);
            msg = await a.PostAsync(b);
        }
    }
}
