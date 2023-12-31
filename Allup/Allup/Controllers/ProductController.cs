﻿using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing.Printing;

namespace Allup.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly int _pageSize = 8;
        public ProductController(AppDbContext context)
        {

            _context = context;
        }
        //Actions
        //1.Index
        //2.LoadMore
        //3.Search
        //4.Modal

        //Index
        public async Task<IActionResult> Index(int currentPage = 1)
        {
            ViewBag.LoadPageIndex = 1;
            IQueryable<Product> products = _context.Products
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.Id);

            return View(PageNatedList<Product>.Create(products, currentPage, _pageSize, 5));
        }
        //LoadMore
        public async Task<IActionResult> LoadMore(int? pageIndex)
        {
            if (pageIndex == null) return BadRequest();

            if (pageIndex <= 0) return BadRequest();

            IQueryable<Product> products = _context.Products
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.Id);


            int maxPage = (int)Math.Ceiling((decimal)products.Count() / _pageSize);
            if (pageIndex > maxPage) return BadRequest();

            products = products.Skip((int)pageIndex * _pageSize).Take(_pageSize);

            return PartialView("_LoadMorePartial", new List<Product>(products));
        }
        //Search
        public async Task<IActionResult> Search(string search, int? categoryId)
        {
            List<Product> products = null;
            if (categoryId != null && await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == categoryId))
            {
                products = await _context.Products
                .Where(p => p.IsDeleted == false && p.CategoryId == (int)categoryId || (
                p.Title.ToLower().Contains(search.ToLower()) ||
                p.Brand != null ? p.Brand.Name.ToLower().Contains(search.ToLower()) : true)).ToListAsync();

            }
            else
            {
                products = await _context.Products
                .Where(p => p.IsDeleted == false || (
                p.Title.ToLower().Contains(search.ToLower()) ||
                p.Brand != null ? p.Brand.Name.ToLower().Contains(search.ToLower()) : true) ||
                p.Category.Name.ToLower().Contains(search.ToLower())
                ).ToListAsync();
            }

            return PartialView("_SearchPartial", products);

        }
        //Modal
        public async Task<IActionResult> Modal(int? id)
        {
            if (id == null) return BadRequest("Id is required");

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).
                FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound("This Id does not exist");

            return PartialView("_ModalPartial", product);
        }


    }
}
