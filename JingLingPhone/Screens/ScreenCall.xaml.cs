/*
* Boghe IMS/RCS Client - Copyright (C) 2010 Mamadou Diop.
*
* Contact: Mamadou Diop <diopmamadou(at)doubango.org>
*	
* This file is part of Boghe Project (http://code.google.com/p/boghe)
*
* Boghe is free software: you can redistribute it and/or modify it under the terms of 
* the GNU General Public License as published by the Free Software Foundation, either version 3 
* of the License, or (at your option) any later version.
*	
* Boghe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
* See the GNU General Public License for more details.
*	
* You should have received a copy of the GNU General Public License along 
* with this program; if not, write to the Free Software Foundation, Inc., 
* 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BogheControls;
using BogheCore.Model;
using System.Collections.ObjectModel;
using BogheApp.Items;
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheCore;
using BogheCore.Sip;
using BogheCore.Utils;
using System.ComponentModel;
using System.Collections;
using BogheXdm;
using System.Globalization;
using log4net;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Media;

using System.Text.RegularExpressions;
using BogheApp.MP3;
using System.Windows.Threading;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenCall.xaml
    /// </summary>
    public partial class ScreenCall : BaseScreen
    {
      public  DispatcherTimer timer = new DispatcherTimer();


        private string m_exeFileFullPath = "";

        private static readonly ILog LOG = LogManager.GetLogger(typeof(ScreenContacts));
        public bool IsCall;


        public ScreenCall()
        {
            InitializeComponent();
            Common.CallMain = this;
            string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
            m_exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += timer_Tick;//计时器事件处理程序
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)

        {    //呼叫超时处理
            if (Common.CallStartTime.AddSeconds(30)<DateTime.Now&&IsCall==true)
            {
                H2.Text = "呼叫超时，请重试！";
                IsCall = false;
                //挂断
                Image_MouseLeftButtonDown_1(null, null);
                timer.IsEnabled = false;
                timer.Stop();

            }




        }

        private void button1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundPlayer player = new SoundPlayer(m_exeFileFullPath + "sound\\1.WAV"); player.Play();       
            this.textBoxTelNum.Text += "1";
        }


        private void buttonx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.textBoxTelNum.Text.Length > 0)
            {
                this.textBoxTelNum.Text = this.textBoxTelNum.Text.Substring(0, this.textBoxTelNum.Text.Length - 1);
            }
        }

        private void buttonCall_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void HJhov(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((Image)sender).Tag.ToString () =="0")
            {
              ((Image)sender).Source = new BitmapImage(new Uri("\\Images\\HJ1.jpg", UriKind.Relative));              
            }

        }

        private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((Image)sender).Tag.ToString() == "0")
            {
                ((Image)sender).Source = new BitmapImage(new Uri("\\Images\\hujiao.jpg", UriKind.Relative));
            }
        }



        private void N1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImageBrush berriesBrush = new ImageBrush();
            berriesBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\jianbian2.jpg", UriKind.Relative));

            ((Grid)sender).Background = berriesBrush;
        }

        private void N1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            ((Grid)sender).Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\jianbian1.jpg", UriKind.Relative)) };
        
        
        }


        private void NumClick(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        public void ShowView(int ntype)
        {
            if (ntype==1)
            {
                View1.Visibility = Visibility.Visible;
                View2.Visibility = Visibility.Hidden;
            }
            else
            {
                View2.Visibility = Visibility.Visible;
                View1.Visibility = Visibility.Hidden;
            }

        }


        public void Mykiven(){
            //播放MP3
            H2.Text = "正在呼叫中,请稍等...";
            Common.CallStartTime = DateTime.Now;
            timer.IsEnabled = true;
            timer.Start();
            IsCall = true;
            Common.MP3Play  = new clsMCI { FileName="sound/ring.mp3" };
            Common.MP3Play.play();
            //呼叫或者挂断
            Hj.Visibility = Visibility.Collapsed;
            Gj.Visibility = Visibility.Visible;

            if (!String.IsNullOrEmpty(this.textBoxTelNum.Text))
            {
                // 根据服务器规则，拨叫的号码前，自动添加0
                string strTel = "tel:0";
                strTel += this.textBoxTelNum.Text;
                String remoteUri = UriUtils.GetValidSipUri(strTel);
                View1.Visibility = Visibility.Hidden;
                //显示号码

                H0.Text = this.textBoxTelNum.Text;
                //获取归属地
                string gsd = "";
                string htm;

                if (textBoxTelNum.Text.Substring(0, 1) != "1")//固定电话
                {
                    htm = new HttpHelper().GetHtml(new HttpItem { URL = "http://www.ip138.com/post/search.asp?zone=" + strTel.Substring(4, 3) + "&action=zone2area", Encoding = System.Text.Encoding.GetEncoding("gbk") }).Html;
                    MatchCollection q = new Regex("<td style=\"padding-left:5%\" noswap class=tdc2>◎&nbsp;([\\u4e00-\\u9fa5]+)&nbsp;([\\u4e00-\\u9fa5]+)&nbsp;邮编：\\d+&nbsp;区号：(\\d+)").Matches(htm);
                    foreach (Match item in q)
                    {
                        if (item.Groups[3].Value == strTel.Substring(4, 4))
                        {
                            gsd = item.Groups[1].Value + " " + item.Groups[2].Value;
                        }
                    }
                    if (gsd.Length == 0)
                    {
                        foreach (Match item in q)
                        {
                            if (item.Groups[4].Value == strTel.Substring(4, 3))
                            {
                                gsd = item.Groups[1].Value;
                            }
                        }

                    }


                }
                else if (textBoxTelNum.Text.Length == 11)
                {

                    htm = new HttpHelper().GetHtml(new HttpItem { URL = "http://www.ip138.com:8080/search.asp?action=mobile&mobile=" + textBoxTelNum.Text }).Html;
                    Match q = new Regex(">(\\D\\S+)&nbsp;(\\S+)</TD>").Match(htm);
                    gsd = q.Groups[1].Value + " " + q.Groups[2].Value;
                }
                else
                {
                    //new M1("您输入的号码有误！").Show();
                    //textBoxTelNum.Text = "";
                    //View1.Visibility = Visibility.Visible;
                    //return;
                }
                H1.Text = gsd.Replace("-","").Replace(">","");
                H0.Text = textBoxTelNum.Text;
                View2.Visibility = Visibility.Visible;
                if (!String.IsNullOrEmpty(remoteUri))
                {
                    if (Common.CurrSession == null)
                    {
                        Common.CurrSession = new SessionWindow(remoteUri);
                        Common.CurrSession.MakeAudioCall(remoteUri);

                    }
                    else
                    {
                        Common.CurrSession.MakeAudioCall(remoteUri);
                    }

                    //MediaActionHanler.MakeAudioCall(remoteUri);

                }



            }
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Mykiven();


        }


















        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {

            Common.MP3Play.StopT();
            if (H2.Text.Contains("超时")==false)
            { 
            View1.Visibility = Visibility.Visible;
            View2.Visibility = Visibility.Hidden;
                }
            Common.CurrSession.AVSession.HoldCall();
            Common.CurrSession.AVSession.HangUpCall();
            Common.CurrSession.AVSession.Dispose();
            Common.CurrSession.timerCall.Stop();
            Common.CurrSession.soundService.StopRingBackTone();
            Common.CurrSession.soundService.StopRingTone();
            Common.CurrSession.Close ();          
            Common.CurrSession.timerCall.Stop();
            }
            catch (Exception)
            {
                
            
            }

            Hj.Visibility = Visibility.Visible ;
            Gj.Visibility = Visibility.Collapsed;
        }

        private void Kiven (object sender, MouseButtonEventArgs e)
        {
            //try
            //{
                if (sender.GetType().FullName.Contains ("Grid"))
                {
                    string fp = m_exeFileFullPath + "sound\\" + ((Grid)sender).Tag.ToString() + ".WAV";
                    this.textBoxTelNum.Text += ((Grid)sender).Tag.ToString();   
                    SoundPlayer player = new SoundPlayer(fp); player.Play();
                                      
                }
                else if (sender.GetType().FullName.Contains ("Text"))
                {    this.textBoxTelNum.Text += ((TextBlock)sender).Tag.ToString(); 
                     SoundPlayer player = new SoundPlayer(m_exeFileFullPath + "sound\\"+( (TextBlock)sender).Tag.ToString ()+".WAV"); player.Play();
                       
                }
            //}
            //catch (Exception)
            //{
              
            //}
        }





        private void Tg_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImageBrush berriesBrush = new ImageBrush();
            berriesBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\baix.jpg", UriKind.Relative));

            ((Grid)sender).Background = berriesBrush;
        }

        private void Tg_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImageBrush berriesBrush = new ImageBrush();
            berriesBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\huix.jpg", UriKind.Relative));

            ((Grid)sender).Background = berriesBrush;
        }

        private void Tg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            textBoxTelNum.Text = "";
        }

        private void textBoxTelNum_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {


        }

        private void textBoxTelNum_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key ==Key.Enter )
            {
                Mykiven();
            }
            string k = e.Key.ToString();
            k = k.Substring(k.Length - 1, 1);
            int ss=-1;
            int.TryParse(k,out ss);


            try
            {

                SoundPlayer player = new SoundPlayer(m_exeFileFullPath + "sound\\" + k + ".WAV"); player.Play();
            }
            catch (Exception)
            {

            }
        }



    }
}
