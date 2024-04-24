﻿namespace NutriBest.Server.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using NutriBest.Server.Data.Models.Base;
    using System;

    public class User : IdentityUser, IEntity
    {
        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
