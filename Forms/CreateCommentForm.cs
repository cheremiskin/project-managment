using System;
using pm.Models;

namespace project_managment.Forms
{
    public class CreateCommentForm
    {
        public string Content { get; set; }

        public Comment ToComment()
        {
            var comment = new Comment
            {
                Content = this.Content,
                CreationDate = DateTime.Now
            };
            return comment;
        }
    }
}