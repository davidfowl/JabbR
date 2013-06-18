namespace JabbR.Services
{
    public interface ICaptchaService
    {
        bool IsValid(string UserIpAddress, string captchaChallenge, string captchaResponse);
        string PublicKey  { get; }
    }
}
