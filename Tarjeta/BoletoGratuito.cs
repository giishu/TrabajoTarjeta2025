using System;

namespace TransporteUrbano
{
    public class BoletoGratuito : Tarjeta
    {
        public BoletoGratuito() : base() { }
        public BoletoGratuito(decimal saldoInicial) : base(saldoInicial) { }

        public override bool DescontarSaldo(decimal monto)
        {
            return true;
        }

        public override string ObtenerTipoTarjeta()
        {
            return "Boleto Gratuito";
        }
    }
}