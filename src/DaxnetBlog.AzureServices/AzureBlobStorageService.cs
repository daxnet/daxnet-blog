using DaxnetBlog.Common.IntegrationServices;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Collections.Generic;

namespace DaxnetBlog.AzureServices
{
    public class AzureBlobStorageService : IMediaObjectStorageService
    {
        private readonly string baseUrl;
        private readonly string storageAccount;
        private readonly string accessKey;

        public AzureBlobStorageService(string baseUrl, string storageAccount, string accessKey)
        {
            this.baseUrl = baseUrl;
            this.storageAccount = storageAccount;
            this.accessKey = accessKey;
        }

        public  async Task<bool> SaveAsync(string container, string fileName, string base64)
        {
            HttpWebResponse response;

            var headers = new SortedList<string, string>();
            var requestBody = base64;
            headers.Add("x-ms-blob-type", "BlockBlob");
            HttpWebRequest request = await this.CreateRESTRequestAsync("PUT", container, fileName, requestBody, headers);
            response = request.GetResponseAsync().Result as HttpWebResponse;
            return response.StatusCode == HttpStatusCode.Created;
        }

        #region Helper Methods

        private async Task<HttpWebRequest> CreateRESTRequestAsync(string method, string container, string fileName, string requestBodyBase64 = null,
                SortedList<string, string> headers = null, string ifMatch = "", string md5 = "")
        {
            byte[] byteArray = null;
            DateTime now = DateTime.UtcNow;
            string uri = this.baseUrl + container + "/" + fileName;
            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = method;

            request.Headers["x-ms-date"] = DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            request.Headers["x-ms-version"] = "2015-12-11";

            //if there are additional headers required, they will be passed in to here,
            //add them to the list of request headers
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }

            //if there is a requestBody, add a header for the Accept-Charset and set the content length
            if (!String.IsNullOrEmpty(requestBodyBase64))
            {
                //request.Headers["Accept-Charset"] = "UTF-8";

                byteArray = Convert.FromBase64String(requestBodyBase64);
            }

            request.Headers["Authorization"] = AuthorizationHeader(method, now, request, string.IsNullOrEmpty(requestBodyBase64) ? "" : byteArray.Length.ToString(), ifMatch, md5);

            if (!String.IsNullOrEmpty(requestBodyBase64))
            {
                var stream = await request.GetRequestStreamAsync();
                await stream.WriteAsync(byteArray, 0, byteArray.Length);
            }

            return request;
        }

        private string GetCanonicalizedHeaders(HttpWebRequest request)
        {
            ArrayList headerNameList = new ArrayList();
            StringBuilder sb = new StringBuilder();

            //retrieve any headers starting with x-ms-, put them in a list and sort them by value.
            foreach (string headerName in request.Headers.AllKeys)
            {
                if (headerName.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    headerNameList.Add(headerName.ToLowerInvariant());
                }
            }
            headerNameList.Sort();

            //create the string that will be the in the right format
            foreach (string headerName in headerNameList)
            {
                StringBuilder builder = new StringBuilder(headerName);
                string separator = ":";
                //get the value for each header, strip out \r\n if found, append it with the key
                var nameValueCollection = new NameValueCollection();
                foreach (var key in request.Headers.AllKeys)
                {
                    nameValueCollection.Add(key, request.Headers[key]);
                }
                foreach (string headerValue in GetHeaderValues(nameValueCollection, headerName))
                {
                    string trimmedValue = headerValue.Replace("\r\n", String.Empty);
                    builder.Append(separator);
                    builder.Append(trimmedValue);
                    //set this to a comma; this will only be used 
                    //if there are multiple values for one of the headers
                    separator = ",";
                }
                sb.Append(builder.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }


        private ArrayList GetHeaderValues(NameValueCollection headers, string headerName)
        {
            ArrayList list = new ArrayList();
            string[] values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (string str in values)
                {
                    list.Add(str.TrimStart(null));
                }
            }
            return list;
        }

        private string AuthorizationHeader(string method, DateTime now, HttpWebRequest request, string contentLength = "", string ifMatch = "", string md5 = "")
        {
            string MessageSignature;

            //this is the raw representation of the message signature 
            MessageSignature = String.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                method,
                contentLength,
                ifMatch,
                GetCanonicalizedHeaders(request),
                GetCanonicalizedResource(request.RequestUri, this.storageAccount),
                md5
                );

            //now turn it into a byte array
            byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(MessageSignature);

            //create the HMACSHA256 version of the storage key
            System.Security.Cryptography.HMACSHA256 SHA256 =
                new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(this.accessKey));

            //Compute the hash of the SignatureBytes and convert it to a base64 string.
            string signature = Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));

            //this is the actual header that will be added to the list of request headers
            string AuthorizationHeader = "SharedKey " + this.storageAccount
                + ":" + Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));

            return AuthorizationHeader;
        }

        private string GetCanonicalizedResource(Uri address, string accountName)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder builder = new StringBuilder("/");
            builder.Append(accountName);     //this is testsnapshots
            builder.Append(address.AbsolutePath);  //this is "/" because for getting a list of containers 
            str.Append(builder.ToString());

            NameValueCollection values2 = new NameValueCollection();
            //address.Query is ?comp=list
            //this ends up with a namevaluecollection with 1 entry having key=comp, value=list 
            //it will have more entries if you have more query parameters
            var parseQueryResult = QueryHelpers.ParseQuery(address.Query);

            NameValueCollection values = new NameValueCollection();
            foreach (var kvp in parseQueryResult)
            {
                values.Add(kvp.Key, kvp.Value);
            }
            foreach (string str2 in values.Keys)
            {
                ArrayList list = new ArrayList(values.GetValues(str2));
                list.Sort();
                StringBuilder builder2 = new StringBuilder();
                foreach (object obj2 in list)
                {
                    if (builder2.Length > 0)
                    {
                        builder2.Append(",");
                    }
                    builder2.Append(obj2.ToString());
                }
                values2.Add((str2 == null) ? str2 : str2.ToLowerInvariant(), builder2.ToString());
            }
            ArrayList list2 = new ArrayList(values2.AllKeys);
            list2.Sort();
            foreach (string str3 in list2)
            {
                StringBuilder builder3 = new StringBuilder(string.Empty);
                builder3.Append(str3);
                builder3.Append(":");
                builder3.Append(values2[str3]);
                str.Append("\n");
                str.Append(builder3.ToString());
            }
            return str.ToString();
        }
        #endregion
    }
}
