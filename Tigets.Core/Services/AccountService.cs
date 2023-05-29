using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Tigets.Core.Models;

namespace Tigets.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly UserManager<Admin> _adminManager;
        private readonly IMapper _mapper;
        private readonly Lazy<Reading> _reading;
        private readonly SignInManager<Admin> _signInManagerAdmin;

        public AccountService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMapper mapper,
            UserManager<Admin> adminManager,
            SignInManager<Admin> signInManagerAdmin
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _reading = new Lazy<Reading>();
            _adminManager = adminManager;
            _signInManagerAdmin = signInManagerAdmin;
        }

        public async Task AddBalance(string username, decimal amount)
        {
            if (username is null)
                throw new ArgumentNullException($"{nameof(username)}");

            // TODO: perhaps it's not a good idea to allow adding negative amount of money
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                throw new Exception("User does not exist.");

            user.Balance += amount;
            await _userManager.UpdateAsync(user);
        }

        public async Task Login(string username, string password)
        {
            if (username is null)
                throw new ArgumentNullException($"{nameof(username)}");

            if (password is null)
                throw new ArgumentNullException($"{nameof(password)}");

            var user = await _userManager.FindByNameAsync(username);

            if (user is null){
                var admin = await _adminManager.FindByNameAsync(username);
                if (admin is null)
                    throw new Exception("User does not exist.");
                else{
                    if (!await _adminManager.CheckPasswordAsync(admin, password))
                        throw new Exception("Incorrect password.");

                    var results = await _signInManagerAdmin.PasswordSignInAsync(admin, password, false, false);
                    if (!results.Succeeded)
                        throw new Exception("Failed to sign in.");
                    return;
                }
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
                throw new Exception("Incorrect password.");

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (!result.Succeeded)
                throw new Exception(result.ToString());
        }

        public async Task Register(UserPostModel userPostModel)
        {
            if (userPostModel is null)
                throw new ArgumentNullException($"{nameof(userPostModel)}");

            var user = await _userManager.FindByNameAsync(userPostModel.UserName);
            if (user != null)
                throw new Exception("User with this username already exists.");

            //                    [A-Z....]+matches at least one or more letter, numbers or symbols. 
            //                                                  () part is optional 0-m times. This is needed bacause < . > must
            //                                                     be proceeded by a char before @       At least one char after @ by [...]+ is required
            string patternText = "[A-Za-z0-9!#$%&'*+/=?^_‘{|}~-]+(.([A-Za-z0-9!#$%&'*+/=?^_‘{|}~-])+)*@[A-Za-z0-9!#$%&'*+/=?^_‘{|}~-]+(.([A-Za-z0-9!#$%&'*+/=?^_‘{|}~-])+)*.[A-Za-z0-9!#$%&'*+/=?^_‘{|}~-]";
            Regex regEmail = new Regex(patternText);                 // all in all the pattern looks something like a(.a)@a(.a).com (.a) - meaning it's an optional par

            if (!regEmail.IsMatch(userPostModel.Email)) throw new Exception("Failed : InvalidEmail");

            user = _mapper.Map<User>(userPostModel);
            user.Id = Guid.NewGuid().ToString();
            user.Balance = 0m;
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, userPostModel.Password);

            if (!result.Succeeded)
                throw new Exception(result.ToString());
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserViewModel> GetProfileData(string username)
        {
            var userData = await _userManager.FindByNameAsync(username);
            if (userData is null)
            {
                throw new Exception("User does not exist.");
            }

            return _mapper.Map<UserViewModel>(userData);
        }

        public string GetAppInfo()
        {
            return _reading.Value.AppInfo;
        }

    }
}
