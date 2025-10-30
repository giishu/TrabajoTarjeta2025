using System;

namespace TransporteUrbano
{
    public class MedioBoleto : Tarjeta
    {
        public MedioBoleto() : base() { }
        public MedioBoleto(decimal saldoInicial) : base(saldoInicial) { }

        public override string ObtenerTipoTarjeta()
        {
            return "Medio Boleto";
        }
    }
}