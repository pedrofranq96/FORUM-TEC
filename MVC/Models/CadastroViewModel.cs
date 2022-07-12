using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels
{
    public class CadastroViewModel: LoginViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite uma senha válida")]
        [StringLength(10, ErrorMessage = "A senha deve conter no mínimo 6 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme a Senha")]
        [Compare("Senha", ErrorMessage = "Senha informada não confere")]
        public string ConfirmaSenha { get; set; }
    }
}
