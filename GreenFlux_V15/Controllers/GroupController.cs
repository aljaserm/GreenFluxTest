using GreenFlux_V15.Data;
using GreenFlux_V15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GreenFlux_V15.Controllers
{
    /// <summary>
    /// Represents a controller for managing groups of charge stations.
    /// </summary>
    public class GroupController : Controller
    {
        private readonly ChargingDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public GroupController(ChargingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all groups.
        /// </summary>
        /// <returns>The view displaying a list of groups.</returns>
        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups.ToListAsync();
            return View(groups);
        }

        /// <summary>
        /// Displays the view to create a new group.
        /// </summary>
        /// <returns>The view to create a new group.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of the new group form.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <returns>The view result based on the create action's outcome.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CapacityInAmps")] Group group)
        {
            if (ModelState.IsValid)
            {
                if (!IsGroupCapacityValid(group))
                {
                    ModelState.AddModelError("CapacityInAmps", "Capacity must be greater than or equal to the sum of MaxCurrent values of Connectors.");
                    return View(group);
                }

                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        /// <summary>
        /// Displays the view to edit a group.
        /// </summary>
        /// <param name="id">The ID of the group to edit.</param>
        /// <returns>The view to edit a group.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            ViewBag.CanAddChargeStation = CanChargeStationBeAddedToGroup(id.Value);
            ViewBag.CanRemoveChargeStation = CanChargeStationBeRemovedFromGroup(id.Value);

            return View(group);
        }

        /// <summary>
        /// Handles the submission of the group edit form.
        /// </summary>
        /// <param name="id">The ID of the group to edit.</param>
        /// <param name="group">The updated group data.</param>
        /// <returns>The view result based on the edit action's outcome.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CapacityInAmps")] Group group)
        {
            if (id != group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Use async/await for database access
                var dbGroup = await _context.Groups.FindAsync(group.Id);
                if (dbGroup == null)
                {
                    return NotFound();
                }

                if (!IsGroupCapacityValid(group))
                {
                    ModelState.AddModelError("CapacityInAmps", "Capacity must be greater than or equal to the sum of MaxCurrent values of Connectors.");
                    return View(group);
                }

                // Use async/await for database update
                _context.Update(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(group);
        }

        /// <summary>
        /// Displays the details of a group.
        /// </summary>
        /// <param name="id">The ID of the group to view details for.</param>
        /// <returns>The view to display group details.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        /// <summary>
        /// Displays the view to delete a group.
        /// </summary>
        /// <param name="id">The ID of the group to delete.</param>
        /// <returns>The view to delete a group.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        /// <summary>
        /// Handles the submission of the group delete form.
        /// </summary>
        /// <param name="id">The ID of the group to delete.</param>
        /// <returns>The view result based on the delete action's outcome.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups.Include(g => g.ChargeStations)
                                             .ThenInclude(cs => cs.Connectors)
                                             .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            _context.Connectors.RemoveRange(group.ChargeStations.SelectMany(cs => cs.Connectors));
            _context.ChargeStations.RemoveRange(group.ChargeStations);
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Private methods...
        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }

        private bool IsGroupCapacityValid(Group group)
        {
            int totalMaxCurrent = group.ChargeStations?.SelectMany(cs => cs.Connectors).Sum(c => c.MaxCurrentInAmps) ?? 0;
            return group.CapacityInAmps >= totalMaxCurrent;
        }

        private bool IsChargeStationAlreadyInGroup(int groupId)
        {
            return _context.ChargeStations.Any(cs => cs.GroupId == groupId);
        }

        private bool CanChargeStationBeAddedToGroup(int groupId)
        {
            return !IsChargeStationAlreadyInGroup(groupId);
        }

        private bool CanChargeStationBeRemovedFromGroup(int chargeStationId)
        {
            return !_context.Connectors.Any(c => c.ChargeStationId == chargeStationId);
        }
    }
}
