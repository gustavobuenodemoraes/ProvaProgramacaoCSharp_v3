using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Script.Serialization;

namespace Desafio2
{
    class Program
    {
        private static Timer aTimer;

        private static string local = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            SetTimer();

            Console.WriteLine("\nPressione a tecla Enter para sair do aplicativo...\n");
            Console.WriteLine("O aplicativo começou em {0:HH:mm:ss.fff}", DateTime.Now);
            Console.ReadLine();
            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("Encerrando o aplicativo...");
        }
        private static void SetTimer()
        {
            //aTimer = new Timer(120000);
            aTimer = new Timer(TimeSpan.FromMinutes(2).TotalMilliseconds);
            aTimer.Elapsed += OnTimedEventAsync;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static async void OnTimedEventAsync(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);

            var moedasApi = await GetMoedasApi();

            if (moedasApi == null)
                return;

            var dadosMoedaCSV = GetDadosMoeda();

            var moedaPeriodo = dadosMoedaCSV
                .Where(
                x => x.ID_MOEDA == moedasApi.Moeda
                && x.DATA_REF <= moedasApi.Data_inicio
                && x.DATA_REF >= moedasApi.Data_fim).ToList();

            var dePara = GetDePara();

            var dadosCotacaoCSV = GetDadosCotacao();

            var cotacao = (from dPara in dePara
                           from dadosCotacao in dadosCotacaoCSV
                           where dPara.cod_cotacao == dadosCotacao.cod_cotacao
                           select new
                           {
                               vlr_cotacao = dadosCotacao.vlr_cotacao,
                               ID_MOEDA = dPara.ID_MOEDA,
                               cod_cotacao = dadosCotacao.cod_cotacao,
                               dat_cotacao = dadosCotacao.dat_cotacao
                           }).ToList();


            var resultado = (from p in moedaPeriodo
                             from c in cotacao
                             where p.ID_MOEDA == c.ID_MOEDA
                             && p.DATA_REF == c.dat_cotacao
                             select new CSV
                             {
                                 ID_MOEDA = p.ID_MOEDA,
                                 cod_cotacao = c.cod_cotacao,
                                 vlr_cotacao = c.vlr_cotacao
                             });

            var nomeArquivo = "Resultado_" + DateTime.Now.ToString("yyyymmdd_HHmmss") + ".csv";

            File.WriteAllText(local + @"\ArquivosExcel\" + nomeArquivo, resultado.ToString());
        }

        private static async Task<Fila> GetMoedasApi()
        {
            Fila moeda = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44391/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Fila");

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    string data = await result.Content.ReadAsStringAsync();
                    var JSserializer = new JavaScriptSerializer();
                    var fila = JSserializer.Deserialize<Fila>(data);

                    moeda = fila;
                }
            };
            return moeda;
        }

        private static List<DadosMoeda> GetDadosMoeda()
        {
            var arquivo = Directory.GetCurrentDirectory();

            return File.ReadAllLines(local + @"\ArquivosExcel\DadosMoeda.csv")
                                                       .Skip(1)
                                                       .Select(v => DadosMoeda.FromCsv(v))
                                                       .ToList();
        }

        private static List<DadosCotacao> GetDadosCotacao()
        {
            var arquivo = Directory.GetCurrentDirectory();

            return File.ReadAllLines(local + @"\ArquivosExcel\DadosCotacao.csv")
                                                       .Skip(1)
                                                       .Select(v => DadosCotacao.FromCsv(v))
                                                       .ToList();
        }

        private static List<DePara> GetDePara()
        {
            List<DePara> dePara = null;
            using (StreamReader r = new StreamReader(local + @"\de-para.json"))
            {

                string json = r.ReadToEnd();

                var JSserializer = new JavaScriptSerializer();
                dePara = JSserializer.Deserialize<List<DePara>>(json);

            }

            return dePara;
        }

        class CSV
        {
            public string ID_MOEDA { get; set; }
            public double vlr_cotacao { get; set; }
            public int cod_cotacao { get; set; }
            public DateTime dat_cotacao { get; set; }

        }
    }
}
