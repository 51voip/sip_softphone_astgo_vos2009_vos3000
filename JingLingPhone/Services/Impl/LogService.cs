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
using log4net;

namespace BogheApp.Services.Impl
{
    class LogService : ILogService
    {
        private ILog log;

        
        #region IService

        /// <summary>
        /// Starts the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.log = LogManager.GetLogger(typeof(LogService));
            if (log == null)
            {
                return false;
            }
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

        #region ILogService

        public void Debug(String TAG, object message)
        {
            if (this.log != null)
            {
                this.log.Debug(String.Format("{0}: {1}", TAG, message));
            }
        }

        public void Info(String TAG, object message)
        {
            if (this.log != null)
            {
                this.log.Info(String.Format("{0}: {1}", TAG, message));
            }
        }

        public void Warn(String TAG, object message)
        {
            if (this.log != null)
            {
                this.log.Warn(String.Format("{0}: {1}", TAG, message));
            }
        }

        public void Error(String TAG, object message)
        {
            if (this.log != null)
            {
                this.log.Error(String.Format("{0}: {1}", TAG, message));
            }
        }

        public void Fatal(String TAG, object message)
        {
            if (this.log != null)
            {
                this.log.Fatal(String.Format("{0}: {1}", TAG, message));
            }
        }

        #endregion
    }
}
