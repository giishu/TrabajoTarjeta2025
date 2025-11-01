using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class BoletoGratuitoLimitesTests
    {
        [Test]
        public void BoletoGratuito_PrimerosDosViajes_Gratis()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);
            var colectivo = new Colectivo("144");

            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto1);
            Assert.That(boleto1.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(5000m));

            tiempoFalso.AgregarMinutos(6);

            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto2);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(5000m));
        }

        [Test]
        public void BoletoGratuito_TercerViajeDelDia_CobraTarifaCompleta()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);
            var colectivo = new Colectivo("144");

            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boleto1.TarifaAbonada, Is.EqualTo(0m));

            tiempoFalso.AgregarMinutos(6);

            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));

            tiempoFalso.AgregarMinutos(6);

            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(3420m)); 
        }

        [Test]
        public void BoletoGratuito_CuartoViajeDelDia_CobraTarifaCompleta()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var boletoGratuito = new BoletoGratuito(10000, tiempoFalso);
            var colectivo = new Colectivo("144");

            // Primeros 2 viajes gratis
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            // Tercer viaje - tarifa completa
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            // Cuarto viaje - tarifa completa
            Boleto boleto4 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boleto4.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(6840m)); // 10000 - 1580 - 1580
        }

        [Test]
        public void BoletoGratuito_NoPermiteViajeAntesDe5Minutos()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var boletoGratuito = new BoletoGratuito(tiempoFalso);
            boletoGratuito.CargarSaldo(5000);
            var colectivo = new Colectivo("144");

            // Primer viaje - debería funcionar
            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto1, "Primer viaje debería funcionar");

            // Intentar segundo viaje a los 3 minutos - debería fallar
            tiempoFalso.AgregarMinutos(3);
            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNull(boleto2, "No debería permitir viaje antes de 5 minutos");

            // Esperar 5 minutos completos - debería funcionar
            tiempoFalso.AgregarMinutos(2);
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto3, "Debería permitir viaje después de 5 minutos");
        }

        [Test]
        public void BoletoGratuito_TercerViajeSinSaldo_NoPermiteViaje()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var boletoGratuito = new BoletoGratuito(1000, tiempoFalso); // Saldo bajo
            var colectivo = new Colectivo("144");

            // Primeros 2 viajes gratis (funcionan aunque tenga poco saldo)
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            // Tercer viaje - necesita saldo para tarifa completa
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNull(boleto3, "No debería permitir viaje sin saldo para tarifa completa");
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(1000m)); // Saldo no cambia
        }

        [Test]
        public void BoletoGratuito_ReiniciaContadorAlDiaSiguiente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);
            var colectivo = new Colectivo("144");

            // Primer viaje - día 1
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            // Segundo viaje - día 1
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            // Tercer viaje - día 1 (tarifa completa)
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(3420m)); // 5000 - 1580

            // Avanzar al día siguiente
            tiempoFalso.AgregarDias(1);
            tiempoFalso.AgregarMinutos(10); // Asegurar que pasen 5 minutos

            // Primer viaje - día 2 (gratuito nuevamente)
            Boleto boleto4 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boleto4.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(3420m)); // Saldo sin cambios
        }
    }
}