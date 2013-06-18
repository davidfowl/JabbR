namespace JabbR.Services
{
    public interface ICaptchaService
    {
        bool IsValid(string UserHostAddress, string captchaChallenge, string captchaResponse);
        string PublicKey  { get; }
    }
}
