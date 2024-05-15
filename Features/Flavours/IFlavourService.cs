﻿namespace NutriBest.Server.Features.Flavours
{
    using NutriBest.Server.Features.Flavours.Models;

    public interface IFlavourService
    {
        Task<int> Create(string name);

        Task<bool> Remove(string name);

        Task<List<FlavourCountServiceModel>> GetProductsByFlavourCount();
    }
}
