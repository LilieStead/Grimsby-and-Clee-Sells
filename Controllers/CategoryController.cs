using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Grimsby_and_Clee_Sells.UserSession;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text;

namespace Grimsby_and_Clee_Sells.Controllers
{
    public class CategoryController : Controller
    {
        private readonly GacsDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(GacsDbContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
        }


        [HttpGet ("getallcategory")]
        public IActionResult GetCategories() 
        {
            var CategoryDM = _categoryRepository.GetAllCategory();
            if (CategoryDM  == null)
            {
                return NotFound();
            }
            return Ok(CategoryDM);
        }
    }
}
