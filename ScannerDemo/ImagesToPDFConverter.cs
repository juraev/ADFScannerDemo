using System.IO;
using ImageMagick;

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
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {

                MagickImageCollection pdfDoc = new MagickImageCollection();

                while(mImages.hasNext())
                {

                    var ms = mImages.getNextImage();

                    MagickImage img = new MagickImage(ms);

                    img.Format = MagickFormat.Pdf;

                    pdfDoc.Add(img);

                }

                pdfDoc.Write(stream);
                stream.Close();
            }
        }
    }
}