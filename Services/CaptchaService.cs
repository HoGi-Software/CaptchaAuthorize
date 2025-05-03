using HoGi.CaptchaAuthorize.Exceptions;
using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HoGi.CaptchaAuthorize.Services;

public class CaptchaService: ICaptchaService
{
    private static readonly ConcurrentDictionary<string, DateTime> ActiveCaptcha = new ConcurrentDictionary<string, DateTime>();

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
    //SpeechSynthesizer synthesizer = new SpeechSynthesizer();
    //synthesizer.Volume = 100;  // 0...100
    //        synthesizer.Rate = -2;     // -10...10
    //        PromptBuilder cultures = new PromptBuilder(
    //            new System.Globalization.CultureInfo("fa-Ir"));
    //cultures.AppendText("4 5 9 7 6");

    //        cultures.StartVoice(new System.Globalization.CultureInfo("fa-Ir"));
    //        cultures.EndVoice();
    //        var a = synthesizer.GetInstalledVoices(new CultureInfo("fa-Ir"));

    //synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult,50,new CultureInfo("fa-Ir"));
    //        var voice = new MemoryStream();
    //synthesizer.SetOutputToWaveStream(voice);
    //        // Synchronous
    //        synthesizer.Speak("4 5 9 7 6");
            
    //        // Asynchronous
    //        //var a=synthesizer.SpeakAsync("Hello World");
    //        return File(voice.GetBuffer(), "audio/wav","captcha.wav", DateTimeOffset.Now, EntityTagHeaderValue.Any);

}