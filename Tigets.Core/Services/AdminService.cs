using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Tigets.Core.Models;

namespace Tigets.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly Lazy<Reading> _reading;

        public AdminService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMapper mapper
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _reading = new Lazy<Reading>();
        }

        public async Task Login(string username, string password)
        {
            if (username is null)
                throw new ArgumentNullException($"{nameof(username)}");

            if (password is null)
                throw new ArgumentNullException($"{nameof(password)}");

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                throw new Exception("User does not exist.");

            if (!await _userManager.CheckPasswordAsync(user, password))
                throw new Exception("Incorrect password.");

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (!result.Succeeded)
                throw new Exception(result.ToString());
        }


        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Admin> GetProfileData(string username)
        {
            var userData = await _userManager.FindByNameAsync(username);
            if (userData is null)
            {
                throw new Exception("User does not exist.");
            }

            return _mapper.Map<Admin>(userData);
        }

        public string GetAppInfo()
        {
            return _reading.Value.AppInfo;
        }

    }
}
