namespace pm.Models.UpdateModels
{
    public class ProjectUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsPrivate { get; set; }

        public Project ToProject(in long id)
        {
            return new Project
            {
                Name = this.Name,
                Description = this.Description,
                IsPrivate = this.IsPrivate,
                Id = id
            };
        }
    }
}