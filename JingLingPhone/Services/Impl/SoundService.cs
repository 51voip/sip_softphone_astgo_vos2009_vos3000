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
using BogheCore.Services;
using BogheCore.Services.Impl;
using System.Media;
using log4net;
using System.IO;
using System.Windows.Media;

namespace BogheApp.Services.Impl
{
    public partial class SoundService : ISoundService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(SoundService));

        private readonly ServiceManager manager;

        private SoundPlayer dtmfPlayer;
        private SoundPlayer ringTonePlayer;
        private SoundPlayer ringBackTonePlayer;
        private SoundPlayer eventPlayer;
        private SoundPlayer connPlayer;

        public SoundService(ServiceManager manager)
        {
            this.manager = manager;            
        }

        #region IService

        public bool Start()
        {
            try
            {
                this.dtmfPlayer = new SoundPlayer();
                this.ringTonePlayer = new SoundPlayer(Properties.Resources.ringtone);
                this.ringBackTonePlayer = new SoundPlayer(Properties.Resources.ringbacktone);
                this.eventPlayer = new SoundPlayer(Properties.Resources.newsms);
                this.connPlayer = new SoundPlayer(Properties.Resources.connevent);               

                return true;
            }
            catch (Exception e)
            {
                LOG.Error("Failed to start sound service", e);
                return false;
            }
        }

        public bool Stop()
        {
            this.dtmfPlayer.Stop();
            this.ringTonePlayer.Stop();
            this.ringBackTonePlayer.Stop();
            this.eventPlayer.Stop();
            this.connPlayer.Stop();

            return true;
        }

        #endregion

        #region ISoundService

        public void PlayDTMF(int number)
        {
            
        }

        public void StopDTMF()
        {
            
        }

        public void PlayRingTone()
        {
            this.ringTonePlayer.PlayLooping();
        }

        public void StopRingTone()
        {
            this.ringTonePlayer.Stop();
        }

        public void PlayRingBackTone()
        {
            this.ringBackTonePlayer.PlayLooping();
        }

        public void StopRingBackTone()
        {
            this.ringBackTonePlayer.Stop();
        }

        public void PlayNewEvent()
        {
            this.eventPlayer.Play();
        }

        public void StopNewEvent()
        {
            this.eventPlayer.Stop();
        }

        public void PlayConnectionChanged(bool connected)
        {
            this.connPlayer.Play();
        }

        public void StopConnectionChanged(bool connected)
        {
            this.connPlayer.Stop();
        }

        #endregion
    }
}
