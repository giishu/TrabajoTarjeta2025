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

        // CAMPOS PARA TRASBORDOS
        private DateTime? _ultimoViajeConPago;
        private string _ultimaLineaUsada;

        // CAMPO PARA USO FRECUENTE - Lista de viajes donde se pagó
        protected List<DateTime> _viajesPagados;

        public string Id { get; private set; }
        public decimal SaldoPendiente => saldoPendiente;

        public Tarjeta()
        {
            saldo = 0;
            saldoPendiente = 0;
            _tiempo = new Tiempo();
            _viajes = new List<DateTime>();
            _viajesPagados = new List<DateTime>();
            Id = GenerarIdUnico();
            _ultimoViajeConPago = null;
            _ultimaLineaUsada = null;
        }

        public Tarjeta(decimal saldoInicial)
        {
            _tiempo = new Tiempo();
            _viajes = new List<DateTime>();
            _viajesPagados = new List<DateTime>();
            saldoPendiente = 0;
            Id = GenerarIdUnico();
            _ultimoViajeConPago = null;
            _ultimaLineaUsada = null;

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
            _viajesPagados = new List<DateTime>();
            Id = GenerarIdUnico();
            _ultimoViajeConPago = null;
            _ultimaLineaUsada = null;
        }

        public Tarjeta(decimal saldoInicial, Tiempo tiempo)
        {
            _tiempo = tiempo;
            _viajes = new List<DateTime>();
            _viajesPagados = new List<DateTime>();
            saldoPendiente = 0;
            Id = GenerarIdUnico();
            _ultimoViajeConPago = null;
            _ultimaLineaUsada = null;

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

        // Método para registrar viajes pagados (cuando monto > 0)
        protected void RegistrarViajePagado()
        {
            _viajesPagados.Add(_tiempo.Now());
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
            RegistrarViaje();

            // Si pagó algo (no trasbordo), registrar como viaje pagado
            if (monto > 0)
            {
                RegistrarViajePagado();
            }

            AcreditarCarga();
            return true;
        }

        // ============================================
        // FUNCIONALIDAD: USO FRECUENTE
        // ============================================

        public int ObtenerViajesDelMes()
        {
            DateTime ahora = _tiempo.Now();
            int mesActual = ahora.Month;
            int anioActual = ahora.Year;

            // Contar solo viajes PAGADOS del mes actual
            return _viajesPagados.Count(v => v.Month == mesActual && v.Year == anioActual);
        }

        public virtual decimal ObtenerDescuentoUsoFrecuente(decimal tarifaBase)
        {
            if (ObtenerTipoTarjeta() != "Normal")
                return 0;

            int viajesEsteMes = ObtenerViajesDelMes();
            int proximoViaje = viajesEsteMes + 1;

            if (proximoViaje >= 30 && proximoViaje <= 59)
                return tarifaBase * 0.20m;
            else if (proximoViaje >= 60 && proximoViaje <= 80)
                return tarifaBase * 0.25m;
            else
                return 0m;
        }

        public virtual decimal CalcularTarifaConDescuento(decimal tarifaBase)
        {
            decimal descuento = ObtenerDescuentoUsoFrecuente(tarifaBase);
            return tarifaBase - descuento;
        }

        // ============================================
        // FUNCIONALIDAD: TRASBORDOS
        // ============================================

        public bool PuedeHacerTrasbordo(string lineaColectivo)
        {
            if (_ultimoViajeConPago == null || _ultimaLineaUsada == null)
                return false;

            if (_ultimaLineaUsada == lineaColectivo)
                return false;

            DateTime ahora = _tiempo.Now();
            TimeSpan diferencia = ahora - _ultimoViajeConPago.Value;
            if (diferencia.TotalHours >= 1)
                return false;

            if (!EsHorarioTrasbordo(ahora))
                return false;

            return true;
        }

        private bool EsHorarioTrasbordo(DateTime fecha)
        {
            DayOfWeek dia = fecha.DayOfWeek;
            int hora = fecha.Hour;

            bool esDiaPermitido = dia >= DayOfWeek.Monday && dia <= DayOfWeek.Saturday;
            bool esHorarioPermitido = hora >= 7 && hora < 22;

            return esDiaPermitido && esHorarioPermitido;
        }

        public void RegistrarViajeConPago(string lineaColectivo)
        {
            _ultimoViajeConPago = _tiempo.Now();
            _ultimaLineaUsada = lineaColectivo;
        }

        public (DateTime? fecha, string linea) ObtenerUltimoViajeConPago()
        {
            return (_ultimoViajeConPago, _ultimaLineaUsada);
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