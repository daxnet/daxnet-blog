using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "asp-captcha")]
    public class CaptchaTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-captcha")]
        public bool EnableCaptcha { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            //if (EnableCaptcha)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        using (var bitmap = new Bitmap(150, 40, PixelFormat.Format32bppArgb))
            //        {
            //            using (var graphics = Graphics.FromImage(bitmap))
            //            {
            //                Rectangle rect = new Rectangle(0, 0, 149, 39);
            //                graphics.FillRectangle(Brushes.White, rect);
            //                Random r = new Random();
            //                int startIndex = r.Next(1, 5);
            //                int length = r.Next(5, 10);
            //                string drawString = Guid.NewGuid().ToString().Replace("-", "0").Substring(startIndex, length);

            //                Font drawFont = new Font("Arial", 16, FontStyle.Bold);
            //                using (SolidBrush drawBrush = new SolidBrush(Color.Gray))
            //                {
            //                    PointF drawPoint = new PointF(15, 10);
            //                    graphics.DrawRectangle(new Pen(Color.White, 0), rect);
            //                    graphics.DrawString(drawString, drawFont, drawBrush, drawPoint);
            //                }

            //                var encryptedString = Convert.ToBase64String(UTF32Encoding.Unicode.GetBytes(drawString));

            //                output.PostElement.AppendFormat("<input type=\"hidden\" name=\"__captcha_image\" id=\"__captcha_image\" value=\"{0}\" />", encryptedString);
            //            }

            //            bitmap.Save(memoryStream, ImageFormat.Jpeg);
            //            byte[] imageBytes = memoryStream.ToArray();
            //            string base64String = Convert.ToBase64String(imageBytes);
            //            output.Attributes["src"] = "data:image/png;base64," + base64String;

            //        }
            //    }
            //}
        }
    }
}
