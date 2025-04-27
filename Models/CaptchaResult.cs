using System;
using System.Text.Json.Serialization;

namespace HoGi.CaptchaAuthorize.Models;

public class CaptchaResult
{
    [Newtonsoft.Json.JsonIgnore]
    [JsonIgnore]
    public byte[] CaptchaByteData { get; set; }
    public string CaptchaBase64Data => Convert.ToBase64String(CaptchaByteData);

    public string Salt { get; set; }

    public string HashedCaptcha { get; set; }


       
}