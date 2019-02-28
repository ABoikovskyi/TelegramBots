using System.IO;
using System.Linq;
using Aspose.Cells;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class ExportService
    {
        private readonly PopCornDbContext _context;

        public ExportService(PopCornDbContext context)
        {
            _context = context;
        }

        public MemoryStream GetUsersReport()
        {
            var workBook = new Workbook();
            var cells = workBook.Worksheets[0].Cells;
            var column = 1;
            var row = 2;
            var usersData = _context.Users.Include(u => u.Subscriptions).Include(u => u.ConcertVisits).ToList();
            var concerts = _context.Concerts.Select(c => new { c.Id, c.Artist }).ToList();

            foreach (var concert in concerts)
            {
                cells[0, column].Value = concert.Artist;
                cells[1, column].Value = "Подписка";
                cells[1, ++column].Value = "Количество посещений";
                column++;
            }

            foreach (var user in usersData)
            {
                column = 1;
                cells[row, 0].Value = $"{user.FirstName} {user.LastName}";
                foreach (var concert in concerts)
                {
                    if (user.Subscriptions.Any(s => s.ConcertId == concert.Id))
                    {
                        cells[row, column].Value = "X";
                    }
                    column++;
                    cells[row, column].Value = user.ConcertVisits.Count(v => v.ConcertId == concert.Id);
                    column++;
                }
                row++;
            }

            var memoryStream = new MemoryStream();
            workBook.Save(memoryStream, SaveFormat.Xlsx);

            return memoryStream;
        }
    }
}
