using System;
using System.Runtime.Serialization;

namespace HoGi.CaptchaAuthorize.Models;

public class CaptchaResult
{
    [IgnoreDataMember]
    public byte[] CaptchaByteData { get; set; }
    public string CaptchaBase64Data => Convert.ToBase64String(CaptchaByteData);

    public string Salt { get; set; }

    public string HashedCaptcha { get; set; }


       
}