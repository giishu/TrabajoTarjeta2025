using System;

namespace TransporteUrbano
{
    public class Boleto
    {
        public DateTime Fecha { get; private set; }
        public decimal TarifaAbonada { get; private set; }
        public decimal SaldoRestante { get; private set; }
        public string LineaColectivo { get; private set; }

        public Boleto(decimal tarifaAbonada, decimal saldoRestante, string lineaColectivo)
        {
            Fecha = DateTime.Now;
            TarifaAbonada = tarifaAbonada;
            SaldoRestante = saldoRestante;
            LineaColectivo = lineaColectivo;
        }

        public override string ToString()
        {
            return $"Boleto - Fecha: {Fecha:dd/MM/yyyy HH:mm:ss}, " +
                   $"Tarifa: ${TarifaAbonada}, " +
                   $"Saldo Restante: ${SaldoRestante}, " +
                   $"Línea: {LineaColectivo}";
        }
    }
}