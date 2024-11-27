using crraut.Models;
using crraut.Models.Responses;
using crraut.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;

namespace crraut.Services
{
    public class SynchronizeService(IMemoryCache cache, IHubContext<QuartzHub> hubContext) {
        public async Task SynchronizeBinance(int limit, int? categoryId) {
            var response = await CheckBinanceAnnouncement(limit, categoryId);
            if(!response.Success) {
                throw new Exception(response.Message.ToString());
            }

            var listingNews = response.Data.Catalogs
                .Where(x => x.CatalogId is 48 or 161)
                .SelectMany(x => x.Articles.Select(article => new ArticleWithCategory { Article = article, CategoryId = x.CatalogId }));
            var mergedNews = listingNews
                .GroupBy(x => x.CategoryId)
                .Select(g => g.OrderByDescending(x => x.Article.ReleaseDate).First())
                .ToList();

            foreach(var news in mergedNews) {
                var title = news.Article.Title;
                var isNewsExists = cache.TryGetValue(title, out _);

                if(!isNewsExists) {
                    cache.Set(title, title, new MemoryCacheEntryOptions {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    });
                    //var publishDate = ConvertObjectToDateTime(news.Article.ReleaseDate);

                    var utcNow = DateTime.UtcNow;
                    //await OpenOrder(title, ExchangeEnum.Binance, publishDate, utcNow);

                    await Send(GenerateMessageForTelegram(new TelegramMessageDto {
                        Title = title,
                        Url = "https://www.binance.com/en/support/announcement",
                        Date = DateTime.Now
                    }));
                }
            }
        }

        private async Task<BinanceAnnouncementResponse> CheckBinanceAnnouncement(int limit, int? categoryId) {
            var client = new RestClient("https://www.binance.com/");
            var requestUrl = $"/bapi/composite/v1/public/cms/article/list/query?type=1&pageSize={limit}&pageNo=1";
            if(categoryId != null) {
                requestUrl += $"&catalogId={categoryId}";
            }
            var request =
                new RestRequest(requestUrl);

            request.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            request.AddHeader("Pragma", "no-cache");
            request.AddHeader("Expires", "0");
            var time = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss");
            var res = await client.ExecuteAsync(request);

            await hubContext.Clients.All.SendAsync("RequestUrl", requestUrl);
            await hubContext.Clients.All.SendAsync("Time", time);
            await hubContext.Clients.All.SendAsync("RequestDate", res.Headers.FirstOrDefault(x => x.Name == "Date").Value);

            return JsonConvert.DeserializeObject<BinanceAnnouncementResponse>(res.Content);
        }

        private string GenerateMessageForTelegram(TelegramMessageDto message) {
            var result = $"<b>Railway.BINANCEAPI</b>\n" +
                         $"{message.Title}\n" +
                         $"{message.Date}";
            //$"<b>Mexc:</b> {(message.IsTokenNameNotFound ? "&#10068;" : (message.IsExistOnMexc ? "&#9989;" : "&#10060;"))}\n" +
            //$"<b>GateIo:</b> {(message.IsTokenNameNotFound ? "&#10068;" : (message.IsExistOnGateIo ? "&#9989;" : "&#10060;"))}\n";

            if(!string.IsNullOrWhiteSpace(message.Url)) {
                result += $"<a href='{message.Url}'>{message.Url}</a>";
            }

            return result;
        }

        public async Task Send(string text) {
            var bot = new TelegramBotClient("6223771338:AAHa46qLYT7UNxJ-TywVzyjnxGvK9d1-B28");
            await bot.SendTextMessageAsync(-1001890604061, text, null, ParseMode.Html);
        }
    }
}
