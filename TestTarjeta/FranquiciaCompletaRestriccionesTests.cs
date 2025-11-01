using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class FranquiciaCompletaRestriccionesTests
    {
        private TiempoFalso tiempoFalso;
        private FranquiciaCompleta franquicia;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            // Iniciar en un horario permitido (10:00 AM) en día hábil (Lunes)
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            franquicia = new FranquiciaCompleta(5000, tiempoFalso);
            colectivo = new Colectivo("144");
        }

        // ===== TESTS DE HORARIOS PERMITIDOS =====

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_10AM_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto, "Debería permitir viaje a las 10:00 AM");
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(0m));
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_6AM_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto, "Debería permitir viaje a las 6:00 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_21_59PM_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto, "Debería permitir viaje a las 21:59 PM");
        }

        // ===== TESTS DE HORARIOS NO PERMITIDOS =====

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_22PM_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto, "No debería permitir viaje a las 22:00 PM");
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(5000m));
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_5_59AM_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto, "No debería permitir viaje a las 5:59 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_Medianoche_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 0, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto, "No debería permitir viaje a medianoche");
        }

        // ===== TESTS DE DÍAS DE LA SEMANA =====

        [Test]
        public void FranquiciaCompleta_Lunes_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto, "Debería permitir viaje el lunes");
        }

        [Test]
        public void FranquiciaCompleta_Viernes_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 18, 10, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto, "Debería permitir viaje el viernes");
        }

        [Test]
        public void FranquiciaCompleta_Sabado_HorarioPermitido_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto, "No debería permitir viaje el sábado");
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(5000m));
        }

        [Test]
        public void FranquiciaCompleta_Domingo_HorarioPermitido_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));
            Boleto boleto = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto, "No debería permitir viaje el domingo");
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(5000m));
        }

        [Test]
        [TestCase(14, DayOfWeek.Monday, true, "Lunes")]
        [TestCase(15, DayOfWeek.Tuesday, true, "Martes")]
        [TestCase(16, DayOfWeek.Wednesday, true, "Miércoles")]
        [TestCase(17, DayOfWeek.Thursday, true, "Jueves")]
        [TestCase(18, DayOfWeek.Friday, true, "Viernes")]
        [TestCase(19, DayOfWeek.Saturday, false, "Sábado")]
        [TestCase(20, DayOfWeek.Sunday, false, "Domingo")]
        public void FranquiciaCompleta_TodosLosDiasDeLaSemana(int dia, DayOfWeek diaSemana, bool deberiaPermitir, string nombreDia)
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, dia, 10, 0, 0));
            Assert.That(tiempoFalso.Now().DayOfWeek, Is.EqualTo(diaSemana));

            Boleto boleto = colectivo.PagarCon(franquicia);

            if (deberiaPermitir)
            {
                Assert.IsNotNull(boleto, $"Debería permitir viaje el {nombreDia}");
            }
            else
            {
                Assert.IsNull(boleto, $"No debería permitir viaje el {nombreDia}");
            }
        }

        // ===== TESTS DE MÉTODOS AUXILIARES =====

        [Test]
        public void PuedeViajarEnEsteHorario_LunesEnHorarioPermitido_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            bool resultado = franquicia.PuedeViajarEnEsteHorario();
            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_SabadoEnHorarioPermitido_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            bool resultado = franquicia.PuedeViajarEnEsteHorario();
            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_DomingoEnHorarioPermitido_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));
            bool resultado = franquicia.PuedeViajarEnEsteHorario();
            Assert.IsFalse(resultado);
        }

        [Test]
        public void ObtenerHorarioPermitido_RetornaInformacionCorrecta()
        {
            var (horaInicio, horaFin, dias) = franquicia.ObtenerHorarioPermitido();
            Assert.That(horaInicio, Is.EqualTo(6));
            Assert.That(horaFin, Is.EqualTo(22));
            Assert.That(dias, Is.EqualTo("Lunes a Viernes"));
        }

        // ===== TESTS DE TRANSICIONES =====

        [Test]
        public void FranquiciaCompleta_TransicionViernesASabado()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 18, 21, 30, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto1);

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto2);
        }

        [Test]
        public void FranquiciaCompleta_TransicionDomingoALunes()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto1);

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 21, 10, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto2);
        }

        [Test]
        public void DescontarSaldo_HorarioPermitido_Lunes_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            bool resultado = franquicia.DescontarSaldo(1580m);
            Assert.IsTrue(resultado);
        }

        [Test]
        public void DescontarSaldo_HorarioNoPermitido_Sabado_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            bool resultado = franquicia.DescontarSaldo(1580m);
            Assert.IsFalse(resultado);
        }

        [Test]
        public void FranquiciaCompleta_ViajesConsecutivosEnHorariosPermitidos_TodosFuncionan()
        {
            decimal saldoInicial = franquicia.ObtenerSaldo();

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 8, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto1);

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 12, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto2);

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 18, 0, 0));
            Boleto boleto3 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto3);

            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }

        [Test]
        public void FranquiciaCompleta_IntentosConsecutivosEnHorariosNoPermitidos_TodosFallan()
        {
            decimal saldoInicial = franquicia.ObtenerSaldo();

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 2, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto1);

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 4, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto2);

            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));
            Boleto boleto3 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto3);

            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }
    }
}