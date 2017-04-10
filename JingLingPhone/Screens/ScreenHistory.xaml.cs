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
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheApp.Items;
using BogheCore;
using BogheCore.Model;
using System.ComponentModel;
using BogheCore.Events;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenHistory.xaml
    /// </summary>
    public partial class ScreenHistory : BaseScreen
    {
        private readonly IHistoryService historyService;
        private ICollectionView historyView;

        private HistoryEvent.StatusType statusToDisplay = HistoryEvent.StatusType.All;
        private MediaType mediaTypeToDisplay = MediaType.All;

        private readonly List<FilterItem> filterItems;

        public ScreenHistory()
        {
            InitializeComponent();

            this.historyService = Win32ServiceManager.SharedManager.HistoryService;

            this.historyService.onHistoryEvent += this.historyService_onHistoryEvent;

            this.filterItems = new List<FilterItem>(new FilterItem[]
            {
                new FilterItem("所有事件", "/BogheApp;component/embedded/16/date_time_16.png", HistoryEvent.StatusType.All, MediaType.All),
                new FilterItem("所有通话记录", "/BogheApp;component/embedded/16/call_16.png", HistoryEvent.StatusType.All, MediaType.AudioVideo),
                new FilterItem("拨出通话", "/BogheApp;component/embedded/16/call_outgoing_16.png", HistoryEvent.StatusType.Outgoing, MediaType.AudioVideo),
                new FilterItem("呼入通话", "/BogheApp;component/embedded/16/call_incoming_16.png", HistoryEvent.StatusType.Incoming, MediaType.AudioVideo),
                //new FilterItem("Missed Calls", "/BogheApp;component/embedded/16/call_missed_16.png", HistoryEvent.StatusType.Missed, MediaType.AudioVideo),
                //new FilterItem("Messaging", "/BogheApp;component/embedded/16/messages_16.png", HistoryEvent.StatusType.All, MediaType.Messaging),
                //new FilterItem("File Transfers", "/BogheApp;component/embedded/16/document_up_down_16.png", HistoryEvent.StatusType.All, MediaType.FileTransfer),
            });

            this.listBox.ItemTemplateSelector = new DataTemplateSelectorHistory();
            this.UpdateSource();

            this.comboBoxFilterCriteria.ItemsSource = this.filterItems;
            this.comboBoxFilterCriteria.SelectedIndex = 0;

            this.historyView.Filter = delegate(object @event)
            {
                HistoryEvent hEvent = @event as HistoryEvent;
                if (hEvent == null)
                {
                    return false;
                }
                if (((hEvent.Status & this.statusToDisplay) == hEvent.Status) &&
                    ((hEvent.MediaType & this.mediaTypeToDisplay) == hEvent.MediaType))
                {
                    return hEvent.DisplayName.StartsWith(this.textBoxSearchCriteria.Text, StringComparison.InvariantCultureIgnoreCase);
                }
                return false;
            };
        }

        public void GetThjl()
        {
            //获取通话记录











        }



        public void Refresh()
        {
            this.historyView.Refresh();
        }

        private void historyService_onHistoryEvent(object sender, HistoryEventArgs e)
        {
            if (e.Type != HistoryEventTypes.RESET)
            {
                return;
            }

            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<HistoryEventArgs>(this.historyService_onHistoryEvent), sender, new object[] { e });
                return;
            }

           this.UpdateSource();
        }

        private void UpdateSource()
        {
            this.listBox.ItemsSource = this.historyService.Events;
            this.historyView = CollectionViewSource.GetDefaultView(this.listBox.ItemsSource);
        }
        
        // OnSelctionChangedEvent will apply filter
        public void Select(HistoryEvent.StatusType statusToDisplay, MediaType mediaTypeToDisplay)
        {
            int index = this.filterItems.FindIndex(x => x.Media == mediaTypeToDisplay && x.Status == statusToDisplay);
            if (index != -1)
            {
                this.comboBoxFilterCriteria.SelectedIndex = index;
            }
        }

        private void Filter(HistoryEvent.StatusType statusToDisplay, MediaType mediaTypeToDisplay)
        {
            this.statusToDisplay = statusToDisplay;
            this.mediaTypeToDisplay = mediaTypeToDisplay;
            this.historyView.Refresh();
        }


        private void BaseScreen_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            

            this.Cursor = Cursors.Arrow;
        }

        private void comboBoxFilterCriteria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterItem item = this.comboBoxFilterCriteria.SelectedItem as FilterItem;
            if (item != null)
            {
                this.Filter(item.Status, item.Media);
            }
        }

        private void textBoxSearchCriteria_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.historyView.Refresh();
        }

        private void imageSearchCriteriaClear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.textBoxSearchCriteria.Text = String.Empty;
        }


        #region FilterItem

        class FilterItem
        {
            readonly String text;
            readonly String imageSource;
            readonly HistoryEvent.StatusType status;
            readonly MediaType media;

            public FilterItem(String text, String imageSource, HistoryEvent.StatusType status, MediaType media)
            {
                this.text = text;
                this.imageSource = imageSource;
                this.status = status;
                this.media = media;
            }

            public String Text
            {
                get { return this.text; }
            }

            public String ImageSource
            {
                get { return this.imageSource; }
            }

            public HistoryEvent.StatusType Status
            {
                get { return this.status; }
            }

            public MediaType Media
            {
                get { return this.media; }
            }
        }

        #endregion
    }
}
