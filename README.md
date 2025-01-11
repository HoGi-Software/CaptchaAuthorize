![CAPTCH_Banner](https://github.com/user-attachments/assets/ccf11397-3737-4bcb-9133-e8caa88466fd)
# HoGi.CaptchaAuthorize
A library in .NET web applications for generating and validating CAPTCHAs in a stateless manner.
it's help you to easy configure and use captcha.

## Usage
1. Service DI
> [!TIP]
> for in memory cache service
>```builder.Services.AddMemoryCache();```
> ```builder.Services.AddDistributedMemoryCache();```

> [!TIP]
> for redis as distribute cache service
>```builder.Services.AddStackExchangeRedisCache(option =>{ option.Configuration = "uri"; option.InstanceName = "instanceName";});```
```sh
    //set AddFundProCaptcha(true) for using distribute cache
    builder.Services.AddHoGiCaptcha(true);
```
