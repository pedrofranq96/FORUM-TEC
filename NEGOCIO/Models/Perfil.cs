using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEGOCIO.Models
{
    public class Perfil
    {

        [Key]
        [Display(Name = "Email do usuário")]
        public string? EmailUsuario { get; set; }

        [Display(Name = "Descrição")]
        public string Texto { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/mm/yyyy")]
        [Display(Name = "Aniversário")]
        public DateTime Aniversario { get; set; }

        [Display(Name = "Perfil criado em")]
        [DataType(DataType.Date)]
        public DateTime Criacao { get; set; }

        [Display(Name = "Foto")]
        public string? CaminhoImg { get; set; }
    }
}
