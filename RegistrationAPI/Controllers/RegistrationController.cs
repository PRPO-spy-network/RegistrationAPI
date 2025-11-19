using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistrationAPI.Classes;
using Registrations.Classes;
using Registrations.Models;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Registrations.Controllers
{
    [ApiController]
	[Route("/registration/")]
	public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
		private readonly IDbContextFactory<PostgresContext> _dbContextFactory;
		private readonly DiscordService _discord;
		private readonly IConfiguration _config;
		public RegistrationController(ILogger<RegistrationController> logger, IDbContextFactory<PostgresContext> dbContextFactory, DiscordService discord, IConfiguration config)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
			_discord = discord;
			_config = config;
		}

        [HttpPost]
        public IActionResult Post([FromBody] RegistrationData data)
        {
			try { 
				using (var dbContext = _dbContextFactory.CreateDbContext())
				{
					int? region = (from r in dbContext.LookupRegistrations
								 where r.Region == data.Region
								 select (int?)r.Id).FirstOrDefault();

					if (region == null)
					{
						return NotFound(new { message = "Region not found!" });
					}

					var dbData = new Registration { CarId = data.Id, RegionId = region };

					dbContext.Registrations.Add(dbData);
					dbContext.SaveChanges();
					_discord.SendDiscordMessageAsync(_config["WEB_HOOK_URL"]!, $"Registered car '{data.Id}' in region '{region}'");
				}

				return Created("", null);
			}catch{
				return StatusCode(500);
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			using (var dbContext = _dbContextFactory.CreateDbContext())
			{
				var registration = (from r in dbContext.Registrations
									from l in dbContext.LookupRegistrations
									where r.RegionId == l.Id && r.CarId == id
									select new {Id = r.CarId, Region = l.Region})
					.FirstOrDefault();
			
				if (registration == null)
				{
					return NotFound(new { message = "Registration not found!" });
				}

				return Ok(registration);
			}
		}

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				using (var dbContext = _dbContextFactory.CreateDbContext())
				{
					var registration = from r in dbContext.Registrations
									  join l in dbContext.LookupRegistrations
									  on r.RegionId equals l.Id
									   select new { Id = r.CarId, Region = l.Region };

					var list = registration.ToList();
					if (registration == list)
					{
						return NotFound(new { message = "Registrations not found!" });
					}

					return Ok(list);
				}
			}
			catch
			{
				return StatusCode(500);
			}
		}


	}
}
