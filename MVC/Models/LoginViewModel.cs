using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Digite um e-mail válido")]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Digite uma senha válida")]
        [StringLength(10, ErrorMessage = "A senha deve conter no mínimo 6 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }
    }

}
