using System;
using System.Collections.Generic;
using System.Linq;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        protected decimal saldo;
        protected decimal saldoPendiente;
        private readonly List<decimal> cargasValidas = new List<decimal>
        {
            2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
        };
        private const decimal LIMITE_SALDO = 56000m;
        private const decimal SALDO_NEGATIVO_PERMITIDO = -1200m;

        protected Tiempo _tiempo;
        protected List<DateTime> _viajes;
        public string Id { get; private set; }

        public decimal SaldoPendiente => saldoPendiente;

        public Tarjeta()
        {
            saldo = 0;
            saldoPendiente = 0;
            _tiempo = new Tiempo();
            _viajes = new List<DateTime>();
            Id = GenerarIdUnico();
        }

        public Tarjeta(decimal saldoInicial)
        {
            _tiempo = new Tiempo();
            _viajes = new List<DateTime>();
            saldoPendiente = 0;
            Id = GenerarIdUnico();

            if (saldoInicial < 0)
            {
                Console.WriteLine("Error: El saldo inicial no puede ser negativo. Se establecerá en 0.");
                saldo = 0;
                return;
            }
            if (saldoInicial > LIMITE_SALDO)
            {
                Console.WriteLine($"Error: El saldo inicial no puede superar el límite de ${LIMITE_SALDO}. Se establecerá en 0.");
                saldo = 0;
                return;
            }
            saldo = saldoInicial;
        }

        public Tarjeta(Tiempo tiempo)
        {
            saldo = 0;
            saldoPendiente = 0;
            _tiempo = tiempo;
            _viajes = new List<DateTime>();
            Id = GenerarIdUnico();
        }

        public Tarjeta(decimal saldoInicial, Tiempo tiempo)
        {
            _tiempo = tiempo;
            _viajes = new List<DateTime>();
            saldoPendiente = 0;
            Id = GenerarIdUnico();

            if (saldoInicial < 0)
            {
                Console.WriteLine("Error: El saldo inicial no puede ser negativo. Se establecerá en 0.");
                saldo = 0;
                return;
            }
            if (saldoInicial > LIMITE_SALDO)
            {
                Console.WriteLine($"Error: El saldo inicial no puede superar el límite de ${LIMITE_SALDO}. Se establecerá en 0.");
                saldo = 0;
                return;
            }
            saldo = saldoInicial;
        }

        protected void RegistrarViaje()
        {
            _viajes.Add(_tiempo.Now());
        }

        protected int ObtenerViajesHoy()
        {
            DateTime hoy = _tiempo.Now().Date;
            return _viajes.Count(v => v.Date == hoy);
        }

        protected DateTime? GetUltimoViaje()
        {
            return _viajes.Count > 0 ? _viajes.Last() : null;
        }

        protected bool HanPasado5MinutosDesdeUltimoViaje()
        {
            var ultimoViaje = GetUltimoViaje();
            if (ultimoViaje == null) return true;

            TimeSpan diferencia = _tiempo.Now() - ultimoViaje.Value;
            return diferencia.TotalMinutes >= 5;
        }

        private string GenerarIdUnico()
        {
            return $"TARJ-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public virtual string ObtenerTipoTarjeta()
        {
            return "Normal";
        }

        public virtual decimal ObtenerSaldo()
        {
            return saldo;
        }

        public bool CargarSaldo(decimal monto)
        {
            if (!cargasValidas.Contains(monto))
                return false;

            AcreditarCarga();

            decimal nuevoSaldo = saldo + monto;

            if (nuevoSaldo <= LIMITE_SALDO)
            {
                saldo = nuevoSaldo;
            }
            else
            {
                decimal excedente = nuevoSaldo - LIMITE_SALDO;
                saldo = LIMITE_SALDO;
                saldoPendiente += excedente;
                Console.WriteLine($"Carga parcial: ${monto - excedente} acreditado, ${excedente} en saldo pendiente");
            }
            return true;
        }

        public void AcreditarCarga()
        {
            if (saldoPendiente > 0 && saldo < LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - saldo;
                decimal montoAAcreditar = Math.Min(saldoPendiente, espacioDisponible);

                saldo += montoAAcreditar;
                saldoPendiente -= montoAAcreditar;

                if (montoAAcreditar > 0)
                {
                    Console.WriteLine($"Saldo pendiente acreditado: ${montoAAcreditar}");
                }
            }
        }

        public virtual bool DescontarSaldo(decimal monto)
        {
            if (monto < 0)
                return false;

            decimal nuevoSaldo = saldo - monto;

            if (nuevoSaldo < SALDO_NEGATIVO_PERMITIDO)
                return false;

            saldo = nuevoSaldo;
            AcreditarCarga();
            return true;
        }

        public List<decimal> ObtenerCargasValidas()
        {
            return new List<decimal>(cargasValidas);
        }

        public decimal ObtenerSaldoNegativoPermitido()
        {
            return SALDO_NEGATIVO_PERMITIDO;
        }

        public decimal ObtenerLimiteSaldo()
        {
            return LIMITE_SALDO;
        }
    }
}