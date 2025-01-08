using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HoGi.CaptchaAuthorize.Extensions;

public static  class CaptchaExtensions
{
    public static string Sha256ForCaptcha(this string text, string salt)
    {
        using var sha256 = SHA256.Create();

        var now = DateTime.Now;

        var saltedValue = Encoding.UTF8
            .GetBytes(text)
            .Concat(Encoding.UTF8.GetBytes(salt))
            .ToArray();

        // Send a sample text to hash.  
        var firstHashedBytes = sha256.ComputeHash(saltedValue);
        var secondHashedBytes = sha256.ComputeHash(firstHashedBytes);
        var thirdHashedBytes = sha256.ComputeHash(secondHashedBytes);

        return $"{now.Hour:00}:{now.Minute:00}:{Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")))}:{BitConverter.ToString(thirdHashedBytes)}";
    }
}