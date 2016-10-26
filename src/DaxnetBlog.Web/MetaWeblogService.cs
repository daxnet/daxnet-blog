using DaxnetBlog.Common.IntegrationServices;
using DaxnetBlog.Web.Security;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using WilderMinds.MetaWeblog;

namespace DaxnetBlog.Web
{
    public class MetaWeblogService : IMetaWeblogProvider
    {
        private const string Container = "files";

        private readonly HttpClient httpClient;
        private readonly UserManager<User> userManager;
        private readonly IMediaObjectStorageService storageService;

        public MetaWeblogService(HttpClient httpClient,
            UserManager<User> userManager,
            IMediaObjectStorageService storageService)
        {
            this.httpClient = httpClient;
            this.userManager = userManager;
            this.storageService = storageService;
        }

        public int AddCategory(string key, string username, string password, NewCategory category)
        {
            throw new NotImplementedException();
        }

        public string AddPost(string blogid, string username, string password, Post post, bool publish)
        {
            User user;
            if (Validate(username, password, out user))
            {
                var result = httpClient.PostAsJsonAsync("blogPosts/create", new
                {
                    Title = post.title,
                    Content = post.description,
                    AccountId = user.Id
                }).Result;

                if (result.StatusCode == HttpStatusCode.Created)
                {
                    return result.Content.ReadAsStringAsync().Result;
                }
            }
            return null;
        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            User user;
            if (Validate(username, password, out user))
            {
                var result = httpClient.DeleteAsync($"blogPosts/delete/{postid}").Result;
                try
                {
                    result.EnsureSuccessStatusCode();
                    return true;
                }
                catch
                {
                   
                }
            }
            return false;
        }

        public bool EditPost(string postid, string username, string password, Post post, bool publish)
        {
            User user;
            if (Validate(username, password, out user))
            {
                var result = httpClient.PutAsJsonAsync($"blogPosts/update/{postid}", new
                {
                    Title = post.title,
                    Content = post.description,
                    DatePublished = post.dateCreated
                }).Result;
                try
                {
                    result.EnsureSuccessStatusCode();
                    return true;
                }
                catch { }
            }
            return false;
        }

        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            User user;
            if (Validate(username, password, out user))
            {
                return new CategoryInfo[]
                {
                    new CategoryInfo
                    {
                        categoryid = "1",
                        title = "所有博客",
                        description = "所有博客"
                    }
                };
            }
            return null;
        }

        public Post GetPost(string postid, string username, string password)
        {
            User user;
            if (Validate(username, password, out user))
            {
                var result = httpClient.GetAsync($"blogPosts/{postid}").Result;
                try
                {
                    result.EnsureSuccessStatusCode();
                    dynamic post = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result);
                    return new Post
                    {
                        dateCreated = (DateTime)post.datePublished,
                        description = (string)post.content,
                        postid = post.id.ToString(),
                        title = (string)post.title,
                        userid = post.accountId.ToString(),
                    };
                }
                catch { }
            }
            return null;
        }

        public Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            User user;
            if (Validate(username, password, out user))
            {
                try
                {
                    var result = httpClient.GetAsync($"blogPosts/paginate/{numberOfPosts}/1").Result;
                    result.EnsureSuccessStatusCode();
                    var blogPosts = (dynamic)JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result);
                    List<Post> posts = new List<Post>();
                    foreach(dynamic post in blogPosts.data)
                    {
                        posts.Add(new Post
                        {
                            categories = new string[] { "所有博客" },
                            permalink = $"http://daxnet.me/BlogPosts/Post/{post.id.ToString()}",
                            dateCreated = (DateTime)post.datePublished,
                            description = (string)post.content,
                            postid = post.id.ToString(),
                            title = (string)post.title,
                            userid = post.accountId.ToString(),
                            wp_slug = $"http://daxnet.me/BlogPosts/Post/{post.id.ToString()}"
                        });
                    }
                    return posts.ToArray();
                }
                catch
                {

                }
            }
            return null;
        }

        public UserInfo GetUserInfo(string key, string username, string password)
        {
            throw new NotImplementedException();
        }

        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            User user;
            if (Validate(username, password, out user))
            {
                return new BlogInfo[]
                    {
                        new BlogInfo
                        {
                            blogid = "1",
                            blogName = "daxnet.me",
                            url = "http://daxnet.me"
                        }
                    };
            }
            return null;
        }

        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            User user;
            if (Validate(username, password, out user))
            {
                var fileName = mediaObject.name.Replace("Windows-Live-Writer/", "");
                foreach(var ch in Path.GetInvalidFileNameChars())
                {
                    fileName = fileName.Replace(ch, '_');
                }

                var urlString = this.storageService.SaveAsync(Container, fileName, mediaObject.bits).Result;
                return new MediaObjectInfo
                {
                    url = urlString
                };
            }
            return null;
        }

        private bool Validate(string username, string password, out User user)
        {
            user = userManager.FindByNameAsync(username).Result;
            if (user == null)
                return false;
            if (!user.IsLocked.HasValue || user.IsLocked.Value)
                return false;
            if (!user.IsAdmin.HasValue || !user.IsAdmin.Value)
                return false;
            return userManager.CheckPasswordAsync(user, password).Result;
        }
    }
}
