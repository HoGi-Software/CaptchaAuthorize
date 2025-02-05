using HoGi.CaptchaAuthorize.Exceptions;
using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using HoGi.CaptchaAuthorize.Models.Configurations;

namespace HoGi.CaptchaAuthorize.Services;

public class CaptchaService: ICaptchaService
{
    private static readonly ConcurrentDictionary<string, DateTime> ActiveCaptcha = new ConcurrentDictionary<string, DateTime>();
   // private readonly IOptions<CaptchaSetting> _setting;

    //public CaptchaService(IOptions<CaptchaSetting> setting)
    //{
    // //   _setting = setting;
      
    //}

    [Obsolete]
    public virtual CaptchaResult GenerateCaptcha(int width = 120, int height = 40)
    {

        var captcha = CaptchaBuilder.Create();

        var captchaImage = CaptchaBuilder.GenerateImage(captcha);

        ActiveCaptcha.TryAdd(captcha.Hash, DateTime.Now.AddMinutes(2));
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }
    [Obsolete]
    public virtual CaptchaResult GenerateCaptcha(string letters, int width = 120, int height = 40)
    {

        var captcha = CaptchaBuilder.Create(letters);

        var captchaImage = CaptchaBuilder.GenerateImage(captcha);

        ActiveCaptcha.TryAdd(captcha.Hash, DateTime.Now.AddMinutes(2));
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }
    public virtual CaptchaResult GenerateCaptcha(ICaptchaConfiguration configuration)
    {
        
        var captcha = CaptchaBuilder.Create(configuration.Letters);

        var captchaImage = CaptchaBuilder.GenerateImage(captcha, configuration.Width, configuration.Height, configuration.FontSize);

        ActiveCaptcha.TryAdd(captcha.Hash, DateTime.Now.AddMinutes(configuration.DurationInMinute));
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }



    public virtual void Validate(ICaptcha captcha)
    {

        if (captcha == null) throw new InvalidCaptchaException();

        if (ActiveCaptcha.TryRemove(captcha.Hash, out var expireTime))
        {
                
            if (DateTime.Now > expireTime) throw new InvalidCaptchaException();
        }
        else
        {
            throw new InvalidCaptchaException();
        }

        using var sha256 = SHA256.Create();

        var splitHash = captcha.Hash.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            
        var saltedValue = Encoding.UTF8
            .GetBytes(captcha.CaptchaCode)
            .Concat(Encoding.UTF8.GetBytes(captcha.Salt))
            .ToArray();

        // Send a simple text to hash.  
        var firstHashedBytes = sha256.ComputeHash(saltedValue);

        var secondHashedBytes = sha256.ComputeHash(firstHashedBytes);

        var thirdHashedBytes = sha256.ComputeHash(secondHashedBytes);

        // Get the hashed string.  
        var result = BitConverter.ToString(thirdHashedBytes) == splitHash[1];

        if (!result)
            throw new InvalidCaptchaException();

    }


}