﻿namespace NutriBest.Server.Data.Models
{
    using System;
    using Microsoft.AspNetCore.Identity;
    using NutriBest.Server.Data.Models.Base;

    public class User : IdentityUser, IDeletableEntity
    {
        public Profile Profile { get; set; } = new Profile();

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string? DeletedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
