﻿using WebApp.Models.Claim;

namespace WebApp.Models.Role
{
    public class UpdateRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ClaimDto> Claims { get; set; }
    }
}
