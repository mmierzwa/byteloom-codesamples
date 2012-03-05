using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using System.Windows.Browser;

namespace UploadClient
{
    public partial class MainPage : UserControl
    {
        public string siteUrl { set; get; }
        public string documentLibraryId { set; get; }
        public int maximumFileSize { set; get; }
        public bool isDisplayedInDialogBox { set; get; }

        public bool addAsNewVersion 
        { 
            get { return (bool)NewVersionCheckBox.IsChecked; } 
        }

        public string versionComments
        {
            get { return VersionCommentsTextBox.Text; }
        }
        
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            RadUploadControl.UploadStarted += new UploadStartedEventHandler(RadUpload_UploadStarted);
            RadUploadControl.FileUploaded += new FilesUploadedEventHandler(RadUploadControl_FileUploaded);
            RadUploadControl.UploadFinished += new RoutedEventHandler(RadUploadControl_UploadFinished);
        }

        private void RadUploadControl_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            var editFormUrl = e.HandlerData.CustomData["editFormUrl"] as string;
            
            if (string.IsNullOrEmpty(editFormUrl))
            {
                return;
            }

            if (isDisplayedInDialogBox)
            {
                editFormUrl = string.Format("{0}&IsDlg=1", editFormUrl);
            }

            HtmlPage.Window.Navigate(new Uri(editFormUrl));
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e) 
        {
            RadUploadControl.AdditionalPostFields.Add("siteUrl", siteUrl);
            RadUploadControl.AdditionalPostFields.Add("documentLibraryId", documentLibraryId);

            var maximumFileSizeInBytes = maximumFileSize * 1024 * 1024;
            RadUploadControl.MaxFileSize = maximumFileSizeInBytes;
            RadUploadControl.MaxUploadSize = maximumFileSizeInBytes;
        }

        private void RadUpload_UploadStarted(object sender, UploadStartedEventArgs args)
        {
            RadUploadControl.AdditionalPostFields.Add("addAsNewVersion", addAsNewVersion.ToString());
            RadUploadControl.AdditionalPostFields.Add("versionComments", versionComments);
        }

        private void RadUploadControl_UploadFinished(object sender, RoutedEventArgs args)
        {
            resetControls();
        }

        private void resetControls()
        {
            NewVersionCheckBox.IsChecked = false;
            VersionCommentsTextBox.Text = string.Empty;

            RadUploadControl.AdditionalPostFields.Remove("addAsNewVersion");
            RadUploadControl.AdditionalPostFields.Remove("versionComments");
        }
    }
}
