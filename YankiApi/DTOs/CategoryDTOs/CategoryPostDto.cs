using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Http.ModelBinding;
using YankiApi.DataAccessLayer;
using YankiApi.Entities;
using YankiApi.Extentions;

namespace YankiApi.DTOs.CategoryDTOs
{
    /// <summary>
    /// Create Product
    /// </summary>
    public class CategoryPostDto
    {
        /// <summary>
        /// Category Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Category Image 
        /// </summary>
        public string? Image { get; set; }
        /// <summary>
        /// Category Image File
        /// </summary>
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public class CategoryPostDtoValidation : AbstractValidator<CategoryPostDto>
        {
            public CategoryPostDtoValidation(AppDbContext context, IWebHostEnvironment webHostEnvironment,IHttpContextAccessor _contextAccessor)
            {
                RuleFor(r => r).Custom(async (r, validate) =>
                {

                    if (context.Categories.Any(c => !c.IsDeleted && c.Name.ToLower().Contains(r.Name.Trim().ToLower())))
                    {
                        validate.AddFailure($"{r.Name}", "Bu adda category var");
                    }
                    if (r.ImageFile is null)
                    {
                        validate.AddFailure("ImageFile", "ImageFile File mutleqdir");
                    }
                    else
                    {
                        if (!r.ImageFile.CheckFileLength(3000))
                        {
                            validate.AddFailure("ImageFile", "ImageFile File Yalniz 300Kb  ola biler");
                        }

                        var requestContext = _contextAccessor?.HttpContext?.Request;
                        var baseUrl = $"{requestContext?.Scheme}://{requestContext?.Host}";

                        

                        string img = await r.ImageFile.CreateFileAsync(webHostEnvironment, "assets", "img", "category");
                        r.Image = baseUrl + $"/assets/img/category/{img}";
                    }
                    r.Name = r.Name.Trim();
                });
            }
        }

    }
}
