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
	public class StatesController : ControllerBase
	{
		private readonly DataContext _dataContext;

		public StatesController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsyc([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.States.Include(c => c.Cities).Where(x => x.Country!.Id == pagination.Id).Paginate(pagination);

			if (!string.IsNullOrWhiteSpace(pagination.Filter))
			{
				queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
			}


			return Ok(await queryable.ToListAsync());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsyc(int id)
		{
			var state = await _dataContext.States.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
			if (state == null)
			{
				return NotFound();
			}
			return Ok(state);
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> GetPages([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.States.Where(x => x.Country!.Id == pagination.Id).AsQueryable();
			if (!string.IsNullOrWhiteSpace(pagination.Filter))
			{
				queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
			}

			double count = await queryable.CountAsync();
			double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
			return Ok(totalPages);
		}

		[HttpPost]
		public async Task<IActionResult> PostAsyc(State state)
		{
			_dataContext.Add(state);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(state);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe un estado/departamento con el mismo nombre.");
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
		public async Task<IActionResult> PutAsyc(State state)
		{
			try
			{
				_dataContext.Update(state);
				await _dataContext.SaveChangesAsync();
				return Ok(state);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe un estado/departamento con el mismo nombre.");
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
			var state = await _dataContext.States.FirstOrDefaultAsync(c => c.Id == id);
			if (state == null)
			{
				return NotFound();
			}

			_dataContext.Remove(state);
			await _dataContext.SaveChangesAsync();
			return NoContent();
		}
	}
}
