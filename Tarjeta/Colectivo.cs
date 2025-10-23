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

            // Obtener la tarifa según el tipo de tarjeta
            decimal tarifaAPagar = ObtenerTarifa(tarjeta);

            // Intentar descontar el saldo
            bool pudoDescontar = tarjeta.DescontarSaldo(tarifaAPagar);

            if (!pudoDescontar)
            {
                Console.WriteLine($"Error: Saldo insuficiente. Se requieren ${tarifaAPagar} y el saldo disponible (incluyendo viaje plus) no es suficiente.");
                return null;
            }

            return new Boleto(tarifaAPagar, tarjeta.ObtenerSaldo(), Linea);
        }

        protected virtual decimal ObtenerTarifa(Tarjeta tarjeta)
        {
            if (tarjeta is MedioBoleto)
            {
                return TARIFA_BASICA / 2;
            }
            return TARIFA_BASICA;
        }

        public decimal ObtenerTarifaBasica()
        {
            return TARIFA_BASICA;
        }
    }
}