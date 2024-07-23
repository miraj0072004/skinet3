using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Core.Specifications;
using Core.Interfaces;
using API.Dtos;
using AutoMapper;
using API.Errors;
using API.Helpers;
namespace API.Controllers
{
    
    public class ProductsController : BaseApiController
    {
        
        private IGenericRepository<Product> _productRepo;
        private IGenericRepository<ProductBrand> _productBrandRepo;
        private IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo,
                                  IGenericRepository<ProductBrand> productBrandRepo,
                                  IGenericRepository<ProductType> productTypeRepo,
                                  IMapper mapper)
         {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

         

         [HttpGet("brands")]
         public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
         {
            var productBrands = await _productBrandRepo.ListAllAsync();
            return Ok(productBrands);            
         }

         [HttpGet("types")]
         public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
         {
            var productTypes = await _productTypeRepo.ListAllAsync();
            return Ok(productTypes);            
         }

         [HttpGet]
         public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery]ProductSpecParams productParams)
         {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await _productRepo.CountAsync(countSpec);

            var products = await _productRepo.ListAsync(spec);

            var data = _mapper
                        .Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,productParams.PageSize, totalItems, data));            
         }

         [HttpGet("{id}")]
         [ProducesResponseType(StatusCodes.Status200OK)]
         [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
         public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
         {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productRepo.GetEntityWithSpec(spec);

            if(product == null) return NotFound(new ApiResponse(404)); 
            
            return _mapper.Map<Product, ProductToReturnDto>(product);
         }
    }
}