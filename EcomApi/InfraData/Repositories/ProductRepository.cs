using Dapper;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Domain.Enums;
using Domain.Interfaces;
using InfraData.Context;
using System.Data;
using System.Text;

namespace InfraData.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DapperContext _context;

    public ProductRepository(DapperContext context)
    {
        _context = context;
    }

    #region Queries Base

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string query = @"
        SELECT
            p.id,
            p.name,
            p.description,
            p.short_description,
            p.sku,
            p.bar_code,
            p.main_image_url,
            p.category_id,
            p.supplier_id,
            p.price,
            p.cost_price,
            p.wholesale_price,
            p.wholesale_min_quantity,
            p.stock_quantity,
            p.minimum_stock,
            p.maximum_stock,
            p.unit_of_measure,
            p.weight,
            p.height,
            p.width,
            p.depth,
            p.brand,
            p.model,
            p.color,
            p.size,
            p.material,
            p.manufacturer,
            p.manufacture_date,
            p.expiration_date,
            p.meta_title,
            p.meta_description,
            p.meta_keywords,
            p.slug,
            p.tags,
            p.is_active,
            p.is_featured,
            p.is_new,
            p.is_digital,
            p.has_variants,
            p.average_rating,
            p.total_reviews,
            p.total_sales,
            p.views_count,
            p.created_at,
            p.updated_at,
            p.created_by_user_id,
            p.updated_by_user_id,
            p.observations,
            p.internal_notes,
            p.promotional_price,
            p.promotion_start_date,
            p.promotion_end_date,
            p.promotion_id,
            p.status,
            p.visibility,
            p.is_track_inventory,
            p.allow_backorder,
            p.expected_stock_date,
            p.video_url,
            p.warranty_info,
            p.shipping_info,
            p.returns_policy,
            p.requires_shipping,

            c.id AS CategoryId,
            c.name AS CategoryName,

            s.id AS SupplierId,
            s.name AS SupplierName,
            s.cnpj AS SupplierCnpj,
            s.email AS SupplierEmail,
            s.phone_number AS SupplierPhone

        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        ORDER BY p.id";

        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<dynamic>(query);

        var products = new List<Product>();

        foreach (var row in result)
        {
            var product = MapProductFromRow(row);
            products.Add(product);
        }

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string query = @"
        SELECT
            p.id,
            p.name,
            p.description,
            p.short_description,
            p.sku,
            p.bar_code,
            p.main_image_url,
            p.category_id,
            p.supplier_id,
            p.price,
            p.cost_price,
            p.wholesale_price,
            p.wholesale_min_quantity,
            p.stock_quantity,
            p.minimum_stock,
            p.maximum_stock,
            p.unit_of_measure,
            p.weight,
            p.height,
            p.width,
            p.depth,
            p.brand,
            p.model,
            p.color,
            p.size,
            p.material,
            p.manufacturer,
            p.manufacture_date,
            p.expiration_date,
            p.meta_title,
            p.meta_description,
            p.meta_keywords,
            p.slug,
            p.tags,
            p.is_active,
            p.is_featured,
            p.is_new,
            p.is_digital,
            p.has_variants,
            p.average_rating,
            p.total_reviews,
            p.total_sales,
            p.views_count,
            p.created_at,
            p.updated_at,
            p.created_by_user_id,
            p.updated_by_user_id,
            p.observations,
            p.internal_notes,
            p.promotional_price,
            p.promotion_start_date,
            p.promotion_end_date,
            p.promotion_id,
            p.status,
            p.visibility,
            p.is_track_inventory,
            p.allow_backorder,
            p.expected_stock_date,
            p.video_url,
            p.warranty_info,
            p.shipping_info,
            p.returns_policy,
            p.requires_shipping,

            c.id AS CategoryId,
            c.name AS CategoryName,

            s.id AS SupplierId,
            s.name AS SupplierName,
            s.cnpj AS SupplierCnpj,
            s.email AS SupplierEmail,
            s.phone_number AS SupplierPhone

        FROM products p
        LEFT JOIN categories c ON p.category_id = c.id
        LEFT JOIN suppliers s ON p.supplier_id = s.id
        WHERE p.id = @Id";

        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<dynamic>(query, new { Id = id });
        var row = result.FirstOrDefault();

        if (row == null)
            return null;

        var product = MapProductFromRow(row);
        
        await LoadProductCollections(connection, product);

        return product;
    }

    #endregion

    #region Mapeamento

    private Product MapProductFromRow(dynamic row)
    {
        Category? category = null;
        if (row.CategoryId != null)
        {
            category = new Category
            {
                Id = row.CategoryId,
                Name = row.CategoryName
            };
        }

        Supplier? supplier = null;
        if (row.SupplierId != null)
        {
            supplier = new Supplier(
                cnpj: row.SupplierCnpj ?? string.Empty,
                name: row.SupplierName ?? string.Empty,
                email: row.SupplierEmail ?? string.Empty,
                phonenumber: row.SupplierPhone ?? string.Empty,
                address: string.Empty,
                neighborhood: string.Empty,
                zipCode: string.Empty,
                stateId: 0,
                cityId: 0
            );
            supplier.SetId(row.SupplierId);
        }

        var hasVariants = row.has_variants ?? false;

        Product product;

        if (hasVariants)
        {
            product = new Product(
                name: (string)row.name,
                description: (string)row.description ?? string.Empty,
                sku: (string)row.sku,
                categoryId: (int)row.category_id,
                shortDescription: (string?)row.short_description,
                mainImageUrl: (string?)row.main_image_url,
                supplierId: (int?)row.supplier_id,
                brand: (string?)row.brand,
                model: (string?)row.model,
                isDigital: row.is_digital ?? false,
                createdByUserId: (int?)row.created_by_user_id
            );
        }
        else
        {
            product = new Product(
                name: (string)row.name,
                description: (string)row.description ?? string.Empty,
                sku: (string)row.sku,
                price: (decimal)row.price,
                categoryId: (int)row.category_id,
                stockQuantity: row.stock_quantity ?? 0,
                shortDescription: (string?)row.short_description,
                barCode: (string?)row.bar_code,
                mainImageUrl: (string?)row.main_image_url,
                supplierId: (int?)row.supplier_id,
                costPrice: (decimal?)row.cost_price,
                unitOfMeasure: (string)row.unit_of_measure ?? "UN",
                weight: (decimal?)row.weight,
                height: (decimal?)row.height,
                width: (decimal?)row.width,
                depth: (decimal?)row.depth,
                brand: (string?)row.brand,
                model: (string?)row.model,
                isDigital: row.is_digital ?? false,
                createdByUserId: (int?)row.created_by_user_id
            );
        }

        product.SetId(row.id);

        if (category != null)
            product.SetCategory(category);
        
        if (supplier != null)
            product.SetSupplier(supplier);

        SetProductProperties(product, row);

        return product;
    }

    private void SetProductProperties(Product product, dynamic row)
    {
        if (row.price != null)
            product.UpdatePrice((decimal)row.price);
        
        if (row.cost_price != null)
            product.SetCostPrice((decimal)row.cost_price);

        if (row.stock_quantity != null)
            product.SetStockQuantity((int)row.stock_quantity);

        if (row.status != null)
        {
            var status = (ProductStatus)row.status;
            if (status == ProductStatus.Active)
                product.Publish();
            else if (status == ProductStatus.Inactive)
                product.Unpublish();
        }

        if (row.promotional_price != null && row.promotion_start_date != null && row.promotion_end_date != null)
        {
            product.ApplyPromotion(
                promotionalPrice: (decimal)row.promotional_price,
                startDate: (DateTime)row.promotion_start_date,
                endDate: (DateTime)row.promotion_end_date,
                promotionId: (int?)row.promotion_id
            );
        }

        if (row.observations != null)
            product.SetObservations((string)row.observations);

        if (row.internal_notes != null)
            product.SetInternalNotes((string)row.internal_notes);

        if (row.updated_at != null)
            product.SetUpdatedAt((DateTime)row.updated_at);

        if (row.average_rating != null || row.total_reviews != null || row.total_sales != null || row.views_count != null)
        {
            var metrics = new ProductMetrics();
            if (row.average_rating != null)
            {
                var field = typeof(ProductMetrics).GetField("<AverageRating>k__BackingField", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(metrics, (decimal)row.average_rating);
            }
            if (row.total_reviews != null)
            {
                var field = typeof(ProductMetrics).GetField("<TotalReviews>k__BackingField", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(metrics, (int)row.total_reviews);
            }
            if (row.total_sales != null)
            {
                var field = typeof(ProductMetrics).GetField("<TotalSales>k__BackingField", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(metrics, (int)row.total_sales);
            }
            if (row.views_count != null)
            {
                var field = typeof(ProductMetrics).GetField("<ViewsCount>k__BackingField", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(metrics, (int)row.views_count);
            }
            product.SetMetrics(metrics);
        }

        if (row.meta_title != null || row.meta_description != null || row.meta_keywords != null || row.slug != null || row.tags != null)
        {
            var seoInfo = new SeoInfo(
                name: product.Name,
                metaTitle: (string?)row.meta_title,
                metaDescription: (string?)row.meta_description,
                metaKeywords: (string?)row.meta_keywords,
                tags: (string?)row.tags
            );
            if (row.slug != null)
            {
                var field = typeof(SeoInfo).GetField("<Slug>k__BackingField", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(seoInfo, (string)row.slug);
            }
            product.SetSeoInfo(seoInfo);
        }

        if (row.weight != null || row.height != null || row.width != null || row.depth != null)
        {
            var dimensions = new Dimensions(
                weight: (decimal?)row.weight,
                height: (decimal?)row.height,
                width: (decimal?)row.width,
                depth: (decimal?)row.depth
            );
            product.SetDimensions(dimensions);
        }

        if (row.brand != null || row.model != null || row.color != null || row.size != null || 
            row.material != null || row.manufacturer != null || row.manufacture_date != null || row.expiration_date != null)
        {
            var specs = new ProductSpecs(
                brand: (string?)row.brand,
                model: (string?)row.model,
                color: (string?)row.color,
                size: (string?)row.size,
                material: (string?)row.material,
                manufacturer: (string?)row.manufacturer,
                manufactureDate: (DateTime?)row.manufacture_date,
                expirationDate: (DateTime?)row.expiration_date
            );
            product.SetSpecs(specs);
        }

        if (row.bar_code != null)
            product.SetBarCode((string)row.bar_code);

        if (row.main_image_url != null)
            product.SetMainImageUrl((string)row.main_image_url);

        if (row.short_description != null)
            product.SetShortDescription((string)row.short_description);
    }

    private async Task LoadProductCollections(IDbConnection connection, Product product)
    {
        await LoadImages(connection, product);
        await LoadVariants(connection, product);
        await LoadAttributes(connection, product);
        await LoadStockMovements(connection, product);
        await LoadReviews(connection, product);
    }

    #endregion

    #region Carregamento de Coleções

    private async Task LoadImages(IDbConnection connection, Product product)
    {
        const string query = @"
            SELECT id, product_id, url, is_main, description, ""order"", created_at
            FROM product_images
            WHERE product_id = @ProductId
            ORDER BY ""order"", created_at";

        var images = await connection.QueryAsync<ProductImage>(query, new { ProductId = product.Id });
        
        foreach (var image in images)
        {
            product.AddImage(
                imageUrl: image.Url, 
                isMain: image.IsMain, 
                description: image.Description ?? string.Empty, 
                order: image.Order
            );
        }
    }

    private async Task LoadVariants(IDbConnection connection, Product product)
    {
        const string query = @"
            SELECT 
                v.id,
                v.product_id,
                v.name,
                v.sku,
                v.bar_code,
                v.price,
                v.cost_price,
                v.stock_quantity,
                v.image_url,
                v.is_active,
                v.minimum_stock,
                v.promotional_price,
                v.promotion_start_date,
                v.promotion_end_date,
                v.created_at,
                v.updated_at
            FROM product_variants v
            WHERE v.product_id = @ProductId AND v.is_active = true
            ORDER BY v.id";

        var variants = await connection.QueryAsync<dynamic>(query, new { ProductId = product.Id });

        foreach (var variantRow in variants)
        {
            const string attrQuery = @"
                SELECT id, variant_id, ""key"", ""value""
                FROM variant_attributes
                WHERE variant_id = @VariantId";

            var attributes = await connection.QueryAsync<VariantAttribute>(attrQuery, new { VariantId = (int)variantRow.id });
            
            var attributesDict = attributes.ToDictionary(a => a.Key, a => a.Value);

            product.AddVariant(
                name: (string)variantRow.name,
                sku: (string)variantRow.sku,
                price: (decimal)variantRow.price,
                stockQuantity: (int)variantRow.stock_quantity,
                attributes: attributesDict,
                imageUrl: (string?)variantRow.image_url,
                barCode: (string?)variantRow.bar_code,
                costPrice: (decimal?)variantRow.cost_price,
                minimumStock: (int?)variantRow.minimum_stock
            );
        }
    }
    private async Task LoadAttributes(IDbConnection connection, Product product)
    {
        const string query = @"
            SELECT id, product_id, ""key"", ""value"", ""group"", display_order, is_visible, is_filterable, created_at, updated_at
            FROM product_attributes
            WHERE product_id = @ProductId
            ORDER BY display_order, created_at";

        var attributes = await connection.QueryAsync<ProductAttribute>(query, new { ProductId = product.Id });

        foreach (var attr in attributes)
        {
            product.AddAttribute(
                key: attr.Key, 
                value: attr.Value, 
                group: attr.Group ?? string.Empty
            );
        }
    }

    private async Task LoadStockMovements(IDbConnection connection, Product product)
    {
        const string query = @"
            SELECT 
                id, product_id, variant_id, quantity, type, reason, 
                document_number, created_at, user_id, user_name,
                unit_cost, total_cost, previous_stock, new_stock, notes
            FROM stock_movements
            WHERE product_id = @ProductId
            ORDER BY created_at DESC
            LIMIT 50";

        var movements = await connection.QueryAsync<StockMovement>(query, new { ProductId = product.Id });
    }

    private async Task LoadReviews(IDbConnection connection, Product product)
    {
        const string query = @"
            SELECT 
                id, product_id, user_id, user_name, user_email, rating, title,
                comment, is_approved, is_verified_purchase, is_recommended,
                created_at, updated_at, approved_at, helpful_count, not_helpful_count,
                admin_response, admin_response_at, images
            FROM product_reviews
            WHERE product_id = @ProductId AND is_approved = true
            ORDER BY created_at DESC
            LIMIT 20";

        var reviews = await connection.QueryAsync<ProductReview>(query, new { ProductId = product.Id });
        
        foreach (var review in reviews)
        {
            product.AddReview(
                rating: review.Rating,
                comment: review.Comment,
                userId: review.UserId,
                userName: review.UserName
            );
        }
    }

    #endregion

    #region CRUD

    public async Task<Product> AddAsync(Product product)
    {
        const string query = @"
            INSERT INTO products (
                name, description, short_description, sku, bar_code, main_image_url,
                category_id, supplier_id, price, cost_price,
                stock_quantity, unit_of_measure,
                weight, height, width, depth,
                brand, model, is_digital, has_variants,
                is_active, is_featured, is_new,
                created_at, created_by_user_id,
                status, visibility
            ) VALUES (
                @Name, @Description, @ShortDescription, @Sku, @BarCode, @MainImageUrl,
                @CategoryId, @SupplierId, @Price, @CostPrice,
                @StockQuantity, @UnitOfMeasure,
                @Weight, @Height, @Width, @Depth,
                @Brand, @Model, @IsDigital, @HasVariants,
                @IsActive, @IsFeatured, @IsNew,
                @CreatedAt, @CreatedByUserId,
                @Status, @Visibility
            )
            RETURNING id;";

        using var connection = _context.CreateConnection();
        
        var parameters = BuildProductParameters(product);
        var id = await connection.QuerySingleAsync<int>(query, parameters);
        
        product.SetId(id);
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        const string query = @"
            UPDATE products SET
                name = @Name,
                description = @Description,
                short_description = @ShortDescription,
                sku = @Sku,
                bar_code = @BarCode,
                main_image_url = @MainImageUrl,
                category_id = @CategoryId,
                supplier_id = @SupplierId,
                price = @Price,
                cost_price = @CostPrice,
                wholesale_price = @WholesalePrice,
                wholesale_min_quantity = @WholesaleMinQuantity,
                stock_quantity = @StockQuantity,
                minimum_stock = @MinimumStock,
                maximum_stock = @MaximumStock,
                unit_of_measure = @UnitOfMeasure,
                weight = @Weight,
                height = @Height,
                width = @Width,
                depth = @Depth,
                brand = @Brand,
                model = @Model,
                color = @Color,
                size = @Size,
                material = @Material,
                manufacturer = @Manufacturer,
                manufacture_date = @ManufactureDate,
                expiration_date = @ExpirationDate,
                meta_title = @MetaTitle,
                meta_description = @MetaDescription,
                meta_keywords = @MetaKeywords,
                slug = @Slug,
                tags = @Tags,
                is_active = @IsActive,
                is_featured = @IsFeatured,
                is_new = @IsNew,
                is_digital = @IsDigital,
                has_variants = @HasVariants,
                average_rating = @AverageRating,
                total_reviews = @TotalReviews,
                total_sales = @TotalSales,
                views_count = @ViewsCount,
                updated_at = @UpdatedAt,
                updated_by_user_id = @UpdatedByUserId,
                observations = @Observations,
                internal_notes = @InternalNotes,
                promotional_price = @PromotionalPrice,
                promotion_start_date = @PromotionStartDate,
                promotion_end_date = @PromotionEndDate,
                promotion_id = @PromotionId,
                status = @Status,
                visibility = @Visibility,
                is_track_inventory = @IsTrackInventory,
                allow_backorder = @AllowBackorder,
                expected_stock_date = @ExpectedStockDate,
                video_url = @VideoUrl,
                warranty_info = @WarrantyInfo,
                shipping_info = @ShippingInfo,
                returns_policy = @ReturnsPolicy,
                requires_shipping = @RequiresShipping
            WHERE id = @Id";

        using var connection = _context.CreateConnection();
        
        var parameters = BuildProductParameters(product);
        parameters.Add("Id", product.Id);
        parameters.Add("UpdatedAt", DateTime.UtcNow);
        parameters.Add("UpdatedByUserId", product.UpdatedByUserId);

        await connection.ExecuteAsync(query, parameters);
        
        await UpdateImages(connection, product);
        await UpdateAttributes(connection, product);
        await UpdateVariants(connection, product);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        const string query = "DELETE FROM products WHERE id = @Id";
        await connection.ExecuteAsync(query, new { Id = id });
    }

    #endregion

    #region Coleções CRUD

    private async Task SaveImages(IDbConnection connection, Product product)
    {
        const string query = @"
            INSERT INTO product_images (product_id, url, is_main, description, ""order"", created_at)
            VALUES (@ProductId, @Url, @IsMain, @Description, @Order, @CreatedAt)";

        foreach (var image in product.Images)
        {
            await connection.ExecuteAsync(query, new
            {
                ProductId = product.Id,
                image.Url,
                image.IsMain,
                image.Description,
                image.Order,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task UpdateImages(IDbConnection connection, Product product)
    {
        const string deleteQuery = "DELETE FROM product_images WHERE product_id = @ProductId";
        await connection.ExecuteAsync(deleteQuery, new { ProductId = product.Id });
        
        await SaveImages(connection, product);
    }

    private async Task SaveAttributes(IDbConnection connection, Product product)
    {
        const string query = @"
            INSERT INTO product_attributes (product_id, ""key"", ""value"", ""group"", display_order, is_visible, is_filterable, created_at)
            VALUES (@ProductId, @Key, @Value, @Group, @DisplayOrder, @IsVisible, @IsFilterable, @CreatedAt)";

        foreach (var attr in product.Attributes)
        {
            await connection.ExecuteAsync(query, new
            {
                ProductId = product.Id,
                attr.Key,
                attr.Value,
                attr.Group,
                attr.DisplayOrder,
                attr.IsVisible,
                attr.IsFilterable,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task UpdateAttributes(IDbConnection connection, Product product)
    {
        const string deleteQuery = "DELETE FROM product_attributes WHERE product_id = @ProductId";
        await connection.ExecuteAsync(deleteQuery, new { ProductId = product.Id });
        
        await SaveAttributes(connection, product);
    }

    private async Task SaveVariants(IDbConnection connection, Product product)
    {
        if (!product.HasVariants || !product.Variants.Any())
            return;

        const string variantQuery = @"
            INSERT INTO product_variants (product_id, name, sku, bar_code, price, cost_price, stock_quantity, 
                image_url, is_active, minimum_stock, promotional_price, promotion_start_date, promotion_end_date, created_at)
            VALUES (@ProductId, @Name, @Sku, @BarCode, @Price, @CostPrice, @StockQuantity, 
                @ImageUrl, @IsActive, @MinimumStock, @PromotionalPrice, @PromotionStartDate, @PromotionEndDate, @CreatedAt)
            RETURNING id";

        const string attrQuery = @"
            INSERT INTO variant_attributes (variant_id, ""key"", ""value"")
            VALUES (@VariantId, @Key, @Value)";

        foreach (var variant in product.Variants)
        {
            var variantId = await connection.QuerySingleAsync<int>(variantQuery, new
            {
                ProductId = product.Id,
                variant.Name,
                variant.Sku,
                variant.BarCode,
                variant.Price,
                variant.CostPrice,
                variant.StockQuantity,
                variant.ImageUrl,
                variant.IsActive,
                variant.MinimumStock,
                variant.PromotionalPrice,
                variant.PromotionStartDate,
                variant.PromotionEndDate,
                CreatedAt = DateTime.UtcNow
            });

            foreach (var attr in variant.Attributes)
            {
                await connection.ExecuteAsync(attrQuery, new
                {
                    VariantId = variantId,
                    attr.Key,
                    attr.Value
                });
            }
        }
    }

    private async Task UpdateVariants(IDbConnection connection, Product product)
    {
        const string deleteVariants = "DELETE FROM product_variants WHERE product_id = @ProductId";
        await connection.ExecuteAsync(deleteVariants, new { ProductId = product.Id });
        
        await SaveVariants(connection, product);
    }

    #endregion

    #region Métodos de Estoque

    public async Task AddStockMovementAsync(StockMovement movement)
    {
        const string query = @"
            INSERT INTO stock_movements (
                product_id, variant_id, quantity, type, reason, document_number,
                created_at, user_id, user_name, unit_cost, total_cost,
                previous_stock, new_stock, notes
            ) VALUES (
                @ProductId, @VariantId, @Quantity, @Type, @Reason, @DocumentNumber,
                @CreatedAt, @UserId, @UserName, @UnitCost, @TotalCost,
                @PreviousStock, @NewStock, @Notes
            )";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new
        {
            movement.ProductId,
            movement.VariantId,
            movement.Quantity,
            movement.Type,
            movement.Reason,
            movement.DocumentNumber,
            movement.CreatedAt,
            movement.UserId,
            movement.UserName,
            movement.UnitCost,
            movement.TotalCost,
            movement.PreviousStock,
            movement.NewStock,
            movement.Notes
        });
    }

    public async Task<IEnumerable<StockMovement>> GetStockMovementsAsync(int productId, int? limit = 50)
    {
        var query = @"
            SELECT id, product_id, variant_id, quantity, type, reason, 
                   document_number, created_at, user_id, user_name,
                   unit_cost, total_cost, previous_stock, new_stock, notes
            FROM stock_movements
            WHERE product_id = @ProductId
            ORDER BY created_at DESC";

        if (limit.HasValue)
            query += $" LIMIT {limit.Value}";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<StockMovement>(query, new { ProductId = productId });
    }

    #endregion

    #region Métodos de Busca

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        const string query = "SELECT id FROM products WHERE sku = @Sku";
        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int?>(query, new { Sku = sku });
        
        if (!id.HasValue)
            return null;
        
        return await GetByIdAsync(id.Value);
    }

    public async Task<IEnumerable<Product>> SearchAsync(string? term, int? categoryId, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 20)
    {
        var sql = new StringBuilder(@"
            SELECT p.id, p.name, p.description, p.short_description, p.sku, p.main_image_url,
                   p.price, p.promotional_price, p.average_rating, p.total_reviews,
                   p.slug, p.brand, p.is_active, p.is_featured, p.is_new,
                   c.id AS CategoryId, c.name AS CategoryName
            FROM products p
            LEFT JOIN categories c ON p.category_id = c.id
            WHERE p.is_active = 1 AND p.status = 1");

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(term))
        {
            sql.Append(" AND (p.name ILIKE @Term OR p.description ILIKE @Term OR p.sku ILIKE @Term)");
            parameters.Add("Term", $"%{term}%");
        }

        if (categoryId.HasValue)
        {
            sql.Append(" AND p.category_id = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            sql.Append(" AND p.price >= @MinPrice");
            parameters.Add("MinPrice", minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            sql.Append(" AND p.price <= @MaxPrice");
            parameters.Add("MaxPrice", maxPrice.Value);
        }

        sql.Append(" ORDER BY p.is_featured DESC, p.total_sales DESC, p.created_at DESC");
        sql.Append($" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
        parameters.Add("Offset", (page - 1) * pageSize);
        parameters.Add("PageSize", pageSize);

        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<dynamic>(sql.ToString(), parameters);

        var products = new List<Product>();
        foreach (var row in result)
        {
            var product = new Product(
                name: (string)row.name,
                description: (string)row.description ?? string.Empty,
                sku: (string)row.sku,
                price: (decimal)row.price,
                categoryId: (int)row.CategoryId,
                stockQuantity: 0,
                shortDescription: (string?)row.short_description,
                mainImageUrl: (string?)row.main_image_url,
                brand: (string?)row.brand
            );
            product.SetId(row.id);
            
            if (row.CategoryId != null)
            {
                var category = new Category
                {
                    Id = row.CategoryId,
                    Name = row.CategoryName
                };
                product.SetCategory(category);
            }
            
            products.Add(product);
        }

        return products;
    }

    public async Task<bool> IsCodeBarUniqueAsync(string barCode)
    {
        const string query = "SELECT COUNT(1) FROM products WHERE bar_code = @BarCode";
        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(query, new { BarCode = barCode });
        return count == 0;
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeProductId = null)
    {
        var query = "SELECT COUNT(1) FROM products WHERE sku = @Sku";
        if (excludeProductId.HasValue)
            query += " AND id != @ExcludeId";

        using var connection = _context.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(query, new { Sku = sku, ExcludeId = excludeProductId });
        return count == 0;
    }

    #endregion

    #region Métodos Auxiliares

    private DynamicParameters BuildProductParameters(Product product)
    {
        var parameters = new DynamicParameters();
        
        parameters.Add("Name", product.Name);
        parameters.Add("Description", product.Description ?? string.Empty);
        parameters.Add("ShortDescription", product.ShortDescription ?? string.Empty);
        parameters.Add("Sku", product.Sku);
        parameters.Add("BarCode", product.BarCode ?? string.Empty);
        parameters.Add("MainImageUrl", product.MainImageUrl ?? string.Empty);
        parameters.Add("CategoryId", product.CategoryId);
        parameters.Add("SupplierId", product.SupplierId);
        
        parameters.Add("Price", product.PriceInfo?.SalePrice ?? 0);
        parameters.Add("CostPrice", product.PriceInfo?.CostPrice ?? 0);
        parameters.Add("WholesalePrice", product.PriceInfo?.WholesalePrice);
        parameters.Add("WholesaleMinQuantity", product.PriceInfo?.WholesaleMinQuantity);
        parameters.Add("PromotionalPrice", product.PriceInfo?.PromotionalPrice);
        parameters.Add("PromotionStartDate", product.PriceInfo?.PromotionStartDate);
        parameters.Add("PromotionEndDate", product.PriceInfo?.PromotionEndDate);
        parameters.Add("PromotionId", product.PriceInfo?.PromotionId);
        
        parameters.Add("StockQuantity", product.StockInfo?.Quantity ?? 0);
        parameters.Add("MinimumStock", product.StockInfo?.MinimumStock);
        parameters.Add("MaximumStock", product.StockInfo?.MaximumStock);
        parameters.Add("UnitOfMeasure", product.StockInfo?.UnitOfMeasure ?? "UN");
        parameters.Add("IsTrackInventory", product.StockInfo?.IsTrackInventory ?? true);
        parameters.Add("AllowBackorder", product.StockInfo?.AllowBackorder ?? false);
        parameters.Add("ExpectedStockDate", product.StockInfo?.ExpectedStockDate);
        
        parameters.Add("Weight", product.Dimensions?.Weight);
        parameters.Add("Height", product.Dimensions?.Height);
        parameters.Add("Width", product.Dimensions?.Width);
        parameters.Add("Depth", product.Dimensions?.Depth);
        
        parameters.Add("Brand", product.Specs?.Brand);
        parameters.Add("Model", product.Specs?.Model);
        parameters.Add("Color", product.Specs?.Color);
        parameters.Add("Size", product.Specs?.Size);
        parameters.Add("Material", product.Specs?.Material);
        parameters.Add("Manufacturer", product.Specs?.Manufacturer);
        parameters.Add("ManufactureDate", product.Specs?.ManufactureDate);
        parameters.Add("ExpirationDate", product.Specs?.ExpirationDate);
        
        parameters.Add("MetaTitle", product.SeoInfo?.MetaTitle);
        parameters.Add("MetaDescription", product.SeoInfo?.MetaDescription);
        parameters.Add("MetaKeywords", product.SeoInfo?.MetaKeywords);
        parameters.Add("Slug", product.SeoInfo?.Slug);
        parameters.Add("Tags", product.SeoInfo?.Tags);
        
        parameters.Add("Status", (int)product.Status);
        parameters.Add("Visibility", (int)product.Visibility);
        parameters.Add("IsActive", product.IsActive);
        parameters.Add("IsFeatured", product.IsFeatured);
        parameters.Add("IsNew", product.IsNew);
        parameters.Add("IsDigital", product.IsDigital);
        parameters.Add("HasVariants", product.HasVariants);
        parameters.Add("RequiresShipping", true);
        
        parameters.Add("AverageRating", product.Metrics?.AverageRating ?? 0);
        parameters.Add("TotalReviews", product.Metrics?.TotalReviews ?? 0);
        parameters.Add("TotalSales", product.Metrics?.TotalSales ?? 0);
        parameters.Add("ViewsCount", product.Metrics?.ViewsCount ?? 0);
        
        parameters.Add("CreatedAt", product.CreatedAt);
        parameters.Add("CreatedByUserId", product.CreatedByUserId);
        
        parameters.Add("Observations", product.Observations ?? string.Empty);
        parameters.Add("InternalNotes", product.InternalNotes ?? string.Empty);
        
        parameters.Add("VideoUrl", (string?)null);
        parameters.Add("WarrantyInfo", (string?)null);
        parameters.Add("ShippingInfo", (string?)null);
        parameters.Add("ReturnsPolicy", (string?)null);
        
        return parameters;
    }

    #endregion
}