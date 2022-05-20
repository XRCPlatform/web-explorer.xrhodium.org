using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;

namespace BitcoinRhExplorer.Library
{
    public static class FileHelper
    {
        public static string SaveFile(HttpPostedFileBase file, string allowedExtensions, string filePathTarget)
        {
            var fileName = string.Empty;

            var sourceFileName = Path.GetFileName(file.FileName);

            if (sourceFileName != null && sourceFileName.Contains("."))
            {
                var fileExtention = sourceFileName.Substring(sourceFileName.LastIndexOf('.') + 1); 

                if (allowedExtensions.Contains(fileExtention))
                {
                    Guid newFileName = Guid.NewGuid();
                    var pathToFolder = HttpContext.Current.Server.MapPath(filePathTarget);

                    fileName = string.Format("{0}.{1}", newFileName, fileExtention);
                    var filepath = Path.Combine(pathToFolder, fileName);

                    file.SaveAs(filepath);
                }
                else
                {
                    throw new BitcoinRhExplorerException(Resources.Resources.Error_WrongExtension);
                }
            }

            return fileName;
        }

        public static void Resize(string folderPath, string sourceFileName, int maxWidth = 100, int maxHeight = 100, string prefix = "thumb_")
        {
            using (var srcImage = Image.FromFile(HttpContext.Current.Server.MapPath(folderPath + sourceFileName)))
            {
                var sourceWidth = srcImage.Width;
                var sourceHeight = srcImage.Height;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = (maxWidth/(float) sourceWidth);
                nPercentH = (maxHeight/(float) sourceHeight);

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }

                var destWidth = (int) (sourceWidth*nPercent);
                var destHeight = (int) (sourceHeight*nPercent);

                var b = new Bitmap(destWidth, destHeight);
                var g = Graphics.FromImage((Image) b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(srcImage, 0, 0, destWidth, destHeight);
                g.Dispose();

                b.Save(HttpContext.Current.Server.MapPath(string.Format("{0}{1}{2}", folderPath, prefix, sourceFileName)));
                b.Dispose();
            }
        }
    }
}
