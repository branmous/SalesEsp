﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Helpers;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class CountriesController : ControllerBase
	{
		private readonly DataContext _dataContext;

		public CountriesController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsyc([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.Countries.Include(x => x.States).AsQueryable();

			if (!string.IsNullOrWhiteSpace(pagination.Filter))
			{
				queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
			}

			return Ok(await queryable
				.OrderBy(x => x.Name)
				.Paginate(pagination)
				.ToListAsync());

		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsyc(int id)
		{
			var country = await _dataContext.Countries.Include(c => c.States!).ThenInclude(s => s.Cities).FirstOrDefaultAsync(c => c.Id == id);
			if (country == null)
			{
				return NotFound();
			}
			return Ok(country);
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> GetPages([FromQuery] PaginationDTO pagination)
		{
			var queryable = _dataContext.Countries.AsQueryable();
			if (!string.IsNullOrWhiteSpace(pagination.Filter))
			{
				queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
			}

			double count = await queryable.CountAsync();
			double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
			return Ok(totalPages);
		}

		[HttpPost]
		public async Task<IActionResult> PostAsyc(Country country)
		{
			_dataContext.Add(country);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(country);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe un país con el mismo nombre.");
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
		public async Task<IActionResult> PutAsyc(Country country)
		{
			try
			{
				_dataContext.Update(country);
				await _dataContext.SaveChangesAsync();
				return Ok(country);
			}
			catch (DbUpdateException dbUpdateException)
			{
				if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
				{
					return BadRequest("Ya existe un país con el mismo nombre.");
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
			var country = await _dataContext.Countries.FirstOrDefaultAsync(c => c.Id == id);
			if (country == null)
			{
				return NotFound();
			}

			_dataContext.Remove(country);
			await _dataContext.SaveChangesAsync();
			return NoContent();
		}
	}
}
