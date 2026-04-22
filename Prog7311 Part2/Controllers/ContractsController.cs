using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Repositories;

namespace Prog7311_Part2.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractRepository _repo;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ClientContextDatabase _context; // Keep ONLY for SelectLists

        public ContractsController(IContractRepository repo, IWebHostEnvironment hostEnvironment, ClientContextDatabase context)
        {
            _repo = repo;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(DateTime? start, DateTime? end, ContractStatus? status)
        {
            var data = await _repo.GetFilteredAsync(start, end, status);
            return View(data);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _repo.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name");

            // This converts your Enum into a list the dropdown can actually read
            var statusList = Enum.GetValues(typeof(ContractStatus))
                                 .Cast<ContractStatus>()
                                 .Select(s => new SelectListItem
                                 {
                                     Text = s.ToString(),
                                     Value = ((int)s).ToString()
                                 }).ToList();

            ViewBag.StatusOptions = statusList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? contractFile)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(contractFile.FileName);
                string path = Path.Combine(_hostEnvironment.WebRootPath, "Contracts", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }
                contract.DocumentPath = fileName;
            }

            ModelState.Remove("Client"); // Prevents validation failure on nav property

            if (ModelState.IsValid)
            {
                await _repo.AddAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _repo.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            // This converts your Enum into a list the dropdown can actually read
            var statusList = Enum.GetValues(typeof(ContractStatus))
                                 .Cast<ContractStatus>()
                                 .Select(s => new SelectListItem
                                 {
                                     Text = s.ToString(),
                                     Value = ((int)s).ToString()
                                 }).ToList();

            ViewBag.StatusOptions = statusList;

            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,StartDate,EndDate,Status,Servicelevel,ClientId,DocumentPath")] Contract contract)
        {
            if (id != contract.ContractId) return NotFound();

            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(contract);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repo.ContractExists(contract.ContractId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _repo.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}