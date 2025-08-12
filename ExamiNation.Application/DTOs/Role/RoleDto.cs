﻿using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Role
{
    public class RoleDto
    {

        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserCount { get; set; }
    }
}
