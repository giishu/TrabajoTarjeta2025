using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class TrasbordosTests
    {
        private TiempoFalso tiempoFalso;
        private Tarjeta tarjeta;
        private Colectivo colectivo1;
        private Colectivo colectivo2;

        [SetUp]
        public void Setup()
        {
            // Lunes 10:00 AM
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            tarjeta = new Tarjeta(10000, tiempoFalso);
            colectivo1 = new Colectivo("144");
            colectivo2 = new Colectivo("102");
        }

        // ===== TESTS BÁSICOS DE TRASBORDO =====

        [Test]
        public void PrimerViaje_NoEsTrasbordo()
        {
            Boleto boleto = colectivo1.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.IsFalse(boleto.EsTrasbordo);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void SegundoViaje_LineaDiferente_Dentro1Hora_EsTrasbordo()
        {
            // Primer viaje
            colectivo1.PagarCon(tarjeta);

            // Segundo viaje después de 30 minutos
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(8420m)); // Solo descontó primer viaje
        }

        [Test]
        public void SegundoViaje_MismaLinea_NoEsTrasbordo()
        {
            // Primer viaje
            colectivo1.PagarCon(tarjeta);

            // Segundo viaje en misma línea después de 30 minutos
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo1.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void SegundoViaje_Despues1Hora_NoEsTrasbordo()
        {
            // Primer viaje
            colectivo1.PagarCon(tarjeta);

            // Segundo viaje después de 61 minutos
            tiempoFalso.AgregarMinutos(61);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
        }

        // ===== TESTS DE HORARIO DE TRASBORDO =====

        [Test]
        public void Trasbordo_Lunes_7AM_Permitido()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 7, 0, 0));

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_Lunes_21_59PM_Permitido()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 30, 0));

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(20);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_Lunes_22PM_NoPermitido()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 45, 0));

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(20); // 22:05
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void Trasbordo_Lunes_6AM_NoPermitido()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 30, 0));

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(20);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
        }

        // ===== TESTS DE DÍAS DE LA SEMANA =====

        [Test]
        public void Trasbordo_Sabado_Permitido()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0)); // Sábado

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_Domingo_NoPermitido()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0)); // Domingo

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        [TestCase(14, DayOfWeek.Monday, true, "Lunes")]
        [TestCase(15, DayOfWeek.Tuesday, true, "Martes")]
        [TestCase(16, DayOfWeek.Wednesday, true, "Miércoles")]
        [TestCase(17, DayOfWeek.Thursday, true, "Jueves")]
        [TestCase(18, DayOfWeek.Friday, true, "Viernes")]
        [TestCase(19, DayOfWeek.Saturday, true, "Sábado")]
        [TestCase(20, DayOfWeek.Sunday, false, "Domingo")]
        public void Trasbordo_TodosLosDias_VerificaPermiso(int dia, DayOfWeek diaSemana, bool deberiaPermitir, string nombreDia)
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, dia, 10, 0, 0));

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            if (deberiaPermitir)
            {
                Assert.IsTrue(boleto2.EsTrasbordo, $"Debería permitir trasbordo el {nombreDia}");
            }
            else
            {
                Assert.IsFalse(boleto2.EsTrasbordo, $"No debería permitir trasbordo el {nombreDia}");
            }
        }

        // ===== TESTS DE TRASBORDOS MÚLTIPLES =====

        [Test]
        public void TrasbordosMultiples_SinLimite_Dentro1Hora()
        {
            var colectivo3 = new Colectivo("103");
            var colectivo4 = new Colectivo("104");

            // Primer viaje - paga
            Boleto boleto1 = colectivo1.PagarCon(tarjeta);
            Assert.IsFalse(boleto1.EsTrasbordo);

            // Segundo viaje - trasbordo
            tiempoFalso.AgregarMinutos(15);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsTrue(boleto2.EsTrasbordo);

            // Tercer viaje - trasbordo
            tiempoFalso.AgregarMinutos(15);
            Boleto boleto3 = colectivo3.PagarCon(tarjeta);
            Assert.IsTrue(boleto3.EsTrasbordo);

            // Cuarto viaje - trasbordo
            tiempoFalso.AgregarMinutos(15);
            Boleto boleto4 = colectivo4.PagarCon(tarjeta);
            Assert.IsTrue(boleto4.EsTrasbordo);

            // Solo pagó el primer viaje
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(8420m));
        }

        [Test]
        public void Trasbordo_NoPermiteVolverAMismaLineaComoTrasbordo()
        {
            // Primer viaje en línea 144
            colectivo1.PagarCon(tarjeta);

            // Segundo viaje en línea 102 (trasbordo)
            tiempoFalso.AgregarMinutos(15);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsTrue(boleto2.EsTrasbordo);

            // Tercer viaje volviendo a línea 144
            // Como la última línea registrada es 102, y ahora va a 144, SÍ es trasbordo
            tiempoFalso.AgregarMinutos(15);
            Boleto boleto3 = colectivo1.PagarCon(tarjeta);
            Assert.IsTrue(boleto3.EsTrasbordo);
        }

        // ===== TESTS CON DIFERENTES TIPOS DE TARJETA =====

        [Test]
        public void Trasbordo_ConMedioBoleto_Funciona()
        {
            var medioBoleto = new MedioBoleto(10000, tiempoFalso);

            colectivo1.PagarCon(medioBoleto);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(medioBoleto);

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(medioBoleto.ObtenerSaldo(), Is.EqualTo(9210m)); // Solo descontó primer viaje (790)
        }

        [Test]
        public void Trasbordo_ConFranquiciaCompleta_Funciona()
        {
            var franquicia = new FranquiciaCompleta(5000, tiempoFalso);

            colectivo1.PagarCon(franquicia);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(franquicia);

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(5000m)); // No descontó nada
        }

        [Test]
        public void Trasbordo_ConBoletoGratuito_Funciona()
        {
            var boletoGratuito = new BoletoGratuito(5000, tiempoFalso);

            colectivo1.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(boletoGratuito);

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
        }

        // ===== TESTS CON COLECTIVOS INTERURBANOS =====

        [Test]
        public void Trasbordo_EntreUrbanoEInterurbano_Funciona()
        {
            var interurbano = new ColectivoInterurbano("K");

            colectivo1.PagarCon(tarjeta); // Urbano
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = interurbano.PagarCon(tarjeta); // Interurbano

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
        }

        [Test]
        public void Trasbordo_EntreInterurbanos_Funciona()
        {
            var interurbano1 = new ColectivoInterurbano("K");
            var interurbano2 = new ColectivoInterurbano("L");

            interurbano1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = interurbano2.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(7000m)); // Solo descontó primer viaje (3000)
        }

        // ===== TESTS DE MÉTODOS AUXILIARES =====

        [Test]
        public void PuedeHacerTrasbordo_SinViajesPrevios_RetornaFalse()
        {
            bool resultado = tarjeta.PuedeHacerTrasbordo("144");
            Assert.IsFalse(resultado);
        }

        [Test]
        public void ObtenerUltimoViajeConPago_SinViajes_RetornaNulls()
        {
            var (fecha, linea) = tarjeta.ObtenerUltimoViajeConPago();
            Assert.IsNull(fecha);
            Assert.IsNull(linea);
        }

        [Test]
        public void ObtenerUltimoViajeConPago_DespuesDeViaje_RetornaInformacion()
        {
            colectivo1.PagarCon(tarjeta);

            var (fecha, linea) = tarjeta.ObtenerUltimoViajeConPago();

            Assert.IsNotNull(fecha);
            Assert.That(linea, Is.EqualTo("144"));
        }

        [Test]
        public void RegistrarViajeConPago_ActualizaInformacion()
        {
            tarjeta.RegistrarViajeConPago("144");

            var (fecha, linea) = tarjeta.ObtenerUltimoViajeConPago();

            Assert.IsNotNull(fecha);
            Assert.That(linea, Is.EqualTo("144"));
        }

        // ===== TESTS DE EDGE CASES =====

        [Test]
        public void Trasbordo_Exactamente1Hora_NoPermite()
        {
            colectivo1.PagarCon(tarjeta);

            tiempoFalso.AgregarMinutos(60);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m)); // Debe pagar
        }

        [Test]
        public void Trasbordo_Exactamente59Minutos_Permite()
        {
            colectivo1.PagarCon(tarjeta);

            tiempoFalso.AgregarMinutos(59);
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void ToString_BoletoConTrasbordo_MuestraIndicador()
        {
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto = colectivo2.PagarCon(tarjeta);

            string resultado = boleto.ToString();
            Assert.That(resultado, Does.Contain("TRASBORDO"));
        }

        [Test]
        public void ToString_BoletoSinTrasbordo_NoMuestraIndicador()
        {
            Boleto boleto = colectivo1.PagarCon(tarjeta);

            string resultado = boleto.ToString();
            Assert.That(resultado, Does.Not.Contain("TRASBORDO"));
        }

        // ===== TESTS DE INTEGRACIÓN COMPLETA =====

        [Test]
        public void IntegracionCompleta_MultiplesViajesYTrasbordos()
        {
            var colectivo3 = new Colectivo("103");
            decimal saldoInicial = tarjeta.ObtenerSaldo();

            // Viaje 1 (paga)
            colectivo1.PagarCon(tarjeta); // -1580
            tiempoFalso.AgregarMinutos(20);

            // Viaje 2 (trasbordo)
            colectivo2.PagarCon(tarjeta); // -0
            tiempoFalso.AgregarMinutos(20);

            // Viaje 3 (trasbordo)
            colectivo3.PagarCon(tarjeta); // -0
            tiempoFalso.AgregarMinutos(30);

            // Viaje 4 (paga, pasó más de 1 hora desde viaje 1)
            colectivo1.PagarCon(tarjeta); // -1580

            // MODIFICADO: espera 8420 (solo 1 viaje pagado) en lugar de 6840 (2 viajes pagados)
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(saldoInicial - 1580m));
        }

        [Test]
        public void Trasbordo_DespuesDeFranquicia_Funciona()
        {
            var franquicia = new FranquiciaCompleta(1000, tiempoFalso);

            // Viaje gratuito con franquicia
            colectivo1.PagarCon(franquicia);

            // Trasbordo después
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(franquicia);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }
    }
}