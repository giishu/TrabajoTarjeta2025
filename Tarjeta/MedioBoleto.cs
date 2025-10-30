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


        public override bool DescontarSaldo(decimal monto)
        {
            if (monto < 0)
                return false;

            // Verificar límite de 5 minutos entre viajes
            if (!HanPasado5MinutosDesdeUltimoViaje())
            {
                Console.WriteLine("Error: Deben pasar 5 minutos entre viajes con Medio Boleto");
                return false;
            }

            // Verificar límite de viajes diarios con descuento
            int viajesHoy = ObtenerViajesHoy();
            decimal tarifaAPagar = viajesHoy < LIMITE_VIAJES_DIARIOS ? monto : monto * 2;

            // Si es tercer viaje o más, pagar tarifa completa (doble)
            if (viajesHoy >= LIMITE_VIAJES_DIARIOS)
            {
                Console.WriteLine($"Atención: {viajesHoy + 1}° viaje del día - Tarifa completa aplicada");
            }

            // Verificar saldo suficiente
            decimal nuevoSaldo = saldo - tarifaAPagar;
            if (nuevoSaldo < ObtenerSaldoNegativoPermitido())
                return false;

            saldo = nuevoSaldo;
            RegistrarViaje();

            return true;
        }


        public (int viajesHoy, bool puedeViajar, decimal tarifaSugerida) ObtenerEstadoViaje()
        {
            int viajesHoy = ObtenerViajesHoy();
            bool puedeViajar = HanPasado5MinutosDesdeUltimoViaje();
            decimal tarifaSugerida = viajesHoy < LIMITE_VIAJES_DIARIOS ? 790m : 1580m;

            return (viajesHoy, puedeViajar, tarifaSugerida);
        }
    }
}