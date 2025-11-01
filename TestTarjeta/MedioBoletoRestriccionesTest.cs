using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class MedioBoletoRestriccionesTests
    {
        private TiempoFalso tiempoFalso;
        private MedioBoleto medioBoleto;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            // Lunes 14 de octubre de 2024 a las 10:00 AM
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            medioBoleto = new MedioBoleto(5000, tiempoFalso);
            colectivo = new Colectivo("144");
        }

        // ===== TESTS DE DÍAS HÁBILES (Lunes a Viernes) =====

        [Test]
        public void MedioBoleto_Lunes_HorarioPermitido_PermiteViaje()
        {
            // Lunes 14 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje el lunes a las 10:00");
        }

        [Test]
        public void MedioBoleto_Martes_HorarioPermitido_PermiteViaje()
        {
            // Martes 15 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 15, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje el martes");
        }

        [Test]
        public void MedioBoleto_Miercoles_HorarioPermitido_PermiteViaje()
        {
            // Miércoles 16 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 16, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje el miércoles");
        }

        [Test]
        public void MedioBoleto_Jueves_HorarioPermitido_PermiteViaje()
        {
            // Jueves 17 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 17, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje el jueves");
        }

        [Test]
        public void MedioBoleto_Viernes_HorarioPermitido_PermiteViaje()
        {
            // Viernes 18 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 18, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje el viernes");
        }

        // ===== TESTS DE FIN DE SEMANA =====

        [Test]
        public void MedioBoleto_Sabado_NoPermiteViaje()
        {
            // Sábado 19 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje el sábado");
            Assert.That(medioBoleto.ObtenerSaldo(), Is.EqualTo(5000m), "No debería descontar saldo");
        }

        [Test]
        public void MedioBoleto_Domingo_NoPermiteViaje()
        {
            // Domingo 20 de octubre a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje el domingo");
            Assert.That(medioBoleto.ObtenerSaldo(), Is.EqualTo(5000m), "No debería descontar saldo");
        }

        // ===== TESTS DE HORARIOS PERMITIDOS =====

        [Test]
        public void MedioBoleto_6AM_Lunes_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 6:00 AM");
        }

        [Test]
        public void MedioBoleto_21_59PM_Lunes_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 59));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 21:59:59");
        }

        [Test]
        public void MedioBoleto_22PM_Lunes_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje a las 22:00");
        }

        [Test]
        public void MedioBoleto_5_59AM_Lunes_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 59));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje a las 5:59:59 AM");
        }

        [Test]
        public void MedioBoleto_Medianoche_Lunes_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 0, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje a medianoche");
        }

        // ===== TESTS COMBINADOS: DÍA + HORARIO =====

        [Test]
        public void MedioBoleto_Sabado_HorarioPermitido_NoPermiteViaje()
        {
            // Sábado a las 10:00 (horario permitido pero día no permitido)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje el sábado aunque sea horario permitido");
        }

        [Test]
        public void MedioBoleto_Lunes_HorarioNoPermitido_NoPermiteViaje()
        {
            // Lunes a las 23:00 (día permitido pero horario no permitido)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje el lunes fuera de horario");
        }

        [Test]
        public void MedioBoleto_Domingo_HorarioNoPermitido_NoPermiteViaje()
        {
            // Domingo a las 23:00 (ni día ni horario permitidos)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 23, 0, 0));

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNull(boleto, "No debería permitir viaje el domingo fuera de horario");
        }

        // ===== TESTS DE MÚLTIPLES VIAJES =====

        [Test]
        public void MedioBoleto_MultiplesViajesEnDiasHabiles_Funcionan()
        {
            // Lunes
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(medioBoleto));

            tiempoFalso.AgregarMinutos(10);

            // Martes
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 15, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(medioBoleto));

            tiempoFalso.AgregarMinutos(10);

            // Miércoles
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 16, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(medioBoleto));
        }

        [Test]
        public void MedioBoleto_IntentosSabadoYDomingo_TodosFallan()
        {
            decimal saldoInicial = medioBoleto.ObtenerSaldo();

            // Sábado
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            Assert.IsNull(colectivo.PagarCon(medioBoleto));

            // Domingo
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 15, 0, 0));
            Assert.IsNull(colectivo.PagarCon(medioBoleto));

            Assert.That(medioBoleto.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }

        // ===== TESTS DE MÉTODOS AUXILIARES =====

        [Test]
        public void PuedeViajarEnEsteHorario_LunesEnHorarioPermitido_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            bool resultado = medioBoleto.PuedeViajarEnEsteHorario();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_SabadoEnHorarioPermitido_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            bool resultado = medioBoleto.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_LunesFueraDeHorario_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            bool resultado = medioBoleto.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void ObtenerHorarioPermitido_RetornaInformacionCorrecta()
        {
            var (horaInicio, horaFin, dias) = medioBoleto.ObtenerHorarioPermitido();

            Assert.That(horaInicio, Is.EqualTo(6));
            Assert.That(horaFin, Is.EqualTo(22));
            Assert.That(dias, Is.EqualTo("Lunes a Viernes"));
        }

        // ===== TESTS DE INTEGRACIÓN CON OTRAS RESTRICCIONES =====

        [Test]
        public void MedioBoleto_SegundoViajeAntes5Minutos_NoPermiteAunqueSeanDiasYHorariosPermitidos()
        {
            // Primer viaje
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(medioBoleto));

            // Intentar segundo viaje a los 3 minutos
            tiempoFalso.AgregarMinutos(3);
            Assert.IsNull(colectivo.PagarCon(medioBoleto), "No debería permitir viaje antes de 5 minutos");
        }

        [Test]
        public void MedioBoleto_TercerViajeDelDia_EnDiaHabil_CobraTarifaCompleta()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(medioBoleto);
            Assert.That(boleto1.TarifaAbonada, Is.EqualTo(790m));

            tiempoFalso.AgregarMinutos(6);

            // Segundo viaje
            Boleto boleto2 = colectivo.PagarCon(medioBoleto);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(790m));

            tiempoFalso.AgregarMinutos(6);

            // Tercer viaje - tarifa completa
            Boleto boleto3 = colectivo.PagarCon(medioBoleto);
            Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m));
        }

        // ===== TEST DE TODOS LOS DÍAS DE LA SEMANA =====

        [Test]
        [TestCase(14, DayOfWeek.Monday, true, "Lunes")]
        [TestCase(15, DayOfWeek.Tuesday, true, "Martes")]
        [TestCase(16, DayOfWeek.Wednesday, true, "Miércoles")]
        [TestCase(17, DayOfWeek.Thursday, true, "Jueves")]
        [TestCase(18, DayOfWeek.Friday, true, "Viernes")]
        [TestCase(19, DayOfWeek.Saturday, false, "Sábado")]
        [TestCase(20, DayOfWeek.Sunday, false, "Domingo")]
        public void MedioBoleto_VerificaTodosLosDiasDeLaSemana(int dia, DayOfWeek diaSemana, bool deberiaPermitir, string nombreDia)
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, dia, 10, 0, 0));

            // Verificar que el día sea correcto
            Assert.That(tiempoFalso.Now().DayOfWeek, Is.EqualTo(diaSemana), $"El día {dia} debería ser {nombreDia}");

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            if (deberiaPermitir)
            {
                Assert.IsNotNull(boleto, $"Debería permitir viaje el {nombreDia}");
            }
            else
            {
                Assert.IsNull(boleto, $"No debería permitir viaje el {nombreDia}");
            }
        }

        // ===== TESTS DE TRANSICIONES =====

        [Test]
        public void MedioBoleto_TransicionViernesASabado_BloqueoEnSegundoViaje()
        {
            // Viernes a las 21:30
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 18, 21, 30, 0));
            Boleto boleto1 = colectivo.PagarCon(medioBoleto);
            Assert.IsNotNull(boleto1, "Viernes debería funcionar");

            // Sábado a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(medioBoleto);
            Assert.IsNull(boleto2, "Sábado no debería funcionar");
        }

        [Test]
        public void MedioBoleto_TransicionDomingoALunes_PermiteEnSegundoViaje()
        {
            // Domingo a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(medioBoleto);
            Assert.IsNull(boleto1, "Domingo no debería funcionar");

            // Lunes a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 21, 10, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(medioBoleto);
            Assert.IsNotNull(boleto2, "Lunes debería funcionar");
        }
    }
}