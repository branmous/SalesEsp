using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Helpers;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;
using System.Linq;

namespace Sales.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController : ControllerBase
	{
		private readonly DataContext _dataContext;

		public CitiesController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsyc([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.Cities.Where(x => x.State!.Id == pagination.Id);
			if (!string.IsNullOrWhiteSpace(pagination.Filter))
			{
				queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
			}

			return Ok(await queryable.OrderBy(c => c.Name).Paginate(pagination).ToListAsync());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsyc(int id)
		{
			var city = await _dataContext.Cities.FirstOrDefaultAsync(c => c.Id == id);
			if (city == null)
			{
				return NotFound();
			}
			return Ok(city);
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> GetPages([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.Cities.Where(x => x.State!.Id == pagination.Id).AsQueryable();
			if (!string.IsNullOrWhiteSpace(pagination.Filter))
			{
				queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
			}

			double count = await queryable.CountAsync();
			double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
			return Ok(totalPages);
		}

		[HttpPost]
		public async Task<IActionResult> PostAsyc(City city)
		{
			_dataContext.Add(city);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(city);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe una ciudad con el mismo nombre.");
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
		public async Task<IActionResult> PutAsyc(City city)
		{
			try
			{
				_dataContext.Update(city);
				await _dataContext.SaveChangesAsync();
				return Ok(city);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe una ciudad con el mismo nombre.");
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
			var city = await _dataContext.Cities.FirstOrDefaultAsync(c => c.Id == id);
			if (city == null)
			{
				return NotFound();
			}

			_dataContext.Remove(city);
			await _dataContext.SaveChangesAsync();
			return NoContent();
		}
	}
}
