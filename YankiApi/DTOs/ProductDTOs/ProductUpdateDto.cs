using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Http.ModelBinding;
using YankiApi.DataAccessLayer;
using YankiApi.Entities;
using YankiApi.Extentions;
using YankiApi.Helpers;

namespace YankiApi.DTOs.ProductDTOs
{
    public class ProductUpdateDto
    {
        /// <summary>
        /// Product Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Product Title
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Product Price
        /// </summary>
        public double? Price { get; set; }
        /// <summary>
        /// Product DiscountPrice
        /// </summary>
        public double? DiscountedPrice { get; set; }
        /// <summary>
        /// Product Extax
        /// </summary>
        public double? ExTax { get; set; }
        /// <summary>
        /// Product Count
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// Product Description
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Product Image
        /// </summary>
        public string? Image { get; set; }
        /// <summary>
        /// Product Images Files
        /// </summary>
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        /// <summary>
        /// Product Images
        /// </summary>
        public List<ProductImage>? ProductImages { get; set; }
        /// <summary>
        /// Product Image File
        /// </summary>
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        /// <summary>
        /// Product Category Id
        /// </summary>
        public int? CategoryId { get; set; }

        public class ProductUpdateDtoValidation : AbstractValidator<ProductUpdateDto>
        {
            public ProductUpdateDtoValidation(AppDbContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor _contextAccessor)
            {

                RuleFor(r => r).Custom(async (r, validate) =>
                {


                    if (r.Id == null) validate.AddFailure("Yalnis ID");

                    Product dbProduct = context.Products
                        .Include(p => p.ProductImages.Where(pImages => !pImages.IsDeleted))
                        .FirstOrDefault(c => c.Id == r.Id && c.IsDeleted == false);

                    if (dbProduct == null) validate.AddFailure("Yalnis ID");

                    int canUpload = 6 - dbProduct.ProductImages.Count();
                    if (r.Files != null && canUpload < r.Files.Count())
                    {
                        validate.AddFailure("Files", $"Maksimum {canUpload} Qeder sekil yukleye bilersiniz");

                    }
                    if (r.Files != null && r.Files.Count() > 0)
                    {
                        var requestContext = _contextAccessor?.HttpContext?.Request;
                        var baseUrl = $"{requestContext?.Scheme}://{requestContext?.Host}";

                        List<ProductImage> productImages = new ();
                        foreach (IFormFile file in r.Files)
                        {
                            var img = await file.CreateFileAsync(webHostEnvironment, "assets", "img", "product");

                            ProductImage productImage = new ()
                            {
                                Image = baseUrl + $"/assets/img/product/{img}",
                                CreatedAt = DateTime.UtcNow.AddDays(4),
                                CreatedBy = "System"
                            };
                            productImages.Add(productImage);
                        }
                        
                        dbProduct.ProductImages.AddRange(productImages);
                    }
                    //StartImageFile
                    if (r.ImageFile != null)
                    {
                        FileHelpers.DeleteFile(dbProduct.Image, webHostEnvironment, "assets", "img", "product");

                        var requestContext = _contextAccessor?.HttpContext?.Request;
                        var baseUrl = $"{requestContext?.Scheme}://{requestContext?.Host}";



                        string img = await r.ImageFile.CreateFileAsync(webHostEnvironment, "assets", "img", "product");
                        r.Image = baseUrl + $"/assets/img/category/{img}";
                    }
                    if (r.Title != null) { dbProduct.Title = r.Title; }
                    if (r.Image != null) { dbProduct.Image = r.Image; }
                    if (r.Price != null) { dbProduct.Price = (double)r.Price; }
                    if (r.Image != null) { dbProduct.Image = r.Image; }
                    if (r.ProductImages != null) { dbProduct.ProductImages = r.ProductImages; }
                    if (r.DiscountedPrice != null) { dbProduct.DiscountedPrice = (double)r.DiscountedPrice; }
                    if (r.Count != null) { dbProduct.Count = (int)r.Count; }
                    if (r.ExTax != null) { dbProduct.ExTax = (double)r.ExTax; }
                    if (r.Description != null) { dbProduct.Description = r.Description; }

                    dbProduct.UpdatetAt = DateTime.UtcNow.AddDays(4);
                    dbProduct.UpdatetBy = "Admin";

                });


            }
        }
            
    }
}
