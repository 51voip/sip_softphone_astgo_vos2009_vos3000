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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using BogheCore.Services.Impl;
using BogheApp.Services.Impl;
using System.IO;
using log4net.Config;
using log4net;
using System.Windows.Markup;
using System.Globalization;

namespace BogheApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ILog LOG;

        // 配置文件和好友列表等信息均存在 C:\Users\Administrator\AppData\Roaming\Doubango\Boghe IMS Client 目录下，
        // 可以通过Win32ServiceManager.SharedManager.ApplicationDataPath 变量来去得上述路径值。

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                String log4net = String.Format(BogheApp.Properties.Resources.log4net_xml, Win32ServiceManager.SharedManager.ApplicationDataPath);
                using (Stream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(log4net)))
                {
                    XmlConfigurator.Configure(stream);
                    LOG = LogManager.GetLogger(typeof(App));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Shutdown();
            }

            LOG.Debug("====================================================");
            LOG.Debug(String.Format("======Starting Boghe - IMS/RCS Client v{0} ====", System.Reflection.Assembly.GetEntryAssembly().GetName().Version));
            LOG.Debug("====================================================");

            //CultureInfo culture = new CultureInfo("fr-FR");     
           // System.Threading.Thread.CurrentThread.CurrentCulture = culture;
           // System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            if (!Win32ServiceManager.SharedManager.Start())
            {
                MessageBox.Show("Failed to start service manager");
                this.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Win32ServiceManager.SharedManager.Stop();
        }
    }
}
