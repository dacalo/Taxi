using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Taxi.Web.Data.Entities
{
    public class TripEntity
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha inicial")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha inicial")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime StartDateLocal => StartDate.ToLocalTime();

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha final")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha final")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime? EndaDateLocal => EndDate?.ToLocalTime();

        [Display(Name = "Origen")]
        [MaxLength(100, ErrorMessage = "El campo {0} no debe ser mayor a {1} caracteres.")]
        public string Source { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} no debe ser mayor a {1} caracteres.")]
        public string Target { get; set; }

        [Display(Name = "Calificación")]
        public float Qualification { get; set; }

        public double SourceLatitude { get; set; }
        public double SourceLongitude { get; set; }
        public double TargetLatitude { get; set; }
        public double TargetLongitude { get; set; }

        [Display(Name = "Comentarios")]
        public string Remarks { get; set; }
        public TaxiEntity Taxi { get; set; }
        public ICollection<TripDetailEntity> TripDetails { get; set; }
        public UserEntity User { get; set; }
    }
}
