using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desafio2
{
    public class DadosCotacao
    {
        public double vlr_cotacao { get; set; }
        public int cod_cotacao { get; set; }
        public DateTime dat_cotacao { get; set; }

        public static DadosCotacao FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            var dadosCotacao = new DadosCotacao();
            dadosCotacao.vlr_cotacao = Convert.ToDouble(values[0]);
            dadosCotacao.cod_cotacao = Convert.ToInt32(values[1]);
            dadosCotacao.dat_cotacao = Convert.ToDateTime(values[2]);
            return dadosCotacao;
        }
    }
}
