using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

namespace waimai
{
   public class postJson
    {
      public  string postDataStr;
      public  string finalStr;
        public string buildJson(string geohash,string consumer_key)
        {
          postDataStr ="{\"requests\":[{\"method\":GET,\"url\":/v1/app_banners?full_image_path=1&geohash="+geohash+"},{\"method\":GET,\"url\":url=/v1/app_activities?consumer_key="+consumer_key+"&full_image_path=1&geohash="+geohash+"},{\"method\":GET,\"url\":url=/v1/restaurants?extras%5B%5D=food_activity&extras%5B%5D=restaurant_activity&full_image_path=1&geohash="+geohash+"&is_premium=1&limit=3&type=geohash},{\"method\":GET,\"url\":url=/v1/restaurants/count?geohash="+geohash+"&is_premium=1&type=geohash},{\"method\":GET,\"url\":url=/v1/restaurants?extras%5B%5D=food_activity&extras%5B%5D=restaurant_activity&full_image_path=1&geohash="+geohash+"&is_premium=0&limit=30&type=geohash},{\"method\":GET,\"url\":url=/v1/restaurants/count?geohash="+geohash+ "&is_premium=0&type=geohash}],\"timeout\":15000}";
            return postDataStr;
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < str.Length; i++)
            //{
            //    char c = str.ToCharArray()[i];
            //    switch (c)
            //    {
            //        case '\"':
            //            sb.Append("\\\""); break;
            //        case '\\':
            //            sb.Append("\\\\"); break;
            //        case '/':
            //            sb.Append("\\/"); break;
            //        case '\b':
            //            sb.Append("\\b"); break;
            //        case '\f':
            //            sb.Append("\\f"); break;
            //        case '\n':
            //            sb.Append("\\n"); break;
            //        case '\r':
            //            sb.Append("\\r"); break;
            //        case '\t':
            //            sb.Append("\\t"); break;
            //        default:
            //            sb.Append(c); break;
            //    }
            //}
            //str = sb.ToString();
            //return true;
       }
         HttpWebRequest request;
        public async void HttpPost(string Url)//HttpClient a,HttpResponseMessage msg, 
        {

            request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            // byte[] postBytes = Encoding.UTF8.GetBytes(postDataStr);
            // request.ContinueTimeout = 15000;
            // //request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            Stream myRequestStream = await request.GetRequestStreamAsync();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8"));
            myStreamWriter.Write(postDataStr);
            // myRequestStream.Write(postBytes, 0, postBytes.Length);
            await myStreamWriter.FlushAsync();
             Debug.WriteLine(postDataStr);
            var response = await request.GetResponseAsync();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Dispose();
            myResponseStream.Dispose();
            finalStr = retString;
        }

    }
}
