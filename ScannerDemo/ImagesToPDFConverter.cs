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

        public void getPDF()
        {
            Document doc = new Document();

            using (var stream = new FileStream("C:\\Users\\gitarist\\Downloads\\img.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
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


//public void Doconvert()
//{
//    Document doc = new Document();

//    using (var stream = new FileStream("C:\\Users\\gitarist\\Downloads\\img.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
//    {
//        PdfWriter writer = PdfWriter.GetInstance(doc, stream);
//        doc.Open();
//        for (int i = 1; i <= 15; i++)
//        {
//            string s = "C:\\Users\\gitarist\\Downloads\\img (" + i + ").jpg";
//            using (var imgStream = new FileStream(s, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//            {
//                var image = Image.GetInstance(imgStream);
//                doc.Add(image);
//            }
//        }
//        doc.Close();
//    }
//}
