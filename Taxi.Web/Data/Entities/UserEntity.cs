using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Taxi.Common.Enums;

namespace Taxi.Web.Data.Entities
{
    public class UserEntity: IdentityUser
    {
        [MaxLength(13, ErrorMessage = "El campo {0} no debe ser mayor a {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string RFC { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe ser mayor a {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe ser mayor a {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; }

        [Display(Name = "Domicilio")]
        [MaxLength(100, ErrorMessage = "El campo {0} no debe ser mayor a {1} caracteres.")]
        public string Address { get; set; }

        [Display(Name = "Foto")]
        public string PicturePath { get; set; }

        [Display(Name = "Foto")]
        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
        ? "http://10.1.114.74:83/images/noimage.png"
        : $"http://10.1.114.74:83/{PicturePath.Substring(1)}";


        [Display(Name = "User Type")]
        public UserType UserType { get; set; }
        [Display(Name = "Login Type")]
        public LoginType LoginType { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public string FullNameWithRFC => $"{FirstName} {LastName}-{RFC}";
        public ICollection<TaxiEntity> Taxis { get; set; }
        public ICollection<TripEntity> Trips { get; set; }

    }
}
