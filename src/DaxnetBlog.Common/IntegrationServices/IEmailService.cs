using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.IntegrationServices
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="toName">To name.</param>
        /// <param name="toAddess">To addess.</param>
        /// <param name="title">The title.</param>
        /// <param name="bodyHtml">The body HTML.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendEmailAsync(string toName, string toAddess, string title, string bodyHtml, CancellationToken cancellationToken = default(CancellationToken));
    }
}
