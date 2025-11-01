using System;

namespace TransporteUrbano
{
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta() : base() { }
        public FranquiciaCompleta(decimal saldoInicial) : base(saldoInicial) { }
        public FranquiciaCompleta(Tiempo tiempo) : base(tiempo) { }
        public FranquiciaCompleta(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo) { }

        public override bool DescontarSaldo(decimal monto)
        {
            // Verificar restricción horaria
            if (!PuedeViajarEnEsteHorario())
            {
                Console.WriteLine("Error: Franquicia Completa no puede viajar en este horario (solo permite viajes entre 6:00 y 22:00)");
                return false;
            }

            RegistrarViaje(); // Registrar viaje aunque no descuente saldo
            return true;
        }

        public override string ObtenerTipoTarjeta()
        {
            return "Franquicia Completa";
        }

        /// <summary>
        /// Verifica si la franquicia puede viajar en el horario actual
        /// Horario permitido: 6:00 a 22:00 (6 AM a 10 PM)
        /// </summary>
        public bool PuedeViajarEnEsteHorario()
        {
            DateTime ahora = _tiempo.Now();
            int hora = ahora.Hour;

            // Permite viajes entre las 6:00 (inclusive) y las 22:00 (exclusive)
            // Es decir: 6:00:00 hasta 21:59:59
            return hora >= 6 && hora < 22;
        }

        /// <summary>
        /// Obtiene el horario permitido para viajar
        /// </summary>
        public (int horaInicio, int horaFin) ObtenerHorarioPermitido()
        {
            return (6, 22);
        }
    }
}