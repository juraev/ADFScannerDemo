using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace ScannerDemo
{
    class ImagesToPDFConverter
    {
        private Images mImages;

        public ImagesToPDFConverter(Images images)
        {
            this.mImages = images;
        }

        public void getPDF(string path)
        {
            Document doc = new Document();
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();
                while(mImages.hasNext())
                {
                    var ms = mImages.getNextImage();
                    var image = Image.GetInstance(ms);
                    doc.Add(image);
                }
                doc.Close();
            }
        }
    }
}