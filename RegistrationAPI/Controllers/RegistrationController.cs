using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
		public RegistrationController(ILogger<RegistrationController> logger, IDbContextFactory<PostgresContext> dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
		}

        [HttpPost]
        public IActionResult Post([FromBody] RegistrationData data)
        {
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
			}

			return Created("", null);
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
			using (var dbContext = _dbContextFactory.CreateDbContext())
			{
				var registration =  from r in dbContext.Registrations 
									from l in dbContext.LookupRegistrations
									where r.RegionId == l.Id
									select new { Id = r.CarId, Region = l.Region };

				if (registration == null)
				{
					return NotFound(new { message = "Registrations not found!" });
				}

				return Ok(registration.ToList());
			}
		}


	}
}
