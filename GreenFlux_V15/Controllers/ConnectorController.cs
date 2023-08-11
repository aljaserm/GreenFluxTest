using GreenFlux_V15.Data;
using GreenFlux_V15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GreenFlux_V15.Controllers
{
    /// <summary>
    /// Represents a controller for managing connectors.
    /// </summary>
    public class ConnectorController : Controller
    {
        private readonly ChargingDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ConnectorController(ChargingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all connectors.
        /// </summary>
        /// <returns>The view displaying a list of connectors.</returns>
        public async Task<IActionResult> Index()
        {
            var connectors = await _context.Connectors.ToListAsync();
            return View(connectors);
        }

        /// <summary>
        /// Displays the view to create a new connector.
        /// </summary>
        /// <returns>The view to create a new connector.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of the new connector form.
        /// </summary>
        /// <param name="connector">The connector to create.</param>
        /// <returns>The view result based on the create action's outcome.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaxCurrentInAmps,ChargeStationId")] Connector connector)
        {
            if (ModelState.IsValid)
            {
                var chargeStation = await _context.ChargeStations.FindAsync(connector.ChargeStationId);
                if (chargeStation == null)
                {
                    ModelState.AddModelError("ChargeStationId", "Invalid ChargeStationId");
                    return View(connector);
                }

                _context.Connectors.Add(connector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(connector);
        }

        /// <summary>
        /// Displays the view to edit a connector.
        /// </summary>
        /// <param name="id">The ID of the connector to edit.</param>
        /// <returns>The view to edit a connector.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var connector = await _context.Connectors.FindAsync(id);
            if (connector == null)
            {
                return NotFound();
            }

            return View(connector);
        }

        /// <summary>
        /// Handles the submission of the connector edit form.
        /// </summary>
        /// <param name="id">The ID of the connector to edit.</param>
        /// <param name="connector">The updated connector data.</param>
        /// <returns>The view result based on the edit action's outcome.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaxCurrentInAmps,ChargeStationId")] Connector connector)
        {
            if (id != connector.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Use async/await for database access
                var chargeStation = await _context.ChargeStations.FindAsync(connector.ChargeStationId);
                if (chargeStation == null)
                {
                    ModelState.AddModelError("ChargeStationId", "Invalid ChargeStationId");
                    return View(connector);
                }

                // Use async/await for database update
                _context.Update(connector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(connector);
        }

        /// <summary>
        /// Displays the view to delete a connector.
        /// </summary>
        /// <param name="id">The ID of the connector to delete.</param>
        /// <returns>The view to delete a connector.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var connector = await _context.Connectors.FindAsync(id);
            if (connector == null)
            {
                return NotFound();
            }

            return View(connector);
        }

        /// <summary>
        /// Handles the submission of the connector delete form.
        /// </summary>
        /// <param name="id">The ID of the connector to delete.</param>
        /// <returns>The view result based on the delete action's outcome.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var connector = await _context.Connectors.FindAsync(id);
            if (connector == null)
            {
                return NotFound();
            }

            _context.Connectors.Remove(connector);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConnectorExists(int id)
        {
            return _context.Connectors.Any(e => e.Id == id);
        }
    }
}
