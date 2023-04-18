using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [ApiController]
    [Route("/api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CountriesController(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsyc()
        {
            return Ok(await _dataContext.Countries.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsyc(Country country)
        {
            _dataContext.Add(country);
            await _dataContext.SaveChangesAsync();
            return Ok(country);
        }
    }
}
