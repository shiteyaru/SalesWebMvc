using SalesWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;

namespace SalesWebMvc.Services {
    public class SalesRecordService {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context) {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate) {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue) {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue) {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }
        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate) {
            var query = _context.SalesRecord.AsQueryable();

            // Aplica as condições de data
            if (minDate.HasValue) {
                query = query.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue) {
                query = query.Where(x => x.Date <= maxDate.Value);
            }

            // Inclui as entidades relacionadas: Seller e Department
            var salesRecordsWithSellers = await query
                .Include(x => x.Seller)                 // Inclui a entidade Seller
                .Include(x => x.Seller.Department)      // Inclui a entidade Department
                .OrderByDescending(x => x.Date)         // Ordena por data
                .ToListAsync();                         // Executa a consulta e carrega os dados na memória

            // Agrupa os registros de vendas por Departamento
            var groupedResult = salesRecordsWithSellers
                .GroupBy(x => x.Seller.Department)        // Agrupa por Department
                .ToList();                               // Converte em lista

            return groupedResult;
        }


    }
}