using System;

namespace Desafio2
{
    public class DadosMoeda
    {
        public string ID_MOEDA { get; set; }
        public DateTime DATA_REF { get; set; }

        public static DadosMoeda FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            var dadosMoeda = new DadosMoeda();
            dadosMoeda.ID_MOEDA = values[0].ToString();
            dadosMoeda.DATA_REF = Convert.ToDateTime(values[1]);
            return dadosMoeda;
        }
    }
}
