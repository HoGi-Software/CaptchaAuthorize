namespace HoGi.CaptchaAuthorize.Interfaces
{
    public interface ICaptchaConfiguration
    {
        int DurationInMinute { get; }
        int Height { get; }
        int Width { get; }
        string Letters { get; }
        int FontSize { get; }
        public byte Length { get; }

        ICaptchaConfiguration WithSize(int width, int height);
        ICaptchaConfiguration WithDurationInMinute(int durationInMinute);
        ICaptchaConfiguration WithLetters(string letters);
        ICaptchaConfiguration WithFontSize(int fontSize);
        ICaptchaConfiguration WithLength(byte length);
    }
}
