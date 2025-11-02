using System;

namespace TransporteUrbano
{
    public class MedioBoleto : Tarjeta
    {
        private const int LIMITE_VIAJES_DIARIOS = 2;
        private const int MINUTOS_ENTRE_VIAJES = 5;

        public MedioBoleto() : base() { }
        public MedioBoleto(decimal saldoInicial) : base(saldoInicial) { }
        public MedioBoleto(Tiempo tiempo) : base(tiempo) { }
        public MedioBoleto(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo) { }

        public override string ObtenerTipoTarjeta()
        {
            return "Medio Boleto";
        }

        public override bool DescontarSaldo(decimal monto)
        {
            if (monto < 0)
                return false;

            if (!PuedeViajarEnEsteHorario())
            {
                Console.WriteLine("Error: Medio Boleto no puede viajar en este horario (solo permite viajes de lunes a viernes de 6:00 a 22:00)");
                return false;
            }

            if (!HanPasado5MinutosDesdeUltimoViaje())
            {
                Console.WriteLine("Error: Deben pasar 5 minutos entre viajes con Medio Boleto");
                return false;
            }

            int viajesHoy = ObtenerViajesHoy();

            if (viajesHoy >= LIMITE_VIAJES_DIARIOS)
            {
                Console.WriteLine($"Atención: {viajesHoy + 1}° viaje del día - Tarifa completa aplicada");
            }

            decimal nuevoSaldo = saldo - monto;
            if (nuevoSaldo < ObtenerSaldoNegativoPermitido())
                return false;

            saldo = nuevoSaldo;
            RegistrarViaje();

            if (monto > 0)
            {
                RegistrarViajePagado();
            }

            AcreditarCarga();
            return true;
        }

        public (int viajesHoy, bool puedeViajar, decimal tarifaSugerida) ObtenerEstadoViaje()
        {
            int viajesHoy = ObtenerViajesHoy();
            bool puedeViajar = HanPasado5MinutosDesdeUltimoViaje();
            decimal tarifaSugerida = viajesHoy < LIMITE_VIAJES_DIARIOS ? 790m : 1580m;

            return (viajesHoy, puedeViajar, tarifaSugerida);
        }

        public decimal ObtenerTarifaReal(decimal tarifaBase)
        {
            int viajesHoy = ObtenerViajesHoy();

            if (viajesHoy >= LIMITE_VIAJES_DIARIOS)
                return tarifaBase * 2;

            return tarifaBase;
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