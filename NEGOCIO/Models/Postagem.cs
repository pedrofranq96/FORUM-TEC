using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEGOCIO.Models
{
    public class Postagem
    {
        public Guid Id { get; set; }

        [Display(Name = "Email do usuário")]
        public string? EmailUsuario { get; set; }

        [Display(Name = "Conteúdo da publicação")]
        public string Texto { get; set; }

        [Display(Name = "Hora de criação")]
        public DateTime HoraPost { get; set; }

        [Display(Name = "Foto")]
        public string? CaminhoImg { get; set; }
    }
}
