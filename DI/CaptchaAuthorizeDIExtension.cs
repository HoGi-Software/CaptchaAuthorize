using System;
using HoGi.CaptchaAuthorize.Attributes;
using HoGi.CaptchaAuthorize.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HoGi.CaptchaAuthorize.DI;

public static class CaptchaAuthorizeDIExtension
{
    public static IServiceCollection AddHoGiCaptcha(this IServiceCollection services, bool useDistributeCache)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddOptions();
    
        services.TryAdd(useDistributeCache
            ? ServiceDescriptor.Singleton<DistributeCaptchaService, DistributeCaptchaService>()
            : ServiceDescriptor.Singleton<CaptchaService, CaptchaService>());


        services.TryAdd(ServiceDescriptor.Singleton<CaptchaFilter, CaptchaFilter>());

        services.TryAdd(ServiceDescriptor.Singleton<CaptchaExceptMobileFilter, CaptchaExceptMobileFilter>());

        return services;
    }
}