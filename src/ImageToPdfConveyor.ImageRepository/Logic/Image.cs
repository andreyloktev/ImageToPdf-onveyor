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

        public Image(string filePath)
        {
            Name = Path.GetFileName(filePath);
            image = DrawingImage.FromFile(filePath);
            Normalize();
        }

        public string Name { get; private set; }

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
            try
            {
                var property = image.GetPropertyItem(OrientationPropertyId);
                var rotateFlip = GetCurrentImageOrientation(GetPropertyValue(property)).GetNormalizationRotation();

                property.Value = BitConverter.GetBytes((short)1);
                image.SetPropertyItem(property);

                if (rotateFlip != RotateFlipType.RotateNoneFlipNone)
                {
                    image.RotateFlip(rotateFlip);
                }
            }
            catch (Exception)
            {
                // Possible image does not have EXIF properties. May simple rotation
            }

            if (image.Width > image.Height)
            {
                image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
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

        private int GetPropertyValue(PropertyItem property)
        {
            switch (property.Type)
            {
                case ShortTypeId:
                    return BitConverter.ToInt16(property.Value, 0);

                case IntTypeId:
                    return BitConverter.ToInt32(property.Value, 0);

                default:
                    throw new Exception("Unsupported image property value type");
            }
        }
    }
}
