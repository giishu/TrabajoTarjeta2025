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
            if (!PuedeViajarEnEsteHorario())
            {
                Console.WriteLine("Error: Franquicia Completa no puede viajar en este horario (solo permite viajes de lunes a viernes de 6:00 a 22:00)");
                return false;
            }

            RegistrarViaje();
            // Nunca registra viaje pagado porque nunca paga
            return true;
        }

        public override string ObtenerTipoTarjeta()
        {
            return "Franquicia Completa";
        }

        public override decimal ObtenerDescuentoUsoFrecuente(decimal tarifaBase)
        {
            return 0;
        }

        public override decimal CalcularTarifaConDescuento(decimal tarifaBase)
        {
            return tarifaBase;
        }

        public bool PuedeViajarEnEsteHorario()
        {
            DateTime ahora = _tiempo.Now();
            int hora = ahora.Hour;
            DayOfWeek dia = ahora.DayOfWeek;

            bool esDiaHabil = dia >= DayOfWeek.Monday && dia <= DayOfWeek.Friday;
            bool esHorarioPermitido = hora >= 6 && hora < 22;

            return esDiaHabil && esHorarioPermitido;
        }

        public (int horaInicio, int horaFin, string dias) ObtenerHorarioPermitido()
        {
            return (6, 22, "Lunes a Viernes");
        }
    }
}