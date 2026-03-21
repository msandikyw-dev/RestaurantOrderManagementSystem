using IPB2.HW.RestaurantOrderManagementSystem.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IPB2.HW.RestaurantOrderManagementSystem.WebApiApp.Features.Report
{
    public class ReportService
    {
        private readonly AppDbContext _db;

        public ReportService(AppDbContext db)
        {
            _db = new AppDbContext();
        }

        public async Task<SalesReportResponse> GetSalesReportAsync(SalesReportRequest request)
        {
            var query = _db.Orders
                .Include(x => x.Payments)
                .AsNoTracking()
                .Where(x => x.OrderDate >= request.StartDate && x.OrderDate <= request.EndDate && x.Status != "Cancelled");

            var data = await query
                .GroupBy(x => x.OrderDate.Value.Date)
                .Select(g => new SalesReportDto
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.SelectMany(o => o.Payments).Sum(p => p.TotalAmount ?? 0)
                })
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return new SalesReportResponse
            {
                Success = true,
                Message = "Sales report generated successfully.",
                Data = data
            };
        }
    }
}
