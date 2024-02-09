﻿using Grimsby_and_Clee_Sells.Data;
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
            try
            {
                var CategoryDM = _categoryRepository.GetAllCategory();
                if (CategoryDM == null)
                {//find all category and if none are found return not found 
                    return NotFound();
                }
                // if found return them all
                return Ok(CategoryDM);
            }
            //if API gets to this point it means it could not reach the database
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }
    }
}
