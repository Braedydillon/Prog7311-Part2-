using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prog7311_Part2.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ClientContextDatabase _context;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(ClientContextDatabase context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var clientContextDatabase = _context.ServiceRequest.Include(s => s.Contract);
            return View(await clientContextDatabase.ToListAsync());
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequest
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }


        private void PopulateContractData()
        {
            var contracts = _context.Contract.Include(c => c.Client).ToList();
            ViewBag.ContractList = contracts.Select(c => new {
                Id = c.ContractId,
                Display = $"{c.Client?.Name} - ID: {c.ContractId} - Start: {c.StartDate:yyyy-MM-dd}",
                Stat = c.Status.ToString()
            }).ToList();
        }
        private void GetCurrency()
        {
            var currencies = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures)
                // 1. Filter out neutral cultures and the Invariant Culture (ID 127 / 0x7F)
                .Where(c => !c.IsNeutralCulture && c.LCID != 127)
                .Select(c => {
                    try
                    {
                        return new System.Globalization.RegionInfo(c.Name).ISOCurrencySymbol;
                    }
                    catch
                    {
                        return null; // Skip anything that fails
                    }
                })
                .Where(symbol => !string.IsNullOrEmpty(symbol))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Currencies = new SelectList(currencies);
        }



        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            PopulateContractData();
            GetCurrency();
            return View();

        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceRequestId,Description,Status,Cost,ContractId")] ServiceRequest serviceRequest, string SourceCurrency)
        {
            var contract = await _context.Contract.FindAsync(serviceRequest.ContractId);
            bool isBlocked = contract?.Status == ContractStatus.Expired || contract?.Status == ContractStatus.OnHold;

            if (isBlocked)
            {
                ModelState.AddModelError("", "Cannot create requests for Expired or On-Hold contracts!");
            }

            if (ModelState.IsValid && !isBlocked)
            {
                // --- THE FIX IS HERE ---
                // 1. Call the service to get the ZAR value
                decimal zarAmount = await _currencyService.ConvertToZAR(serviceRequest.Cost, SourceCurrency);

                // 2. OVERWRITE the Cost with the ZAR version
                serviceRequest.Cost = zarAmount;

                // 3. NOW save to the database
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateContractData();
            GetCurrency();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequest.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Status", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceRequestId,Description,Status,Cost,ContractId")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.ServiceRequestId))
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
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Status", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequest
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequest.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequest.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequest.Any(e => e.ServiceRequestId == id);
        }
    }
}
