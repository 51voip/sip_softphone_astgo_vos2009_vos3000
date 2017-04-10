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
using org.doubango.tinyWRAP;
using BogheCore.Model;

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        PrefVideoSize[] PrefVideoSizeValues = new PrefVideoSize[]
            {
                new PrefVideoSize("SQCIF (128 x 98)", tmedia_pref_video_size_t.tmedia_pref_video_size_sqcif),
                new PrefVideoSize("QCIF (176 x 144)", tmedia_pref_video_size_t.tmedia_pref_video_size_qcif),
                new PrefVideoSize("QVGA (320 x 240)", tmedia_pref_video_size_t.tmedia_pref_video_size_qvga),
                new PrefVideoSize("CIF (352 x 288)", tmedia_pref_video_size_t.tmedia_pref_video_size_cif),
                new PrefVideoSize("HVGA (480 x 320)", tmedia_pref_video_size_t.tmedia_pref_video_size_hvga),
                new PrefVideoSize("VGA (640 x 480)", tmedia_pref_video_size_t.tmedia_pref_video_size_vga),
                new PrefVideoSize("4CIF (704 x 576)", tmedia_pref_video_size_t.tmedia_pref_video_size_4cif),
                new PrefVideoSize("SVGA (800 x 600)", tmedia_pref_video_size_t.tmedia_pref_video_size_svga),
                new PrefVideoSize("480P (852 x 480)", tmedia_pref_video_size_t.tmedia_pref_video_size_480p),
                new PrefVideoSize("720P (1280 x 720)", tmedia_pref_video_size_t.tmedia_pref_video_size_720p),
                new PrefVideoSize("16CIF (1408 x 1152)", tmedia_pref_video_size_t.tmedia_pref_video_size_16cif),
                new PrefVideoSize("1080P (1920 x 1080)", tmedia_pref_video_size_t.tmedia_pref_video_size_1080p)
            };

        void InitializeQoS()
        {
            new String[] { 
                    Configuration.QoSStrengthToString(tmedia_qos_strength_t.tmedia_qos_strength_none), 
                    Configuration.QoSStrengthToString(tmedia_qos_strength_t.tmedia_qos_strength_optional),
                    Configuration.QoSStrengthToString(tmedia_qos_strength_t.tmedia_qos_strength_mandatory) 
            }.ToList().ForEach(x => this.comboBoxPreconditionStrength.Items.Add(x));
            new String[]{
                Configuration.QoSTypeToString(tmedia_qos_stype_t.tmedia_qos_stype_none),
                Configuration.QoSTypeToString(tmedia_qos_stype_t.tmedia_qos_stype_segmented),
                Configuration.QoSTypeToString(tmedia_qos_stype_t.tmedia_qos_stype_e2e),
                
            }.ToList().ForEach(x => this.comboBoxPreconditionType.Items.Add(x));
            new String[]{
                Configuration.QoSBandwidthToString(tmedia_bandwidth_level_t.tmedia_bl_low),
                Configuration.QoSBandwidthToString(tmedia_bandwidth_level_t.tmedia_bl_medium),
                Configuration.QoSBandwidthToString(tmedia_bandwidth_level_t.tmedia_bl_hight)
            }.ToList().ForEach(x => this.comboBoxPreconditionBandwidth.Items.Add(x));

            this.comboBoxSessionTimerRefreser.Items.Add("none");
            this.comboBoxSessionTimerRefreser.Items.Add("uac");
            this.comboBoxSessionTimerRefreser.Items.Add("uas");

            this.comboBoxPrefVideoSize.ItemsSource = PrefVideoSizeValues;
        }

        void LoadQoS()
        {
            tmedia_pref_video_size_t prefVideoSize = (tmedia_pref_video_size_t)Enum.Parse(typeof(tmedia_pref_video_size_t),
                this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PREF_VIDEO_SIZE, Configuration.DEFAULT_QOS_PREF_VIDEO_SIZE));
            int prefVideoSizeIndex = PrefVideoSizeValues.ToList().FindIndex(x => x.Value == prefVideoSize);
            String strength = this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PRECOND_STRENGTH, Configuration.DEFAULT_QOS_PRECOND_STRENGTH);
            String type = this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PRECOND_TYPE, Configuration.DEFAULT_QOS_PRECOND_TYPE);
            String bandwidth = this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.BANDWIDTH, Configuration.DEFAULT_QOS_BANDWIDTH);
            String refresher = this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS_REFRESHER, Configuration.DEFAULT_QOS_SESSION_TIMERS_REFRESHER);
            int timeout = this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS_TIMEOUT, Configuration.DEFAULT_QOS_SESSION_TIMERS_TIMEOUT);

            this.comboBoxPrefVideoSize.SelectedIndex = Math.Max(0, prefVideoSizeIndex);
            this.checkBoxSessionTimersEnable.IsChecked = this.configurationService.Get(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS, Configuration.DEFAULT_QOS_SESSION_TIMERS);
            this.comboBoxPreconditionStrength.SelectedValue = Configuration.QoSStrengthToString((tmedia_qos_strength_t)Enum.Parse(typeof(tmedia_qos_strength_t), strength));
            this.comboBoxPreconditionType.SelectedValue = Configuration.QoSTypeToString((tmedia_qos_stype_t)Enum.Parse(typeof(tmedia_qos_stype_t), type));
            this.comboBoxPreconditionBandwidth.SelectedValue = Configuration.QoSBandwidthToString((tmedia_bandwidth_level_t)Enum.Parse(typeof(tmedia_bandwidth_level_t), bandwidth));
            this.comboBoxSessionTimerRefreser.SelectedValue = refresher;
            this.textBoxSessionTimersTimeout.Text = timeout.ToString();
        }

        bool UpdateQoS()
        {
            tmedia_pref_video_size_t prefVideoSize = (this.comboBoxPrefVideoSize.SelectedValue as PrefVideoSize).Value;
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PREF_VIDEO_SIZE, prefVideoSize.ToString());
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS, 
                this.checkBoxSessionTimersEnable.IsChecked.Value);            
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS_REFRESHER, this.comboBoxSessionTimerRefreser.SelectedValue.ToString());
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.SESSION_TIMERS_TIMEOUT, this.textBoxSessionTimersTimeout.Text);
            
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PRECOND_STRENGTH,
                Configuration.QoSStrengthFromString(this.comboBoxPreconditionStrength.SelectedValue.ToString()).ToString());
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.PRECOND_TYPE,
                Configuration.QoSTypeFromString(this.comboBoxPreconditionType.SelectedValue.ToString()).ToString());
            this.configurationService.Set(Configuration.ConfFolder.QOS, Configuration.ConfEntry.BANDWIDTH,
                Configuration.QoSBandwidthFromString(this.comboBoxPreconditionBandwidth.SelectedValue.ToString()).ToString());


            // Transmit values to the native part (global)
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
            MediaSessionMgr.defaultsSetPrefVideoSize(prefVideoSize);

            return true;
        }


        public class PrefVideoSize
        {
            readonly String text;
            readonly tmedia_pref_video_size_t value;

            public PrefVideoSize(String text, tmedia_pref_video_size_t value)
            {
                this.text = text;
                this.value = value;
            }

            public String Text
            {
                get { return this.text; }
            }

            public tmedia_pref_video_size_t Value
            {
                get { return this.value; }
            }
        }
    }
}
