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
using System.Windows.Data;
using System.ComponentModel;
using BogheCore.Model;

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        List<Codec> codecs;

        private void InitializeCodecs()
        {
            codecs = new List<Codec>(new Codec[]{
                new Codec("G.722", "G.722 (16 KHz)", tdav_codec_id_t.tdav_codec_id_g722),
                new Codec("PCMA", "PCMA (8 KHz)", tdav_codec_id_t.tdav_codec_id_pcma),
                new Codec("PCMU", "PCMU (8 KHz)", tdav_codec_id_t.tdav_codec_id_pcmu),
            });
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_gsm))
            {
                codecs.Add(new Codec("GSM", "GSM (8 KHz)", tdav_codec_id_t.tdav_codec_id_gsm));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_amr_nb_oa))
            {
                codecs.Add(new Codec("AMR-NB-OA", "AMR Narrow Band Octet Aligned (8 KHz)", tdav_codec_id_t.tdav_codec_id_amr_nb_oa));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_amr_nb_be))
            {
                codecs.Add(new Codec("AMR-NB-BE", "AMR Narrow Band Bandwidth Efficient (8 KHz)", tdav_codec_id_t.tdav_codec_id_amr_nb_be));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_ilbc))
            {
                codecs.Add(new Codec("iLBC", "internet Low Bitrate Codec (8 KHz)", tdav_codec_id_t.tdav_codec_id_ilbc));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_speex_nb))
            {
                codecs.Add(new Codec("Speex-NB", "Speex (8 KHz)", tdav_codec_id_t.tdav_codec_id_speex_nb));
                codecs.Add(new Codec("Speex-WB", "Speex (16 KHz)", tdav_codec_id_t.tdav_codec_id_speex_wb));
                codecs.Add(new Codec("Speex-UWB", "Speex (32 KHz)", tdav_codec_id_t.tdav_codec_id_speex_uwb));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_g729ab))
            {
                codecs.Add(new Codec("G.729", "G729 Annex A/B (8 KHz)", tdav_codec_id_t.tdav_codec_id_g729ab));
            }


            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_h264_bp))
            {
                codecs.Add(new Codec("H264-BP", "H.264 Base Profile", tdav_codec_id_t.tdav_codec_id_h264_bp));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_h264_mp))
            {
                codecs.Add(new Codec("H264-MP", "H.264 Main Profile", tdav_codec_id_t.tdav_codec_id_h264_mp));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_vp8))
            {
                codecs.Add( new Codec("VP8", "Google's VP8", tdav_codec_id_t.tdav_codec_id_vp8));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_mp4ves_es))
            {
                codecs.Add( new Codec("MP4V-ES", "MPEG-4 Part 2", tdav_codec_id_t.tdav_codec_id_mp4ves_es));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_theora))
            {
                codecs.Add( new Codec("Theora", "Theora", tdav_codec_id_t.tdav_codec_id_theora));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_h263))
            {
                codecs.Add( new Codec("H.263", "H.263", tdav_codec_id_t.tdav_codec_id_h263));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_h263p))
            {
                codecs.Add( new Codec("H.263-1998", "H.263-1998", tdav_codec_id_t.tdav_codec_id_h263p));
            }
            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_h263pp))
            {
                codecs.Add( new Codec("H.263-2000", "H.263-2000", tdav_codec_id_t.tdav_codec_id_h263pp));
            }

            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_red))
            {
                codecs.Add(new Codec("RED", "Redundant data", tdav_codec_id_t.tdav_codec_id_red));
            }

            if (SipStack.isCodecSupported(tdav_codec_id_t.tdav_codec_id_t140))
            {
                codecs.Add(new Codec("T.140", "Realtime text", tdav_codec_id_t.tdav_codec_id_t140));
            }

            this.listBoxCodecs.ItemsSource = codecs;
            ICollectionView view = CollectionViewSource.GetDefaultView(this.listBoxCodecs.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("CodecType"));
        }

        private void LoadCodecs()
        {
            codecs.ForEach(x => x.IsEnabled = ((x.Id & (tdav_codec_id_t)this.sipService.Codecs) == x.Id));
        }        

        private bool UpdateCodecs()
        {
            tdav_codec_id_t codecIds = tdav_codec_id_t.tdav_codec_id_none;
            this.codecs.ForEach(x => codecIds |= x.IsEnabled ? x.Id : tdav_codec_id_t.tdav_codec_id_none);
            this.sipService.Codecs = (int)codecIds;

            this.configurationService.Set(Configuration.ConfFolder.MEDIA,
                        Configuration.ConfEntry.CODECS, this.sipService.Codecs);

            return true;
        }

        class Codec
        {
            readonly String name;
            readonly String description;
            readonly tdav_codec_id_t id;
            bool enabled;

            internal Codec(String name, String description, tdav_codec_id_t id)
            {
                this.name = name;
                this.description = description;
                this.id = id;
            }

            public String Name
            {
                get { return this.name; }
            }

            public String Description
            {
                get { return this.description; }
            }

            public tdav_codec_id_t Id
            {
                get { return this.id; }
            }

            public bool IsEnabled
            {
                get { return this.enabled; }
                set { this.enabled = value; }
            }

            public String CodecType
            {
                get
                {
                    switch (this.id)
                    {
                        case tdav_codec_id_t.tdav_codec_id_pcma:
                        case tdav_codec_id_t.tdav_codec_id_pcmu:
                        case tdav_codec_id_t.tdav_codec_id_gsm: 
                        case tdav_codec_id_t.tdav_codec_id_amr_nb_oa:
                        case tdav_codec_id_t.tdav_codec_id_amr_nb_be:
                        case tdav_codec_id_t.tdav_codec_id_ilbc:
                        case tdav_codec_id_t.tdav_codec_id_speex_nb:
                        case tdav_codec_id_t.tdav_codec_id_speex_wb:
                        case tdav_codec_id_t.tdav_codec_id_speex_uwb:
                        case tdav_codec_id_t.tdav_codec_id_g729ab:
                        case tdav_codec_id_t.tdav_codec_id_g722:
                            return "Audio Codecs";
                        case tdav_codec_id_t.tdav_codec_id_t140:
                        case tdav_codec_id_t.tdav_codec_id_red:
                            return "Other Codecs";
                        default:
                            return "Video Codecs";
                    }
                }
            }
        }
    }
}
