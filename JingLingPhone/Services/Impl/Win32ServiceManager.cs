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
using BogheCore.Services.Impl;
using BogheCore.Services;
using log4net;
using BogheCore.Utils;
using System.IO;
using System.Windows.Forms;
using org.doubango.tinyWRAP;
using BogheCore.Model;

namespace BogheApp.Services.Impl
{
    class Win32ServiceManager : ServiceManager
    {
        private static ILog LOG = LogManager.GetLogger(typeof(Win32ServiceManager));

        private static Win32ServiceManager singleton = null;
        private static Boolean initialized = false;
        private const String MULI_INSTANCE_FILE = "./.multiinstance";
        private readonly bool multiInstance;
        private String applicationDataPath;

        private IWin32ScreenService screenService;
        private ILogService logService;
        private IConfigurationService configurationService;
        private ISipService sipService;
        private IContactService contactService;
        private IXcapService xcapService;
        private IHistoryService historyService;
        private ISoundService soundService;
        private IStateMonitorService stateMonitorService;

        /// <summary>
        /// Shared Service manager
        /// </summary>
        public static Win32ServiceManager SharedManager
        {
            get
            {
                if (Win32ServiceManager.singleton == null)
                {
                    Win32ServiceManager.singleton = new Win32ServiceManager();
                    UriUtils.ServiceManager = Win32ServiceManager.singleton;
                }
                return Win32ServiceManager.singleton;
            }
        }

        public Win32ServiceManager()
        {
            this.multiInstance = System.IO.File.Exists(Win32ServiceManager.MULI_INSTANCE_FILE);
            if (!initialized)
            {
                SipStack.initialize();
                initialized = true;
            }
        }

        /// <summary>
        /// Starts the manager
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool ret = true;

            LOG.Debug("Start Service Manager");


            ret &= this.LogService.Start();
            ret &= this.ConfigurationService.Start();

            ret &= this.Win32ScreenService.Start();
            ret &= this.SipService.Start();
            ret &= this.XcapService.Start();
            ret &= this.ContactService.Start();
            ret &= this.HistoryService.Start();
            ret &= this.SoundService.Start();
            ret &= this.StateMonitorService.Start();

            // Register plugins
            // MediaSessionMgr.registerAudioPluginFromFile("audio_webrtc.dll");

            // Set user preferences (global defaults)
            // could be changed per session
            MediaSessionMgr.defaultsSetAgcEnabled(true);
            MediaSessionMgr.defaultsSetEchoSuppEnabled(true);
            MediaSessionMgr.defaultsSetEchoTail(500);
            MediaSessionMgr.defaultsSetEchoSkew(0);
            MediaSessionMgr.defaultsSetNoiseSuppEnabled(true);
            MediaSessionMgr.defaultsSetVadEnabled(false);
            MediaSessionMgr.defaultsSetJbMaxLateRate(1);
            MediaSessionMgr.defaultsSetRtcpEnabled(true);
            MediaSessionMgr.defaultsSetRtcpMuxEnabled(true);

