using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessionalAPI.Data;
using ProfessionalAPI.DTOs.Products;
using ProfessionalAPI.Models;
using System.Security.Claims;

namespace ProfessionalAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/products?page=1&size=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var query = _db.Products
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size).Take(size)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock))
            .ToListAsync();

        return Ok(new { total, page, size, items });
    }

    // GET api/products/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p is null || !p.IsActive) return NotFound();
        return Ok(new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock));
    }

    // POST api/products
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            UserId = CurrentUserId
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock));
    }

    // PUT api/products/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null || !product.IsActive) return NotFound();
        if (product.UserId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/products/{id}  (soft delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null || !product.IsActive) return NotFound();
        if (product.UserId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        product.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}