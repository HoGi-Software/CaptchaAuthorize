using HoGi.CaptchaAuthorize.Interfaces;
using HoGi.CaptchaAuthorize.Models;

namespace HoGi.CaptchaAuthorize.Services
{
    public interface ICaptchaService
    {
        CaptchaResult GenerateCaptcha(int width = 120, int height = 40);
        CaptchaResult GenerateCaptcha(string letters, int width = 120, int height = 40);
        void Validate(ICaptcha captcha);
    }
}
