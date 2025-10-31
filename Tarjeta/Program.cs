using System;
using TransporteUrbano;

namespace TransporteUrbano
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Probando Nuevas Funcionalidades del Boleto ===");
            Console.WriteLine();

            // Probar tarjeta normal
            TestTarjetaNormal();

            // Probar medio boleto
            TestMedioBoleto();

            // Probar saldo negativo
            TestSaldoNegativo();

            // Probar franquicia completa
            TestFranquiciaCompleta();

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void TestTarjetaNormal()
        {
            Console.WriteLine("\n--- Probando Tarjeta Normal ---");
            Tarjeta tarjetaNormal = new Tarjeta(5000);
            Colectivo colectivo = new Colectivo("144");

            Console.WriteLine($"Tarjeta ID: {tarjetaNormal.Id}");
            Console.WriteLine($"Tipo: {tarjetaNormal.ObtenerTipoTarjeta()}");

            Boleto boleto = colectivo.PagarCon(tarjetaNormal);
            if (boleto != null)
            {
                Console.WriteLine("\nBoleto generado:");
                Console.WriteLine(boleto.ToString());
            }
        }

        static void TestMedioBoleto()
        {
            Console.WriteLine("\n--- Probando Medio Boleto ---");
            MedioBoleto medioBoleto = new MedioBoleto(5000);
            Colectivo colectivo = new Colectivo("135");

            Console.WriteLine($"Tarjeta ID: {medioBoleto.Id}");
            Console.WriteLine($"Tipo: {medioBoleto.ObtenerTipoTarjeta()}");

            Boleto boleto = colectivo.PagarCon(medioBoleto);
            if (boleto != null)
            {
                Console.WriteLine("\nBoleto generado:");
                Console.WriteLine(boleto.ToString());
                Console.WriteLine($"¿Tarifa reducida? {boleto.TarifaAbonada == 790m}");
            }
        }

        static void TestSaldoNegativo()
        {
            Console.WriteLine("\n--- Probando Saldo Negativo ---");
            Tarjeta tarjetaPobre = new Tarjeta(1000);
            Colectivo colectivo = new Colectivo("115");

            Console.WriteLine($"Saldo inicial: ${tarjetaPobre.ObtenerSaldo()}");

            Boleto boleto = colectivo.PagarCon(tarjetaPobre);
            if (boleto != null)
            {
                Console.WriteLine("\nBoleto con saldo negativo:");
                Console.WriteLine(boleto.ToString());
                Console.WriteLine($"¿Tiene saldo negativo? {boleto.TieneSaldoNegativo}");
                Console.WriteLine($"Monto total abonado: ${boleto.MontoTotalAbonado}");
            }
        }

        static void TestFranquiciaCompleta()
        {
            Console.WriteLine("\n--- Probando Franquicia Completa ---");
            FranquiciaCompleta franquicia = new FranquiciaCompleta(2000);
            Colectivo colectivo = new Colectivo("107");

            Console.WriteLine($"Tarjeta ID: {franquicia.Id}");
            Console.WriteLine($"Tipo: {franquicia.ObtenerTipoTarjeta()}");

            Boleto boleto = colectivo.PagarCon(franquicia);
            if (boleto != null)
            {
                Console.WriteLine("\nBoleto franquicia completa:");
                Console.WriteLine(boleto.ToString());
                Console.WriteLine($"¿Tarifa gratuita? {boleto.TarifaAbonada == 0m}");
            }
        }
    }
}