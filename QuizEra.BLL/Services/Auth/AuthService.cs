using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.DTOs.Auth;
using QuizEra.DAL.DataBase;
using Microsoft.AspNetCore.Identity;
using QuizEra.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Auth
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly QuizEraDBContext _context;

        public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        QuizEraDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IdentityResult> RegisterStudentAsync(RegisterStudentDto dto)
        {
            // 1. Create Identity User
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            // 2. Create user in AspNetUsers
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return result;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Student");

            if (!roleResult.Succeeded)
            {
                return roleResult;
            }
            // 3. Create Student
            var student = new Student(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                user.Id
            );

            // 4. Save Student
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // 5. Return success
            return result;
        }
        public async Task<bool> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                dto.Password,
                false,
                false
            );

            return result.Succeeded;
        }


        public async Task<bool> RegisterInstructorAsync(RegisterInstructorDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                return false;

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return false;

            var roleResult = await _userManager.AddToRoleAsync(user, "Instructor");

            if (!roleResult.Succeeded)
                return false;

            var instructor = new Instructor(
                dto.Name,
                dto.Email,
                user.Id
            );

            _context.Instructors.Add(instructor);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
