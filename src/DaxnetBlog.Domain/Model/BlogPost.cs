using DaxnetBlog.Common;
using System;

namespace DaxnetBlog.Domain.Model
{
    public class BlogPost : IEntity<int>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DatePublished { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        public int? UpVote { get; set; }

        public int? DownVote { get; set; }

        public int? Visits { get; set; }

        public bool? IsDeleted { get; set; }

        public override string ToString() => Title;
    }
}
