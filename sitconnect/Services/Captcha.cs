using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace sitconnect.Services
{
    public class Captcha
    {
        public static bool ReCaptchaPassed (string gRecaptchaResponse, string siteKey)
        {
            HttpClient httpClient = new HttpClient();

            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={siteKey}&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK) 
            {
                return false;
            }
            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);

            if (JSONdata.success != "true" || JSONdata.score <= 0.5m)
            {
                return false;
            }

            return true;
        }
    }
}