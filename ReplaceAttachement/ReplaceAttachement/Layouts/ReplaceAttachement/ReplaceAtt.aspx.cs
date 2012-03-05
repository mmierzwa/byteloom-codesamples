using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;
using System.Text;

namespace ReplaceAttachement.Layouts.ReplaceAttachement
{
    public partial class ReplaceAtt : LayoutsPageBase
    {
        const string libraryName = "LargeFiles";
        const string fileName = "test.txt";
        const int defaultContentLength = 150;

        protected void Page_Load(object sender, EventArgs e)
        {
            ContentBox.Text = getRandomContent();
        }

        protected void Replace_Clicked(object sender, EventArgs args)
        {
            SPWeb webSite = SPContext.Current.Web;
            SPDocumentLibrary library = webSite.Lists[libraryName] as SPDocumentLibrary;

            if (library == null)
            {
                throw new SPException("There is no document lirbary named " + libraryName);
            }

            using (MemoryStream docStream = new MemoryStream())
            {
                BinaryWriter docWriter = new BinaryWriter(docStream);
                docWriter.Write(ContentBox.Text);

                SPFile file = find(library, fileName);

                if (file == null)
                {
                    throw new SPException("File " + fileName + " not found");
                }

                file.CheckOut();
                file.SaveBinary(docStream);
                file.CheckIn(string.Empty, SPCheckinType.OverwriteCheckIn);

                docWriter.Close();
            }
        }

        private static string getRandomContent()
        {
            Random rng = new Random((int)DateTime.Now.Ticks);
            StringBuilder content = new StringBuilder();

            for (int charNo = 0; charNo < defaultContentLength; charNo++) 
            {
                content.Append(Convert.ToChar(rng.Next(32, 126)));
            }

            return content.ToString();
        }

        private static SPFile find(SPDocumentLibrary library, string fileName)
        {
            SPFileCollection files = library.RootFolder.Files;
            for (int fileIdx = 0; fileIdx < files.Count; fileIdx++)
            {
                if (files[fileIdx].Name == fileName)
                {
                    return files[fileIdx];
                }
            }

            return null;
        }
    }
}
