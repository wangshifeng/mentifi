using System;
using System.Data;
using System.Linq;
using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Repository.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hub3c.Mentify.API.Filters
{
    /// <inheritdoc />
    public class ValidateModelStateAttribute : IActionFilter
    {
        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                throw new ApplicationException(ErrorMessage(context.ModelState));

            if (context.HttpContext.User?.Identity != null && context.HttpContext.User.Claims.Any(e => e.Type == "mid"))
            {
                var unitOfWork = context.HttpContext.RequestServices.GetService<IUnitOfWork>();
                var systemUserRepository = unitOfWork.GetRepository<SystemUser>();
                var systemUser = systemUserRepository.GetFirstOrDefault(
                    predicate: a => a.MemberId == context.HttpContext.User.GetMid(),
                    include: a => a.Include(b => b.EduUser).Include(c => c.Business).ThenInclude(e => e.EduUniversity));
                var eduUser = systemUser?.EduUser.SingleOrDefault();
                if (eduUser != null && eduUser.IsBlocked)
                    throw new InvalidConstraintException($"Your account has been blocked. Please contact your university administrator for further information");
            }

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private string ErrorMessage(ModelStateDictionary modelState)
        {
            return string.Join(";",
                modelState.SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage));
        }
    }
}
