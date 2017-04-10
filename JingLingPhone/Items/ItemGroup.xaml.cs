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
using BogheControls.Utils;
using BogheApp.Services.Impl;
using BogheApp.Screens;
using BogheCore.Services;
using BogheApp.Services;

namespace BogheApp.Items
{
    /// <summary>
    /// Interaction logic for ItemGroup.xaml
    /// </summary>
    public partial class ItemGroup : BaseItem<String>
    {
        private Group group;

        public ItemGroup()
        {
            InitializeComponent();
            this.ValueLoaded += ItemGroup_ValueLoaded;
        }

        private void ItemGroup_ValueLoaded(object sender, EventArgs e)
        {
            this.group = Win32ServiceManager.SharedManager.ContactService.GroupFind(this.Value);
            if (group == null)
            {
                return;
            }

            this.labelDisplayName.Content = group.DisplayName;
            switch (group.Authorization)
            {
                case BogheXdm.Authorization.Allowed:
                default:
                    this.GridGradienStop.Color = Colors.Green;
                    break;
                case BogheXdm.Authorization.Blocked:
                case BogheXdm.Authorization.PoliteBlocked:
                    this.GridGradienStop.Color = Colors.Red;
                    break;

                case BogheXdm.Authorization.Revoked:
                    this.GridGradienStop.Color = Colors.Salmon;
                    break;
            }
        }

        private void ctxMenu_AddContact_Click(object sender, RoutedEventArgs e)
        {
            ScreenContactEdit screenEditContact = new ScreenContactEdit(null, this.group);
            Win32ServiceManager.SharedManager.Win32ScreenService.Show(screenEditContact);
        }

        private void ctxMenu_EditGroup_Click(object sender, RoutedEventArgs e)
        {
            ScreenGroupEdit screenGroupEdit = new ScreenGroupEdit(this.group);
            Win32ServiceManager.SharedManager.Win32ScreenService.Show(screenGroupEdit);
        }

        private void ctxMenu_DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            Win32ServiceManager.SharedManager.ContactService.GroupDelete(this.group);
        }
    }
}
