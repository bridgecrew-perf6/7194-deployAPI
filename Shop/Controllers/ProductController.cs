﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return products;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices] DataContext context)
        {
            var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return product;
        }

        [HttpGet]
        [Route("{id:Int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices] DataContext context)
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post([FromBody] Product model, [FromServices] DataContext context)
        {
            if (ModelState.IsValid) 
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Delete(int id, [FromServices] DataContext context)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto excluido como sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
        }
    }
}
