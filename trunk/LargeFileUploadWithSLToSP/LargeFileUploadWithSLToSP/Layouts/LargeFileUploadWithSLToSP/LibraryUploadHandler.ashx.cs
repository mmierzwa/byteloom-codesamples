using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows;
using System.Web;
using Microsoft.SharePoint;
using System.IO;
using Microsoft.SharePoint.Utilities;

namespace LargeFileUploadWithSLToSP
{
    public class LibraryUploadHandler : RadUploadHandler
    {
        private const string fileNameFormVariableName = "0_RadUAG_fileName";
        private const string chunksTempFolderPath = @"C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\14\Template\Layouts\LargeFileUploadWithSLToSP\TempChunks";

        private string spSiteUrl
        {
            get { return GetQueryParameter("siteUrl"); }
        }

        private string fileName
        {
            get { return Request.Form[fileNameFormVariableName]; }
        }

        private Guid documentLibraryId
        {
            get 
            { 
                string documentLibraryId = GetQueryParameter("documentLibraryId");
                Guid documentLibraryGuid;
                if (!tryParseGuid(documentLibraryId, out documentLibraryGuid))
                {
                    throw new SPException("Mandatory parameter \"documentLibraryId\" is missing or has invalid format");
                }
                
                return documentLibraryGuid;
            }
        }

        private bool addAsNewVersion
        {
            get 
            {
                bool addAsNew;
                if (!bool.TryParse(GetQueryParameter("addAsNewVersion"), out addAsNew))
                {
                    throw new SPException("Mandatory parameter \"addAsNewVersion\" is missing or has invalid format");
                }

                return addAsNew;
            }
        }

        private string versionComments
        {
            get { return GetQueryParameter("versionComments"); }
        }

        private SPDocumentLibrary documentLibrary;
        private SPFolder targetFolder;

        public override Dictionary<string, object> GetAssociatedData()
        {
            Dictionary<string, object> associatedData = base.GetAssociatedData();

            if (IsFinalFileRequest())
            {
                var uploadedFile = uploadFileToDocumentLibrary();

                // add soruce url taken from upload page in order to pass it to edit form and return
                // to original view after metadata edit
                var editFormUrl = getEditFormUrl(documentLibrary, uploadedFile, versionComments, targetFolder);

                associatedData.Add("editFormUrl", editFormUrl);
            }

            return associatedData;
        }

        private SPFile uploadFileToDocumentLibrary()
        {
            SPFile uploadedFile = null;
            string filePath = string.Format("{0}\\{1}", chunksTempFolderPath, fileName);

            try
            {
                using (var siteCollection = new SPSite(spSiteUrl))
                {
                    var webSite = siteCollection.RootWeb;

                    webSite.AllowUnsafeUpdates = true;

                    documentLibrary = getTargetDocumentLibrary(webSite);
                    // change folder path if other folder than root is required:
                    targetFolder = getTargetFolder(documentLibrary, string.Empty);

                    uploadedFile = uploadFileToDocumentLibrary(filePath);

                    webSite.AllowUnsafeUpdates = false;
                }
            }
            finally
            {
                File.Delete(filePath);
            }

            return uploadedFile;
        }

        private SPFile uploadFileToDocumentLibrary(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                SPFileCollectionAddParameters uploadParams = new SPFileCollectionAddParameters()
                {
                    Overwrite = addAsNewVersion,
                    CheckInComment = versionComments,
                    CheckRequiredFields = true,
                    AutoCheckoutOnInvalidData = true
                };

                return targetFolder.Files.Add(fileName, fileStream, uploadParams);
            }
        }

        private SPDocumentLibrary getTargetDocumentLibrary(SPWeb webSite)
        {
            SPDocumentLibrary targetDocumentLibrary = webSite.Lists[documentLibraryId] as SPDocumentLibrary;

            if (targetDocumentLibrary == null)
            {
                throw new SPException(string.Format("Document library {0} does not exist or list is not a document library", documentLibraryId));
            }

            return targetDocumentLibrary;
        }

        private SPFolder getTargetFolder(SPDocumentLibrary documentLibrary, string folderPath)
        {
            string targetFolderUrl = string.Format("{0}/{1}", documentLibrary.RootFolder.ServerRelativeUrl, folderPath);
            return documentLibrary.ParentWeb.GetFolder(targetFolderUrl);
        }

        private static bool tryParseGuid(string guidStr, out Guid guid)
        {
            try
            {
                guid = new Guid(guidStr);
                return true;
            }
            catch
            {
                guid = Guid.Empty;
                return false;
            }
        }

        private string getEditFormUrl(SPDocumentLibrary documentLibrary, SPFile uploadedFile, string checkInComment, SPFolder rootFolder)
        {
            string defaultSourceUrl = getDefaultViewUrl(documentLibrary);
            return getEditFormUrl(documentLibrary, uploadedFile, checkInComment, rootFolder, defaultSourceUrl);
        }

        private string getEditFormUrl(SPDocumentLibrary documentLibrary, SPFile uploadedFile, string checkInComment, SPFolder rootFolder, string sourceUrl)
        {
            StringBuilder url = new StringBuilder();

            url.Append(getEditFormUrl(documentLibrary));
            url.Append("?Mode=Upload");
            appendParameter(url, "CheckInComment", checkInComment);
            appendParameter(url, "ID", uploadedFile.Item.ID.ToString());
            appendParameter(url, "RootFolder", rootFolder.ServerRelativeUrl);
            appendParameter(url, "Source", sourceUrl);

            return url.ToString();
        }

        private string getEditFormUrl(SPDocumentLibrary documentLibrary)
        {
            return getFormUrl(documentLibrary, PAGETYPE.PAGE_EDITFORM);
        }

        private string getDefaultViewUrl(SPDocumentLibrary documentLibrary)
        {
            return string.Format("{0}/{1}", documentLibrary.ParentWeb.Url, documentLibrary.DefaultView.Url);
        }

        private string getFormUrl(SPDocumentLibrary documentLibrary, PAGETYPE pageType)
        {
            return string.Format("{0}/{1}", documentLibrary.ParentWeb.Url, documentLibrary.Forms[pageType].Url);
        }

        private void appendParameter(StringBuilder url, string key, string value)
        {
            if (value != null)
            {
                url.AppendFormat("&{0}={1}", key, SPHttpUtility.UrlKeyValueEncode(value));
            }
        }
    }
}
