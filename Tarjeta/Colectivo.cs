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
            decimal tarifaBase = ObtenerTarifa(tarjeta);

            // Para MedioBoleto y BoletoGratuito, obtener la tarifa real considerando límites
            decimal tarifaReal = tarifaBase;
            if (tarjeta is MedioBoleto medioBoleto)
            {
                tarifaReal = medioBoleto.ObtenerTarifaReal(tarifaBase);
            }
            else if (tarjeta is BoletoGratuito boletoGratuito)
            {
                tarifaReal = boletoGratuito.ObtenerTarifaReal(TARIFA_BASICA);
            }
            else if (tarjeta.ObtenerTipoTarjeta() == "Normal")
            {
                // NUEVO: Aplicar descuento por uso frecuente a tarjetas normales
                tarifaReal = tarjeta.CalcularTarifaConDescuento(TARIFA_BASICA);
            }

            // Verificar si habrá saldo negativo después del pago
            bool produciraSaldoNegativo = (tarjeta.ObtenerSaldo() - tarifaReal) < 0;

            // Intentar descontar el saldo
            bool pudoDescontar = tarjeta.DescontarSaldo(tarifaReal);

            if (!pudoDescontar)
            {
                Console.WriteLine($"Error: Saldo insuficiente. Se requieren ${tarifaReal} y el saldo disponible (incluyendo viaje plus) no es suficiente.");
                return null;
            }

            // Crear boleto con la tarifa real cobrada
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
            if (tarjeta is FranquiciaCompleta)
                return 0;
            else if (tarjeta is MedioBoleto)
                return TARIFA_BASICA / 2;
            else if (tarjeta is BoletoGratuito)
                return 0; // Este valor será ajustado por ObtenerTarifaReal()
            else
                return TARIFA_BASICA;
        }

        public decimal ObtenerTarifaBasica()
        {
            return TARIFA_BASICA;
        }
    }
}