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
using BogheCore.Model;
using System.IO;
using BogheControls.Utils;
using System.Windows.Media;

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        private void LoadPresence()
        {
            this.checkBoxPresencePublish.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.PRESENCE_PUB, Configuration.DEFAULT_RCS_PRESENCE_PUB);
            this.checkBoxPresenceSubscribe.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.PRESENCE_SUB, Configuration.DEFAULT_RCS_PRESENCE_SUB);
            this.checkBoxPresenceRLS.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.RLS, Configuration.DEFAULT_RCS_RLS);
            this.textBoxPresenceFreeText.Text = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.FREE_TEXT, Configuration.DEFAULT_RCS_FREE_TEXT);
            this.textBoxPresenceHomePage.Text = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.HOME_PAGE, Configuration.DEFAULT_RCS_HOME_PAGE);
            this.textBoxHyperAvailabilityTimeout.Text = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.HYPERAVAILABILITY_TIMEOUT, Configuration.DEFAULT_RCS_HYPERAVAILABILITY_TIMEOUT).ToString();
            if (!String.IsNullOrEmpty(this.xcapService.Avatar))
            {
                if (!this.SetAvatarFromBase64String(this.xcapService.Avatar))
                {
                    this.AvatarFromLocalFile();
                }
            }
            else
            {
                this.AvatarFromLocalFile();
            }
        }

        private bool UpdatePresence()
        {
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.PRESENCE_PUB, this.checkBoxPresencePublish.IsChecked.HasValue ? this.checkBoxPresencePublish.IsChecked.Value : Configuration.DEFAULT_RCS_PRESENCE_PUB);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.PRESENCE_SUB, this.checkBoxPresenceSubscribe.IsChecked.HasValue ? this.checkBoxPresenceSubscribe.IsChecked.Value : Configuration.DEFAULT_RCS_PRESENCE_SUB);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.RLS, this.checkBoxPresenceRLS.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.FREE_TEXT, this.textBoxPresenceFreeText.Text);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.HOME_PAGE, this.textBoxPresenceHomePage.Text);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.HYPERAVAILABILITY_TIMEOUT, this.textBoxHyperAvailabilityTimeout.Text);

            return true;
        }

        private void AvatarFromLocalFile()
        {
            if (System.IO.File.Exists(MainWindow.AVATAR_PATH))
            {
                this.imageAvatar.Source = new ImageSourceConverter().ConvertFromInvariantString(MainWindow.AVATAR_PATH) as ImageSource;
            }
        }

        private bool SetAvatarFromBase64String(String b64)
        {
            System.Drawing.Image avatar;
            if (!String.IsNullOrEmpty(b64))
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(b64)))
                    {
                        avatar = System.Drawing.Bitmap.FromStream(stream);
                        this.imageAvatar.Source = MyImageConverter.FromBitmap(avatar as System.Drawing.Bitmap);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Failed to get avatar", ex);
                }
            }
            return false;
        }

        private static bool GetThumbnailImageAborted()
        {
            return false;
        }

        private System.Drawing.Image GetAvatarFromFilePath(String filePath, out System.Drawing.Imaging.ImageFormat rawFormat)
        {
            rawFormat = null;

            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(filePath))
                {
                    int width = image.Width;
                    int height = image.Height;

                    /* Must be 60x80 or 80x60 */
                    if (width >= 60 || height >= 60)
                    {

                        /* Must be 4/3 or 3/4 */
                        bool canResize = ((width * 3 / 4) == height) || ((height * 3 / 4) == width);
                        if (canResize)
                        {
                            System.Drawing.Image thumb = width > height
                                                     ? image.GetThumbnailImage(80, 60, GetThumbnailImageAborted, IntPtr.Zero)
                                                     : image.GetThumbnailImage(60, 80, GetThumbnailImageAborted, IntPtr.Zero);
                            rawFormat = image.RawFormat;
                            return thumb;
                        }
                        else
                        {
                            LOG.Error("Invalid ratio");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Failed to resize avatar", ex);
            }

            return null;
        }

        private String GetAvatarBase64FromFilePath(String filePath, out String mimeType)
        {
            mimeType = null;

            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return String.Empty;
            }
            
            String b64 = String.Empty;

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    System.Drawing.Imaging.ImageFormat rawFormat;
                    System.Drawing.Image avatar; 
                    if ((avatar = this.GetAvatarFromFilePath(filePath, out rawFormat)) != null)
                    {
                        avatar.Save(stream, rawFormat);
                        b64 = Convert.ToBase64String(stream.GetBuffer());
                        mimeType = this.GetAvatarMimeType(rawFormat);
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Failed to resize avatar", ex);
            }

            return b64;
        }

        private String GetAvatarMimeType(System.Drawing.Imaging.ImageFormat imageFormat)
        {
            if (imageFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid || imageFormat.Guid == System.Drawing.Imaging.ImageFormat.MemoryBmp.Guid)
            {
                return "image/bmp";
            }
            else if (imageFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                return "image/gif";
            }
            else if (imageFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
            {
                return "image/jpeg";
            }
            else if (imageFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
            {
                return "image/png";
            }
            else if (imageFormat.Guid == System.Drawing.Imaging.ImageFormat.Tiff.Guid)
            {
                return "image/tiff";
            }
            else return null;
        }
    }
}
