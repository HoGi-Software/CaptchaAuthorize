using System;

namespace HoGi.CaptchaAuthorize.Exceptions;

public class InvalidCaptchaException:Exception
{
    public InvalidCaptchaException():base("Captcha is Invalid or Expired. ")
    {
            
    }
    public InvalidCaptchaException(string message) : base(message)
    {

    }
}