using System;

namespace DaxnetBlog.Common
{
    public static class EnvironmentVariables
    {
        private const string KEY_DAXNETBLOG_SQL_STR = @"DAXNETBLOG_SQL_STR";
        private const string KEY_DAXNETBLOG_SMTP_SERVERNAME = @"DAXNETBLOG_SMTP_SERVERNAME";
        private const string KEY_DAXNETBLOG_SMTP_USERNAME = @"DAXNETBLOG_SMTP_USERNAME";
        private const string KEY_DAXNETBLOG_SMTP_PASSWORD = @"DAXNETBLOG_SMTP_PASSWORD";
        private const string KEY_DAXNETBLOG_SVC_BASEURL = @"DAXNETBLOG_SVC_BASEURL";
        private const string KEY_DAXNETBLOG_AZURE_STORAGE_BASEURL = @"DAXNETBLOG_AZURE_STORAGE_BASEURL";
        private const string KEY_DAXNETBLOG_AZURE_STORAGE_ACCT = @"DAXNETBLOG_AZURE_STORAGE_ACCT";
        private const string KEY_DAXNETBLOG_AZURE_STORAGE_KEY = @"DAXNETBLOG_AZURE_STORAGE_KEY";
        
        private const string DefaultConnectionString = @"Server=localhost; Database=DaxnetBlogDB; Integrated Security=SSPI;";

        private static readonly Crypto crypto = Crypto.Create(CryptoTypes.EncTypeTripleDes);

        public static string ServerDatabaseConnectionString
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_SQL_STR);
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = DefaultConnectionString;
                }
                else
                {
                    try
                    {
                        connectionString = crypto.Decrypt(connectionString, Crypto.GlobalKey);
                    }
                    catch
                    {
                        connectionString = DefaultConnectionString;
                    }
                }
                return connectionString;
            }
        }

        public static string WebSmtpServerName
        {
            get
            {
                return Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_SMTP_SERVERNAME);
            }
        }

        public static string WebSmtpUserName
        {
            get
            {
                try
                {
                    var userNameEncryptedString = Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_SMTP_USERNAME);
                    if (!string.IsNullOrEmpty(userNameEncryptedString))
                        return crypto.Decrypt(userNameEncryptedString, Crypto.GlobalKey);
                }
                catch { }
                return null;
            }
        }

        public static string WebSmtpPassword
        {
            get
            {
                try
                {
                    var passwordEncryptedString = Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_SMTP_PASSWORD);
                    if (!string.IsNullOrEmpty(passwordEncryptedString))
                        return crypto.Decrypt(passwordEncryptedString, Crypto.GlobalKey);
                }
                catch { }
                return null;
            }
        }

        public static string WebServiceBaseUrl
        {
            get
            {
                return Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_SVC_BASEURL);
            }
        }

        public static string WebAzureStorageAccount
        {
            get
            {
                try
                {
                    var encryptedStorageAccount = Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_AZURE_STORAGE_ACCT);
                    if (!string.IsNullOrEmpty(encryptedStorageAccount))
                    {
                        return crypto.Decrypt(encryptedStorageAccount, Crypto.GlobalKey);
                    }
                }
                catch { }
                return null;
            }
        }

        public static string WebAzureStorageKey
        {
            get
            {
                try
                {
                    var encryptedStorageKey = Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_AZURE_STORAGE_KEY);
                    if (!string.IsNullOrEmpty(encryptedStorageKey))
                    {
                        return crypto.Decrypt(encryptedStorageKey, Crypto.GlobalKey);
                    }
                }
                catch { }
                return null;
            }
        }

        public static string WebAzureStorageBaseUrl
        {
            get
            {
                try
                {
                    var encryptedStorageBaseUrl = Environment.GetEnvironmentVariable(KEY_DAXNETBLOG_AZURE_STORAGE_BASEURL);
                    if (!string.IsNullOrEmpty(encryptedStorageBaseUrl))
                    {
                        return crypto.Decrypt(encryptedStorageBaseUrl, Crypto.GlobalKey);
                    }
                }
                catch { }
                return null;
            }
        }
    }
}
