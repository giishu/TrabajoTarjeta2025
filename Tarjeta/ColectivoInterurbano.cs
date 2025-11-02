using System;

namespace TransporteUrbano
{
    public class ColectivoInterurbano : Colectivo
    {
        private const decimal TARIFA_INTERURBANA = 3000;

        public ColectivoInterurbano(string linea) : base(linea)
        {
        }

        protected override decimal ObtenerTarifa(Tarjeta tarjeta)
        {
            if (tarjeta is FranquiciaCompleta)
                return 0;
            else if (tarjeta is MedioBoleto)
                return TARIFA_INTERURBANA / 2;
            else if (tarjeta is BoletoGratuito)
                return 0; // Este valor será ajustado por ObtenerTarifaReal()
            else
                return TARIFA_INTERURBANA;
        }

        public decimal ObtenerTarifaInterurbana()
        {
            return TARIFA_INTERURBANA;
        }
    }
}