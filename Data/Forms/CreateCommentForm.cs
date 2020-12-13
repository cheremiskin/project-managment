using System;
using System.ComponentModel.DataAnnotations;
using pm.Models;

namespace project_managment.Forms
{
    public class CreateCommentForm
    {
        [StringLength(512, MinimumLength = 1, ErrorMessage = "Count of characters should be less than 512")]
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