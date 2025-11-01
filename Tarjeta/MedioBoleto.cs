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

            // Verificar límite de 5 minutos entre viajes
            if (!HanPasado5MinutosDesdeUltimoViaje())
            {
                Console.WriteLine("Error: Deben pasar 5 minutos entre viajes con Medio Boleto");
                return false;
            }

            // Verificar límite de viajes diarios con descuento
            int viajesHoy = ObtenerViajesHoy();

            // Si es tercer viaje o más, mostrar mensaje
            if (viajesHoy >= LIMITE_VIAJES_DIARIOS)
            {
                Console.WriteLine($"Atención: {viajesHoy + 1}° viaje del día - Tarifa completa aplicada");
            }

            // Verificar saldo suficiente
            decimal nuevoSaldo = saldo - monto;
            if (nuevoSaldo < ObtenerSaldoNegativoPermitido())
                return false;

            saldo = nuevoSaldo;
            RegistrarViaje(); // Registrar viaje antes de acreditar carga
            AcreditarCarga(); // Llamar a AcreditarCarga después de registrar
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

            // Si ya usó sus 2 viajes con descuento, cobra tarifa completa
            if (viajesHoy >= LIMITE_VIAJES_DIARIOS)
                return tarifaBase * 2; // Tarifa completa (1580)

            return tarifaBase; // Tarifa con descuento (790)
        }

        // NUEVO: Override para que MedioBoleto NO tenga descuento por uso frecuente
        public override decimal ObtenerDescuentoUsoFrecuente(decimal tarifaBase)
        {
            return 0; // Medio Boleto no tiene descuento por uso frecuente
        }

        public override decimal CalcularTarifaConDescuento(decimal tarifaBase)
        {
            return tarifaBase; // No aplica descuento por uso frecuente
        }
    }
}