            MediaSessionMgr.defaultsSetVolume(this.ConfigurationService.Get(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.AUDIO_VOLUME, Configuration.DEFAULT_GENERAL_AUDIO_VOLUME));
            if (this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS, Configuration.DEFAULT_QOS_SESSION_TIMERS))
            {
                MediaSessionMgr.defaultsSetInviteSessionTimers(
                    this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS_TIMEOUT, Configuration.DEFAULT_QOS_SESSION_TIMERS_TIMEOUT),
                    this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS_REFRESHER, Configuration.DEFAULT_QOS_SESSION_TIMERS_REFRESHER));
            }
            else 
            {
                MediaSessionMgr.defaultsSetInviteSessionTimers(0, null);
            }
            MediaSessionMgr.defaultsSetPrefVideoSize(
                (tmedia_pref_video_size_t)Enum.Parse(typeof(tmedia_pref_video_size_t), this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PREF_VIDEO_SIZE, Configuration.DEFAULT_QOS_PREF_VIDEO_SIZE), true)
                );

            MediaSessionMgr.defaultsSetSRtpMode(
                (tmedia_srtp_mode_t)Enum.Parse(typeof(tmedia_srtp_mode_t), this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.SRTP_MODE, Configuration.DEFAULT_SECURITY_SRTP_MODE), true)
                );

            MediaSessionMgr.defaultsSetProfile(
                (tmedia_profile_t)Enum.Parse(typeof(tmedia_profile_t), this.configurationService.Get(Configuration.ConfFolder.MEDIA, Configuration.ConfEntry.PROFILE, Configuration.DEFAULT_MEDIA_PROFILE), true)
                );

            MediaSessionMgr.defaultsSetIceEnabled(
                this.configurationService.Get(Configuration.ConfFolder.NATT, Configuration.ConfEntry.USE_ICE, Configuration.DEFAULT_NATT_USE_ICE)
                );
            return ret;
        }

        /// <summary>
        /// Stops the manager
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool ret = true;

            LOG.Debug("Stop Service Manager");

            // Log service should be last to stop

            ret &= this.Win32ScreenService.Stop();
            ret &= this.SipService.Stop();
            ret &= this.XcapService.Stop();
            ret &= this.ContactService.Stop();
            ret &= this.HistoryService.Stop();
            ret &= this.SoundService.Stop();
            ret &= this.StateMonitorService.Stop();

            ret &= this.ConfigurationService.Stop();
            ret &= this.LogService.Stop();

            return ret;
        }

        public override String ApplicationDataPath
        {
            get
            {
                if (this.applicationDataPath == null)
                {
                    if (this.multiInstance)
                    {
                        this.applicationDataPath = Path.Combine(Application.StartupPath, "User");
                    }
                    else
                    {
                        String applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        this.applicationDataPath = Path.Combine(applicationData, "Doubango\\Boghe IMS Client");
                    }
                    Directory.CreateDirectory(this.applicationDataPath);
                }
                return this.applicationDataPath;
            }
        }

        public override bool IsMultiInstanceEnabled
        {
            get
            {
                return this.multiInstance;
            }
        }

        public override String BuildStoragePath(String folder)
        {
            return Path.Combine(this.ApplicationDataPath, folder);
        }

        

        /// <summary>
        /// Screen Service
        /// </summary>
        public IWin32ScreenService Win32ScreenService
        {
            get
            {
                if (this.screenService == null)
                {
                    this.screenService = new ScreenService();
                }
                return this.screenService;
            }
        }

        /// <summary>
        /// Log Service
        /// </summary>
        public ILogService LogService
        {
            get
            {
                if (this.logService == null)
                {
                    this.logService = new LogService();
                }
                return this.logService;
            }
        }

        #region ServiceManager

        public override IConfigurationService ConfigurationService
        {
            get
            {
                if (this.configurationService == null)
                {
#if REG_CONF
                    this.configurationService = new RegConfigurationService();
#else
                    this.configurationService = new XmlConfigurationService(this);
#endif
                }
                return this.configurationService;
            }
        }

        public override IScreenService ScreenService
        {
            get
            {
                return this.Win32ScreenService;
            }
        }

        public override ISipService SipService
        {
            get
            {
                if (this.sipService == null)
                {
                    this.sipService = new SipService(this);
                }
                return this.sipService;
            }
        }

        public override IContactService ContactService
        {
            get
            {
                if (this.contactService == null)
                {
                    this.contactService = new ContactService(this);
                }
                return this.contactService;
            }
        }

        public override IXcapService XcapService
        {
            get
            {
                if (this.xcapService == null)
                {
                    this.xcapService = new XcapService(this);
                }
                return this.xcapService;
            }
        }

        public override IHistoryService HistoryService
        {
            get
            {
                if (this.historyService == null)
                {
                    this.historyService = new HistoryService(this);
                }
                return this.historyService;
            }
        }

        public override ISoundService SoundService
        {
            get
            {
                if (this.soundService == null)
                {
                    this.soundService = new SoundService(this);
                }
                return this.soundService;
            }
        }

        public override IStateMonitorService StateMonitorService
        {
            get
            {
                if (this.stateMonitorService == null)
                {
                    this.stateMonitorService = new StateMonitorService();
                }
                return this.stateMonitorService;
            }
        }

        public override System.Windows.Threading.Dispatcher Dispatcher
        {
            get
            {
                return App.Current.Dispatcher;
            }
        }

        #endregion
    }
}
