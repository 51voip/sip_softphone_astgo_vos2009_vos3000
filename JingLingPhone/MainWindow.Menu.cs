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
using BogheApp.Services.Impl;
using BogheApp.Screens;
using BogheCore.Model;
using System.Windows.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BogheApp
{
    partial class MainWindow
    {
        #region MenuItemFile

        private void MenuItemFile_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            if (menuItem == null) return;

            //if (menuItem == this.MenuItemFile_SignIn)
            //{
            //    this.sipService.Register();
            //}
            //else if (menuItem == this.MenuItemFile_SignOut)
            //{
            //    App.Current.Shutdown();
            //    // 启动
            //    Process.Start(@"NetPhone.exe", "");
            //}
            //else if (menuItem == this.MenuItemFile_Registrations)
            //{
            //    this.screenService.ScreenRegistrations.Registrations = this.registrations;
            //    this.screenService.Show(ScreenType.Registrations);
            //}
            //else if (menuItem == this.MenuItemFile_Exit)
            //{
            //    App.Current.Shutdown();
            //}
        }

        #endregion

        #region MenuItemAddressBook

        private void MenuItemEAB_Click(object sender, RoutedEventArgs e)
        {
            //MenuItem menuItem = e.OriginalSource as MenuItem;
            //if (menuItem == null) return;

            //if (menuItem == this.MenuItemEAB_Refresh)
            //{
            //    this.contactService.Download();
            //}
            //else if (menuItem == this.MenuItemEAB_Reset)
            //{
            //}
            //else if (menuItem == this.MenuItemEAB_AddContact)
            //{
            //    ScreenContactEdit screenEditContact = new ScreenContactEdit(null, null);
            //    this.screenService.Show(screenEditContact);
            //}
            //else if (menuItem == this.MenuItemEAB_EditContact)
            //{
            //}
            //else if (menuItem == this.MenuItemEAB_DeleteContact)
            //{
            //}
            //else if (menuItem == this.MenuItemEAB_AddGroup)
            //{
            //    ScreenGroupEdit screenGroupEdit = new ScreenGroupEdit(null);
            //    this.screenService.Show(screenGroupEdit);
            //}
            //else if (menuItem == this.MenuItemEAB_EditGroup)
            //{
            //}
            //else if (menuItem == this.MenuItemEAB_DeleteGroup)
            //{
            //}
            ////else if (menuItem == this.MenuItemEAB_Authorizations)
            ////{
            ////    this.screenService.ScreenWatchers.Watchers = this.watchers;
            ////    this.screenService.Show(ScreenType.Authorizations);
            ////}
        }

        #endregion

        #region MenuItemHistory

        private void MenuItemHistory_Click(object sender, RoutedEventArgs e)
        {
            //MenuItem menuItem = e.OriginalSource as MenuItem;
            //if (menuItem == null) return;

            //if (menuItem == this.MenuItemHistory_ShowHistory)
            //{
            //    this.screenService.ScreenHistory.Select(HistoryEvent.StatusType.All, BogheCore.MediaType.All);
            //    this.screenService.Show(ScreenType.History);
            //}
            //else if (menuItem == this.MenuItemHistory_ShowCalls)
            //{
            //    this.screenService.ScreenHistory.Select(HistoryEvent.StatusType.All, BogheCore.MediaType.AudioVideo);
            //    this.screenService.Show(ScreenType.History);
            //}
            //else if (menuItem == this.MenuItemHistory_ShowFileTransfers)
            //{
            //    this.screenService.ScreenHistory.Select(HistoryEvent.StatusType.All, BogheCore.MediaType.FileTransfer);
            //    this.screenService.Show(ScreenType.History);
            //}
            //else if (menuItem == this.MenuItemHistory_ShowMessages)
            //{
            //    this.screenService.ScreenHistory.Select(HistoryEvent.StatusType.All, BogheCore.MediaType.Messaging);
            //    this.screenService.Show(ScreenType.History);
            //}
            //else if (menuItem == this.MenuItemHistory_Clear)
            //{
            //    this.historyService.Clear();
            //}
        }

        #endregion

        #region MenuItemTools

        private void MenuItemTools_Click(object sender, RoutedEventArgs e)
        {
            //MenuItem menuItem = e.OriginalSource as MenuItem;
            //if (menuItem == null) return;
            

            //if (menuItem == this.MenuItemTools_Options)
            //{
            //    this.screenService.ScreenOptions.onAvatarChanded += (_sender, _e) =>
            //    {
            //        if (System.IO.File.Exists(_e.Value))
            //        {
            //            //this.imageAvatar.Source = new ImageSourceConverter().ConvertFromInvariantString(_e.Value) as ImageSource;
            //        }
            //    };
            //    this.screenService.Show(ScreenType.Options);
            //}
        }

        #endregion

        #region MenuItemHelp

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            //MenuItem menuItem = e.OriginalSource as MenuItem;
            //if (menuItem == null) return;


            //if (menuItem == this.MenuItemHelp_About)
            //{
            //    Win32ServiceManager.SharedManager.Win32ScreenService.Show(ScreenType.About);
            //}
            //else if (menuItem == this.MenuItemHelp_BogheWebsite)
            //{
            //    System.Diagnostics.Process.Start("http://code.google.com/p/boghe/");
            //}
            //else if (menuItem == this.MenuItemHelp_Check4Updates)
            //{
            //}
            //else if (menuItem == this.MenuItemHelp_GettingStarted)
            //{
            //}
            //else if (menuItem == this.MenuItemHelp_IssueTracker)
            //{
            //    System.Diagnostics.Process.Start("http://code.google.com/p/boghe/issues/list");
            //}
            //else if (menuItem == this.MenuItemHelp_RequestInfo)
            //{
            //    System.Diagnostics.Process.Start("mailto:diop m am ado u @ d ou b ang o . org".Replace(" ", String.Empty));
            //}
            //else if (menuItem == this.MenuItemHelpDoubangoWebsite)
            //{
            //    System.Diagnostics.Process.Start("http://www.doubango.org");
            //}
        }

        #endregion
    }
}
