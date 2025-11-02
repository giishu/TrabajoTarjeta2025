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
            boleto = new Boleto(1580m, 3420m, "144", "Normal", "TEST123", false);
        }

        [Test]
        public void Constructor_CreaBoletoConLosDatosCorretos()
        {
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boleto.SaldoRestante, Is.EqualTo(3420m));
            Assert.That(boleto.LineaColectivo, Is.EqualTo("144"));
            Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
            Assert.That(boleto.IdTarjeta, Is.EqualTo("TEST123"));
        }

        [Test]
        public void Constructor_EstableceLaFechaAlMomentoDeCreacion()
        {
            DateTime ahora = DateTime.Now;
            var boletoNuevo = new Boleto(1580m, 3420m, "144", "Normal", "TEST123", false);

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
            Assert.That(boleto.Fecha.Year, Is.GreaterThanOrEqualTo(2024));
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
            var boleto1 = new Boleto(1580m, 3420m, "100", "Normal", "TEST1", false);
            System.Threading.Thread.Sleep(10);
            var boleto2 = new Boleto(1580m, 2840m, "100", "Normal", "TEST2", false);

            Assert.That(boleto1.Fecha, Is.Not.EqualTo(boleto2.Fecha));
        }

        [Test]
        public void Constructor_ConDiferentesValores_GuardaTodosCorrectamente()
        {
            var boleto2 = new Boleto(1580m, 8420m, "105", "Medio Boleto", "MB123", false);

            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
            Assert.That(boleto2.SaldoRestante, Is.EqualTo(8420m));
            Assert.That(boleto2.LineaColectivo, Is.EqualTo("105"));
            Assert.That(boleto2.TipoTarjeta, Is.EqualTo("Medio Boleto"));
        }

        // ===== NUEVOS TESTS PARA AUMENTAR COBERTURA =====

        [Test]
        public void Constructor_ConTrasbordo_EsTrasbordo()
        {
            var boletoTrasbordo = new Boleto(0m, 5000m, "102", "Normal", "TEST456", false, true);
            Assert.IsTrue(boletoTrasbordo.EsTrasbordo);
        }

        [Test]
        public void Constructor_SinTrasbordo_NoEsTrasbordo()
        {
            var boletoNormal = new Boleto(1580m, 3420m, "144", "Normal", "TEST123", false, false);
            Assert.IsFalse(boletoNormal.EsTrasbordo);
        }

        [Test]
        public void ToString_ConTrasbordo_MuestraIndicadorTrasbordo()
        {
            var boletoTrasbordo = new Boleto(0m, 5000m, "102", "Normal", "TEST456", false, true);
            string resultado = boletoTrasbordo.ToString();

            Assert.That(resultado, Does.Contain("TRASBORDO"));
        }

        [Test]
        public void ToString_SinTrasbordo_NoMuestraIndicadorTrasbordo()
        {
            string resultado = boleto.ToString();
            Assert.That(resultado, Does.Not.Contain("TRASBORDO"));
        }

        [Test]
        public void Constructor_ConSaldoNegativo_TieneSaldoNegativoTrue()
        {
            var boletoConSaldoNegativo = new Boleto(1580m, -580m, "144", "Normal", "TEST789", true);
            Assert.IsTrue(boletoConSaldoNegativo.TieneSaldoNegativo);
        }

        [Test]
        public void Constructor_SinSaldoNegativo_TieneSaldoNegativoFalse()
        {
            Assert.IsFalse(boleto.TieneSaldoNegativo);
        }

        [Test]
        public void ToString_ConSaldoNegativo_MuestraMontoTotalAbonado()
        {
            var boletoConSaldoNegativo = new Boleto(1580m, -580m, "144", "Normal", "TEST789", true);
            string resultado = boletoConSaldoNegativo.ToString();

            Assert.That(resultado, Does.Contain("Monto Total Abonado"));
            Assert.That(resultado, Does.Contain("incluye saldo negativo"));
        }

        [Test]
        public void ToString_SinSaldoNegativo_NoMuestraMontoTotalAbonado()
        {
            string resultado = boleto.ToString();
            Assert.That(resultado, Does.Not.Contain("Monto Total Abonado"));
        }

        [Test]
        public void MontoTotalAbonado_ConSaldoNegativo_SumaAbsoluto()
        {
            var boletoConSaldoNegativo = new Boleto(1580m, -580m, "144", "Normal", "TEST789", true);
            Assert.That(boletoConSaldoNegativo.MontoTotalAbonado, Is.EqualTo(2160m)); // 1580 + 580
        }

        [Test]
        public void MontoTotalAbonado_SinSaldoNegativo_IgualTarifa()
        {
            Assert.That(boleto.MontoTotalAbonado, Is.EqualTo(1580m));
        }

        [Test]
        public void ToString_ConMedioBoleto_MuestraTipoCorrectamente()
        {
            var boletoMedio = new Boleto(790m, 4210m, "144", "Medio Boleto", "MB001", false);
            string resultado = boletoMedio.ToString();

            Assert.That(resultado, Does.Contain("Medio Boleto"));
        }

        [Test]
        public void ToString_ConFranquiciaCompleta_MuestraTipoCorrectamente()
        {
            var boletoFranquicia = new Boleto(0m, 5000m, "144", "Franquicia Completa", "FC001", false);
            string resultado = boletoFranquicia.ToString();

            Assert.That(resultado, Does.Contain("Franquicia Completa"));
        }

        [Test]
        public void ToString_ConBoletoGratuito_MuestraTipoCorrectamente()
        {
            var boletoGratuito = new Boleto(0m, 5000m, "144", "Boleto Gratuito", "BG001", false);
            string resultado = boletoGratuito.ToString();

            Assert.That(resultado, Does.Contain("Boleto Gratuito"));
        }

        [Test]
        public void Propiedad_TipoTarjeta_RetornaValorCorreto()
        {
            Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
        }

        [Test]
        public void Propiedad_IdTarjeta_RetornaValorCorreto()
        {
            Assert.That(boleto.IdTarjeta, Is.EqualTo("TEST123"));
        }

        [Test]
        public void ToString_ContrasbordoYSaldoNegativo_MuestraAmbos()
        {
            var boletoComplejo = new Boleto(1580m, -200m, "102", "Normal", "TEST999", true, true);
            string resultado = boletoComplejo.ToString();

            Assert.That(resultado, Does.Contain("TRASBORDO"));
            Assert.That(resultado, Does.Contain("Monto Total Abonado"));
        }

        [Test]
        public void Constructor_ConSaldoPositivo_MontoTotalAbonado_IgualTarifa()
        {
            var boletoPositivo = new Boleto(1580m, 3420m, "144", "Normal", "TEST", false);
            Assert.That(boletoPositivo.MontoTotalAbonado, Is.EqualTo(1580m));
        }

        [Test]
        public void ToString_ConLineaInterurbana_MuestraLineaCorrecta()
        {
            var boletoInterurbano = new Boleto(3000m, 2000m, "K", "Normal", "TEST", false);
            string resultado = boletoInterurbano.ToString();

            Assert.That(resultado, Does.Contain("Línea: K"));
        }
    }
}