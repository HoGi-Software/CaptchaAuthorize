using Microsoft.AspNetCore.Mvc.Filters;

namespace HoGi.CaptchaAuthorize.Attributes;

public class CaptchaExceptMobileFilter : CaptchaFilter
{
    public CaptchaExceptMobileFilter():base()
    {
        
    }
    public CaptchaExceptMobileFilter(bool distributeCacheEnable=false) :base(distributeCacheEnable)
    {
        
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var osType = context.HttpContext.Request.Headers["OS-Type"];
        if (osType == "ANDROID" || osType == "IOS")
        {
            return;
        }
        base.OnActionExecuting(context);
      
    }
}