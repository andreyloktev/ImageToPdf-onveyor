namespace ImageToPdfConveyor.ImageRepository.Logic
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using ImageToPdfConveyor.ImageRepository.Extensions;
    using ImageToPdfConveyor.ObjectModel.Contracts;

    using DrawingImage = System.Drawing.Image;

    internal sealed class Image : IImage
    {
        private DrawingImage image;

        private const int OrientationPropertyId = 0x0112;

        private const int ShortTypeId = 3;

        private const int IntTypeId = 4;

        private readonly string name;

        public Image(string filePath)
        {
            name = Path.GetFileName(filePath);
            image = DrawingImage.FromFile(filePath);
            Normalize();
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public void Scale(Rectangle rectangle)
        {
            var destImage = new Bitmap(rectangle.Width, rectangle.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            image = destImage;
        }

        public int Height 
        {
            get
            {
                return image.Height;
            }
        }

        public int Width 
        {
            get
            {
                return image.Width;
            }
        }

        public Stream DataStream 
        {
            get
            {
                var memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Jpeg);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream;
            }
        }

        private void Normalize()
        {
            bool needRotate90CW = false;

            // Determine orientation: Landscape or Portrait. After normalizstion, height and width can change place
            if (image.Width > image.Height)
            {
                needRotate90CW = true;
            }

            try
            {
                var rotateFlip = GetCurrentImageOrientation(GetPropertyValue()).GetNormalizationRotation();

                var peoperty = image.GetPropertyItem(OrientationPropertyId);
                peoperty.Value = BitConverter.GetBytes((short)1);
                image.SetPropertyItem(peoperty);

                if (rotateFlip != RotateFlipType.RotateNoneFlipNone)
                {
                    image.RotateFlip(rotateFlip);
                }
            }
            catch (Exception)
            {
                // Possible image does not have EXIF properties. May simple rotation
            }

            //if (needRotate90CW)
            //{
            //    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            //}
        }

        private ImageOrientation GetCurrentImageOrientation(int propertyValue)
        {
            switch (propertyValue)
            {
                case 1:
                    return ImageOrientation.TopLeft;
                case 2:
                    return ImageOrientation.TopRight;
                case 3:
                    return ImageOrientation.BottomRight;
                case 4:
                    return ImageOrientation.BottomLeft;
                case 5:
                    return ImageOrientation.LeftTop;
                case 6:
                    return ImageOrientation.RightTop;
                case 7:
                    return ImageOrientation.RightBottom;
                case 8:
                    return ImageOrientation.LeftBottom;
            }

            return ImageOrientation.Unknown;
        }

        private int GetPropertyValue()
        {
            var orientationProperty = image.GetPropertyItem(OrientationPropertyId);
            switch (orientationProperty.Type)
            {
                case ShortTypeId:
                    return GetShortArray(orientationProperty.Value, orientationProperty.Len)[0];

                case IntTypeId:
                    return GetIntArray(orientationProperty.Value, orientationProperty.Len)[0];

                default:
                    throw new Exception("Unsupported image property value type");
            }
        }

        private short[] GetShortArray(byte[] value, int length)
        {
            var resultLength = length / sizeof(short);
            short[] result = new short[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                result[i] = (short)(value[i + 1] << 1 | value[i]);
            }

            return result;
        }

        private int[] GetIntArray(byte[] value, int length)
        {
            var resultLength = length / sizeof(int);
            int[] result = new int[resultLength];

            for (int i = 0; i < resultLength; i++)
            {
                result[i] = value[i + 3] << 3 | value[i + 2] << 2 | value[i + 1] << 1 | value[0];
            }

            return result;
        }
    }
}
