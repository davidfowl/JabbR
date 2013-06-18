using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace JabbR.Services
{
    //checks with Googles ReCaptcha service
    public class CaptchaService : ICaptchaService
    {
        private readonly string reCaptchaPrivateKey;
        private readonly string reCaptchaPublicKey;

        public CaptchaService()
        {
            //get the keys
            reCaptchaPrivateKey = ConfigurationManager.AppSettings["jabbr:reCaptchaPrivateKey"];
            reCaptchaPublicKey = ConfigurationManager.AppSettings["jabbr:reCaptchaPublicKey"];

        }

        public bool IsValid(string UserHostAddress, string captchaChallenge, string captchaResponse)
        {
            string poststring = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                            this.reCaptchaPrivateKey,
                            UserHostAddress,
                            captchaChallenge,
                            captchaResponse);

            byte[] postdata = Encoding.UTF8.GetBytes(poststring);

            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postdata.Length;

            using (Stream writer = webRequest.GetRequestStream())
            {
                writer.Write(postdata, 0, postdata.Length);
            }
            using (System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse())
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
                return reCaptchaPublicKey;
            }
        }
    }
}