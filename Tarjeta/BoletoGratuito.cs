using System;

namespace TransporteUrbano
{
    public class BoletoGratuito : Tarjeta
    {
        private const int LIMITE_VIAJES_GRATUITOS = 2;
        private const int MINUTOS_ENTRE_VIAJES = 5;

        public BoletoGratuito() : base() { }
        public BoletoGratuito(decimal saldoInicial) : base(saldoInicial) { }
        public BoletoGratuito(Tiempo tiempo) : base(tiempo) { }
        public BoletoGratuito(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo) { }

        public override string ObtenerTipoTarjeta()
        {
            return "Boleto Gratuito";
        }

        public override bool DescontarSaldo(decimal monto)
        {
            if (monto < 0)
                return false;

            // NUEVO: Verificar restricción horaria
            if (!PuedeViajarEnEsteHorario())
            {
                Console.WriteLine("Error: Boleto Gratuito no puede viajar en este horario (solo permite viajes de lunes a viernes de 6:00 a 22:00)");
                return false;
            }

            if (!HanPasado5MinutosDesdeUltimoViaje())
            {
                Console.WriteLine("Error: Deben pasar 5 minutos entre viajes con Boleto Gratuito");
                return false;
            }

            int viajesHoy = ObtenerViajesHoy();

            if (viajesHoy >= LIMITE_VIAJES_GRATUITOS)
            {
                Console.WriteLine($"Atención: {viajesHoy + 1}° viaje del día - Tarifa completa aplicada");

                if (saldo < monto)
                {
                    Console.WriteLine($"Error: Saldo insuficiente para viaje pago. Saldo actual: ${saldo}, requerido: ${monto}");
                    return false;
                }

                saldo -= monto;
            }

            RegistrarViaje();
            AcreditarCarga();
            return true;
        }

        public decimal ObtenerTarifaReal(decimal tarifaBase)
        {
            int viajesHoy = ObtenerViajesHoy();

            if (viajesHoy >= LIMITE_VIAJES_GRATUITOS)
            {
                return tarifaBase;
            }

            return 0;
        }

        public (int viajesHoy, bool puedeViajar, decimal tarifaSugerida) ObtenerEstadoViaje()
        {
            int viajesHoy = ObtenerViajesHoy();
            bool puedeViajar = HanPasado5MinutosDesdeUltimoViaje();
            decimal tarifaSugerida = viajesHoy < LIMITE_VIAJES_GRATUITOS ? 0m : 1580m;

            return (viajesHoy, puedeViajar, tarifaSugerida);
        }

        public override decimal ObtenerDescuentoUsoFrecuente(decimal tarifaBase)
        {
            return 0; // Boleto Gratuito no tiene descuento por uso frecuente
        }

        public override decimal CalcularTarifaConDescuento(decimal tarifaBase)
        {
            return tarifaBase; // No aplica descuento por uso frecuente
        }

        /// <summary>
        /// Verifica si la franquicia puede viajar en el horario actual
        /// Horario permitido: Lunes a Viernes de 6:00 a 22:00
        /// </summary>
        public bool PuedeViajarEnEsteHorario()
        {
            DateTime ahora = _tiempo.Now();
            int hora = ahora.Hour;
            DayOfWeek dia = ahora.DayOfWeek;

            // Verificar que sea lunes a viernes
            bool esDiaHabil = dia >= DayOfWeek.Monday && dia <= DayOfWeek.Friday;

            // Verificar horario 6:00 a 22:00
            bool esHorarioPermitido = hora >= 6 && hora < 22;

            return esDiaHabil && esHorarioPermitido;
        }

        /// <summary>
        /// Obtiene el horario y días permitidos para viajar
        /// </summary>
        public (int horaInicio, int horaFin, string dias) ObtenerHorarioPermitido()
        {
            return (6, 22, "Lunes a Viernes");
        }
    }
}