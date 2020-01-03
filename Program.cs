using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Threading;

namespace twittertest
{
    class Program
    {
        static void Main(string[] args)
        {
            //(API key)(API secret key)(Access token)(Access token secret)の4つの引数が必要
            //ユーザータイムラインだけ使う場合はアクセストークン二種類は要らないかも？
            var tokens = CoreTweet.Tokens.Create("9npFqYcjoggZxQuj5j2oOAhsM"
                , "YIGzpq26PMqUcxhYrNDAb6pWKaCoGI1E8M7Wu2oG2xxSeNBAvl"
                , ""
                , "");

            //253087929-croRwhy0sBh2MG1p4mfeGnpDb6PK8FEGhIrqTkyu
            //GNp8zbbl3kwf45UNHCz9V6rYbkJprOVsj9PFhxYzh40Ie

            var parm = new Dictionary<string, object>();  //条件指定用Dictionary
            parm["count"] = 5;  //取得数
            parm["screen_name"] = "makiog33";  //取得したいユーザーID
                                               //makiog33
                                               //makiog3

            Test(tokens,parm);

            Console.ReadLine();
        }
        //urlからダウンロードする処理 コピペ 非同期メソッド
        //http: //shirakamisauto.hatenablog.com/entry/2016/02/17/120847
        //並列処理できるっぽいから戻り値voidでdl完了を待たずにやってるけど完了の余地はあるかも
        static async void DownloadImgAsync(string imgUri, string outputPath)
        {
            var client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(
                imgUri,
                HttpCompletionOption.ResponseContentRead);

            using (var fileStream = File.Create(outputPath))
            using (var httpStream = await res.Content.ReadAsStreamAsync())
                httpStream.CopyTo(fileStream);
        }
        static async void Test(CoreTweet.Tokens tokens, Dictionary<string, object> parm)
        {
            var tweets = await tokens.Statuses.UserTimelineAsync(parm);

            foreach (var tweet in tweets)
            {
                if (tweet.Entities.Media != null && tweet.Entities.Media.Length != 0)
                {
                    var img_count = tweet.ExtendedEntities.Media.Length;
                    for (; img_count > 0; img_count--)
                    {
                        var url = tweet.ExtendedEntities.Media[img_count - 1].MediaUrlHttps;
                        //for文だと先に引かれないためimg_countを-1している
                        //while文だと先に-1する処理にできるが、一文増えるので多分for文の記述のほうがスッキリする
                        var img_name = tweet.ExtendedEntities.Media[img_count - 1].Id;

                        Console.WriteLine(url + "\n");
                        //フォルダ指定 適当に絶対パス指定にしてるけど相対パス指定にする
                        DownloadImgAsync(url,
                            @"C:\新しいフォルダー\" + img_name + ".png");

                    }

                    Console.WriteLine(tweet.User.Name + "(@" + tweet.User.ScreenName + ")\r\n" + tweet.Text + "\r\n(at " + tweet.CreatedAt + ")" + " media_count " + tweet.ExtendedEntities.Media.Length + "\r\n");
                    Console.WriteLine("--------------");

                }
                //メディアなしの場合
                //elseで同じような処理を括るのは気持ち悪いので後で直したい
                else
                {
                    Console.WriteLine(tweet.User.Name + "(@" + tweet.User.ScreenName + ")\r\n" + tweet.Text + "\r\n(at " + tweet.CreatedAt + ")" + " media_count 0" + "\r\n");
                    Console.WriteLine("--------------");
                }
            }
        }
    }
}
