using Microsoft.EntityFrameworkCore;
using Sales.API.Services;
using Sales.Shared.Entities;
using Sales.Shared.Responses;

namespace Sales.API.Data
{
	public class SeedDb
	{
		private readonly DataContext _context;
		private readonly IApiService _apiService;

		public SeedDb(DataContext context, IApiService apiService)
		{
			_context = context;
			_apiService = apiService;
		}

		public async Task SeedAsync()
		{
			await _context.Database.EnsureCreatedAsync();
			await CheckCategoriesAsync();
			await CheckCountriesAsync();
		}

		private async Task CheckCategoriesAsync()
		{
			if (!_context.Categories.Any())
			{
				_context.Categories.Add(new Category
				{
					Name = "Calzado"
				});

				_context.Categories.Add(new Category
				{
					Name = "Ropa"
				});

				_context.Categories.Add(new Category
				{
					Name = "Deportes"
				});

				_context.Categories.Add(new Category
				{
					Name = "Moda"
				});

				_context.Categories.Add(new Category
				{
					Name = "Celulares y accesorios"
				});

				_context.Categories.Add(new Category
				{
					Name = "Dormitorio"
				});

				_context.Categories.Add(new Category
				{
					Name = "Hogar y muebles"
				});

				_context.Categories.Add(new Category
				{
					Name = "Moda y accesorios"
				});

				_context.Categories.Add(new Category
				{
					Name = "Salud y belleza"
				});

				_context.Categories.Add(new Category
				{
					Name = "Ropa infantil"
				});

				_context.Categories.Add(new Category
				{
					Name = "Bebés"
				});

				_context.Categories.Add(new Category
				{
					Name = "Juguetería"
				});
			}

			await _context.SaveChangesAsync();
		}
		private async Task CheckCountriesAsync()
		{

			if (!_context.Countries.Any())
			{
				Response<List<CountryResponse>> responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
				if (responseCountries.IsSuccess)
				{
					var countries = responseCountries.Result!;
					foreach (CountryResponse countryResponse in countries)
					{
						var country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;
						if (country == null)
						{
							country = new() { Name = countryResponse.Name!, States = new List<State>() };
							Response<List<StateResponse>> responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
							if (responseStates.IsSuccess)
							{
								var states = responseStates.Result!;
								foreach (StateResponse stateResponse in states!)
								{
									var state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
									if (state == null)
									{
										state = new() { Name = stateResponse.Name!, Cities = new List<City>() };
										Response<List<CityResponse>> responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
										if (responseCities.IsSuccess)
										{
											var cities = responseCities.Result!;
											foreach (CityResponse cityResponse in cities)
											{
												if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
												{
													continue;
												}

												var city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
												if (city == null)
												{
													state.Cities.Add(new City() { Name = cityResponse.Name! });
												}
											}
										}
										if (state.CitiesNumber > 0)
										{
											country.States.Add(state);
										}
									}
								}
							}
							if (country.StatesNumber > 0)
							{
								_context.Countries.Add(country);
								await _context.SaveChangesAsync();
							}
						}
					}
				}
			}

		}
	}
}
