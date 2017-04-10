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
using System.Windows.Controls;
using BogheApp.Screens;
using BogheControls;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace BogheApp.Services.Impl
{
    class ScreenService : IWin32ScreenService
    {
        private TabControl tabControl;
        private Label labelProgressInfo;

        private ScreenType loadedScreens = ScreenType.None;

        private ScreenAbout screenAbout;
        private ScreenAuthentication screenAuthentication;
        private ScreenOptions screenOptions;
        private ScreenContacts screenContacts;
        private ScreenHistory screenHistory;
        private ScreenRegistrations screenRegistrations;
        private ScreenWatchers screenWatchers;
        private ScreenPersonalCenter screenPersonalCenter;
        private ScreenCall screenCall;

        BaseScreen lastScreen = null;

        #region IService

        /// <summary>
        /// Starts the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            return true;
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            return true;
        }

        #endregion

        #region IScreenService

        public void SetProgressLabel(Label labelProgressInfo)
        {
            this.labelProgressInfo = labelProgressInfo;
        }

        #endregion

        #region IWin32ScreenService

        public void SetTabControl(TabControl tabControl)
        {
            this.tabControl = tabControl;
        }

        public void SetProgressInfo(String text)
        {
            if (this.labelProgressInfo == null)
            {
                return;
            }

            if (this.labelProgressInfo.Dispatcher.Thread == Thread.CurrentThread)
            {
                this.labelProgressInfo.Content = text;
            }
            else
            {
                this.labelProgressInfo.Dispatcher.BeginInvoke((ThreadStart)delegate ()
                {
                    this.labelProgressInfo.Content = text;
                }, 
                null);
            }
        }

        public ScreenAbout ScreenAbout
        {
            get
            {
                if (this.screenAbout == null)
                {
                    this.screenAbout = new ScreenAbout();
                }
                return this.screenAbout;
            }
        }

        public ScreenCall ScreenCall
        {
            get
            {
                if (this.screenCall == null)
                {
                    this.screenCall = new ScreenCall();
                }
                return this.screenCall;
            }
        }

        public ScreenPersonalCenter ScreenPersonalCenter
        {
            get
            {
                if (this.screenPersonalCenter == null)
                {
                    this.screenPersonalCenter = new ScreenPersonalCenter();
                }
                return this.screenPersonalCenter;
            }
        }
        public ScreenAuthentication ScreenAuthentication 
        {
            get
            {
                if (this.screenAuthentication == null)
                {
                    this.screenAuthentication = new ScreenAuthentication();
                }
                return this.screenAuthentication;
            }
        }

        public ScreenOptions ScreenOptions 
        {
            get
            {
                if (this.screenOptions == null)
                {
                    this.screenOptions = new ScreenOptions();
                }
                return this.screenOptions;
            }
        }

        public ScreenContacts ScreenContacts
        {
            get
            {
                if (this.screenContacts == null)
                {
                    this.screenContacts = new ScreenContacts();
                }
                return this.screenContacts;
            }
        }

        public ScreenHistory ScreenHistory
        {
            get
            {
                if (this.screenHistory == null)
                {
                    this.screenHistory = new ScreenHistory();
                }
                return this.screenHistory;
            }
        }

        public ScreenRegistrations ScreenRegistrations
        {
            get
            {
                if (this.screenRegistrations == null)
                {
                    this.screenRegistrations = new ScreenRegistrations();
                }
                return this.screenRegistrations;
            }
        }
        
        public ScreenWatchers ScreenWatchers
        {
            get
            {
                if (this.screenWatchers == null)
                {
                    this.screenWatchers = new ScreenWatchers();
                }
                return this.screenWatchers;
            }
        }

        public void Show(BaseScreen baseScreen, int insertIndex)
        {
            int index = -1;
            foreach (TabItem item in this.tabControl.Items)
            {
                index ++;
                if (item.Content == null)
                {
                    continue;
                }
                BaseScreen _baseScreen = item.Content as BaseScreen;

                if (_baseScreen == baseScreen)
                {
                    this.tabControl.SelectedIndex = index;
                    return;
                }
            }

            TabItem tabItem = this.CreateTabItem(baseScreen, true);

            // Events
            baseScreen.BeforeLoading();
            if (lastScreen != null) lastScreen.BeforeUnLoading();

            if (insertIndex >= 0 && insertIndex < this.tabControl.Items.Count)
            {
                this.tabControl.Items.Insert(insertIndex, tabItem);
            }
            else
            {
                this.tabControl.Items.Add(tabItem);
            }
            this.tabControl.SelectedItem = tabItem;

            // Events
            if (lastScreen != null) lastScreen.AfterUnLoading();
            baseScreen.AfterLoading();
            lastScreen = baseScreen;
        }

        public void Show(BaseScreen baseScreen)
        {
            this.Show(baseScreen, -1);
        }

        public void Show(ScreenType type, int insertIndex)
        {
            TabItem tabItem = null;

            // sg++ modify 2014-3-6
            if ((this.loadedScreens & type) == type)
            {
                int index = this.GetScreenIndex(type);
                if (index != -1)
                {
                    this.tabControl.SelectedIndex = index;
                }
                return;
            }

            //this.tabControl.SelectedIndex = 0;
            // ++sg

            switch (type)
            {
                case ScreenType.About:
                    tabItem = this.CreateTabItem(this.ScreenAbout, true);
                    break;
                case ScreenType.Authentication:
                    tabItem = this.CreateTabItem(this.ScreenAuthentication, false);
                    break;
                case ScreenType.Options:
                    tabItem = this.CreateTabItem(this.ScreenOptions, true);
                    break;
                case ScreenType.History:
                    tabItem = this.CreateTabItem(this.ScreenHistory, false);
                    break;
                case ScreenType.Contacts:
                    tabItem = this.CreateTabItem(this.ScreenContacts, false);
                    break;
                case ScreenType.Registrations:
                    tabItem = this.CreateTabItem(this.ScreenRegistrations, true);
                    break;
                case ScreenType.Authorizations:
                    tabItem = this.CreateTabItem(this.ScreenWatchers, true);
                    break;
                case ScreenType.Call:
                    tabItem = this.CreateTabItem(this.ScreenCall, false);
                    break;
                case ScreenType.PersonalCenter:
                    tabItem = this.CreateTabItem(this.ScreenPersonalCenter, false);
                    break;
            }

            if (tabItem != null)
            {
                // Events
                (tabItem.Content as BaseScreen).BeforeLoading();
                if (lastScreen != null) lastScreen.BeforeUnLoading();

                if (insertIndex >= 0 && insertIndex < this.tabControl.Items.Count)
                {
                    this.tabControl.Items.Insert(insertIndex, tabItem);
                }
                else
                {
                    this.tabControl.Items.Add(tabItem);
                }
                this.tabControl.SelectedItem = tabItem;
                this.loadedScreens |= type;

                // Events
                if (lastScreen != null) lastScreen.AfterUnLoading();
                (tabItem.Content as BaseScreen).AfterLoading();
                lastScreen = (tabItem.Content as BaseScreen);
            }
        }

        public void Show(ScreenType type)
        {
            this.Show(type, -1);
        }

        public void Hide(ScreenType type)
        {
            if ((this.loadedScreens & type) != type)
            {
                return;
            }

            TabItem tabItem = this.GetItem(type);
            if (tabItem != null)
            {
                BaseScreen screenToHide = tabItem.Content as BaseScreen;
                // Events
                screenToHide.BeforeUnLoading();

                this.tabControl.Items.Remove(tabItem);
                tabItem.Content = null;
                this.loadedScreens &= ~type;

                // Events
                screenToHide.AfterUnLoading();
            }
        }

        public void Hide(BaseScreen baseScreen)
        {
            TabItem tabItem = this.GetItem(baseScreen);
            if (tabItem != null)
            {
                // Events
                baseScreen.BeforeUnLoading();

                this.tabControl.Items.Remove(tabItem);
                tabItem.Content = null;
                this.loadedScreens &= ~((ScreenType)baseScreen.BaseScreenType);

                // Events
                baseScreen.AfterUnLoading();
            }
        }

        public void HideAllExcept(ScreenType type)
        {
again:
            foreach (TabItem tabItem in this.tabControl.Items)
            {
                BaseScreen baseScreen = tabItem.Content as BaseScreen;
                if ((baseScreen.BaseScreenType & (int)type) != baseScreen.BaseScreenType)
                {
                    this.Hide(baseScreen);
                    goto again;
                }
            }
        }

        #endregion

        private TabItem GetItem(ScreenType type)
        {
            foreach (TabItem tabItem in this.tabControl.Items)
            {
                if (tabItem.Content == null)
                {
                    continue;
                }
                BaseScreen screen = tabItem.Content as BaseScreen;
                if (screen != null && (ScreenType)screen.BaseScreenType == type)
                {
                    return tabItem;
                }
            }
            return null;
        }

        private TabItem GetItem(BaseScreen baseScreen)
        {
            foreach (TabItem tabItem in this.tabControl.Items)
            {
                if (tabItem.Content == null)
                {
                    continue;
                }
                BaseScreen screen = tabItem.Content as BaseScreen;
                if (screen != null && screen.BaseScreenId.Equals(baseScreen.BaseScreenId))
                {
                    return tabItem;
                }
            }
            return null;
        }

        private int GetScreenIndex(ScreenType type)
        {
            int index = -1;
            foreach (TabItem tabItem in this.tabControl.Items)
            {
                index++;

                if (tabItem.Content == null)
                {
                    continue;
                }
                BaseScreen screen = tabItem.Content as BaseScreen;
                if (screen != null && (ScreenType)screen.BaseScreenType == type)
                {
                    return index;
                }
            }
            return -1;
        }

        private TabItem CreateTabItem(BaseScreen baseScreen, bool closeable)
        {
            TabItem tabItem = closeable ? new CloseableTabItem() : new TabItem();
            tabItem.Header = baseScreen.BaseScreenTitle;
            tabItem.Content = baseScreen;

            //为了隐藏掉选项卡
            tabItem.Width = 0;

            baseScreen.Width = Double.NaN;
            baseScreen.Height = Double.NaN;

            

            return tabItem;
        }
    }
}
