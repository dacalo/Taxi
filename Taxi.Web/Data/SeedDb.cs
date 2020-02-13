using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Web.Data.Entities;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckTaxisAsync();
        }

        private async Task CheckTaxisAsync()
        {
            if (!_context.Taxis.Any())
            {
                _context.Taxis.Add(new TaxiEntity
                {
                    Plaque = "DCL123",
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "FCA UNAM",
                            Target = "FES Cuautitlán",
                            Remarks = "Muy buen servicio"
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "CU",
                            Target = "Prepa 4",
                            Remarks = "Conductor muy amable"
                        }
                    }
                });
                _context.Taxis.Add(new TaxiEntity
                {
                    Plaque = "LCD321",
                    Trips = new List<TripEntity> 
                    { 
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "Dr Velasco",
                            Target = "Dr Martínez del Río",
                            Remarks = "Muy buen servicio"
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8F,
                            Source = "Dinamarca",
                            Target = "Chihuahua",
                            Remarks = "Excelente platica"
                        }
                    }
                });
                await _context.SaveChangesAsync();
            }
        }
    }
}
