using System;
using System.Linq;
using System.Reflection;
using HoGi.CaptchaAuthorize.Exceptions;
using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Services;
using HoGi.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace HoGi.CaptchaAuthorize.Attributes;

public class CaptchaFilter : IActionFilter
{

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var badResult = new BadRequestObjectResult(new AggregateException<HoGiException>() { Errors = { new InvalidCaptchaException() } });
        try
        {

            var model = context.ActionArguments
                .Select(s => s.Value)
                .FirstOrDefault(x => x.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Any(y => y.PropertyType.GetInterface(nameof(ICaptcha)) is not null));
                


            var captcha = (model?.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.PropertyType.GetInterface(nameof(ICaptcha)) is not null)?
                .GetValue(model)) as ICaptcha;

            if (captcha is null || string.IsNullOrEmpty(captcha.CaptchaCode) || string.IsNullOrEmpty(captcha.Hash))
                context.Result = badResult;

            try
            {
                var captchaService = context.HttpContext.RequestServices.GetRequiredService<DistributeCaptchaService>();
                captchaService.Validate(captcha);
            }
            catch (Exception)
            {
                //var captchaService = context.HttpContext.RequestServices.GetRequiredService<CaptchaService>();
                //captchaService.Validate(captcha);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            context.Result = badResult;
        }

    }
        
}