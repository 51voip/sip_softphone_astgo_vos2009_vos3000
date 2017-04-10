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
using System.Windows.Input;
using BogheCore.Model;
using System.Windows;
using org.doubango.tinyWRAP;

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        void InitializeSecurity()
        {
            new String[] { 
                    Configuration.SRtpModeToString(tmedia_srtp_mode_t.tmedia_srtp_mode_none), 
                    Configuration.SRtpModeToString(tmedia_srtp_mode_t.tmedia_srtp_mode_optional),
                    Configuration.SRtpModeToString(tmedia_srtp_mode_t.tmedia_srtp_mode_mandatory) 
            }.ToList().ForEach(x => this.comboBoxSRTPModes.Items.Add(x)); 
        }

        private void LoadSecurity()
        {
            String srtpMode = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.SRTP_MODE, Configuration.DEFAULT_SECURITY_SRTP_MODE);
            this.textBoxTLSPrivateKey.Text = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.TLS_PRIV_KEY_FILE, Configuration.DEFAULT_TLS_PRIV_KEY_FILE);
            this.textBoxTLSPublicKey.Text = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.TLS_PUB_KEY_FILE, Configuration.DEFAULT_TLS_PUB_KEY_FILE);
            this.textBoxTLSCert.Text = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.TLS_CA_FILE, Configuration.DEFAULT_TLS_CA_FILE);

            this.checkBoxIPSecSecAgreeEnabled.IsChecked = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_SEC_AGREE, Configuration.DEFAULT_SECURITY_IPSEC_SEC_AGREE);
            this.comboBoxIPSecAlgorithm.SelectedValue = this.comboBoxIPSecAlgorithm.FindName(
                this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_ALGO, Configuration.DEFAULT_SECURITY_IPSEC_ALGO)
                .Replace("-", "_"));
            this.comboBoxIPSecEAlgorithm.SelectedValue = this.comboBoxIPSecEAlgorithm.FindName(
                this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_EALGO, Configuration.DEFAULT_SECURITY_IPSEC_EALGO)
                .Replace("-", "_"));
            this.comboBoxIPSecMode.SelectedValue = this.comboBoxIPSecMode.FindName(
                this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_MODE, Configuration.DEFAULT_SECURITY_IPSEC_MODE));
            this.comboBoxIPSecProtocol.SelectedValue = this.comboBoxIPSecProtocol.FindName(
                this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_PROTO, Configuration.DEFAULT_SECURITY_IPSEC_PROTO));
            this.comboBoxSRTPModes.SelectedValue = Configuration.SRtpModeToString((tmedia_srtp_mode_t)Enum.Parse(typeof(tmedia_srtp_mode_t), srtpMode));
        }

        private bool UpdateSecurity()
        {
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.TLS_PRIV_KEY_FILE, this.textBoxTLSPrivateKey.Text);
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.TLS_PUB_KEY_FILE, this.textBoxTLSPublicKey.Text);
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.TLS_CA_FILE, this.textBoxTLSCert.Text);

            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_SEC_AGREE, this.checkBoxIPSecSecAgreeEnabled.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_ALGO, (this.comboBoxIPSecAlgorithm.SelectedValue as System.Windows.Controls.ComboBoxItem).Tag.ToString());
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_EALGO, (this.comboBoxIPSecEAlgorithm.SelectedValue as System.Windows.Controls.ComboBoxItem).Tag.ToString());
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_MODE, (this.comboBoxIPSecMode.SelectedValue as System.Windows.Controls.ComboBoxItem).Tag.ToString());
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IPSEC_PROTO, (this.comboBoxIPSecProtocol.SelectedValue as System.Windows.Controls.ComboBoxItem).Tag.ToString());

            tmedia_srtp_mode_t srtpMode = Configuration.SRtpModeFromString(this.comboBoxSRTPModes.SelectedValue.ToString());
            this.configurationService.Set(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.SRTP_MODE, srtpMode.ToString());

            // Transmit values to the native part (global)
            MediaSessionMgr.defaultsSetSRtpMode(srtpMode);

            return true;
        }

        private void buttonTLS_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = false
            };
            if (fileDialog.ShowDialog() == true)
            {
                if (e.Source == this.buttonTlsPrivateKey)
                {
                    this.textBoxTLSPrivateKey.Text = fileDialog.FileName;
                }
                else if (e.Source == this.buttonTLSPublicKey)
                {
                    this.textBoxTLSPublicKey.Text = fileDialog.FileName;
                }
                else if (e.Source == this.buttonTLSCert)
                {
                    this.textBoxTLSCert.Text = fileDialog.FileName;
                }
            }
        }
    }
}
