using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockTrackerApp
{
    public class ChatBot
    {
        public async Task<string> GetPrediction(string userInput)
        {
            
            string stockSymbol = ExtractStockSymbol(userInput);

            if (string.IsNullOrEmpty(stockSymbol))
            {
                return "Tôi không tìm thấy mã chứng khoán trong câu hỏi. Vui lòng hỏi như 'AAPL tăng hay giảm?'.";
            }

            
            string apiKey = "QiALPWZhwZvxYNYE677fdlKGJN7igxZ2";
            string url = $"https://api.polygon.io/v2/aggs/ticker/{stockSymbol}/prev?apiKey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<dynamic>(response);

                    if (data?.results == null || data?.results.Count == 0)
                    {
                        return $"Tôi không tìm thấy dữ liệu cho mã {stockSymbol}.";
                    }

                    
                    double openPrice = data?.results[0]?.o ?? 0;
                    double closePrice = data?.results[0]?.c ?? 0;

                    
                    return closePrice > openPrice
                        ? $"Dựa vào phân tích thì giá {stockSymbol} có khả năng tăng. Lưu ý đây chỉ là dự đoán doán do còn các yếu tố khác có thể ảnh hưởng đến giá chứng khoán."
                        : $"Dựa vào phân tích thì giá {stockSymbol} có khả năng giảm. Lưu ý đây chỉ là dự đoán doán do còn các yếu tố khác có thể ảnh hưởng đến giá chứng khoán.";
                }
                catch (Exception ex)
                {
                    return $"Đã xảy ra lỗi khi lấy dữ liệu cho mã {stockSymbol}: {ex.Message}";
                }
            }
        }

        private string ExtractStockSymbol(string input)
        {
            Match match = Regex.Match(input, @"\b[A-Z]{1,5}\b");
            return match.Success ? match.Value : string.Empty;
        }
    }
}
