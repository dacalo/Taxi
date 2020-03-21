using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Taxi.Web.Data.Entities
{
    public class TripDetailEntity
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime DateLocal => Date.ToLocalTime();

        [Display(Name ="Domicilio")]
        [MaxLength(500, ErrorMessage ="El campo {0} debe tener hasta {1} caracteres.")]
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TripEntity Trip { get; set; }
    }
}
