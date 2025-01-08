using HoGi.Shared.Exceptions;

namespace HoGi.CaptchaAuthorize.Exceptions;

public class InvalidCaptchaException:HoGiException
{
    public InvalidCaptchaException():base(-1,"کپچا نا معتبر است.")
    {
            
    }
}