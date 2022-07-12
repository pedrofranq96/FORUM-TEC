using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NEGOCIO.Models;

namespace API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilsController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PerfilsController(APIContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: api/Perfils
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Perfil>>> GetPerfil()
        {
            if (_context.Perfil == null)
            {
                return NotFound();
            }
            return await _context.Perfil.ToListAsync();
        }

        // GET: api/Perfils/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Perfil>> GetPerfil(string id)
        {
            if (_context.Perfil == null)
            {
                return NotFound();
            }
            var perfil = await _context.Perfil.FindAsync(id);

            if (perfil == null)
            {
                return NotFound();
            }

            return perfil;
        }

        // PUT: api/Perfils/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfil(string id, Perfil perfil)
        {
            if (id != perfil.EmailUsuario)
            {
                return BadRequest();
            }

            var buscaPerfil = await _context.Perfil.FindAsync(id);

            if (perfil.CaminhoImg == null)
            {
                buscaPerfil.EmailUsuario = perfil.EmailUsuario;
                buscaPerfil.Texto = perfil.Texto;
                buscaPerfil.Aniversario = perfil.Aniversario;
                buscaPerfil.Criacao = perfil.Criacao;
                buscaPerfil.CaminhoImg = buscaPerfil.CaminhoImg;
            }
            else
            {
                buscaPerfil.EmailUsuario = perfil.EmailUsuario;
                buscaPerfil.Texto = perfil.Texto;
                buscaPerfil.Aniversario = perfil.Aniversario;
                buscaPerfil.Criacao = perfil.Criacao;
                buscaPerfil.CaminhoImg = perfil.CaminhoImg;
            }

            try
            {
                _context.Perfil.Update(buscaPerfil);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerfilExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Perfils
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Perfil>> PostPerfil(Perfil perfil)
        {
            if (_context.Perfil == null)
            {
                return Problem("Entity set 'APIContext.Perfil'  is null.");
            }
            _context.Perfil.Add(perfil);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PerfilExists(perfil.EmailUsuario))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPerfil", new { id = perfil.EmailUsuario }, perfil);
        }


        // DELETE: api/Perfils/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfil(string id)
        {
            if (_context.Perfil == null)
            {
                return NotFound();
            }

            await ExcluirPerfil(id);
            var perfil = await _context.Perfil.FindAsync(id);
            if (perfil == null)
            {
                return NotFound();
            }

            _context.Perfil.Remove(perfil);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task ExcluirPerfil(string id)
        {

            var perfil = await _context.Perfil.FindAsync(id);
            var postagem = await _context.Postagem.Where(x => x.EmailUsuario == perfil.EmailUsuario).ToListAsync();

            if (postagem != null)
            {_context.Postagem.RemoveRange(postagem);}
            if (perfil != null)
            {_context.Perfil.RemoveRange(perfil);}
            await _context.SaveChangesAsync();
        }



        private bool PerfilExists(string id)
        {
            return (_context.Perfil?.Any(e => e.EmailUsuario == id)).GetValueOrDefault();
        }
    }
}
