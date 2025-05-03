![CAPTCH_Banner](https://github.com/user-attachments/assets/ccf11397-3737-4bcb-9133-e8caa88466fd)
# HoGi.CaptchaAuthorize
A library in .NET web applications for generating and validating CAPTCHAs in a stateless manner.
it's help you to easy configure and use captcha.

## Usage
### Service DI
> [!TIP]
> for in memory cache service
>```builder.Services.AddMemoryCache();```
> ```builder.Services.AddDistributedMemoryCache();```

> [!TIP]
> for redis as distribute cache service
>```builder.Services.AddStackExchangeRedisCache(option =>{ option.Configuration = "uri"; option.InstanceName = "instanceName";});```
```sh
    //set AddHoGiCaptcha(true) for using distribute cache
    builder.Services.AddHoGiCaptcha(true);
```
### Example Get Captcha
```sh
 [HttpGet]
 [ProducesResponseType(typeof(CaptchaResult), StatusCodes.Status200OK)]
 public IActionResult Get([FromServices] CaptchaService service)
 {
     return Ok(service.GenerateCaptcha());
 }
```
distribute cache support
``` sh
[HttpGet]
[ProducesResponseType(typeof(CaptchaResult), StatusCodes.Status200OK)]
public IActionResult Get([FromServices] DistributeCaptchaService service)
{
    return Ok(service.GenerateCaptcha());
}
```
### Example Validating Captcha
```sh
 [HttpPost]
 [ServiceFilter(typeof(CaptchaFilter), Order = 1)]
 public IActionResult Post([FromBody] Model model)
 {
    //do something(model);
    return Ok());
 }
```
