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

namespace BogheApp.Screens
{
    partial class ScreenWatchers
    {
        public override String BaseScreenTitle
        {
            get { return "Authorizations"; }
        }

        public override int BaseScreenType
        {
            get { return (int)BogheApp.Screens.ScreenType.Authorizations; }
        }

        public override void BeforeLoading()
        {

        }

        public override void AfterLoading()
        {

        }

        public override void BeforeUnLoading()
        {

        }

        public override void AfterUnLoading()
        {

        }
    }
}
