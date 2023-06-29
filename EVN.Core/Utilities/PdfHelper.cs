using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace EVN.Core.Utilities
{
    public sealed class PdfHelper
    {
        private PdfHelper()
        {
        }

        public static PdfHelper Instance { get; } = new PdfHelper();

        public void CombineImageToPdf(List<string> filePaths, string targetPath, out string error)
        {

            error = "";
            try
            {
                PdfHelper.Instance.SaveImageAsPdf(filePaths, targetPath);
            }
            catch (Exception ex)
            {
                error=ex.Message;
            }
        }

        private void SaveImageAsPdf(List<string> images, string pdfFileName)
        {
            using (var document = new PdfDocument())
            {
                document.Options.FlateEncodeMode = PdfFlateEncodeMode.BestCompression;
                document.Options.UseFlateDecoderForJpegImages = PdfUseFlateDecoderForJpegImages.Automatic;
                document.Options.NoCompression = false;
                // Defaults to false in debug build,
                // so we set it to true.
                document.Options.CompressContentStreams = true;
                XSize size = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
                foreach (var imageFileName in images)
                {
                    PdfPage page = document.AddPage();
                    if (page.Orientation == PageOrientation.Landscape)
                    {
                        page.Width = size.Height;
                        page.Height = size.Width;
                    }
                    else
                    {
                        page.Width = size.Width;
                        page.Height = size.Height;
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ImageHelper.ResizeImage(imageFileName, stream, (int)page.Width.Value, (int)page.Height.Value, true, 85, ".jpg");
                        //ImageHelper.ReduceImageQuality(imageFileName, stream, 100);                    
                        using (XImage img = XImage.FromStream(stream))
                        {
                            // export to test
                            //using (var fileStream = File.Create(@"C:\Users\linhp\Pictures\0.png"))
                            //{
                            //    stream.Seek(0, SeekOrigin.Begin);
                            //    stream.CopyTo(fileStream);
                            //    stream.Flush();
                            //}

                            // Calculate new height to keep image ratio                

                            var width = Math.Min(page.Width.Point, img.PixelWidth);
                            var height = (int)(((double)width / (double)img.PixelWidth) * img.PixelHeight);

                            XGraphics gfx = XGraphics.FromPdfPage(page);
                            gfx.DrawImage(img, 0, 0, width, height);
                        }
                    }
                }
                document.Save(pdfFileName);
            }
        }
    }
}
