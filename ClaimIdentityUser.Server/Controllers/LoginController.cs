using ClaimIdentityUser.Server.Dtos;
using ClaimIdentityUser.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClaimIdentityUser.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        /*private readonly ClaimIdentityUser _claimIdentityUser;*/
        private readonly ITokenService _token;
       
        public LoginController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService token
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _token = token;
        }

        [HttpGet("AuthorizeData")]
        [Authorize]
        public string Get()
        {
            return "hello";
        }


        /*Login via user name*/
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            
            var result = await _signInManager.
                PasswordSignInAsync
                (login.UserName, login.password, true, lockoutOnFailure: false);


            
            if (result.Succeeded)
            {
                var claimdata = new ClaimsDto
                {
                    username = login.UserName,
                    role="Admin"
                };
                var token = await _token.GenerateToken(claimdata);
                return Ok(token);
            }
            else
            {
                return BadRequest("login failed");
            }
        }

      


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto user)
        {
            //check if user is valid or not
            if (user.Email == null && user.UserName==null) {
                return BadRequest(new {status=400 , message="Please Enter user name and password"});
            }

            //check if user exists or not
            
            var userexitance =await _userManager.FindByEmailAsync(user.Email);
            if(userexitance != null)
            {
                return BadRequest(new {status=400,message="user aready exits"});
            }
                
            

            var userdata = new IdentityUser
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            //add user 
            var result=await _userManager.CreateAsync(userdata, user.Password);

            if (result.Succeeded)
            {
                //add to role
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _userManager.AddToRoleAsync(userdata, "Admin");
                return Ok(new { status = 200, message = "User is Created Sucessfully", existance = userexitance });
            }
            

            return BadRequest(new {staus=400,message="User Name should be unique"});
            
            
        }

 
        

    }
}
