using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using System.Web.UI;
using System.Linq;

namespace LargeFileUploadWithSLToSP
{
    public partial class LibraryUpload : LayoutsPageBase
    {
        #region Attributes

        private string spSiteUrl
        {
            get { return SPContext.Current.Site.Url; }
        }

        private string uploadClientSilverlightUrl
        {
            get { return string.Format("{0}/UploadClient/UploadClient.xap", spSiteUrl); }
        }

        private string documentLibraryId
        {
            get { return Request.QueryString["documentLibraryId"]; }
        }

        private int maximumFileSize
        {
            get { return base.Site.WebApplication.MaximumFileSize; }
        }

        private bool isDisplayedInDialogBox
        {
            get { return Request.QueryString["IsDlg"] == "1"; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            uploadClientPlaceHolder.Controls.Add(new LiteralControl(buildUploadClientObject()));
        }

        #region Silverlight embeded object code build

        private string buildUploadClientObject()
        {
            StringBuilder clientCode = new StringBuilder();

            appendObjectBegin(clientCode, 
                new Pair("style", "display:block;"),
                new Pair("data", "data:application/x-Silverlight-2,"),
                new Pair("type", "APPlication/x-Silverlight-2"),
                new Pair("width", "411px"),
                new Pair("height", "363px"));
            appendParam(clientCode, "source", uploadClientSilverlightUrl);
            appendParam(clientCode, "onError", "onSilverlightError");
            appendInitParams(clientCode,
                new Pair("siteUrl", spSiteUrl),
                new Pair("MS.SP.url", spSiteUrl),
                new Pair("documentLibraryId", documentLibraryId),
                new Pair("maxFileSize", maximumFileSize.ToString()),
                new Pair("isDisplayedInDialogBox", isDisplayedInDialogBox.ToString()));
            appendObjectEnd(clientCode);

            return clientCode.ToString();
        }

        private void appendObjectBegin(StringBuilder clientCode, params Pair[] attributes)
        {
            clientCode.Append("<object");
            
            foreach (var attribute in attributes)
            {
                appendAttribute(clientCode, (string)attribute.First, (string)attribute.Second);
            }
            
            clientCode.Append(" >\r\n");
        }

        private void appendObjectEnd(StringBuilder clientCode)
        {
            clientCode.Append("</object>\r\n");
        }

        private void appendAttribute(StringBuilder clientCode, string name, string value) 
        {
            clientCode.AppendFormat(" {0}=\"{1}\"", name, value);
        }

        private void appendInitParams(StringBuilder clientCode, params Pair[] initParams)
        {
            string initParamsStr = string.Join(",", 
                initParams.Select(param => string.Format("{0}={1}", (string)param.First, (string)param.Second)).ToArray());
            appendParam(clientCode, "initparams", initParamsStr);
        }

        private void appendParam(StringBuilder clientCode, string name, string value) 
        {
            clientCode.AppendFormat("\t<param name=\"{0}\" value=\"{1}\" />", name, value);
        }

        #endregion
    }   
}
