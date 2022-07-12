using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NEGOCIO.Models;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PostagensController : ControllerBase
    {
        private readonly APIContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public PostagensController(APIContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: api/Postagens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Postagem>>> GetPostagem()
        {
            if (_context.Postagem == null)
            {
                return NotFound();
            }
            return await _context.Postagem.OrderByDescending(x => x.HoraPost).ToListAsync();
            
        }



        // GET: api/Postagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Postagem>> GetPostagem(Guid id)
        {
            if (_context.Postagem == null)
            {
                return NotFound();
            }
            var postagem = await _context.Postagem.FirstOrDefaultAsync(m => m.Id == id);

            if (postagem == null)
            {
                return NotFound();
            }
            return postagem;

        }



        // PUT: api/Postagens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostagem(Guid id, Postagem postagem)
        {
            if (id != postagem.Id)
            {
                return BadRequest();
            }

            var postagens = await _context.Postagem.FindAsync(id);

            if (postagem.CaminhoImg == null)
            {

                
                postagens.Id = postagem.Id;
                postagens.Texto = postagem.Texto;
                postagens.HoraPost = DateTime.Now;
                postagens.EmailUsuario = postagem.EmailUsuario;
                postagens.CaminhoImg = postagens.CaminhoImg;
            }
            else
            {
                postagens.Id = postagem.Id;
                postagens.Texto = postagem.Texto;
                postagens.EmailUsuario = postagem.EmailUsuario;
                postagens.CaminhoImg = postagem.CaminhoImg;
                postagens.HoraPost = DateTime.Now;
            }

            try
            {
                _context.Postagem.Update(postagens);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostagemExists(id))
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

        // POST: api/Postagens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Postagem>> PostPostagem(Postagem postagem)
        {
            if (_context.Postagem == null)
            {
                return Problem("Entity set 'APIContext.Postagem'  is null.");
            }


            _context.Postagem.Add(postagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostagem", new { id = postagem.Id }, postagem);
        }

        // DELETE: api/Postagens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostagem(Guid id)
        {
            if (_context.Postagem == null)
            {
                return NotFound();
            }
            var postagem = await _context.Postagem.FindAsync(id);
            if (postagem == null)
            {
                return NotFound();
            }

            _context.Postagem.Remove(postagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostagemExists(Guid id)
        {
            return (_context.Postagem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
