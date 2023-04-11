using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Http.ModelBinding;
using YankiApi.DataAccessLayer;
using YankiApi.Entities;
using YankiApi.Extentions;

namespace YankiApi.DTOs.ProductDTOs
{
    /// <summary>
    /// Create Product
    /// </summary>
    public class ProductPostDto
    {
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


    }

    public class ProductPostDtoValidation : AbstractValidator<ProductPostDto>
    {

        public ProductPostDtoValidation(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            RuleFor(p => p.Title)
                .MaximumLength(200).WithMessage("Max 200 simvol")
                .NotEmpty().WithMessage("Mecburidir");
            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Max 1000 simvol")
                .NotEmpty().WithMessage("Mecburidir");
            RuleFor(r => r).Custom(async (r, validate) =>
            {
                if (!context.Categories.Any(c => !c.IsDeleted && c.Id == r.CategoryId))
                {
                    validate.AddFailure("CategoryId", $"Daxil olunan Category Id {r.CategoryId} Yalnisdir");
                }

                if (r.ImageFile is null)
                {
                    validate.AddFailure("ImageFile", "ImageFile File mutleqdir");
                }
                else
                {
                    if (!r.ImageFile.CheckFileContentType("image/jpeg"))
                    {
                        validate.AddFailure("ImageFile", "ImageFile File Yalniz JPG Formatda ola biler");
                    }

                    if (!r.ImageFile.CheckFileLength(300))
                    {
                        validate.AddFailure("ImageFile", "ImageFile File Yalniz 300Kb  ola biler");
                    }

                    r.Image = await r.ImageFile.CreateFileAsync(webHostEnvironment, "assets", "img", "product");
                }

                if (r?.Files?.Count() <= 6)
                {
                    if (r.Files is not null && r.Files.Count() > 0)
                    {
                        List<ProductImage> productImages = new();
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
                            ProductImage productImage = new()
                            {
                                Image = await file.CreateFileAsync(webHostEnvironment, "assets", "img", "product"),
                                CreatedAt = DateTime.UtcNow.AddDays(4),
                                CreatedBy = "System"
                            };
                            productImages.Add(productImage);
                        }

                        r.ProductImages = productImages;
                    }
                }
                else
                {
                    validate.AddFailure("Files", "max 6 shekil");
                }

                string code = r.Title[..2] + context.Categories.FirstOrDefault(c => c.Id == r.CategoryId).Name[..1];
            });
        }

    }

}
