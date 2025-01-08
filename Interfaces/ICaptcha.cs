namespace HoGi.CaptchaAuthorize.Interfaces;
//public interface ICaptchaItem
//{
//    public string Code { get; set; }

//    public string Salt { get; set; }

//    public string Hash { get; set; }
//}

public interface ICaptcha
{
    string CaptchaCode { get; set; }

    string Salt { get; set; }

    string Hash { get; set; }
    //CaptchaItem Captcha { get; set; }
}