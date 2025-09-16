using System;
using System.Collections.Generic;

namespace Tarjeta
{
    public class Tarjeta
    {
        private decimal saldo;
        private readonly List<decimal> cargasValidas = new List<decimal>
        {
            2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
        };
        private const decimal LIMITE_SALDO = 40000;

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

        public decimal ObtenerSaldo()
        {
            return saldo;
        }

        public bool CargarSaldo(decimal monto)
        {
            if (!cargasValidas.Contains(monto))
                return false;

            if (saldo + monto > LIMITE_SALDO)
                return false;

            saldo += monto;
            return true;
        }

        public bool DescontarSaldo(decimal monto)
        {
            if (monto < 0)
                return false;

            if (saldo < monto)
                return false;

            saldo -= monto;
            return true;
        }

        public List<decimal> ObtenerCargasValidas()
        {
            return new List<decimal>(cargasValidas);
        }
    }
}