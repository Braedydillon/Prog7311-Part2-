using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Repositories; 
using System.Threading.Tasks;

namespace Prog7311_Part2.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _repo; 

        public ClientsController(IClientRepository repo)
        {
            _repo = repo;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            // We use the search method from the repo. 
            // If searchString is null, the repo logic returns all clients anyway.
            var clients = await _repo.SearchClientsAsync(searchString);
            return View(clients);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _repo.GetByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Name,ContactDetails,Region")] Client client)
        {
            if (ModelState.IsValid)
            {
                await _repo.AddAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _repo.GetByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(client);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repo.ClientExists(client.ClientId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _repo.GetByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}