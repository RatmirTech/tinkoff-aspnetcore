using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InitPay()
        {
            var tinkoffInfo = new TinkoffInfo
            {
                TerminalKey = "TinkoffBankTest",
                Amount = 500,
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
                    Items = new Items
                    {
                        Name = "тикет 50",
                        Price = 5000,
                        Quantity = 1.00,
                        Amount = 5000,
                        Tax = "vat10"
                    }
                }
            };
            string json = JsonConvert.SerializeObject(tinkoffInfo);
            HttpContent content = new StringContent(json);

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://business.tinkoff.ru/openapi/sandbox/secured/api/v1/payment/ruble-transfer/pay");
            request.Headers.Add("Accept", "application/json");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);
            ViewBag.Mir = response.ToString();
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

            public Items Items { get; set; }
        }

        public class Items
        {
            public string Name { get; set; }

            public double Price { get; set; }

            public double Quantity { get; set; }

            public double Amount { get; set; }

            public string Tax { get; set; }
        }
    }
}
