using System;

namespace TransporteUrbano
{
    public class Tiempo
    {
        public Tiempo() { }

        public virtual DateTime Now()
        {
            return DateTime.Now;
        }
    }
}