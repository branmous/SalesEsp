using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Helpers;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly DataContext _dataContext;

		public CategoriesController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsyc([FromQuery] PaginationDTO pagination)
		{
			var queryable = await _dataContext.Categories.Paginate(pagination).ToListAsync();
			return Ok(queryable);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsyc(int id)
		{
			var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			return Ok(category);
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> GetPages([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.Categories.AsQueryable();
			double count = await queryable.CountAsync();
			double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
			return Ok(totalPages);
		}

		[HttpPost]
		public async Task<IActionResult> PostAsyc(Category category)
		{
			_dataContext.Add(category);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(category);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe una categoria con el mismo nombre.");
				}
				else
				{
					return BadRequest(dbUpdateException.InnerException.Message);
				}
			}
			catch (Exception exception)
			{
				return BadRequest(exception.Message);
			}

		}

		[HttpPut]
		public async Task<IActionResult> PutAsyc(Category category)
		{
			try
			{
				_dataContext.Update(category);
				await _dataContext.SaveChangesAsync();
				return Ok(category);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe una categoria con el mismo nombre.");
				}
				else
				{
					return BadRequest(dbUpdateException.InnerException.Message);
				}
			}
			catch (Exception exception)
			{
				return BadRequest(exception.Message);
			}

		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsyc(int id)
		{
			var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			_dataContext.Remove(category);
			await _dataContext.SaveChangesAsync();
			return NoContent();
		}
	}
}
