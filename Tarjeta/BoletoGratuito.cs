using System;

namespace TransporteUrbano
{
    public class BoletoGratuito : Tarjeta
    {
        public BoletoGratuito() : base()
        {
        }

        public BoletoGratuito(decimal saldoInicial) : base(saldoInicial)
        {
        }

        public override bool DescontarSaldo(decimal monto)
        {
            // El boleto gratuito no descuenta saldo
            return true;
        }
    }
}