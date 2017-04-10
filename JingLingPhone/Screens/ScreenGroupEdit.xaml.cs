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
using BogheCore.Services;
using BogheApp.Services;
using BogheApp.Services.Impl;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenGroupEdit.xaml
    /// </summary>
    public partial class ScreenGroupEdit : BaseScreen
    {
        private Group group;
        private bool editMode;

        private readonly IContactService contactService;
        private readonly IWin32ScreenService screenService;

        public ScreenGroupEdit(Group group)
        {
            InitializeComponent();

            if ((this.group = group) != null)
            {
                this.textBoxDisplayName.Text = this.group.DisplayName ?? this.group.DisplayName;
                this.labelTitle.Content = "编辑分组";
                this.editMode = true;
            }
            else
            {
                this.labelTitle.Content = "增加分组";
                this.editMode = false;
            }

            this.editMode = (group != null);
            this.contactService = Win32ServiceManager.SharedManager.ContactService;
            this.screenService = Win32ServiceManager.SharedManager.Win32ScreenService;
        }
        
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.group == null)
            {
                this.group = new Group();
                this.group.Authorization = BogheXdm.Authorization.Allowed;
            }

            this.group.DisplayName = this.textBoxDisplayName.Text;

            if (this.editMode)
            {
                this.contactService.GroupUpdate(this.group);
            }
            else
            {
                this.contactService.GroupAdd(this.group);
            }

            this.screenService.Show(ScreenType.Contacts);
            this.screenService.Hide(this);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.screenService.Show(ScreenType.Contacts);
            this.screenService.Hide(this);
        }
    }
}
