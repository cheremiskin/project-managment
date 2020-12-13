using System;
using pm.Models;

namespace project_managment.Data.Models.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Info { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsAdmin { get; set; }

        public UserDto(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Info = user.Info;
            BirthDate = user.BirthDate;
            IsAdmin = user.RoleId == 1;
        }
        
        public UserDto(){}
    }
}