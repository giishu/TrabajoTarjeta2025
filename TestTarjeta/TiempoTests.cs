using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class TiempoTests
    {
        [Test]
        public void Tiempo_Now_RetornaFechaActual()
        {
            var tiempo = new Tiempo();
            DateTime ahora = DateTime.Now;

            DateTime resultado = tiempo.Now();

            Assert.That(resultado, Is.EqualTo(ahora).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void Tiempo_Constructor_NoLanzaExcepcion()
        {
            Assert.DoesNotThrow(() => new Tiempo());
        }

        [Test]
        public void Tiempo_Now_LlamadosConsecutivos_RetornanFechasSimilares()
        {
            var tiempo = new Tiempo();

            DateTime tiempo1 = tiempo.Now();
            DateTime tiempo2 = tiempo.Now();

            Assert.That((tiempo2 - tiempo1).TotalSeconds, Is.LessThan(1));
        }
    }

    public class TiempoFalsoTests
    {
        [Test]
        public void TiempoFalso_ConstructorSinParametros_InicializaConFechaPredeterminada()
        {
            var tiempoFalso = new TiempoFalso();

            DateTime resultado = tiempoFalso.Now();

            Assert.That(resultado, Is.EqualTo(new DateTime(2024, 10, 14, 10, 0, 0)));
        }

        [Test]
        public void TiempoFalso_ConstructorConFecha_InicializaConFechaIndicada()
        {
            var fechaInicial = new DateTime(2024, 12, 25, 15, 30, 0);
            var tiempoFalso = new TiempoFalso(fechaInicial);

            DateTime resultado = tiempoFalso.Now();

            Assert.That(resultado, Is.EqualTo(fechaInicial));
        }

        [Test]
        public void AgregarDias_Suma1Dia_AumentaFechaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarDias(1);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 15, 10, 0, 0)));
        }

        [Test]
        public void AgregarDias_Suma7Dias_AumentaFechaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarDias(7);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 21, 10, 0, 0)));
        }

        [Test]
        public void AgregarMinutos_Suma30Minutos_AumentaFechaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarMinutos(30);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 14, 10, 30, 0)));
        }

        [Test]
        public void AgregarMinutos_Suma90Minutos_CambiaDiaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 23, 0, 0));

            tiempoFalso.AgregarMinutos(90);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 15, 0, 30, 0)));
        }

        [Test]
        public void AgregarHoras_Suma2Horas_AumentaFechaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarHoras(2);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 14, 12, 0, 0)));
        }

        [Test]
        public void AgregarHoras_Suma24Horas_CambiaDiaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarHoras(24);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 15, 10, 0, 0)));
        }

        [Test]
        public void EstablecerTiempo_CambiaFechaCompleta()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var nuevaFecha = new DateTime(2025, 1, 1, 0, 0, 0);

            tiempoFalso.EstablecerTiempo(nuevaFecha);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(nuevaFecha));
        }

        [Test]
        public void AgregarDias_MultiplesLlamadas_AcumulaDias()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarDias(1);
            tiempoFalso.AgregarDias(2);
            tiempoFalso.AgregarDias(3);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 20, 10, 0, 0)));
        }

        [Test]
        public void AgregarMinutos_MultiplesLlamadas_AcumulaMinutos()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarMinutos(15);
            tiempoFalso.AgregarMinutos(20);
            tiempoFalso.AgregarMinutos(25);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 14, 11, 0, 0)));
        }

        [Test]
        public void AgregarHoras_MultiplesLlamadas_AcumulaHoras()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarHoras(1);
            tiempoFalso.AgregarHoras(2);
            tiempoFalso.AgregarHoras(3);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 14, 16, 0, 0)));
        }

        [Test]
        public void TiempoFalso_CombinacionDeMetodos_FuncionaCorrectamente()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarDias(1);
            tiempoFalso.AgregarHoras(5);
            tiempoFalso.AgregarMinutos(30);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 15, 15, 30, 0)));
        }

        [Test]
        public void AgregarDias_ConNumerosNegativos_RestaFecha()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarDias(-1);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 13, 10, 0, 0)));
        }

        [Test]
        public void AgregarMinutos_ConNumerosNegativos_RestaFecha()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 30, 0));

            tiempoFalso.AgregarMinutos(-30);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 14, 10, 0, 0)));
        }

        [Test]
        public void AgregarHoras_ConNumerosNegativos_RestaFecha()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));

            tiempoFalso.AgregarHoras(-2);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 10, 14, 8, 0, 0)));
        }

        [Test]
        public void TiempoFalso_HeredaDeTiempo()
        {
            var tiempoFalso = new TiempoFalso();

            Assert.IsInstanceOf<Tiempo>(tiempoFalso);
        }

        [Test]
        public void Now_EsVirtual_PermiteOverride()
        {
            Tiempo tiempo = new TiempoFalso(new DateTime(2024, 1, 1, 0, 0, 0));

            DateTime resultado = tiempo.Now();

            Assert.That(resultado, Is.EqualTo(new DateTime(2024, 1, 1, 0, 0, 0)));
        }

        [Test]
        public void AgregarDias_Cero_NoModificaFecha()
        {
            var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var fechaOriginal = tiempoFalso.Now();

            tiempoFalso.AgregarDias(0);

            Assert.That(tiempoFalso.Now(), Is.EqualTo(fechaOriginal));
        }

        [Test]
        public void EstablecerTiempo_LlamadoMultiplesVeces_UsaUltimaFecha()
        {
            var tiempoFalso = new TiempoFalso();

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 1, 1, 0, 0, 0));
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 6, 15, 12, 0, 0));
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 12, 31, 23, 59, 59));

            Assert.That(tiempoFalso.Now(), Is.EqualTo(new DateTime(2024, 12, 31, 23, 59, 59)));
        }
    }
}