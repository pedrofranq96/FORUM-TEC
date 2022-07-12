
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NEGOCIO.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        // GET: api/<UsuariosController>
        [HttpGet]
        public ActionResult Get()
        {
            var users = _userManager.Users.ToList();

            List<Usuario> usuarios = new List<Usuario>();

            foreach (var user in users)
            {
                var claimResult = _userManager.GetClaimsAsync(user).Result;
                var claimNome = claimResult.FirstOrDefault(c => c.Type == "Nome");
                var nome = claimNome != null ? claimNome.Value : String.Empty;

                usuarios.Add(new Usuario
                {
                    Nome = nome,
                    Email = user.Email,
                    Senha = ""
                });
            }

            return Ok(usuarios);
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }



        // PUT api/<UsuariosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsuariosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // POST api/<UsuariosController>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<Usuario> Post([FromBody] Usuario usuario)
        {
            var user = new IdentityUser { UserName = usuario.Email, Email = usuario.Email };
            var result = _userManager.CreateAsync(user, usuario.Senha).Result;

            var claimResult = _userManager.AddClaimAsync(user, new Claim("Nome", usuario.Nome)).Result;

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.First());
            }

            if (!claimResult.Succeeded)
            {
                return BadRequest(claimResult.Errors.First());
            }
            return CreatedAtAction(nameof(Post), new { id = user.Id }, user);

        }

        // Gerador de token
        // api/usuarios/token
        [AllowAnonymous]
        [HttpPost("token")]
        public string PostToken([FromBody] Usuario usuario)
        {
            
            var user = _userManager.FindByEmailAsync(usuario.Email).Result;

            if (user == null)
            {
                return "Usuário não existe";
            }


            
            if (!_userManager.CheckPasswordAsync(user, usuario.Senha).Result)
            {
                return "Senha inválida";
            }

           
            var key = Encoding.UTF8.GetBytes("geqwa5qe@%!@@RVqwcvr2#$V24a515");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
               
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}