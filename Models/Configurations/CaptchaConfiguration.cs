using System;
using HoGi.CaptchaAuthorize.Interfaces;

namespace HoGi.CaptchaAuthorize.Models.Configurations
{
    internal class CaptchaConfiguration : ICaptchaConfiguration
    {
        public CaptchaConfiguration()
        {
            this.WithSize(120, 40)
                .WithLetters(string.Empty)
                .WithLength(5)
                .WithDurationInMinute(2)
                .WithFontSize(20);

        }
        /// <inheritdoc />
        public int DurationInMinute { get; private set; }

        /// <inheritdoc />
        public int Height { get; private set; }

        /// <inheritdoc />
        public int Width { get; private set; }

        /// <inheritdoc />
        public string Letters { get; private set; }

        /// <inheritdoc />
        public int FontSize { get; private set; }

        /// <inheritdoc />
        public byte Length { get; private set; }

        /// <inheritdoc />
        public ICaptchaConfiguration WithSize(int width, int height)
        {
            if (width <= 0)
                width = 120;
            if (height <= 0)
                height = 40;

            Width = width;
            Height = height;
            return this;
        }

        /// <inheritdoc />
        public ICaptchaConfiguration WithDurationInMinute(int durationInMinute)
        {
            if (durationInMinute <= 0)
                durationInMinute = 2;
            DurationInMinute = durationInMinute;
            return this;
        }

        /// <inheritdoc />
        public ICaptchaConfiguration WithLetters(string letters)
        {
            if (string.IsNullOrEmpty(letters))
                letters = "0123456789";
            Letters = letters;
            return this;
        }

        /// <inheritdoc />
        public ICaptchaConfiguration WithFontSize(int fontSize)
        {
            if (fontSize <= 0)
                fontSize = 20;
            FontSize = fontSize;
            return this;
        }

        /// <inheritdoc />
        public ICaptchaConfiguration WithLength(byte length)
        {
            if (length <= 3)
                length = 5;
            Length = length;
            return this;
        }
    }
}
