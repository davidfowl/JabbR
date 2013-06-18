using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace JabbR.Services
{
    //checks with Googles ReCaptcha service
    public class CaptchaService : ICaptchaService
    {
        private readonly string _reCaptchaPrivateKey;
        private readonly string _reCaptchaPublicKey;

        public CaptchaService(ApplicationSettings applicationSettings)
        {
            //get the keys

            _reCaptchaPrivateKey = applicationSettings.ReCaptchaPrivateKey;
            _reCaptchaPublicKey = applicationSettings.ReCaptchaPublicKey;

        }

        public bool IsValid(string UserHostAddress, string captchaChallenge, string captchaResponse)
        {
            string poststring = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                            this._reCaptchaPrivateKey,
                            UserHostAddress,
                            captchaChallenge,
                            captchaResponse);

            byte[] postdata = Encoding.UTF8.GetBytes(poststring);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postdata.Length;

            using (Stream writer = webRequest.GetRequestStream())
            {
                writer.Write(postdata, 0, postdata.Length);
            }
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (var stream = new StreamReader(webResponse.GetResponseStream()))
                {
                    string firstLine = stream.ReadLine();
                    if (firstLine != "true")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }


        public string PublicKey
        {
            get
            {
                return _reCaptchaPublicKey;
            }
        }
    }
}