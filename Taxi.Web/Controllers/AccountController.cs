﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;
using Taxi.Web.Models;

namespace Taxi.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly DataContext _context;

        public AccountController(
            IUserHelper userHelper,
            IImageHelper imageHelper,
            ICombosHelper combosHelper,
            IConfiguration configuration,
            IMailHelper mailHelper,
            DataContext context)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _combosHelper = combosHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Error al iniciar sesión.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                UserTypes = _combosHelper.GetComboRoles()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.PictureFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.PictureFile, "users");
                }

                UserEntity user = await _userHelper.AddUserAsync(model, path);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El correo ya está en uso.");
                    model.UserTypes = _combosHelper.GetComboRoles();
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                var response = _mailHelper.SendMail(model.Username, "Correo de confirmación",
                    $"<h1>Correo de confirmación</h1>" +
                    $"Para habilitar al usuario, " +
                    $"por favor de clic en el enlace:</br></br><a href = \"{tokenLink}\">Confirmar correo</a>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar al usuario, han sido enviadas al correo.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }

            model.UserTypes = _combosHelper.GetComboRoles();
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            UserEntity user = await _userHelper.GetUserAsync(User.Identity.Name);
            EditUserViewModel model = new EditUserViewModel
            {
                Address = user.Address,
                RFC = user.RFC,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PicturePath = user.PicturePath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.PicturePath;

                if (model.PictureFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.PictureFile, "Users");
                }

                UserEntity user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.RFC = model.RFC;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.PicturePath = path;

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user != null)
            {
                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ChangeUser");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            UserEntity user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            Microsoft.AspNetCore.Identity.IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserEntity user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El correo no corresponde a un usuario registrado.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "Taxi Password Reset", $"<h1>Taxi Password Reset</h1>" +
                    $"Para recuperar su contraseña de clic en el enlace:</br></br>" +
                    $"<a href = \"{link}\">Recuperar Contraseña</a>");
                ViewBag.Message = "Las instrucciones para recuperar su contraseña han sido enviadas a su correo.";
                return View();

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            UserEntity user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                Microsoft.AspNetCore.Identity.IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "La contraseña se reestableció satisfactoriamente.";
                    return View();
                }

                ViewBag.Message = "Error al reestablecer la contraseña.";
                return View(model);
            }

            ViewBag.Message = "Usuario no encontrado.";
            return View(model);
        }

        public async Task<IActionResult> ConfirmUserGroup(int requestId, string token)
        {
            if (requestId == 0 || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            UserGroupRequestEntity userGroupRequest = await _context.UserGroupRequests
                .Include(ugr => ugr.ProposalUser)
                .Include(ugr => ugr.RequiredUser)
                .FirstOrDefaultAsync(ugr => ugr.Id == requestId &&
                                            ugr.Token == new Guid(token));
            if (userGroupRequest == null)
            {
                return NotFound();
            }

            await AddGroupAsync(userGroupRequest.ProposalUser, userGroupRequest.RequiredUser);
            await AddGroupAsync(userGroupRequest.RequiredUser, userGroupRequest.ProposalUser);

            userGroupRequest.Status = UserGroupStatus.Accepted;
            _context.UserGroupRequests.Update(userGroupRequest);
            await _context.SaveChangesAsync();
            return View();
        }

        private async Task AddGroupAsync(UserEntity proposalUser, UserEntity requiredUser)
        {
            UserGroupEntity userGroup = await _context.UserGroups
                .Include(ug => ug.Users)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(ug => ug.User.Id == proposalUser.Id);
            if (userGroup != null)
            {
                UserGroupDetailEntity user = userGroup.Users.FirstOrDefault(u => u.User.Id == requiredUser.Id);
                if (user == null)
                {
                    userGroup.Users.Add(new UserGroupDetailEntity { User = requiredUser });
                }

                _context.UserGroups.Update(userGroup);
            }
            else
            {
                _context.UserGroups.Add(new UserGroupEntity
                {
                    User = proposalUser,
                    Users = new List<UserGroupDetailEntity>
                    {
                        new UserGroupDetailEntity { User = requiredUser }
                    }
                });
            }
        }

        public async Task<IActionResult> RejectUserGroup(int requestId, string token)
        {
            if (requestId == 0 || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            UserGroupRequestEntity userGroupRequest = await _context
                .UserGroupRequests.FirstOrDefaultAsync(ugr => ugr.Id == requestId &&
                                                              ugr.Token == new Guid(token));
            if (userGroupRequest == null)
            {
                return NotFound();
            }

            userGroupRequest.Status = UserGroupStatus.Rejected;
            _context.UserGroupRequests.Update(userGroupRequest);
            await _context.SaveChangesAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.Trips)
                .Where(u => u.UserType == UserType.User)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync());
        }


    }

}
