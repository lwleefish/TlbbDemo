using Demo.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using O2S.Components.PDFRender4NET;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Demo.Utils
{
    class PdfHelper
    {
        float width = 0;
        float height = 0;
        /// <summary>
        /// 将PDF文档转换为图片的方法
        /// </summary>
        /// <param name="pdfInputPath">PDF文件路径</param>
        /// <param name="imagefile">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">设置所需图片格式</param>
        /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
        public void ConvertPDF2Image(string pdffile, string imagefile, string imageName, int startPageNum, int endPageNum, ImageFormat imageFormat, Definition definition)
        {
            PDFFile pdfFile = PDFFile.Open(pdffile);

            if (!Directory.Exists(imagefile))
            {
                Directory.CreateDirectory(imagefile);
            }
            // validate pageNum
            if (startPageNum <= 0)
            {
                startPageNum = 1;
            }

            if (endPageNum > pdfFile.PageCount)
            {
                endPageNum = pdfFile.PageCount;
            }
            if (startPageNum > endPageNum)
            {
                int tempPageNum = startPageNum;
                startPageNum = endPageNum;
                endPageNum = startPageNum;
            }
            // start to convert each page
            for (int i = startPageNum; i <= endPageNum; i++)
            {
                Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                pageImage.Save(imagefile + imageName + i.ToString() + "." + imageFormat.ToString(), imageFormat);
                pageImage.Dispose();
            }
            pdfFile.Dispose();
        }

        public void ConvertPDF2Image(string pdffile, string imagefile, ImageFormat imageFormat, Definition definition)
        {
            PDFFile pdfFile = null;
            Bitmap pageImage = null;
            try
            {
                pdfFile = PDFFile.Open(pdffile);
                if (pdfFile.PageCount > 0)
                {
                    height = pdfFile.GetPageSize(0).Height;
                    width = pdfFile.GetPageSize(0).Width;
                }
                //if (!Directory.Exists(imageOutputPath))
                //{
                //    Directory.CreateDirectory(imageOutputPath);
                //}
                pageImage = pdfFile.GetPageImage(0, 56 * (int)definition);
                pageImage.Save(imagefile, imageFormat);
            }
            catch (Exception ex) { }
            finally
            {
                pageImage?.Dispose();
                pdfFile?.Dispose();
            }
        }

        void ConvertJPG2PDF(string jpgfile, string pdf)
        {
            iTextSharp.text.Rectangle pdfsize = new iTextSharp.text.Rectangle(width, height);
            var document = new Document(pdfsize, 0, 0, 0, 0);
            using (var stream = new FileStream(pdf, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();
                using (var imageStream = new FileStream(jpgfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var image = iTextSharp.text.Image.GetInstance(imageStream);
                    if (image.Height > pdfsize.Height || image.Width > pdfsize.Width)
                    {
                        image.ScaleToFit(pdfsize.Width, pdfsize.Height);
                    }
                    image.Alignment = Element.ALIGN_MIDDLE;
                    document.Add(image);
                }
                document.Close();
            }
        }

    }
}
