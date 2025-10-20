using NUnit.Framework;
using System;

namespace TestTransporteUrbano
{
    using TransporteUrbano;

    public class BoletoTests
    {
        private Boleto boleto;

        [SetUp]
        public void Setup()
        {
            boleto = new Boleto(1580m, 3420m, "144");
        }

        [Test]
        public void Constructor_CreaBoletoConLosDatosCorretos()
        {
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boleto.SaldoRestante, Is.EqualTo(3420m));
            Assert.That(boleto.LineaColectivo, Is.EqualTo("144"));
        }

        [Test]
        public void Constructor_EstableceLaFechaAlMomentoDeCreacion()
        {
            DateTime ahora = DateTime.Now;
            var boletoNuevo = new Boleto(1580m, 3420m, "144");

            Assert.IsNotNull(boletoNuevo.Fecha);
            Assert.That(boletoNuevo.Fecha, Is.EqualTo(ahora).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void Propiedad_TarifaAbonada_RetornaValorCorreto()
        {
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void Propiedad_SaldoRestante_RetornaValorCorreto()
        {
            Assert.That(boleto.SaldoRestante, Is.EqualTo(3420m));
        }

        [Test]
        public void Propiedad_LineaColectivo_RetornaValorCorreto()
        {
            Assert.That(boleto.LineaColectivo, Is.EqualTo("144"));
        }

        [Test]
        public void Propiedad_Fecha_RetornaFechaValida()
        {
            Assert.IsNotNull(boleto.Fecha);
            Assert.That(boleto.Fecha.Year, Is.GreaterThanOrEqualTo(2025));
        }

        [Test]
        public void ToString_RetornaFormatoCorreto()
        {
            string resultado = boleto.ToString();

            Assert.That(resultado, Does.Contain("Boleto"));
            Assert.That(resultado, Does.Contain("1580"));
            Assert.That(resultado, Does.Contain("3420"));
            Assert.That(resultado, Does.Contain("144"));
        }

        [Test]
        public void ToString_ContieneLasPropriedadesCorrectas()
        {
            string resultado = boleto.ToString();

            Assert.That(resultado, Does.Contain("Tarifa:"));
            Assert.That(resultado, Does.Contain("Saldo Restante:"));
            Assert.That(resultado, Does.Contain("Línea:"));
            Assert.That(resultado, Does.Contain("Fecha:"));
        }

        [Test]
        public void MultiplesInstancias_TienenFechasDiferentes()
        {
            var boleto1 = new Boleto(1580m, 3420m, "100");
            System.Threading.Thread.Sleep(10);
            var boleto2 = new Boleto(1580m, 2840m, "100");

            Assert.That(boleto1.Fecha, Is.Not.EqualTo(boleto2.Fecha));
        }

        [Test]
        public void Constructor_ConDiferentesValores_GuardaTodosCorrectamente()
        {
            var boleto2 = new Boleto(1580m, 8420m, "105");

            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boleto2.SaldoRestante, Is.EqualTo(8420m));
            Assert.That(boleto2.LineaColectivo, Is.EqualTo("105"));
        }
    }
}