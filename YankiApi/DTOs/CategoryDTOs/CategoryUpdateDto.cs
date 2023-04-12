using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.ProductDTOs;
using YankiApi.Entities;
using YankiApi.Extentions;
using YankiApi.Helpers;

namespace YankiApi.DTOs.CategoryDTOs
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        /// <summary>
        /// Category Title
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

        public class CategoryUpdateDtoValidatio : AbstractValidator<CategoryUpdateDto>
        {
            public CategoryUpdateDtoValidatio(AppDbContext context, IWebHostEnvironment webHostEnvironment)
            {
                RuleFor(r => r).Custom(async (r, validate) =>
                {
                    if (r.Id == null) validate.AddFailure("Yalnis ID");

                    Category category =  context.Categories.FirstOrDefault(c => c.Id == r.Id && !c.IsDeleted );

                    if (category == null) validate.AddFailure("Yalnis ID");
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
                        FileHelpers.DeleteFile(category.Image, webHostEnvironment, "assets", "img", "category");

                        category.Image = await r.ImageFile.CreateFileAsync(webHostEnvironment, "assets", "img", "category");
                    }
                    category.UpdatetAt = DateTime.UtcNow.AddDays(4);
                    category.UpdatetBy = "Admin";
                    category.Name = r.Name.Trim();
                });
            }
        }
    }
}
