namespace pm.Models.UpdateModels
{
    public class CommentUpdate
    {
        public string Content { get; set; }

        public Comment ToComment(long id)
        {
            return new Comment
            {
                Content = this.Content,
                Id = id
            };
        }
    }
}