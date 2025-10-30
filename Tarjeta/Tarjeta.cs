using System;
using System.Collections.Generic;
using System.Linq;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        protected decimal saldo;
        private readonly List<decimal> cargasValidas = new List<decimal>
        {
            2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
        };
        private const decimal LIMITE_SALDO = 40000;
        private const decimal SALDO_NEGATIVO_PERMITIDO = -1200;

        protected Tiempo _tiempo;
        protected List<DateTime> _viajes;

        public Tarjeta()
        {
            saldo = 0;
            _tiempo = new Tiempo();
            _viajes = new List<DateTime>();
        }

        public Tarjeta(decimal saldoInicial)
        {
            _tiempo = new Tiempo();
            _viajes = new List<DateTime>();

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
            _tiempo = tiempo;
            _viajes = new List<DateTime>();
        }

        public Tarjeta(decimal saldoInicial, Tiempo tiempo)
        {
            _tiempo = tiempo;
            _viajes = new List<DateTime>();

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


        public virtual decimal ObtenerSaldo()
        {
            return saldo;
        }

        public bool CargarSaldo(decimal monto)
        {
            if (!cargasValidas.Contains(monto))
                return false;

            decimal nuevoSaldo = saldo + monto;

            if (nuevoSaldo > LIMITE_SALDO)
                return false;

            saldo = nuevoSaldo;
            return true;
        }

        public virtual bool DescontarSaldo(decimal monto)
        {
            if (monto < 0)
                return false;

            decimal nuevoSaldo = saldo - monto;

            if (nuevoSaldo < SALDO_NEGATIVO_PERMITIDO)
                return false;

            saldo = nuevoSaldo;
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
    }
}