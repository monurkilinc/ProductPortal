using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.Web.Controllers
{
    public class SupportTicketController : Controller
    {
        private readonly ISupportTicketService _ticketService;

        public SupportTicketController(ISupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _ticketService.GetAllAsync();
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupportTicket ticket)
        {
            if (ModelState.IsValid)
            {
                var result = await _ticketService.AddAsync(ticket);
                if (result.Success)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", result.Message);
            }
            return View(ticket);
        }
    }
}
