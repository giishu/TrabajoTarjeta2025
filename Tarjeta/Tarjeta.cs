using System;
using System.Collections.Generic;

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

        public Tarjeta()
        {
            saldo = 0;
        }

        public Tarjeta(decimal saldoInicial)
        {
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

        public virtual decimal ObtenerSaldo()
        {
            return saldo;
        }

        public bool CargarSaldo(decimal monto)
        {
            if (!cargasValidas.Contains(monto))
                return false;

            // Si hay saldo negativo, primero se descuenta de la carga
            decimal nuevoSaldo = saldo + monto;

            // Validar que no exceda el límite superior
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

            // Verificar que no se exceda el límite de saldo negativo
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