using HoGi.CaptchaAuthorize.Extensions;
using HoGi.CaptchaAuthorize.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HoGi.CaptchaAuthorize.Services;

public static class CaptchaBuilder
{
    private static string _letters = "1234567890";
        
        
    public static Captcha Create()
    {

        var captchaCode = GenerateCaptchaCode();

        var salt = GetSalt();
        var captcha = new Captcha
        {
            Salt = salt,
            Hash = captchaCode.Sha256ForCaptcha(salt)
        };
            
        return captcha;

    }
    public static Captcha Create(string letters)
    {
        SetLetters(letters);

        var captchaCode = GenerateCaptchaCode();

        var salt = GetSalt();
        var captcha = new Captcha
        {
            Salt = salt,
            Hash = captchaCode.Sha256ForCaptcha(salt)
        };

        return captcha;

    }

    public static void SetLetters(string letters)
    {
        if (string.IsNullOrEmpty(letters))
        {
            throw new ArgumentNullException();
        }

        _letters = letters;
    }

    public static byte[] GenerateImage(Captcha captcha, int width = 120, int height = 40,int fontSize=20)
    {
        using var bitmap = new Bitmap(width, height);

        using var graphics = Graphics.FromImage(bitmap);

        var random = new Random();
        var hatchBrush = new HatchBrush(
            hatchstyle: (HatchStyle)Enum.Parse(typeof(HatchStyle), random.Next(0, 52).ToString()),
            Color.AliceBlue,
            Color.FromArgb(20, random.Next(100, 255), random.Next(150, 200), random.Next(150, 255)));

        graphics.FillRectangle(hatchBrush, 0, 0, width, height);

        var font = new Font(SystemFonts.DialogFont.FontFamily, fontSize);

        graphics.DrawString(captcha.CaptchaCode, font, new SolidBrush(Color.DimGray), new PointF(random.Next(0, 10), random.Next(0, 5)));

        using var memoryStream = new MemoryStream();

        bitmap.Save(memoryStream, ImageFormat.Jpeg);

        memoryStream.Flush();

        return memoryStream.ToArray();
    }
    private static string GenerateCaptchaCode(int length=5)
    {
        var random = new Random();
        var maxLength = _letters.Length - 1;

        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            var index = random.Next(maxLength);
            builder.Append(_letters[index]);
        }

        return builder.ToString();
    }
       
    private static string GetSalt()
    {
        var bytes = new byte[128 / 8];
        using var keyGenerator = RandomNumberGenerator.Create();
        keyGenerator.GetBytes(bytes);
        return BitConverter.ToString(bytes);
    }
}