using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class ImageValidationMethods
    {
        private readonly byte[] bmp = { 66, 77 };
        private readonly byte[] png = { 137, 80, 78, 71, 13, 10, 26, 10 };
        private readonly byte[] gif = { 71, 73, 70, 56 };
        private readonly byte[] jpg = { 255, 216, 255, 224 };
        private readonly byte[] jpg_exif = { 255, 216, 255, 225 };
        private readonly byte[] jpg_spiff = { 255, 216, 255, 232 };
        private readonly byte[] jpeg_cannon = { 255, 216, 255, 226 };
        private readonly byte[] jpeg_samsung = { 255, 216, 255, 227 };

        public bool IsValidImageFormat(byte[] array)
        {
            if (IsBMP(array) || IsPNG(array) || IsGIF(array) || IsJPG(array) || IsJPEG(array))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string ReturnImageExtension(byte[] array)
        {
            if (IsBMP(array))
            {
                return ".bmp";
            }
            else if (IsPNG(array))
            {
                return ".png";
            }
            else if (IsGIF(array))
            {
                return ".gif";
            }
            else if (IsJPG(array))
            {
                return ".jpg";
            }
            else if (IsJPEG(array))
            {
                return ".jpeg";
            }
            else
            {
                return null;
            }
        }

        public bool IsBMP(byte[] array)
        {
            return array[0..2].SequenceEqual(bmp);
        }

        public bool IsPNG(byte[] array)
        {
            return array[0..8].SequenceEqual(png);
        }

        public bool IsGIF(byte[] array)
        {
            return array[0..4].SequenceEqual(gif);
        }

        public bool IsJPG(byte[] array)
        {
            if (array[0..4].SequenceEqual(jpg))
            {
                return true;
            }
            else if (array[0..4].SequenceEqual(jpg_exif))
            {
                return true;
            }
            else if (array[0..4].SequenceEqual(jpg_spiff))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsJPEG(byte[] array)
        {
            if (array[0..4].SequenceEqual(jpeg_cannon))
            {
                return true;
            }
            else if (array[0..4].SequenceEqual(jpeg_samsung))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
