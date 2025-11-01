using System;

namespace TransporteUrbano
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta() : base() { }
        public FranquiciaCompleta(decimal saldoInicial) : base(saldoInicial) { }
        public FranquiciaCompleta(Tiempo tiempo) : base(tiempo) { }
        public FranquiciaCompleta(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo) { }

        public override bool DescontarSaldo(decimal monto)
        {
            // Verificar restricción horaria
            if (!PuedeViajarEnEsteHorario())
            {
                Console.WriteLine("Error: Franquicia Completa no puede viajar en este horario (solo permite viajes de lunes a viernes de 6:00 a 22:00)");
                return false;
            }

            RegistrarViaje(); // Registrar viaje aunque no descuente saldo
            return true;
        }

        public override string ObtenerTipoTarjeta()
        {
            return "Franquicia Completa";
        }

        public override decimal ObtenerDescuentoUsoFrecuente(decimal tarifaBase)
        {
            return 0; // Franquicia Completa no tiene descuento por uso frecuente
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

            // Verificar horario 6:00 a 22:00 (6:00:00 hasta 21:59:59)
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