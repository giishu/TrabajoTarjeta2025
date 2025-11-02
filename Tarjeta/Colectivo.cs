using System;

namespace TransporteUrbano
{
    public class Colectivo
    {
        public string Linea { get; private set; }
        private const decimal TARIFA_BASICA = 1580;

        public Colectivo(string linea)
        {
            Linea = string.IsNullOrWhiteSpace(linea) ? "N/A" : linea;
        }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null) return null;

            // 1. Verificar si es trasbordo
            bool esTrasbordo = tarjeta.PuedeHacerTrasbordo(Linea);

            // 2. Obtener tarifa base según tipo de tarjeta
            decimal tarifaBase = ObtenerTarifa(tarjeta);
            decimal tarifaReal = tarifaBase;

            // 3. Si NO es trasbordo, calcular la tarifa real según el tipo de tarjeta
            if (!esTrasbordo)
            {
                // Para tarjetas normales: aplicar descuento por uso frecuente
                if (tarjeta.ObtenerTipoTarjeta() == "Normal")
                {
                    tarifaReal = tarjeta.CalcularTarifaConDescuento(tarifaBase);
                }
                // Para medio boleto: aplicar su lógica específica
                else if (tarjeta is MedioBoleto mb)
                {
                    tarifaReal = mb.ObtenerTarifaReal(tarifaBase);
                }
                // Para boleto gratuito: aplicar su lógica específica
                else if (tarjeta is BoletoGratuito bg)
                {
                    tarifaReal = bg.ObtenerTarifaReal(TARIFA_BASICA);
                }
                // FranquiciaCompleta ya retorna 0 en ObtenerTarifa
            }
            else
            {
                // Si es trasbordo, tarifa = 0
                tarifaReal = 0;
            }

            // 4. Verificar si quedará con saldo negativo
            bool saldoNegativo = (tarjeta.ObtenerSaldo() - tarifaReal) < 0;

            // 5. Intentar descontar el saldo
            bool pudoPagar = tarjeta.DescontarSaldo(tarifaReal);

            if (!pudoPagar) return null;

            // 6. Registrar el viaje (incluso si es trasbordo gratuito)
            // Esto actualiza la línea para futuros trasbordos
            tarjeta.RegistrarViajeConPago(Linea);

            // 7. Crear y retornar el boleto
            return new Boleto(
                tarifaAbonada: tarifaReal,
                saldoRestante: tarjeta.ObtenerSaldo(),
                lineaColectivo: Linea,
                tipoTarjeta: tarjeta.ObtenerTipoTarjeta(),
                idTarjeta: tarjeta.Id,
                tieneSaldoNegativo: saldoNegativo,
                esTrasbordo: esTrasbordo
            );
        }

        protected virtual decimal ObtenerTarifa(Tarjeta tarjeta)
        {
            if (tarjeta is FranquiciaCompleta)
                return 0;
            if (tarjeta is MedioBoleto)
                return TARIFA_BASICA / 2;
            if (tarjeta is BoletoGratuito)
                return 0;

            return TARIFA_BASICA;
        }

        public decimal ObtenerTarifaBasica() => TARIFA_BASICA;
    }
}