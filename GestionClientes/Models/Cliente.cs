using System;
using System.ComponentModel.DataAnnotations;

namespace GestionClientes.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Escriba los nombres")]
        [StringLength(100)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Escriba los apellidos")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Escriba el documento")]
        [StringLength(20)]
        [Display(Name = "Documento")]
        public string Documento { get; set; }

        [EmailAddress(ErrorMessage = "El correo no tiene un formato valido")]
        [StringLength(100)]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [StringLength(20)]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }

        [StringLength(200)]
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
