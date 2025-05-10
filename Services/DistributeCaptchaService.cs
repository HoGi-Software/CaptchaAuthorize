using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HoGi.CaptchaAuthorize.Exceptions;
using HoGi.Commons.Interfaces.Caches;
using Microsoft.Extensions.Caching.Distributed;

namespace HoGi.CaptchaAuthorize.Services;

public class DistributeCaptchaService: ICaptchaService
{
    private readonly ICacheService _cacheService;

    public DistributeCaptchaService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public virtual CaptchaResult GenerateCaptcha(int width = 120, int height = 40,int expireInMinutes=2)
    {

        var captcha = CaptchaBuilder.Create();

        var captchaImage = CaptchaBuilder.GenerateImage(captcha);

        _cacheService.GetOrAdd(captcha.Hash,()=> DateTime.Now.AddMinutes(expireInMinutes), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, expireInMinutes, 0)
        });
           
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }
    public virtual CaptchaResult GenerateCaptcha(string letters, int width = 120, int height = 40, int expireInMinutes = 2)
    {

        var captcha = CaptchaBuilder.Create(letters);

        var captchaImage = CaptchaBuilder.GenerateImage(captcha);

        _cacheService.GetOrAdd(captcha.Hash, () => DateTime.Now.AddMinutes(expireInMinutes),new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0,0,expireInMinutes,0)
        });
        return new CaptchaResult
        {
            CaptchaByteData = captchaImage,
            HashedCaptcha = captcha.Hash,
            Salt = captcha.Salt
        };

    }
    public virtual void Validate(ICaptcha captcha)
    {

        if (captcha == null)
        {
            Console.WriteLine("captcha is null");
            throw new InvalidCaptchaException();
        }

        var data = _cacheService.Get<DateTime>(captcha.Hash);
        if (DateTime.Now >data )
        { Console.WriteLine($"{data} is expired" );
            throw new InvalidCaptchaException();
        }
     

        using var sha256 = SHA256.Create();
        var splitHash = captcha.Hash.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        
        Console.WriteLine(string.Join(":",splitHash));

        var saltedValue = Encoding.UTF8
            .GetBytes(captcha.CaptchaCode)
            .Concat(Encoding.UTF8.GetBytes(captcha.Salt))
            .ToArray();
        // Send a simple text to hash.  
        var firstHashedBytes = sha256.ComputeHash(saltedValue);
        var secondHashedBytes = sha256.ComputeHash(firstHashedBytes);
        var thirdHashedBytes = sha256.ComputeHash(secondHashedBytes);
        // Get the hashed string.  
        Console.WriteLine(splitHash[1]);
        Console.WriteLine(BitConverter.ToString(thirdHashedBytes));

        var result = BitConverter.ToString(thirdHashedBytes) == splitHash[3];
        Console.WriteLine(result);
        if (!result)
            throw new InvalidCaptchaException();

    }


}