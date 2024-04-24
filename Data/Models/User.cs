namespace NutriBest.Server.Data.Models
{
    using System;
    using Microsoft.AspNetCore.Identity;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using static Validation.User;

    public class User : IdentityUser, IEntity
    {
        [MaxLength(NameMaxLength)]
        public string? Name { get; set; }

        public Gender? Gender { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
