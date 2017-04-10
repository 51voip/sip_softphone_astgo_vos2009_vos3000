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
using System.Windows.Shapes;
using BogheCore.Utils;

namespace BogheApp
{
    /// <summary>
    /// Interaction logic for CallTransferWindow.xaml
    /// </summary>
    public partial class CallTransferWindow : Window
    {
        private String transferUri = null;

        public CallTransferWindow()
        {
            InitializeComponent();
        }

        public String TransferUri
        {
            get
            {
                return transferUri;
            }
        }

        private void buttonTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.textBoxNumber.Text))
            {
                this.transferUri = UriUtils.GetValidSipUri(this.textBoxNumber.Text);
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
