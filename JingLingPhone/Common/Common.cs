using BogheApp.MP3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BogheApp
{
   public static  class Common
    {
       public static string Uid = "";
       public static string  displayName;
       public static string Password ;
       public static Double Uye = 0.0;
       public static string Uname = "";
       public static SessionWindow CurrSession;
       public static Screens.ScreenCall CallMain;
       public static clsMCI MP3Play;
       public static DateTime CallStartTime ;
       public static bool IsLoginOut;
       public static MainWindow MainW;
       public static void WriteUsers(LocalUsers locusers, string FilepPath)
       {
           try
           {

               XmlSerializer t = new XmlSerializer(typeof(LocalUsers));
               Stream stream = new FileStream(FilepPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
               t.Serialize(stream, locusers);
               stream.Close();
           }
           catch (Exception)
           {


           }

       }
       public static LocalUsers ReadUsers(string FilepPath)
       {
           LocalUsers Users = null;
           try
           {
               XmlSerializer xs = new XmlSerializer(typeof(LocalUsers));

               Stream stream = new FileStream(FilepPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

               Users = (LocalUsers)xs.Deserialize(stream);



               return Users;
           }
           catch (Exception)
           {

               return Users;
           }

       }












    }

   public class LocalUsers
   {
       public string UserName { get; set; }

       public string UserPass { get; set; }
       public bool IsSaveUser { get; set; }

       public bool IsAutoLogin { get; set; }

   }

   public class THJL
   {
        string cookie = "";
        //创建HTTP访问类对象
       Kiven.HttpHelper http = new Kiven.HttpHelper();
        Kiven.HttpItem item = null;

       public string GetThjl(string mypass)
       {
                       //参数类
            item = new Kiven.HttpItem()
           {
               URL = "http://115.239.227.121/chs/login.jsp",//URL     必需项
               //Encoding = "utf-8",//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
               Method = "Post",//URL     可选项 默认为Get
               ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
               Postdata = "loginType=0&name=" + Common.displayName + "&pass=" + mypass,//Post数据 使用URLEncode是为了解决中文用户名或者密码的问题    可选项GET时不需要写
           };
            //得到HTML代码

            string html =http.GetHtml(item);
            cookie = item.Cookie;
            return cookie;


       }









   }
}
