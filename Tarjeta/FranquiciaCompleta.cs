using System;

namespace TransporteUrbano
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta() : base() { }
        public FranquiciaCompleta(decimal saldoInicial) : base(saldoInicial) { }

        public override bool DescontarSaldo(decimal monto)
        {
            RegistrarViaje(); // Registrar viaje aunque no descuente saldo
            return true;
        }

        public override string ObtenerTipoTarjeta()
        {
            return "Franquicia Completa";
        }

        // NUEVO: Override para que FranquiciaCompleta NO tenga descuento por uso frecuente
        public override decimal ObtenerDescuentoUsoFrecuente(decimal tarifaBase)
        {
            return 0; // Franquicia Completa no tiene descuento por uso frecuente (ya es gratis)
        }

        public override decimal CalcularTarifaConDescuento(decimal tarifaBase)
        {
            return tarifaBase; // No aplica descuento por uso frecuente
        }
    }
}