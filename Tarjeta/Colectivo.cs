using System;

namespace Tarjeta
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

            if (tarjeta.ObtenerSaldo() < TARIFA_BASICA)
            {
                Console.WriteLine($"Error: Saldo insuficiente. Se requieren ${TARIFA_BASICA} y solo tiene ${tarjeta.ObtenerSaldo()}");
                return null;
            }

            bool pudoDescontar = tarjeta.DescontarSaldo(TARIFA_BASICA);

            if (!pudoDescontar)
            {
                Console.WriteLine("Error: No se pudo descontar el saldo de la tarjeta");
                return null;
            }

            return new Boleto(TARIFA_BASICA, tarjeta.ObtenerSaldo(), Linea);
        }

        public decimal ObtenerTarifaBasica()
        {
            return TARIFA_BASICA;
        }
    }
}