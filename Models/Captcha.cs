using HoGi.CaptchaAuthorize.Interfaces;

namespace HoGi.CaptchaAuthorize.Models;

public class Captcha : ICaptcha
{
    public string CaptchaCode { get ; set ; }
    public string Salt { get ; set; }
    public string Hash { get ; set ; }
}//end class
//end namespace