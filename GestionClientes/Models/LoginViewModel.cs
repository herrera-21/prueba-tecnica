using System.ComponentModel.DataAnnotations;

namespace GestionClientes.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Escriba el usuario")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "Escriba la contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
