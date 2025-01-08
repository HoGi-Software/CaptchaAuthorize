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

public class CaptchaExceptMobileFilter : IActionFilter
{

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        //#if !DEBUG
        var badResult = new BadRequestObjectResult(new AggregateException<HoGiException>() { Errors = { new InvalidCaptchaException() } });

        var osType = context.HttpContext.Request.Headers["OS-Type"];
        if (osType == "ANDROID" || osType == "IOS")
        {
            return;
        }

        var model = context.ActionArguments
            .Select(s => s.Value)
            .FirstOrDefault(x =>
                x.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Any(y => y.PropertyType.GetInterface(nameof(ICaptcha)) is not null));

        var captcha = (model?.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance) ?? Array.Empty<PropertyInfo>())
            .FirstOrDefault(x => x.PropertyType.GetInterface(nameof(ICaptcha)) is not null)?
            .GetValue(model) as ICaptcha;

        if (string.IsNullOrEmpty(captcha?.CaptchaCode))
        {
            context.Result = badResult;
        }
        else
        {
            var captchaService = context.HttpContext.RequestServices.GetRequiredService<DistributeCaptchaService>();

            if (string.IsNullOrEmpty(captcha.CaptchaCode) || string.IsNullOrEmpty(captcha.Hash))
                context.Result = badResult;
            try
            {
                captchaService.Validate(captcha);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Result = badResult;
            }
        }
           
    }
}