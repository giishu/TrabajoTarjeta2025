using System;

namespace Tarjeta
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Tarjetas de Transporte Urbano ===");
            Console.WriteLine();

            Tarjeta miTarjeta = new Tarjeta();
            Console.WriteLine($"Tarjeta creada. Saldo inicial: ${miTarjeta.ObtenerSaldo()}");

            Console.WriteLine("\nCargando $5000 a la tarjeta...");
            bool cargaExitosa = miTarjeta.CargarSaldo(5000);
            Console.WriteLine($"Carga exitosa: {cargaExitosa}");
            Console.WriteLine($"Nuevo saldo: ${miTarjeta.ObtenerSaldo()}");

            Colectivo colectivo = new Colectivo("144");
            Console.WriteLine($"\nColectivo línea {colectivo.Linea} listo");
            Console.WriteLine($"Tarifa básica: ${colectivo.ObtenerTarifaBasica()}");

            try
            {
                Console.WriteLine("\nPagando pasaje...");
                Boleto boleto = colectivo.PagarCon(miTarjeta);

                if (boleto != null)
                {
                    Console.WriteLine("Boleto generado:");
                    Console.WriteLine(boleto.ToString());
                    Console.WriteLine($"Saldo restante en tarjeta: ${miTarjeta.ObtenerSaldo()}");
                }
                else
                {
                    Console.WriteLine("No se pudo generar el boleto.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}