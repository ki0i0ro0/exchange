using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace StockView
{
    public class GetExchangeAPI
    {
        public ExchangeData exhangeList;
        public string API_URL = "https://api.exchangeratesapi.io/latest";

        // データを取得するメソッド
        public async Task<ExchangeData> AsyncGetWebAPIData(string Country)
        {
            // Listの作成
            exhangeList = new ExchangeData();
            // HttpClientの作成 
            HttpClient httpClient = new HttpClient();
            // 非同期でAPIからデータを取得
            Task<string> stringAsync = httpClient.GetStringAsync(API_URL + "?base="+  Country);
            string result = await stringAsync;
            // JSON形式のデータをデシリアライズ
            exhangeList = JsonSerializer.Deserialize<ExchangeData>(result);
            if (Country=="EUR")
            {
                exhangeList.rates.Add("EUR", 1);
            }
            // List でデータを返す
            return exhangeList;
        }
    }
    /// <summary>
    /// 為替データ
    /// </summary>
    public class ExchangeData
    {
        public Dictionary<string,float> rates { get; set; }
        public string baseMoney { get; set; }
        public string date { get; set; }
    }

   
}