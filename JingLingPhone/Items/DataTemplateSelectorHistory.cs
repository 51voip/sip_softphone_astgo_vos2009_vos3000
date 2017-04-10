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
using BogheCore.Model;
using System.Windows.Controls;

namespace BogheApp.Items
{
    internal class DataTemplateSelectorHistory : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = null;
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                if (item is HistoryAVCallEvent)
                {
                    template = element.FindResource("DataTemplateHistoryAVCallEvent") as DataTemplate;
                }
                else if (item is HistoryChatEvent)
                {
                    template = element.FindResource("DataTemplateHistoryChatEvent") as DataTemplate;
                }
                else if (item is HistoryShortMessageEvent)
                {
                    template = element.FindResource("DataTemplateHistoryShortMessageEvent") as DataTemplate;
                }
                else if (item is HistoryFileTransferEvent)
                {
                    template = element.FindResource("DataTemplateHistoryFileTransferEvent") as DataTemplate;
                }
            }

            return template;
        }
    }
}
