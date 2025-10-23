using System;

namespace TransporteUrbano
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta() : base()
        {
        }

        public FranquiciaCompleta(decimal saldoInicial) : base(saldoInicial)
        {
        }

        public override bool DescontarSaldo(decimal monto)
        {
            // La franquicia completa siempre puede pagar, no descuenta saldo
            return true;
        }
    }
}