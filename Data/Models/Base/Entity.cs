﻿
namespace NutriBest.Server.Data.Models.Base
{
    using System.ComponentModel.DataAnnotations;

    public abstract class Entity : IEntity
    {
        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
