using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerDemo
{
    class Images
    {
        Queue images;

        public Images()
        {
            images = new Queue();
        }
            
        public void add(WIA.ImageFile img)
        {
            images.Enqueue(img);
        }

        public bool hasNext()
        {
            return images.Count > 0;
        }

        public int size()
        {
            return images.Count;
        }

        public MemoryStream getNextImage()
        {

            if (images.Count == 0)
            {
                throw new NoImagesLeftException();
            }
            else
            {
                WIA.ImageFile imageFile = (WIA.ImageFile)images.Dequeue();

                Byte[] imageBytes = (byte[])imageFile.FileData.get_BinaryData();

                MemoryStream ms = new MemoryStream(imageBytes);

                return ms;
            }
        }
    }
    
    class NoImagesLeftException : Exception
    {
        public NoImagesLeftException() { }
    }
}
