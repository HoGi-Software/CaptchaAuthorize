using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;
using HoGi.CaptchaAuthorize.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HoGi.CaptchaAuthorize.Attributes;

public  class CaptchaFilter : IActionFilter
{
    private readonly bool _distributeCacheEnable;

    public CaptchaFilter()
    {
        _distributeCacheEnable = false;
    }
    public CaptchaFilter(bool distributeCacheEnable=false)
    {
        _distributeCacheEnable = distributeCacheEnable;
    }

    public virtual void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public virtual void OnActionExecuting(ActionExecutingContext context)
    {
        var badResult = new BadRequestObjectResult(new InvalidCaptchaException());
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

            var captchaService = (_distributeCacheEnable
                ?(ICaptchaService) context.HttpContext.RequestServices.GetRequiredService<DistributeCaptchaService>()
                : context.HttpContext.RequestServices.GetRequiredService<CaptchaService>());

            captchaService.Validate(captcha);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            context.Result = badResult;
        }

    }
        
}