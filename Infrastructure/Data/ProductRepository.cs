using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<ProductBrand>> GetProductBrandsAsync()
        {
            var productsBrands = await _context.ProductBrands.ToListAsync();
            return productsBrands;
        }
        public async Task<IReadOnlyCollection<ProductType>> GetProductTypesAsync()
        {
            var productsTypes = await _context.ProductTypes.ToListAsync();
            return productsTypes;
        }
        

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                        .Include(p => p.ProductType)
                        .Include(p => p.ProductBrand)
                        .FirstOrDefaultAsync(p=>p.Id==id);
        }

        public async Task<IReadOnlyCollection<Product>> GetProductsAsync()
        {
            var products = await _context.Products
                           .Include(p => p.ProductType)
                           .Include(p => p.ProductBrand)
                           .ToListAsync();
            return products;
        }

        
    }
}