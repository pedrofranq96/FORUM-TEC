using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace API.Data
{
    public class APIContext : IdentityDbContext<IdentityUser>
    {
        public APIContext (DbContextOptions<APIContext> options)
            : base(options)
        {
        }

        public DbSet<NEGOCIO.Models.Perfil>? Perfil { get; set; }

        public DbSet<NEGOCIO.Models.Postagem>? Postagem { get; set; }
    }
}
