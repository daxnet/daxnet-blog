using DaxnetBlog.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Controllers
{
    public class CaptchaController : Controller
    {
        private static readonly Crypto crypto = Crypto.Create(CryptoTypes.EncAes);

        public CaptchaController()
        {

        }

        /// <summary>
        /// Updates the captcha and return the HTML string representing the
        /// updated captcha.
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateCaptcha()
        {
            return PartialView("_CaptchaPartial");
        }

        /// <summary>
        /// Verifies the captcha.
        /// </summary>
        /// <param name="captchaString">The captcha string.</param>
        /// <param name="encryptedString">The encrypted string.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult VerifyCaptcha(string captchaString, string encryptedString)
        {
            var enc = crypto.Encrypt(captchaString, "*trak");
            return Ok(enc == encryptedString);
        }
    }
}
