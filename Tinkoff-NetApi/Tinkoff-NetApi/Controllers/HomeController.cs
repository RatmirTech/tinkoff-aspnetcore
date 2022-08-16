using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using Tinkoff_NetApi.Models;

namespace Tinkoff_NetApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Key = 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InitPay()
        {
            var tinkoffInfo = new TinkoffInfo
            {
                TerminalKey = "TinkoffBankTest",
                Amount = 100,
                OrderId = "2105024",
                Description = "test ticket",
                Data = new Data
                {
                    Phone = "+71234567890",
                    Email = "a@test.com"
                },
                Receipt = new Receipt
                {
                    Email = "a@test.ru",
                    Phone = "+71234567890",
                    EmailCompany = "b@test.com",
                    Taxation = "osn",
                    Items = new Item[]
                    {
                        new Item
                        {
                            Name = "тикет 50",
                            Price = 100,
                            Quantity = 1.00,
                            Amount = 100,
                            Tax = "vat10"
                        }
                    }
                }
            };
            string json = JsonConvert.SerializeObject(tinkoffInfo);
            HttpContent content = new StringContent(json);

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://securepay.tinkoff.ru/v2/Init");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);
            string jsonResponse = System.Text.Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync());
            TinkoffResponse tinkoffResponse = JsonConvert.DeserializeObject<TinkoffResponse>(jsonResponse);
            ViewBag.Mir = tinkoffResponse.PaymentURL;
            ViewBag.Key = 1;

            return View("Index");

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public class TinkoffInfo
        {
            public string TerminalKey { get; set; }

            public double Amount { get; set; }

            public string OrderId { get; set; }

            public string Description { get; set; }

            public Data Data { get; set; }

            public Receipt Receipt { get; set; }
        }

        public class Data
        {
            public string Phone { get; set; }

            public string Email { get; set; }
        }

        public class Receipt
        {
            public string Email { get; set; }

            public string Phone { get; set; }

            public string EmailCompany { get; set; }

            public string Taxation { get; set; }

            public IList<Item> Items { get; set; }
        }

        public class Item
        {
            public string Name { get; set; }

            public double Price { get; set; }

            public double Quantity { get; set; }

            public double Amount { get; set; }

            public string Tax { get; set; }
        }

        public class TinkoffResponse
        {
            public string Success { get; set; }

            public string ErrorCode { get; set; }

            public string TerminalKey { get; set; }

            public string Status { get; set; }

            public string PaymentId { get; set; }

            public string OrderId { get; set; }

            public double Amount { get; set; }

            public string PaymentURL { get; set; }
        }
    }
}
