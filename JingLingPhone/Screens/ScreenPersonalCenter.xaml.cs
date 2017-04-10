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
using BogheApp.Services.Impl;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using System.Windows.Threading;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenPersonalCenter.xaml
    /// </summary>
    public partial class ScreenPersonalCenter : BaseScreen
    {
        private string CONFIG_NAME = "User.xml";

        public ScreenPersonalCenter()
        {
            InitializeComponent();



            string dispName =Common.displayName ;
            string passwordBox = Common.Password;
            this.label2.Text = Common.Uye.ToString ("0.00");
            this.label1.Text= dispName;
        }

        //充值
        private void textBlock1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new CZ().Show();
        }

        //话单查询
        private void textBlock2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string strQQ = "http://www.qqsxy.com/";
            Process.Start(strQQ);
        }

        //修改密码
        private void textBlock3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new GM().Show();
        }

        // 立即充值
        private void label3_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void label3_MouseLeave(object sender, MouseEventArgs e)
        {
           
        }

        private void label4_MouseEnter(object sender, MouseEventArgs e)
        {
         
        }

        private void label4_MouseLeave(object sender, MouseEventArgs e)
        {
           
        }

        private void label5_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void label5_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }
    }
}
