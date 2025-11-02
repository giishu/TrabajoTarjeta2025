using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class ColectivoInterurbanoTests
    {
        private TiempoFalso tiempoFalso;
        private ColectivoInterurbano colectivoInterurbano;

        [SetUp]
        public void Setup()
        {
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0)); // Lunes 10:00
            colectivoInterurbano = new ColectivoInterurbano("K");
        }

        // ===== TESTS BÁSICOS =====

        [Test]
        public void Constructor_CreaColectivoInterurbanoConLineaCorrecta()
        {
            Assert.That(colectivoInterurbano.Linea, Is.EqualTo("K"));
        }

        [Test]
        public void ObtenerTarifaInterurbana_Retorna3000()
        {
            Assert.That(colectivoInterurbano.ObtenerTarifaInterurbana(), Is.EqualTo(3000m));
        }

        // ===== TESTS CON TARJETA NORMAL =====

        [Test]
        public void PagarCon_TarjetaNormal_CobraTarifaInterurbana()
        {
            var tarjeta = new Tarjeta(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(3000m));
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(2000m));
        }

        [Test]
        public void PagarCon_TarjetaNormalSinSaldoSuficiente_PermiteSaldoNegativo()
        {
            var tarjeta = new Tarjeta(2000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(3000m));
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(-1000m));
        }

        [Test]
        public void PagarCon_TarjetaNormalExcedeSaldoNegativo_NoPermiteViaje()
        {
            var tarjeta = new Tarjeta(1500, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNull(boleto);
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(1500m));
        }

        // ===== TESTS CON MEDIO BOLETO =====

        [Test]
        public void PagarCon_MedioBoleto_CobraMitadDeTarifaInterurbana()
        {
            var medioBoleto = new MedioBoleto(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(1500m)); // 3000 / 2
            Assert.That(medioBoleto.ObtenerSaldo(), Is.EqualTo(3500m));
        }

        [Test]
        public void PagarCon_MedioBoleto_TercerViaje_CobraTarifaCompletaInterurbana()
        {
            var medioBoleto = new MedioBoleto(10000, tiempoFalso);

            // Primer y segundo viaje
            colectivoInterurbano.PagarCon(medioBoleto);
            tiempoFalso.AgregarMinutos(6);
            colectivoInterurbano.PagarCon(medioBoleto);
            tiempoFalso.AgregarMinutos(6);

            // Tercer viaje
            Boleto boleto3 = colectivoInterurbano.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto3);
            Assert.That(boleto3.TarifaAbonada, Is.EqualTo(3000m)); // Tarifa completa
        }

        [Test]
        public void PagarCon_MedioBoleto_FueraDeHorario_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0)); // Sábado
            var medioBoleto = new MedioBoleto(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(medioBoleto);

            Assert.IsNull(boleto);
        }

        // ===== TESTS CON FRANQUICIA COMPLETA =====

        [Test]
        public void PagarCon_FranquiciaCompleta_NoCobraTarifa()
        {
            var franquicia = new FranquiciaCompleta(1000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(franquicia);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(1000m));
        }

        [Test]
        public void PagarCon_FranquiciaCompleta_FueraDeHorario_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0)); // Sábado
            var franquicia = new FranquiciaCompleta(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(franquicia);

            Assert.IsNull(boleto);
        }

        // ===== TESTS CON BOLETO GRATUITO =====

        [Test]
        public void PagarCon_BoletoGratuito_PrimerosDosViajesGratis()
        {
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);

            Boleto boleto1 = colectivoInterurbano.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto1);
            Assert.That(boleto1.TarifaAbonada, Is.EqualTo(0m));

            tiempoFalso.AgregarMinutos(6);

            Boleto boleto2 = colectivoInterurbano.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto2);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
        }

        [Test]
        public void PagarCon_BoletoGratuito_TercerViaje_CobraTarifaBasica()
        {
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);

            colectivoInterurbano.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);
            colectivoInterurbano.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            Boleto boleto3 = colectivoInterurbano.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto3);
            // Boleto Gratuito cobra tarifa BÁSICA (1580), no interurbana
            Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void PagarCon_BoletoGratuito_FueraDeHorario_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0)); // Domingo
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(boletoGratuito);

            Assert.IsNull(boleto);
        }

        // ===== TESTS DE INTEGRACIÓN =====

        [Test]
        public void PagarCon_MultiplesViajes_TarjetaNormal_DescuentaCorrectamente()
        {
            var tarjeta = new Tarjeta(10000, tiempoFalso);

            colectivoInterurbano.PagarCon(tarjeta); // 10000 - 3000 = 7000
            colectivoInterurbano.PagarCon(tarjeta); // 7000 - 3000 = 4000
            colectivoInterurbano.PagarCon(tarjeta); // 4000 - 3000 = 1000

            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(1000m));
        }

        [Test]
        public void ColectivoInterurbano_HeredaDeColectivo()
        {
            Assert.IsInstanceOf<Colectivo>(colectivoInterurbano);
        }

        [Test]
        public void PagarCon_DespuesDeCargaSaldo_FuncionaCorrectamente()
        {
            var tarjeta = new Tarjeta(tiempoFalso);
            tarjeta.CargarSaldo(5000);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(3000m));
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(2000m));
        }

        // ===== TESTS DE BOLETO =====

        [Test]
        public void Boleto_ContieneLineaCorrecta()
        {
            var tarjeta = new Tarjeta(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.That(boleto.LineaColectivo, Is.EqualTo("K"));
        }

        [Test]
        public void Boleto_ContieneTipoTarjetaCorrecto()
        {
            var medioBoleto = new MedioBoleto(5000, tiempoFalso);

            Boleto boleto = colectivoInterurbano.PagarCon(medioBoleto);

            Assert.That(boleto.TipoTarjeta, Is.EqualTo("Medio Boleto"));
        }

        // ===== TESTS DE COMPARACIÓN CON COLECTIVO URBANO =====

        [Test]
        public void TarifaInterurbana_EsMayorQueTarifaUrbana()
        {
            var colectivoUrbano = new Colectivo("144");

            Assert.That(colectivoInterurbano.ObtenerTarifaInterurbana(),
                       Is.GreaterThan(colectivoUrbano.ObtenerTarifaBasica()));
        }

        [Test]
        public void PagarCon_ComparacionConUrbano_DiferenciaCorrecta()
        {
            var tarjeta1 = new Tarjeta(5000, tiempoFalso);
            var tarjeta2 = new Tarjeta(5000, tiempoFalso);
            var colectivoUrbano = new Colectivo("144");

            colectivoInterurbano.PagarCon(tarjeta1);
            colectivoUrbano.PagarCon(tarjeta2);

            decimal diferencia = tarjeta2.ObtenerSaldo() - tarjeta1.ObtenerSaldo();
            Assert.That(diferencia, Is.EqualTo(1420m)); // 3000 - 1580
        }
    }
}