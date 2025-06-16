using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIBQ.Models;

namespace SIBQ.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // POST: /Login
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return RedirectToLoginWithError("Dados inválidos.");
            }

            var user = await _userManager.FindByEmailAsync(model.Nickname);
            if (user == null)
            {
                return RedirectToLoginWithError("Usuário ou senha inválidos.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("ListaSimulados", "Simulado");
            }

            return RedirectToLoginWithError("Falha na tentativa de login. Por favor, verifique suas credenciais.");
        }

        // POST: /Logout
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        // Método privado para centralizar o redirecionamento em caso de erro no login
        private IActionResult RedirectToLoginWithError(string errorMessage)
        {
            TempData["LoginError"] = errorMessage; // Pode usar TempData para exibir mensagem na view Login
            return RedirectToAction("Login", "Home");
        }
    }
}