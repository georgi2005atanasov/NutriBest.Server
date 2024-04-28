﻿using NutriBest.Server.Features.Carts.Models;

namespace NutriBest.Server.Features.Carts
{
    public interface ICartService
    {
        Task<int?> Add(int productId, int count);

        Task<bool> Remove(int productId, int count);

        Task<bool> Clean();

        Task<CartServiceModel> Get();
    }
}
