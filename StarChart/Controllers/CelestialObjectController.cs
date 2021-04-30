
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


		[HttpPost]
		public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
			_context.CelestialObjects.Add(celestialObject);
			_context.SaveChanges();

			return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

		[HttpPut("{id}")]
		public IActionResult Update(int id, CelestialObject celestialObject)
        {
			var oldData = _context.CelestialObjects.Find(id);

			if (oldData == null)
				return NotFound();

			oldData.Name = celestialObject.Name;
			oldData.OrbitalPeriod = celestialObject.OrbitalPeriod;
			oldData.OrbitedObjectId = celestialObject.OrbitedObjectId;

			_context.CelestialObjects.Update(oldData);
			_context.SaveChanges();

			return NoContent();
        }

		[HttpPatch("{id}/{name}")]
		public IActionResult RenameObject(int id, string name)
        {
			var body = _context.CelestialObjects.Find(id);

			if (body == null)
				return NotFound();

			body.Name = name;
			_context.CelestialObjects.Update(body);
			_context.SaveChanges();

			return NoContent();
        }

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
        {
			var root = _context.CelestialObjects.Find(id);

			if (root == null)
				return NotFound();

			var itemsToRemove = new List<CelestialObject>();
			itemsToRemove.Add(root);

			for (int i = 0; i < itemsToRemove.Count; i++)
            {
				var children = _context.CelestialObjects.Where(c => c.OrbitedObjectId == itemsToRemove[i].Id);
				itemsToRemove.AddRange(children);
            }

			_context.CelestialObjects.RemoveRange(itemsToRemove);
			_context.SaveChanges();

			return NoContent();
        }

    }
}
