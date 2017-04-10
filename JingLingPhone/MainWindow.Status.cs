using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BogheCore;

namespace BogheApp
{
    partial class MainWindow
    {
        public class Status
        {
            readonly string text;
            readonly PresenceStatus value;
            readonly string imageSource;

            public Status(string text, PresenceStatus value, String imageSource)
            {
                this.text = text;
                this.value = value;
                this.imageSource = imageSource;
            }

            public String Text
            {
                get { return this.text; }
            }

            public PresenceStatus Value
            {
                get { return this.value; }
            }

            public String ImageSource
            {
                get { return this.imageSource; }
            }
        }
    }
}
