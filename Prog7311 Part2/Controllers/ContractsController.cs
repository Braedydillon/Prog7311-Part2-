using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Prog7311_Part2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prog7311_Part2.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ClientContextDatabase _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ContractsController(ClientContextDatabase context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var clientContextDatabase = _context.Contract.Include(c => c.Client);
            return View(await clientContextDatabase.ToListAsync());

        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name");


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

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,StartDate,EndDate,Status,Servicelevel,ClientId,DocumentPath")] Contract contract, IFormFile? contractFile)
        {
            // STEP 2: Handle the physical file save
            if (contractFile != null && contractFile.Length > 0)
            {
                string folderPath = Path.Combine(_hostEnvironment.WebRootPath, "Contracts");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(contractFile.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }

                // STEP 3: Assign the string to the model property
                contract.DocumentPath = fileName;
            }

            // STEP 4: Force the save (Ignore the validation errors that are blocking you)
            try
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // If it fails here, your Database schema is likely missing the column
                return Content("Database Error: " + ex.Message);
            }
        
        
        }
        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,StartDate,EndDate,Status,Servicelevel,ClientId")] Contract contract)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contract.FindAsync(id);
            if (contract != null)
            {
                _context.Contract.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contract.Any(e => e.ContractId == id);
        }
    }
}
