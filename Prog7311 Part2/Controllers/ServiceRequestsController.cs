using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Repositories;
using Prog7311_Part2.Services;
using System.Threading.Tasks; // Required for Task

namespace Prog7311_Part2.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestRepository _repo;
        private readonly IContractRepository _contractRepo;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestRepository repo,
            IContractRepository contractRepo,
            ICurrencyService currencyService)
        {
            _repo = repo;
            _contractRepo = contractRepo;
            _currencyService = currencyService;
        }
        public async Task<IActionResult> Index(string searchString, string status)
        {
            // If both parameters are empty, the repo logic should return all records.
            // If parameters are provided, it returns the filtered list.
            var requests = await _repo.GetFilteredRequestsAsync(searchString, status);
            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var serviceRequest = await _repo.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create()
        {
            // FIX: Added await
            await PopulateContractData();
            GetCurrency();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest, string SourceCurrency)
        {
            // Business Rule Validation
            if (await _repo.IsContractBlocked(serviceRequest.ContractId))
            {
                ModelState.AddModelError("", "Cannot create requests for Expired or On-Hold contracts!");
            }

            if (ModelState.IsValid)
            {
                serviceRequest.Cost = await _currencyService.ConvertToZAR(serviceRequest.Cost, SourceCurrency);
                await _repo.AddAsync(serviceRequest);
                await _repo.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // FIX: Added await
            await PopulateContractData();
            GetCurrency();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var serviceRequest = await _repo.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            // FIX: Added await
            await PopulateContractData();
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceRequestId,Description,Status,Cost,ContractId")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(serviceRequest);
                    await _repo.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _repo.GetByIdAsync(id);
                    if (exists == null) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // FIX: Added await
            await PopulateContractData();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var serviceRequest = await _repo.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();
            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractData()
        {
            // The repo now correctly returns Task<IEnumerable<Contract>>
            var contracts = await _contractRepo.GetAllWithClientsAsync();

            // SelectListItem ensures the 'object' error doesn't happen in the View
            var list = contracts.Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                // c.Client is accessible because of .Include(c => c.Client) in your Repo
                Text = $"{(c.Client != null ? c.Client.Name : "No Client")} - ID: {c.ContractId} - Status: {c.Status}"
            }).ToList();

            ViewBag.ContractList = list;
        }

        private void GetCurrency()
        {
            var currencies = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture && c.LCID != 127)
                .Select(c => {
                    try { return new System.Globalization.RegionInfo(c.Name).ISOCurrencySymbol; }
                    catch { return null; }
                })
                .Where(symbol => !string.IsNullOrEmpty(symbol))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Currencies = new SelectList(currencies);
        }
        
    }
}