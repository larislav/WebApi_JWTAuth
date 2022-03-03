using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private static string WebApiURL = "https://localhost:44363/";

        // GET: Home
        public async Task<ActionResult> Index()
        {
            var tokenBased = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebApiURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseMessage = await client.GetAsync("Account/ValidLogin?userName=admin&userPassword=admin");

                var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                if (!responseMessage.IsSuccessStatusCode)
                    return Content(resultMessage);
                tokenBased = JsonConvert.DeserializeObject<string>(resultMessage);
                Session["TokenNumber"] = tokenBased;
                Session["UserName"] = "admin";

            }
            return Content(tokenBased);
        }

        public async Task<ActionResult> GetEmployee()
        {
            string returnMessage = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebApiURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    Session["TokenNumber"].ToString() + ":" + Session["UserName"].ToString());

                var responseMessage = await client.GetAsync("Account/GetEmployee");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                    returnMessage = JsonConvert.DeserializeObject<string>(resultMessage);
                }
            }
            return Content(returnMessage);
        }
    }
}