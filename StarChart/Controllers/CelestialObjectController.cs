
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {

		private readonly ApplicationDbContext _context;


        public CelestialObjectController(ApplicationDbContext context)
        {
			_context = context;
        }


		[HttpGet("{id:int}", Name = "GetById")]
		public IActionResult GetById(int id)
        {
			// Why calling Include(c => c.Satellites) causes a null reference exception is beyond me.
			var body = _context.CelestialObjects.Where(celest => celest.Id == id).FirstOrDefault();

			if (body == null)
				return NotFound();

			var loadSatellites = body.Satellites.Count;

			return Ok(body);
        }

		[HttpGet("{name}")]
		public IActionResult GetByName(string name)
        {
			// Why calling Include(c => c.Satellites) causes a null reference exception is beyond me.
			var bodies = _context.CelestialObjects.Where(b => string.Equals(b.Name, name));

			if (!bodies.Any())
				return NotFound();

			foreach (var b in bodies)
            {
				b.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == b.Id).ToList<CelestialObject>();
            }

			return Ok(bodies);
        }

		[HttpGet]
		public IActionResult GetAll()
        {
			// Why calling Include(c => c.Satellites) causes a null reference exception is beyond me.
			var bodies = _context.CelestialObjects;

			foreach (var body in bodies)
            {
				body.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == body.Id).ToList<CelestialObject>();
            }

			return Ok(bodies);
        }

    }
}
