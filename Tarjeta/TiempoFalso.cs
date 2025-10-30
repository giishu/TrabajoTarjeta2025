using System;

namespace TransporteUrbano
{
    public class TiempoFalso : Tiempo
    {
        private DateTime _tiempo;

        public TiempoFalso()
        {
            _tiempo = new DateTime(2024, 10, 14, 10, 0, 0);
        }

        public TiempoFalso(DateTime fechaInicial)
        {
            _tiempo = fechaInicial;
        }

        public override DateTime Now()
        {
            return _tiempo;
        }

        public void AgregarDias(int cantidad)
        {
            _tiempo = _tiempo.AddDays(cantidad);
        }

        public void AgregarMinutos(int cantidad)
        {
            _tiempo = _tiempo.AddMinutes(cantidad);
        }

        public void AgregarHoras(int cantidad)
        {
            _tiempo = _tiempo.AddHours(cantidad);
        }

        public void EstablecerTiempo(DateTime nuevoTiempo)
        {
            _tiempo = nuevoTiempo;
        }
    }
}