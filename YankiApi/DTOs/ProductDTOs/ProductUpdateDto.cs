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
        public int Id { get; set; }
        /// <summary>
        /// Product Title
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Product Price
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Product DiscountPrice
        /// </summary>
        public double DiscountedPrice { get; set; }
        /// <summary>
        /// Product Extax
        /// </summary>
        public double ExTax { get; set; }
        /// <summary>
        /// Product Count
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Product Description
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Product Long Description
        /// </summary>
        public string? LongDescription { get; set; }
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
        public int CategoryId { get; set; }
        public DateTime UpdatetAt { get; private set; }
        public string UpdatetBy { get; private set; }

        public class ProductUpdateDtoValidation : AbstractValidator<ProductUpdateDto>
        {
            public ProductUpdateDtoValidation(AppDbContext context, IWebHostEnvironment webHostEnvironment)
            {
               
                RuleFor(r => r).Custom(async (r, validate) =>
                {
                    

                    if (r.Id == null ) validate.AddFailure("Yalnis ID");

                    Product dbProduct = await context.Products
                        .Include(p => p.ProductImages.Where(pImages => pImages.IsDeleted == false))
                        .FirstOrDefaultAsync(c => c.Id == r.Id && c.IsDeleted == false);

                    if (dbProduct == null) validate.AddFailure("Yalnis ID");

                    int canUpload = 6 - dbProduct.ProductImages.Count();
                    if (r.Files != null && canUpload < r.Files.Count())
                    {
                        validate.AddFailure("Files", $"Maksimum {canUpload} Qeder sekil yukleye bilersiniz");

                    }
                    if (r.Files != null && r.Files.Count() > 0)
                    {
                        List<ProductImage> productImages = new List<ProductImage>();
                        foreach (IFormFile file in r.Files)
                        {
                            if (!file.CheckFileContentType("image/jpeg"))
                            {
                                validate.AddFailure("file", "Main File Yalniz JPG Formatda ola biler");
                            }
                            if (!file.CheckFileLength(300))
                            {
                                validate.AddFailure("file", "Main File Yalniz 300Kb  ola biler");
                            }
                            ProductImage productImage = new ProductImage()
                            {
                                Image = await file.CreateFileAsync(webHostEnvironment, "assets", "img", "product"),
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
                        if (!r.ImageFile.CheckFileContentType("image/jpeg"))
                        {
                            validate.AddFailure("MainFile", "Main File Yalniz JPG Formatda ola biler");
                        }
                        if (!r.ImageFile.CheckFileLength(300))
                        {
                            validate.AddFailure("MainFile", "Main File Yalniz 300Kb  ola biler");
                        }
                        FileHelpers.DeleteFile(dbProduct.Image, webHostEnvironment, "assets", "img", "product");

                        dbProduct.Image = await r.ImageFile.CreateFileAsync(webHostEnvironment, "assets", "img", "product");
                    }
                    if (r.Price != null) { dbProduct.Price = r.Price; }
                    if (r.DiscountedPrice != null) { dbProduct.DiscountedPrice = r.DiscountedPrice; }
                    if (r.Count != null) { dbProduct.Count = r.Count; }
                    if (r.ExTax != null) { dbProduct.ExTax = r.ExTax; }
                    if (r.Description != null) { dbProduct.Description = r.Description; }

                    r.UpdatetAt = DateTime.UtcNow.AddDays(4);
                    r.UpdatetBy = "Admin";
                    await context.SaveChangesAsync();
                   
                });

               
            }
        }
            
    }
}
