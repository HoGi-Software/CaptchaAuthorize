using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Models;
using HoGi.Shared.Exceptions;
using HoGi.ToolsAndExtensions.Extensions;
using HoGi.ToolsAndExtensions.Interfaces;

namespace HoGi.CaptchaAuthorize.Services;

public class DistributeCaptchaService
{
    private readonly ICacheService _cacheService;

    public DistributeCaptchaService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public virtual CaptchaResult GenerateCaptcha(int width = 120, int height = 40)
    {

        var captcha = CaptchaFactory.Create();

        var captchaImage = CaptchaFactory.GenerateImage(captcha);

        _cacheService.Add(captcha.Hash,()=> DateTime.Now.AddMinutes(2));
           
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }
    public virtual CaptchaResult GenerateCaptcha(string letters, int width = 120, int height = 40)
    {

        var captcha = CaptchaFactory.Create(letters);

        var captchaImage = CaptchaFactory.GenerateImage(captcha);

        _cacheService.Add(captcha.Hash, () => DateTime.Now.AddMinutes(2));
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }
    public virtual void Validate(ICaptcha captcha)
    {

        if (captcha == null) throw new GeneralException("عبارت امنیتی منقضی یا نامعتبر می باشد.");

        if (DateTime.Now > _cacheService.Get<DateTime>(captcha.Hash))
        {
            throw new GeneralException("عبارت امنیتی منقضی یا نامعتبر می باشد.");
        }
        //else
        //{
        //    throw new GeneralException("عبارت امنیتی منقضی یا نامعتبر می باشد.");
        //}

        using var sha256 = SHA256.Create();
        var splitHash = captcha.Hash.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);


        var saltedValue = Encoding.UTF8
            .GetBytes(captcha.CaptchaCode.Safe())
            .Concat(Encoding.UTF8.GetBytes(captcha.Salt))
            .ToArray();
        // Send a simple text to hash.  
        var firstHashedBytes = sha256.ComputeHash(saltedValue);
        var secondHashedBytes = sha256.ComputeHash(firstHashedBytes);
        var thirdHashedBytes = sha256.ComputeHash(secondHashedBytes);
        // Get the hashed string.  
        var result = BitConverter.ToString(thirdHashedBytes) == splitHash[1];

        if (!result)
            throw new GeneralException("عبارت امنیتی منقضی یا نامعتبر می باشد.");

    }


}