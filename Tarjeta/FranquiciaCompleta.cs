using System;

namespace TransporteUrbano
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta() : base() { }
        public FranquiciaCompleta(decimal saldoInicial) : base(saldoInicial) { }

        public override bool DescontarSaldo(decimal monto)
        {
            return true;
        }

        public override string ObtenerTipoTarjeta()
        {
            return "Franquicia Completa";
        }
    }
}