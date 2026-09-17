using Application.DTOs;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Web.Mvc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IValidator<ProductCreateRequest> _createValidator;
    private readonly IValidator<ProductUpdateRequest> _updateValidator;

    public ProductsController(
        ProductService productService,
        IValidator<ProductCreateRequest> createValidator,
        IValidator<ProductUpdateRequest> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();
        
        var response = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.PriceInfo?.SalePrice ?? 0,
            Sku = p.Sku,
            BarCode = p.BarCode,
            ImageUrl = p.MainImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            StockQuantity = p.StockInfo?.Quantity ?? 0,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            HasVariants = p.HasVariants
        });
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new
            {
                Message = "Produto não encontrado."
            });
        }

        var response = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            Price = product.PriceInfo?.SalePrice ?? 0,
            CostPrice = product.PriceInfo?.CostPrice ?? 0,
            Sku = product.Sku,
            BarCode = product.BarCode,
            ImageUrl = product.MainImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            SupplierId = product.SupplierId,
            StockQuantity = product.StockInfo?.Quantity ?? 0,
            MinimumStock = product.StockInfo?.MinimumStock,
            UnitOfMeasure = product.StockInfo?.UnitOfMeasure,
            Weight = product.Dimensions?.Weight,
            Height = product.Dimensions?.Height,
            Width = product.Dimensions?.Width,
            Depth = product.Dimensions?.Depth,
            Brand = product.Specs?.Brand,
            Model = product.Specs?.Model,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            IsNew = product.IsNew,
            IsDigital = product.IsDigital,
            HasVariants = product.HasVariants,
            AverageRating = product.Metrics?.AverageRating ?? 0,
            TotalReviews = product.Metrics?.TotalReviews ?? 0,
            TotalSales = product.Metrics?.TotalSales ?? 0,
            ViewsCount = product.Metrics?.ViewsCount ?? 0,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Status = product.Status.ToString(),
            Visibility = product.Visibility.ToString(),
            Slug = product.SeoInfo?.Slug,
            MetaTitle = product.SeoInfo?.MetaTitle,
            MetaDescription = product.SeoInfo?.MetaDescription,
            Observations = product.Observations,
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                Url = i.Url,
                IsMain = i.IsMain,
                Description = i.Description,
                Order = i.Order
            }).ToList(),
            Attributes = product.Attributes.Select(a => new ProductAttributeDto
            {
                Id = a.Id,
                Key = a.Key,
                Value = a.Value,
                Group = a.Group,
                DisplayOrder = a.DisplayOrder
            }).ToList()
        };

        return Ok(response);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            string imagePath = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }
                imagePath = $"/uploads/products/{fileName}";
            }

            // Criar o produto - SEM ID ainda
            var product = new Product(
                name: request.Name,
                description: request.Description,
                sku: request.Sku,
                price: request.Price,
                categoryId: request.CategoryId,
                stockQuantity: request.StockQuantity ?? 0,
                shortDescription: request.ShortDescription,
                barCode: request.BarCode,
                mainImageUrl: imagePath,
                supplierId: request.SupplierId,
                costPrice: request.CostPrice,
                unitOfMeasure: request.UnitOfMeasure ?? "UN",
                weight: request.Weight,
                height: request.Height,
                width: request.Width,
                depth: request.Depth,
                brand: request.Brand,
                model: request.Model,
                isDigital: request.IsDigital ?? false,
                createdByUserId: null
            );

            // SALVAR O PRODUTO PRIMEIRO - Isso vai gerar o ID
            var createdProduct = await _productService.AddAsync(product);

            // AGORA SIM, adicionar atributos e imagens (com o ID já gerado)
            if (request.Attributes != null)
            {
                foreach (var attr in request.Attributes)
                {
                    createdProduct.AddAttribute(attr.Key, attr.Value, attr.Group);
                }
            }

            if (request.AdditionalImages != null)
            {
                foreach (var image in request.AdditionalImages)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        createdProduct.AddImage($"/uploads/products/{fileName}", false);
                    }
                }
            }

            // Atualizar o produto com as imagens e atributos
            await _productService.UpdateAsync(createdProduct);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = createdProduct.Id },
                createdProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProductAsync(
        int id,
        [FromForm] ProductUpdateRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(new
                {
                    Error = "O ID informado não corresponde ao ID da URL."
                });
            }

            var validationResult = await _updateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    Error = "Produto não encontrado."
                });
            }

            string imagePath = string.IsNullOrWhiteSpace(request.ExistingImageUrl)
                ? product.MainImageUrl
                : request.ExistingImageUrl;

            if (request.Image != null && request.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(product.MainImageUrl))
                {
                    var trimmedPath = product.MainImageUrl
                        .TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar);

                    var oldImagePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        trimmedPath
                    );

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Image.FileName)}";
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "products"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                imagePath = $"/uploads/products/{fileName}";
            }

            // Atualizar informações básicas
            product.Update(
                name: request.Name,
                description: request.Description,
                price: request.Price ?? product.PriceInfo.SalePrice,
                sku: request.Sku,
                barCode: request.BarCode ?? product.BarCode,
                imageUrl: imagePath,
                categoryId: request.CategoryId
            );

            // Atualizar preço de custo se fornecido
            if (request.CostPrice.HasValue)
            {
                product.UpdateCostPrice(request.CostPrice.Value);
            }

            // Atualizar estoque se fornecido
            if (request.StockQuantity.HasValue)
            {
                product.SetStockQuantity(request.StockQuantity.Value);
            }

            // Atualizar status
            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                {
                    product.Publish();
                }
                else
                {
                    product.Unpublish();
                }
            }

            await _productService.UpdateAsync(product);

            return Ok(new
            {
                Message = "Produto atualizado com sucesso.",
                Product = product
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpPost("{id}/sell")]
    public async Task<IActionResult> SellProductAsync(int id, [FromBody] ProductSaleRequest request)
    {
        try
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new
                {
                    Error = "Quantidade deve ser maior que zero."
                });
            }

            await _productService.SellAsync(id, request.Quantity, request.Reason, request.DocumentNumber, request.UserId);

            return Ok(new
            {
                Message = "Venda registrada com sucesso.",
                Quantity = request.Quantity
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpGet("{id}/movements")]
    public async Task<IActionResult> GetStockMovements(int id, [FromQuery] int? limit)
    {
        try
        {
            var movements = await _productService.GetStockMovementsAsync(id, limit);
            return Ok(movements);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductAsync(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Produto deletado com sucesso."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpPost("{id}/attributes")]
    public async Task<IActionResult> AddAttribute(int id, [FromBody] AddAttributeRequest request)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { Error = "Produto não encontrado." });
            }

            product.AddAttribute(request.Key, request.Value, request.Group);
            await _productService.UpdateAsync(product);

            return Ok(new { Message = "Atributo adicionado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("{id}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddImage(int id, [FromForm] AddImageRequest request)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { Error = "Produto não encontrado." });
            }

            if (request.Image != null && request.Image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "products"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/products/{fileName}";
                product.AddImage(imageUrl, request.IsMain, request.Description, request.Order);
                await _productService.UpdateAsync(product);

                return Ok(new { Message = "Imagem adicionada com sucesso.", Url = imageUrl });
            }

            return BadRequest(new { Error = "Imagem não fornecida." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    private IActionResult ValidationError(ValidationResult validationResult)
    {
        return BadRequest(new
        {
            Errors = validationResult.Errors.Select(x => new
            {
                Campo = x.PropertyName,
                Mensagem = x.ErrorMessage
            })
        });
    }
}