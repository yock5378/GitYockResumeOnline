using iTextSharp.text;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExhibitsPortal.Utils.HtmlToPdf
{
    public class BlankImageProvider : IImageProvider
    {
        //Store a reference to the main document so that we can access the page size and margins
        private Document MainDoc;
        //Constructor
        public BlankImageProvider(Document doc)
        {
            this.MainDoc = doc;
        }

        public string GetImageRootPath()
        {
            return "";
        }

        public void Reset()
        {
            return;
        }

        public iTextSharp.text.Image Retrieve(string src)
        {
            return null;

        }

        public void Store(string src, iTextSharp.text.Image img)
        {
            return;
        }
    }
}