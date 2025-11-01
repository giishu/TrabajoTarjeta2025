using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class BoletoGratuitoTests
    {
        private BoletoGratuito boletoGratuito;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            boletoGratuito = new BoletoGratuito(tiempoFalso);
            boletoGratuito.CargarSaldo(5000);
            colectivo = new Colectivo("144");
        }

        [Test]
        public void Constructor_SinParametros_CreaTarjetaBoletoGratuitoConSaldoCero()
        {
            var bg = new BoletoGratuito();
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(0m));
        }

        [Test]
        public void Constructor_ConSaldoInicial_CreaTarjetaConSaldo()
        {
            var bg = new BoletoGratuito(3000);
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(3000m));
        }

        [Test]
        public void PagarCon_BoletoGratuito_PrimerosViajesGratis()
        {
            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(5000m));
        }

        [Test]
        public void PagarCon_BoletoGratuito_NoDescontaSaldo()
        {
            var bg = new BoletoGratuito(5000);
            Boleto boleto = colectivo.PagarCon(bg);

            Assert.IsNotNull(boleto);
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(5000m)); // Mantiene el saldo
        }

        [Test]
        public void PagarCon_BoletoGratuito_MultiplesViajes_NuncaDescontaSaldoEnPrimerosDos()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var bg = new BoletoGratuito(2000, tiempoFalso);

            colectivo.PagarCon(bg);
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(2000m));

            // Esperar 6 minutos
            tiempoFalso.AgregarMinutos(6);

            colectivo.PagarCon(bg);
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(2000m));
        }

        [Test]
        public void PagarCon_BoletoGratuito_SinSaldo_SiguePudiendoPagarPrimerosDos()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var bgSinSaldo = new BoletoGratuito(0, tiempoFalso);

            Boleto boleto1 = colectivo.PagarCon(bgSinSaldo);
            Assert.IsNotNull(boleto1);

            // Esperar 6 minutos
            tiempoFalso.AgregarMinutos(6);

            Boleto boleto2 = colectivo.PagarCon(bgSinSaldo);
            Assert.IsNotNull(boleto2);

            Assert.That(bgSinSaldo.ObtenerSaldo(), Is.EqualTo(0m));
        }

        [Test]
        public void CargarSaldo_BoletoGratuito_FuncionaNormalmente()
        {
            var bg = new BoletoGratuito();
            bool resultado = bg.CargarSaldo(5000);

            Assert.IsTrue(resultado);
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(5000m));
        }

        [Test]
        public void DescontarSaldo_BoletoGratuito_PrimerosDosViajesSiempreRetornaTrue()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var bg = new BoletoGratuito(100, tiempoFalso);

            bool resultado = bg.DescontarSaldo(1580);
            Assert.IsTrue(resultado);
            Assert.That(bg.ObtenerSaldo(), Is.EqualTo(100m));
        }
    }
}