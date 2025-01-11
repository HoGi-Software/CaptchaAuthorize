namespace HoGi.CaptchaAuthorize.Interfaces;

public interface ICaptcha
{
    string CaptchaCode { get; set; }

    string Salt { get; set; }

    string Hash { get; set; }
}