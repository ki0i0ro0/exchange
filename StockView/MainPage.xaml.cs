using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace StockView
{
    public partial class MainPage : ContentPage
    {
        public ListView listView;
        public string Country="JPY"; 
        private Dictionary<string,string> JapaneseCountryName= new Dictionary<string, string>() {
            { "JPY", "日本円" }
            ,{ "USD", "米ドル" }
            ,{ "EUR", "ユーロ" }
            ,{ "CNY", "中国元" }
        };

        private Dictionary<string, FlagImage> ExchangeFlag = new Dictionary<string, FlagImage>() {
            { "JPY", new FlagImage{sUri= "jpy.png",img = new Image() } }
            ,{ "USD", new FlagImage{sUri= "usd.png",img = new Image() }}
            ,{ "EUR", new FlagImage{sUri= "eur.png",img = new Image() }}
            ,{ "CNY", new FlagImage{sUri= "china.png" ,img = new Image()}}
        };

        class FlagImage
        {
            public string sUri { get; set; }
            public Image img { get; set; }
        }

        // データの型定義
        class Data
        {
            public string Text { get; set; }
            public string Detail { get; set; }
            public string Icon { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();


            DataTemplate cell2 = new DataTemplate(typeof(ImageCell));
            cell2.SetBinding(ImageCell.TextProperty, "Text");
            cell2.SetBinding(ImageCell.DetailProperty, "Detail");
            cell2.SetBinding(ImageCell.ImageSourceProperty, "Icon");
            cell2.SetValue(ImageCell.TextColorProperty, Color.Green);

            listView = new ListView
            {
                RowHeight = 60 //Cellの高さを設定
                ,ItemTemplate = cell2
            };
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { listView }
            };

            fetchArticles(new GetExchangeAPI());

            listView.ItemSelected += async(sender, e) => {
                var tmp = new Data();
                tmp = (Data)e.SelectedItem;
                Country = tmp.Text.Substring(0, 3);
                fetchArticles(new GetExchangeAPI());
            };
        }
        // 非同期でデータ取得のメソッドを実行するメソッド
        async void fetchArticles(GetExchangeAPI api)
        {
            try
            {
                // 取得したデータをListに設定
                ExchangeData exchangeList = await api.AsyncGetWebAPIData(Country);
                
                var items = new List<Data>();

                foreach (KeyValuePair<string, float> value in exchangeList.rates)
                {
                    string CountryName = JapaneseCountryName.ContainsKey(value.Key) ? JapaneseCountryName[value.Key] : value.Key;
                    if (!JapaneseCountryName.ContainsKey(value.Key))
                    {
                        continue;
                    }

                    //ExchangeFlag[value.Key].img.Source = new UriImageSource
                    //{
                    //    CachingEnabled = true,
                    //    Uri = new Uri(ExchangeFlag[value.Key].sUri)
                    //};

                    items.Add(new Data
                    {
                        Text = value.Key + value.Value.ToString(),
                        Detail = value.Value.ToString(),
                        Icon = ExchangeFlag[value.Key].sUri
                    });
                }
                listView.ItemsSource = items;
            }
            // エラー表示処理
            catch (System.Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "OK");
            }
        }
    }
}