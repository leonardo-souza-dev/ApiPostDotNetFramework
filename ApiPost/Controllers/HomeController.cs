using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ApiPost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var foo = new Foo();
            foo.Bar = "ApiPostSln";

            var response = PostAsync<Teste>(foo).Result;

            ViewBag.ConteudoDoResponse = response.Conteudo;
            ViewBag.Enviado = response.Enviado;

            return View();
        }


        public async Task<T> PostAsync<T>(object data) where T : class, new()
        {
            HttpClient httpClient = new HttpClient();

            var url = "https://apinodejsteste.herokuapp.com/api/teste";
            //var url = "http://localhost:3001/api/teste";

            try
            {
                string content = JsonConvert.SerializeObject(data);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PostAsync(url, byteContent).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new T();
                }
                
                var responseDeserialized = JsonConvert.DeserializeObject<T>(result);

                return responseDeserialized;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new System.Exception($"response :{responseContent}", ex);
                }
                throw;
            }
        }
    }

    public class Foo
    {
        public string Bar { get; set; }
    }

    public class Teste
    {
        public string Conteudo { get; set; }
        public string Enviado { get; set; }
    }
}
