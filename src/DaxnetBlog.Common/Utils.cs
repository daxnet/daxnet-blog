using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Reflection;
using System.IO;
using System.IO.Compression;

namespace DaxnetBlog.Common
{
    /// <summary>
    /// Provides the method extensions globally.
    /// </summary>
    public static class Utils
    {
        private static readonly Random rnd = new Random(DateTime.Now.Millisecond);
        private static Lazy<Type[]> SimpleTypesInternal = new Lazy<Type[]>(() =>
        {
            var types = new[]
                           {
                              typeof (Enum),
                              typeof (string),
                              typeof (char),
                              typeof (Guid),

                              typeof (bool),
                              typeof (byte),
                              typeof (short),
                              typeof (int),
                              typeof (long),
                              typeof (float),
                              typeof (double),
                              typeof (decimal),

                              typeof (sbyte),
                              typeof (ushort),
                              typeof (uint),
                              typeof (ulong),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan),
                          };


            var nullableTypes = from t in types
                                where t != typeof(Enum) && t != typeof(string)
                                select typeof(Nullable<>).MakeGenericType(t);

            return types.Concat(nullableTypes).ToArray();
        });

        public static bool IsSimpleType(this Type src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            return src.GetTypeInfo().IsEnum || 
                (src.GetTypeInfo().IsGenericType && 
                    src.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    src.GetTypeInfo().GetGenericArguments().First().GetTypeInfo().IsEnum) ||
                SimpleTypesInternal.Value.Contains(src);
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }

        public static string GetUniqueStringValue(int length)
        {
            var candidatesBuilder = new StringBuilder();
            for (var i = 0; i < 5; i++)
            {
                candidatesBuilder.Append(Guid.NewGuid().ToString().Replace("-", "").ToUpper());
            }
            var candidates = candidatesBuilder.ToString();

            var result = new StringBuilder("P");
            for (var i = 0; i < length; i++)
            {
                var pos = rnd.Next(candidates.Length);
                result.Append(candidates[pos]);
            }
            return result.ToString();
        }

        public static string Compress(this string src)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(src);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static string Decompress(this string src)
        {
            byte[] gZipBuffer = Convert.FromBase64String(src);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
