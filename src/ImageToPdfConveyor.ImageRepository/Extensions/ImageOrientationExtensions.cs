namespace ImageToPdfConveyor.ImageRepository.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    internal static class ImageOrientationExtensions
    {
        public static RotateFlipType GetNormalizationRotation(this ImageOrientation imageOrientation)
        {
            switch (imageOrientation)
            {
                case ImageOrientation.TopLeft:
                    break;

                case ImageOrientation.TopRight:
                    return RotateFlipType.RotateNoneFlipX;

                case ImageOrientation.BottomRight:
                    return RotateFlipType.Rotate180FlipNone;

                case ImageOrientation.BottomLeft:
                    return RotateFlipType.Rotate180FlipX;

                case ImageOrientation.LeftTop:
                    return RotateFlipType.Rotate90FlipX;

                case ImageOrientation.RightTop:
                    return RotateFlipType.Rotate90FlipNone;

                case ImageOrientation.RightBottom:
                    return RotateFlipType.Rotate270FlipX;

                case ImageOrientation.LeftBottom:
                    return RotateFlipType.Rotate270FlipNone;

            }

            return RotateFlipType.RotateNoneFlipNone;
        }
    }
}
