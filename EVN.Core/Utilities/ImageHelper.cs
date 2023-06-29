using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EVN.Core.Utilities
{
    public sealed class ImageHelper
    {
        private ImageHelper() { }
        private static ImageCodecInfo jpgEncoder;

        public static void ResizeImage(string inFile, Stream outputStream, double maxDimension, long level)
        {
            //
            // Load via stream rather than Image.FromFile to release the file
            // handle immediately
            //
            using (Stream stream = new FileStream(inFile, FileMode.Open))
            {
                using (Image inImage = Image.FromStream(stream))
                {
                    double width;
                    double height;

                    if (inImage.Height < inImage.Width)
                    {
                        width = maxDimension;
                        height = (maxDimension / (double)inImage.Width) * inImage.Height;
                    }
                    else
                    {
                        height = maxDimension;
                        width = (maxDimension / (double)inImage.Height) * inImage.Width;
                    }
                    using (Bitmap bitmap = new Bitmap((int)width, (int)height))
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.DrawImage(inImage, 0, 0, bitmap.Width, bitmap.Height);
                            if (inImage.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                            {
                                if (jpgEncoder == null)
                                {
                                    ImageCodecInfo[] ici = ImageCodecInfo.GetImageDecoders();
                                    foreach (ImageCodecInfo info in ici)
                                    {
                                        if (info.FormatID == ImageFormat.Jpeg.Guid)
                                        {
                                            jpgEncoder = info;
                                            break;
                                        }
                                    }
                                }
                                if (jpgEncoder != null)
                                {
                                    EncoderParameters ep = new EncoderParameters(1);
                                    ep.Param[0] = new EncoderParameter(Encoder.Quality, level);
                                    bitmap.Save(outputStream, jpgEncoder, ep);
                                }
                                else
                                    bitmap.Save(outputStream, inImage.RawFormat);
                            }
                            else
                            {
                                //
                                // Fill with white for transparent GIFs
                                //
                                graphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                                bitmap.Save(outputStream, inImage.RawFormat);
                            }
                        }
                    }
                }
            }
        }

        public static void GetImageSize(string inFile, out int width, out int height)
        {
            using (Stream stream = new FileStream(inFile, FileMode.Open))
            {
                using (Image src_image = Image.FromStream(stream))
                {
                    width = src_image.Width;
                    height = src_image.Height;
                }
            }
        }

        public static bool ResizeImage(string sourceFile, string targetFile, int maxWidth, int maxHeight, bool preserverAspectRatio, int quality)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                String extension = System.IO.Path.GetExtension(targetFile).ToLower();
                var success = ResizeImage(sourceFile, stream, maxWidth, maxHeight, preserverAspectRatio, quality, extension);
                if (success)
                {
                    using (var fileStream = File.Create(targetFile))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                        stream.Flush();
                    }
                }
                return success;
            }

        }

        public static bool ResizeImage(string sourceFile, Stream targetStream, int maxWidth, int maxHeight, bool preserverAspectRatio, int quality, string extension)
        {
            System.Drawing.Image sourceImage;

            string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}/{sourceFile}";

            //Check whether File exists.
            if (!System.IO.File.Exists(fullPath))
            {
                return false;
            }
            System.IO.Stream sourceStream;
            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int length = Convert.ToInt32(fs.Length);
                byte[] buff = new byte[length];
                fs.Read(buff, 0, length);
                sourceStream = fs;
                Graphics oGraphics = null;
                System.Drawing.Image oResampled = null;
                try
                {
                    sourceImage = System.Drawing.Image.FromStream(sourceStream);

                    // If 0 is passed in any of the max sizes it means that that size must be ignored,
                    // so the original image size is used.
                    maxWidth = maxWidth == 0 ? sourceImage.Width : maxWidth;
                    maxHeight = maxHeight == 0 ? sourceImage.Height : maxHeight;

                    Size oSize;
                    if (preserverAspectRatio)
                    {
                        // Gets the best size for aspect ratio resampling
                        oSize = GetAspectRatioSize(maxWidth, maxHeight, sourceImage.Width, sourceImage.Height);
                    }
                    else
                        oSize = new Size(maxWidth, maxHeight);



                    if (sourceImage.PixelFormat == PixelFormat.Indexed || sourceImage.PixelFormat == PixelFormat.Format1bppIndexed || sourceImage.PixelFormat == PixelFormat.Format4bppIndexed || sourceImage.PixelFormat == PixelFormat.Format8bppIndexed || sourceImage.PixelFormat.ToString() == "8207")
                        oResampled = new Bitmap(oSize.Width, oSize.Height, PixelFormat.Format24bppRgb);
                    else
                        oResampled = new Bitmap(oSize.Width, oSize.Height, sourceImage.PixelFormat);

                    // Creates a Graphics for the oResampled image
                    oGraphics = Graphics.FromImage(oResampled);

                    // The Rectangle that holds the Resampled image size
                    Rectangle oRectangle;
                    // Keep transparent
                    oGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // High quality resizing
                    if (quality > 80)
                    {
                        oGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        // If HighQualityBicubic is used, bigger Rectangle is required to remove the white border
                        oRectangle = new Rectangle(-1, -1, oSize.Width + 1, oSize.Height + 1);
                    }
                    else
                    {
                        oRectangle = new Rectangle(0, 0, oSize.Width, oSize.Height);
                    }

                    // Place a white background (for transparent images).
                    oGraphics.FillRectangle(new SolidBrush(Color.White), oRectangle);

                    // Draws over the oResampled image the resampled Image
                    oGraphics.DrawImage(sourceImage, oRectangle);

                    sourceImage.Dispose();

                    if (extension == ".jpg" || extension == ".jpeg")
                    {
                        ImageCodecInfo oCodec = GetJpgCodec();

                        if (oCodec != null)
                        {
                            EncoderParameters aCodecParams = new EncoderParameters(1);
                            aCodecParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            oResampled.Save(targetStream, oCodec, aCodecParams);
                        }
                        else
                        {
                            // TJT: Fixme - stream save requires oCodec?
                            //oResampled.Save( targetStream, oCodec );
                        }
                    }
                    else
                    {
                        switch (extension)
                        {
                            case ".png":
                                oResampled.Save(targetStream, System.Drawing.Imaging.ImageFormat.Png);
                                break;

                            case ".bmp":
                                oResampled.Save(targetStream, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            default:
                                throw new Exception("not support " + extension);
                        }
                    }
                }
                finally
                {
                    if (oGraphics != null)
                        oGraphics.Dispose();

                    if (oResampled != null)
                        oResampled.Dispose();

                    if (sourceStream != null)
                        sourceStream.Close();
                }
            }
       
            return true;
        }

        public static bool CreateThumb(string sourceFilePath, string thumbFilePath, int w = 0, int h = 0, bool keep_ratio = true)
        {
            var allow = true;
            FileInfo fi = new FileInfo(thumbFilePath);
            if (fi.Exists)
            {
                if (fi.Length == 0)
                    System.IO.File.Delete(thumbFilePath);
                else
                    allow = false;
            }

            if (allow)
            {
                if (!System.IO.File.Exists(sourceFilePath))
                    return false;

                if (w != 0 && h != 0)
                    keep_ratio = false;

                return ResizeImage(sourceFilePath, thumbFilePath, w, h, keep_ratio, 90);
                // return ImageTools.OptimizeImage(sourceFilePath, thumbFilePath, w, h, keep_ratio);
            }

            return true;
        }

        //public static bool OptimizeImage(string sourceFile, string targetFile, int maxWidth, int maxHeight, bool preserverAspectRatio)
        //{

        //    FIBITMAP img = FreeImage.LoadEx(sourceFile);
        //    Size oSize;
        //    if (preserverAspectRatio)
        //    {
        //        // Gets the best size for aspect ratio resampling
        //        oSize = GetAspectRatioSize(maxWidth, maxHeight, (int)FreeImage.GetWidth(img), (int)FreeImage.GetHeight(img));
        //    }
        //    else
        //        oSize = new Size(maxWidth, maxHeight);
        //    img = FreeImage.Rescale(img, oSize.Width, oSize.Height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
        //    //save the image out to disk    
        //    FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, img, targetFile, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL);

        //    return true;
        //}

        private static ImageCodecInfo GetJpgCodec()
        {
            ImageCodecInfo[] aCodecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo oCodec = null;

            for (int i = 0; i < aCodecs.Length; i++)
            {
                if (aCodecs[i].MimeType.Equals("image/jpeg"))
                {
                    oCodec = aCodecs[i];
                    break;
                }
            }

            return oCodec;
        }

        private static Size GetAspectRatioSize(int maxWidth, int maxHeight, int actualWidth, int actualHeight)
        {
            // Creates the Size object to be returned
            Size oSize = new System.Drawing.Size(maxWidth, maxHeight);

            // Calculates the X and Y resize factors
            float iFactorX = (float)maxWidth / (float)actualWidth;
            float iFactorY = (float)maxHeight / (float)actualHeight;

            // If some dimension have to be scaled
            if (iFactorX != 1 || iFactorY != 1)
            {
                // Uses the lower Factor to scale the opposite size
                if (iFactorX < iFactorY) { oSize.Height = (int)Math.Round((float)actualHeight * iFactorX); }
                else if (iFactorX > iFactorY) { oSize.Width = (int)Math.Round((float)actualWidth * iFactorY); }
            }

            if (oSize.Height <= 0) oSize.Height = 1;
            if (oSize.Width <= 0) oSize.Width = 1;

            // Returns the Size
            return oSize;
        }
    }
}
