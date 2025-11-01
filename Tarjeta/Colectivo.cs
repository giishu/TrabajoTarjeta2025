using System;

namespace TransporteUrbano
{
    public class Colectivo
    {
        public string Linea { get; private set; }
        private const decimal TARIFA_BASICA = 1580;

        public Colectivo(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                Console.WriteLine("Error: La línea del colectivo no puede estar vacía. Se establecerá como 'N/A'.");
                Linea = "N/A";
                return;
            }
            Linea = linea;
        }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                Console.WriteLine("Error: La tarjeta no puede ser null.");
                return null;
            }

            decimal tarifaBase = ObtenerTarifa(tarjeta);

            decimal tarifaReal = tarifaBase;
            if (tarjeta is MedioBoleto medioBoleto)
            {
                tarifaReal = medioBoleto.ObtenerTarifaReal(tarifaBase);
            }

            bool produciraSaldoNegativo = (tarjeta.ObtenerSaldo() - tarifaReal) < 0;

            bool pudoDescontar = tarjeta.DescontarSaldo(tarifaBase);

            if (!pudoDescontar)
            {
                Console.WriteLine($"Error: Saldo insuficiente. Se requieren ${tarifaReal} y el saldo disponible (incluyendo viaje plus) no es suficiente.");
                return null;
            }


            return new Boleto(
                tarifaAbonada: tarifaReal,
                saldoRestante: tarjeta.ObtenerSaldo(),
                lineaColectivo: Linea,
                tipoTarjeta: tarjeta.ObtenerTipoTarjeta(),
                idTarjeta: tarjeta.Id,
                tieneSaldoNegativo: produciraSaldoNegativo
            );
        }

        protected virtual decimal ObtenerTarifa(Tarjeta tarjeta)
        {
            if (tarjeta is FranquiciaCompleta || tarjeta is BoletoGratuito)
                return 0;
            else if (tarjeta is MedioBoleto)
                return TARIFA_BASICA / 2;
            else
                return TARIFA_BASICA;
        }

        public decimal ObtenerTarifaBasica()
        {
            return TARIFA_BASICA;
        }
    }
}