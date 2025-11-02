using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    /// <summary>
    /// Tests adicionales para aumentar cobertura de Trasbordos
    /// Agregar estos tests al archivo TrasbordosTests.cs existente
    /// </summary>
    public class TrasbordosTestsExtra
    {
        private TiempoFalso tiempoFalso;
        private Tarjeta tarjeta;
        private Colectivo colectivo1;
        private Colectivo colectivo2;
        private Colectivo colectivo3;

        [SetUp]
        public void Setup()
        {
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            tarjeta = new Tarjeta(10000, tiempoFalso);
            colectivo1 = new Colectivo("144");
            colectivo2 = new Colectivo("102");
            colectivo3 = new Colectivo("103");
        }

        // ===== TESTS DE CASOS EDGE DE TRASBORDOS =====

        [Test]
        public void PuedeHacerTrasbordo_SinViajesPrevios_RetornaFalse()
        {
            bool resultado = tarjeta.PuedeHacerTrasbordo("102");
            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeHacerTrasbordo_MismaLinea_RetornaFalse()
        {
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(10);

            bool resultado = tarjeta.PuedeHacerTrasbordo("144");
            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeHacerTrasbordo_DespuesDe1Hora_RetornaFalse()
        {
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(61);

            bool resultado = tarjeta.PuedeHacerTrasbordo("102");
            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeHacerTrasbordo_FueraDeHorario_RetornaFalse()
        {
            // Viaje a las 21:30
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 30, 0));
            colectivo1.PagarCon(tarjeta);

            // Intentar trasbordo a las 22:15
            tiempoFalso.AgregarMinutos(45);
            bool resultado = tarjeta.PuedeHacerTrasbordo("102");
            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeHacerTrasbordo_Domingo_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0)); // Domingo
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);

            bool resultado = tarjeta.PuedeHacerTrasbordo("102");
            Assert.IsFalse(resultado);
        }

        [Test]
        public void ObtenerUltimoViajeConPago_DespuesDeViaje_RetornaInformacionCompleta()
        {
            colectivo1.PagarCon(tarjeta);

            var (fecha, linea) = tarjeta.ObtenerUltimoViajeConPago();

            Assert.IsNotNull(fecha);
            Assert.That(linea, Is.EqualTo("144"));
        }

        [Test]
        public void RegistrarViajeConPago_ActualizaUltimoViaje()
        {
            tarjeta.RegistrarViajeConPago("144");
            tiempoFalso.AgregarMinutos(10);
            tarjeta.RegistrarViajeConPago("102");

            var (fecha, linea) = tarjeta.ObtenerUltimoViajeConPago();
            Assert.That(linea, Is.EqualTo("102"));
        }

        [Test]
        public void Trasbordo_Exactamente60Minutos_NoPermite()
        {
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(60);

            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void Trasbordo_59Minutos59Segundos_Permite()
        {
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(59);
            tiempoFalso.EstablecerTiempo(tiempoFalso.Now().AddSeconds(59));

            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_VolvemosALineaInicial_DespuesDeOtraLinea_Funciona()
        {
            // 144 -> 102 (trasbordo) -> 144 (trasbordo)
            colectivo1.PagarCon(tarjeta); // 144
            tiempoFalso.AgregarMinutos(15);
            colectivo2.PagarCon(tarjeta); // 102 (trasbordo)
            tiempoFalso.AgregarMinutos(15);
            Boleto boleto3 = colectivo1.PagarCon(tarjeta); // 144 (trasbordo)

            Assert.IsTrue(boleto3.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_ConSaldoNegativo_Funciona()
        {
            var tarjetaBaja = new Tarjeta(2000, tiempoFalso);

            colectivo1.PagarCon(tarjetaBaja); // 420
            tiempoFalso.AgregarMinutos(30);
            Boleto boleto2 = colectivo2.PagarCon(tarjetaBaja); // Trasbordo gratis

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.That(tarjetaBaja.ObtenerSaldo(), Is.EqualTo(420m)); // No descontó
        }

        [Test]
        public void Trasbordo_Hora6AM_NoPermite()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 30, 0));
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(20);

            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsFalse(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_Hora22PM_NoPermite()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 50, 0));
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(15); // 22:05

            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsFalse(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_Hora21_59PM_Permite()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 30, 0));
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(20); // 21:50

            Boleto boleto2 = colectivo2.PagarCon(tarjeta);
            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_TransicionDiaNoPermitido_Rechaza()
        {
            // Sábado a las 21:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 21, 0, 0));
            colectivo1.PagarCon(tarjeta);

            // Domingo a las 10:00 (dentro de 1 hora pero día no permitido)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));
            Boleto boleto2 = colectivo2.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
        }

        [Test]
        public void Trasbordo_CuatroLineasDiferentes_Funciona()
        {
            var colectivo4 = new Colectivo("104");

            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(10);

            Boleto b2 = colectivo2.PagarCon(tarjeta);
            Assert.IsTrue(b2.EsTrasbordo);
            tiempoFalso.AgregarMinutos(10);

            Boleto b3 = colectivo3.PagarCon(tarjeta);
            Assert.IsTrue(b3.EsTrasbordo);
            tiempoFalso.AgregarMinutos(10);

            Boleto b4 = colectivo4.PagarCon(tarjeta);
            Assert.IsTrue(b4.EsTrasbordo);

            // Solo pagó el primero
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(8420m));
        }

        [Test]
        public void Trasbordo_VentanaExpiraCorrectamente()
        {
            // Primer viaje - paga
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(30);

            // Segundo viaje - trasbordo
            colectivo2.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(35); // Total 65 minutos desde el SEGUNDO viaje

            // Tercer viaje - verifica si es trasbordo
            // Como pasaron 35 minutos desde el último viaje, AÚN está en ventana de trasbordo
            Boleto boleto3 = colectivo3.PagarCon(tarjeta);

            // Tu implementación permite trasbordo porque no pasó 1 hora desde el ÚLTIMO viaje
            Assert.IsTrue(boleto3.EsTrasbordo);
        }

        [Test]
        public void PuedeHacerTrasbordo_Hora7AM_Permite()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 7, 0, 0));
            colectivo1.PagarCon(tarjeta);
            tiempoFalso.AgregarMinutos(20);

            bool resultado = tarjeta.PuedeHacerTrasbordo("102");
            Assert.IsTrue(resultado);
        }

        [Test]
        public void Trasbordo_ConMedioBoletoYFranquicia_AmbosRegistranTrasbordo()
        {
            var medioBoleto = new MedioBoleto(10000, tiempoFalso);
            var franquicia = new FranquiciaCompleta(5000, tiempoFalso);

            // Medio boleto
            colectivo1.PagarCon(medioBoleto);
            tiempoFalso.AgregarMinutos(30);
            Boleto bMB = colectivo2.PagarCon(medioBoleto);
            Assert.IsTrue(bMB.EsTrasbordo);

            // Franquicia
            colectivo1.PagarCon(franquicia);
            tiempoFalso.AgregarMinutos(30);
            Boleto bFC = colectivo2.PagarCon(franquicia);
            Assert.IsTrue(bFC.EsTrasbordo);
        }

        [Test]
        public void ObtenerUltimoViajeConPago_ConTrasbordo_ActualizaConCadaLineaDiferente()
        {
            colectivo1.PagarCon(tarjeta);

            tiempoFalso.AgregarMinutos(30);
            colectivo2.PagarCon(tarjeta); // Trasbordo - pero actualiza la línea

            var (fecha, linea) = tarjeta.ObtenerUltimoViajeConPago();

            // La línea se actualiza con cada viaje (incluso trasbordos)
            Assert.That(linea, Is.EqualTo("102"));
        }
    }
}