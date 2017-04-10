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
using BogheCore.Sip.Events;
using BogheCore;
using System.Threading;
using log4net;
using System.IO;
using System.Xml.Serialization;
using BogheCore.Generated.regingo;
using BogheCore.Model;
using BogheCore.Sip;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenRegistrations.xaml
    /// </summary>
    public partial class ScreenRegistrations : BaseScreen
    {
        MyObservableCollection<RegistrationInfo> registrations;

        public ScreenRegistrations()
        {
            InitializeComponent();
        }

        public MyObservableCollection<RegistrationInfo> Registrations
        {
            get { return this.registrations; }
            set
            {
                this.registrations = value;
                this.listBox.ItemsSource = value;
            }
        }
    }
}
