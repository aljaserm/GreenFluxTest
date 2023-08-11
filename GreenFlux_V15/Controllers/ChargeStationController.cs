using GreenFlux_V15.Data;
using GreenFlux_V15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GreenFlux_V15.Controllers
{
    /// <summary>
    /// Represents a controller for managing charge stations.
    /// </summary>
    public class ChargeStationController : Controller
    {
        private readonly ChargingDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargeStationController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ChargeStationController(ChargingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all charge stations.
        /// </summary>
        /// <returns>The view displaying a list of charge stations.</returns>
        public async Task<IActionResult> Index()
        {
            var chargeStations = await _context.ChargeStations.Include(cs => cs.Group).ToListAsync();
            return View(chargeStations);
        }

        /// <summary>
        /// Displays the view to create a new charge station.
        /// </summary>
        /// <returns>The view to create a new charge station.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Groups = _context.Groups.ToList();
            return View();
        }

        /// <summary>
        /// Handles the submission of the new charge station form.
        /// </summary>
        /// <param name="chargeStation">The charge station to create.</param>
        /// <returns>The view result based on the create action's outcome.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CapacityInAmps,GroupId")] ChargeStation chargeStation)
        {
            if (ModelState.IsValid)
            {
                var group = await _context.Groups.FindAsync(chargeStation.GroupId);
                if (group == null)
                {
                    ModelState.AddModelError("GroupId", "Invalid GroupId");
                    ViewBag.Groups = _context.Groups.ToList();
                    return View(chargeStation);
                }

                _context.ChargeStations.Add(chargeStation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Groups = _context.Groups.ToList();
            return View(chargeStation);
        }

        /// <summary>
        /// Displays the view to edit a charge station.
        /// </summary>
        /// <param name="id">The ID of the charge station to edit.</param>
        /// <returns>The view to edit a charge station.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chargeStation = await _context.ChargeStations.FindAsync(id);
            if (chargeStation == null)
            {
                return NotFound();
            }

            ViewBag.Groups = _context.Groups.ToList();
            return View(chargeStation);
        }

        /// <summary>
        /// Handles the submission of the charge station edit form.
        /// </summary>
        /// <param name="id">The ID of the charge station to edit.</param>
        /// <param name="chargeStation">The updated charge station data.</param>
        /// <returns>The view result based on the edit action's outcome.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CapacityInAmps,GroupId")] ChargeStation chargeStation)
        {
            if (id != chargeStation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Use async/await for database access
                var group = await _context.Groups.FindAsync(chargeStation.GroupId);
                if (group == null)
                {
                    ModelState.AddModelError("GroupId", "Invalid GroupId");
                    ViewBag.Groups = await _context.Groups.ToListAsync();
                    return View(chargeStation);
                }

                // Use async/await for database update
                _context.Update(chargeStation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Use async/await for populating ViewBag
            ViewBag.Groups = await _context.Groups.ToListAsync();
            return View(chargeStation);
        }

        /// <summary>
        /// Displays the view to delete a charge station.
        /// </summary>
        /// <param name="id">The ID of the charge station to delete.</param>
        /// <returns>The view to delete a charge station.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chargeStation = await _context.ChargeStations.Include(cs => cs.Group).FirstOrDefaultAsync(cs => cs.Id == id);
            if (chargeStation == null)
            {
                return NotFound();
            }

            return View(chargeStation);
        }

        /// <summary>
        /// Handles the submission of the charge station delete form.
        /// </summary>
        /// <param name="id">The ID of the charge station to delete.</param>
        /// <returns>The view result based on the delete action's outcome.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chargeStation = await _context.ChargeStations.FindAsync(id);
            if (chargeStation == null)
            {
                return NotFound();
            }

            _context.ChargeStations.Remove(chargeStation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChargeStationExists(int id)
        {
            return _context.ChargeStations.Any(cs => cs.Id == id);
        }
    }
}
