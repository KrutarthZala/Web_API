using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Product_CRUD_Web_API.Implementation;
using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Product_CRUD_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region User Registration
        [HttpPost("register")]
        public IActionResult Registration(string userName, string userEmail, string userPassword)
        {
            // Validate all empty fields or not
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword))
            {
                return BadRequest(new { StatusCode=400, Message = "Please fill all fields." });
            }

            UserModel user = _unitOfWork.User.Get(u => u.UserEmail == userEmail);

            // Check if the user with the same email already exists 
            if (user is not null)
            {
                return Conflict(new {StatusCode=409, Message = "User with the same email already exists" });
            }

            var newUser = new UserModel
            {
                UserName = userName,
                UserEmail = userEmail,
                UserPassword = userPassword
            };

            _unitOfWork.User.Add(newUser);
            _unitOfWork.Save();

            return Ok(new {StatusCode=200, Message = "User Registered Successfully" });
        }
        #endregion

        #region User Log In
        [HttpPost("login")]
        public IActionResult Login(string userEmail, string userPassword)
        {
            // Validate empty fields or not
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword))
            {
                return BadRequest(new { StatusCode=400, Message = "Email and password are required" });
            }

            // Retrieve user from database by email
            var user = _unitOfWork.User.Get(u => u.UserEmail == userEmail);

            // Check if user exists and password matches
            if (user == null || user.UserPassword != userPassword)
            {
                return Unauthorized(new {StatusCode=401, Message = "Invalid email or password" });
            }

            // Authentication successful
            return Ok(new { StatusCode=200, Message = "Logged in successful" });
        }
        #endregion

        #region User Registration with JWT token
        [HttpPost("registerWithJWTToken")]
        public IActionResult RegistrationWithJWTToken(string userName, string userEmail, string userPassword)
        {
            // Validate fields are empty or not
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword))
            {
                return BadRequest(new { StatusCode = 400, Message = "Please fill all fields." });
            }
            UserModel user = _unitOfWork.User.Get(u => u.UserEmail == userEmail);

            if (user is not null)
            {
                return Conflict(new { StatusCode = 409, Message = "User with the same email already exists" });
            }
            // Generate JWT token for the registered user
            var JWTToken = GenerateJWTToken(userEmail);

            var newUser = new UserModel
            {
                UserName = userName,
                UserEmail = userEmail,
                UserPassword = userPassword,
                UserJWTToken = JWTToken,
                JWTTokenIssueDate = DateTime.Now,
                JWTTokenExpiryDate = DateTime.Now.AddDays(7)
            };

            _unitOfWork.User.Add(newUser);
            _unitOfWork.Save();


            return Ok(new {StatusCode=200,Message="User Registered Successfully", Token = JWTToken });

        }
        #endregion

        #region Log In With JWT Token
        [HttpPost("logInWithJWTToken")]
        public IActionResult LogInWithJWTToken(string userEmail, string userPassword)
        {
    
            // If the registration token is valid, check email and password as well
            var user = _unitOfWork.User.Get(u => u.UserEmail == userEmail && u.UserPassword == userPassword);
            if (user == null)
            {
                return Unauthorized(new { StatusCode = 401, Message = "Invalid  email or password" });
            }

            var registrationToken = user.UserJWTToken;
            if (registrationToken != null && user.JWTTokenExpiryDate > DateTime.Now)
            {
                // Validate Registration token
                var decodedToken = ValidateRegistrationToken(registrationToken);
                if (decodedToken == null)
                {
                    return Unauthorized(new { StatusCode = 401, Message = "Invalid  registration token" });
                }
            }
            else
            {
                return BadRequest(new { StatusCode = 400, Message = "Your token was null or expired." });
            }


            // Return the JWT token to the client
            return Ok(new
            {
                StatusCode=200,
                Message="User Logged In Successfully",
                Token = registrationToken,
                ProtectedResource = "This is protected resource."
            });
        }
        #endregion

        #region Generate JWT token
        // Method to generate JWT token for the user
        private string GenerateJWTToken(string userEmail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("5^8xZ2@3w!sE7*rQahjsahdjhuf567werqsdf");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, userEmail)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region Validate Registration Token
        private ClaimsPrincipal ValidateRegistrationToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("5^8xZ2@3w!sE7*rQahjsahdjhuf567werqsdf")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Disable clock skew
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
