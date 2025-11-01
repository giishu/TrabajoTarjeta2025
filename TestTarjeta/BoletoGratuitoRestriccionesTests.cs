using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class BoletoGratuitoRestriccionesTests
    {
        private TiempoFalso tiempoFalso;
        private BoletoGratuito boletoGratuito;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            // Lunes 14 de octubre de 2024 a las 10:00 AM
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            boletoGratuito = new BoletoGratuito(5000, tiempoFalso);
            colectivo = new Colectivo("144");
        }

        // ===== TESTS DE DÍAS HÁBILES (Lunes a Viernes) =====

        [Test]
        public void BoletoGratuito_Lunes_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje el lunes a las 10:00");
        }

        [Test]
        public void BoletoGratuito_Martes_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 15, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje el martes");
        }

        [Test]
        public void BoletoGratuito_Miercoles_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 16, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje el miércoles");
        }

        [Test]
        public void BoletoGratuito_Jueves_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 17, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje el jueves");
        }

        [Test]
        public void BoletoGratuito_Viernes_HorarioPermitido_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 18, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje el viernes");
        }

        // ===== TESTS DE FIN DE SEMANA =====

        [Test]
        public void BoletoGratuito_Sabado_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje el sábado");
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(5000m), "No debería descontar saldo");
        }

        [Test]
        public void BoletoGratuito_Domingo_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje el domingo");
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(5000m), "No debería descontar saldo");
        }

        // ===== TESTS DE HORARIOS PERMITIDOS =====

        [Test]
        public void BoletoGratuito_6AM_Lunes_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 6:00 AM");
        }

        [Test]
        public void BoletoGratuito_21_59PM_Lunes_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 59));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 21:59:59");
        }

        [Test]
        public void BoletoGratuito_22PM_Lunes_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje a las 22:00");
        }

        [Test]
        public void BoletoGratuito_5_59AM_Lunes_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 59));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje a las 5:59:59 AM");
        }

        [Test]
        public void BoletoGratuito_Medianoche_Lunes_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 0, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje a medianoche");
        }

        // ===== TESTS COMBINADOS: DÍA + HORARIO =====

        [Test]
        public void BoletoGratuito_Sabado_HorarioPermitido_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje el sábado aunque sea horario permitido");
        }

        [Test]
        public void BoletoGratuito_Lunes_HorarioNoPermitido_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje el lunes fuera de horario");
        }

        [Test]
        public void BoletoGratuito_Domingo_HorarioNoPermitido_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 23, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje el domingo fuera de horario");
        }

        // ===== TESTS DE MÚLTIPLES VIAJES =====

        [Test]
        public void BoletoGratuito_MultiplesViajesEnDiasHabiles_Funcionan()
        {
            // Lunes
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(boletoGratuito));

            tiempoFalso.AgregarMinutos(10);

            // Martes
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 15, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(boletoGratuito));

            tiempoFalso.AgregarMinutos(10);

            // Miércoles
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 16, 10, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(boletoGratuito));
        }

        [Test]
        public void BoletoGratuito_IntentosSabadoYDomingo_TodosFallan()
        {
            decimal saldoInicial = boletoGratuito.ObtenerSaldo();

            // Sábado
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            Assert.IsNull(colectivo.PagarCon(boletoGratuito));

            // Domingo
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 15, 0, 0));
            Assert.IsNull(colectivo.PagarCon(boletoGratuito));

            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }

        // ===== TESTS DE MÉTODOS AUXILIARES =====

        [Test]
        public void PuedeViajarEnEsteHorario_LunesEnHorarioPermitido_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            bool resultado = boletoGratuito.PuedeViajarEnEsteHorario();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_SabadoEnHorarioPermitido_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            bool resultado = boletoGratuito.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_LunesFueraDeHorario_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            bool resultado = boletoGratuito.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void ObtenerHorarioPermitido_RetornaInformacionCorrecta()
        {
            var (horaInicio, horaFin, dias) = boletoGratuito.ObtenerHorarioPermitido();

            Assert.That(horaInicio, Is.EqualTo(6));
            Assert.That(horaFin, Is.EqualTo(22));
            Assert.That(dias, Is.EqualTo("Lunes a Viernes"));
        }

        // ===== TESTS DE INTEGRACIÓN CON OTRAS RESTRICCIONES =====

        [Test]
        public void BoletoGratuito_PrimerosDosViajesGratis_EnDiasHabiles()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto1);
            Assert.That(boleto1.TarifaAbonada, Is.EqualTo(0m));

            tiempoFalso.AgregarMinutos(6);

            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto2);
            Assert.That(boleto2.TarifaAbonada, Is.EqualTo(0m));
        }

        [Test]
        public void BoletoGratuito_TercerViaje_EnDiaHabil_CobraTarifaCompleta()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            // Primeros dos viajes gratis
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);

            // Tercer viaje - tarifa completa
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto3);
            Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void BoletoGratuito_SegundoViajeAntes5Minutos_NoPermiteAunqueSeanDiasYHorariosPermitidos()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            // Primer viaje
            Assert.IsNotNull(colectivo.PagarCon(boletoGratuito));

            // Intentar segundo viaje a los 3 minutos
            tiempoFalso.AgregarMinutos(3);
            Assert.IsNull(colectivo.PagarCon(boletoGratuito), "No debería permitir viaje antes de 5 minutos");
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
        public void BoletoGratuito_VerificaTodosLosDiasDeLaSemana(int dia, DayOfWeek diaSemana, bool deberiaPermitir, string nombreDia)
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, dia, 10, 0, 0));

            Assert.That(tiempoFalso.Now().DayOfWeek, Is.EqualTo(diaSemana), $"El día {dia} debería ser {nombreDia}");

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

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
        public void BoletoGratuito_TransicionViernesASabado_BloqueoEnSegundoViaje()
        {
            // Viernes a las 21:30
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 18, 21, 30, 0));
            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto1, "Viernes debería funcionar");

            // Sábado a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNull(boleto2, "Sábado no debería funcionar");
        }

        [Test]
        public void BoletoGratuito_TransicionDomingoALunes_PermiteEnSegundoViaje()
        {
            // Domingo a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 20, 10, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNull(boleto1, "Domingo no debería funcionar");

            // Lunes a las 10:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 21, 10, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.IsNotNull(boleto2, "Lunes debería funcionar");
        }

        // ===== TESTS ADICIONALES PARA MAYOR COBERTURA =====

        [Test]
        public void BoletoGratuito_ViajesDiferentesDiasHabiles_ContadoresSeparados()
        {
            // Lunes: 2 viajes gratis
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            colectivo.PagarCon(boletoGratuito);
            tiempoFalso.AgregarMinutos(6);
            colectivo.PagarCon(boletoGratuito);

            // Martes: debería poder hacer 2 viajes gratis nuevamente
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 15, 10, 0, 0));
            Boleto boletoMartes1 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boletoMartes1.TarifaAbonada, Is.EqualTo(0m), "Primer viaje del martes debería ser gratis");

            tiempoFalso.AgregarMinutos(6);
            Boleto boletoMartes2 = colectivo.PagarCon(boletoGratuito);
            Assert.That(boletoMartes2.TarifaAbonada, Is.EqualTo(0m), "Segundo viaje del martes debería ser gratis");
        }

        [Test]
        public void BoletoGratuito_SinSaldo_PrimerosDosViajesFuncionan()
        {
            var bgSinSaldo = new BoletoGratuito(0, tiempoFalso);
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto1 = colectivo.PagarCon(bgSinSaldo);
            Assert.IsNotNull(boleto1, "Primer viaje sin saldo debería funcionar en día hábil");

            tiempoFalso.AgregarMinutos(6);

            Boleto boleto2 = colectivo.PagarCon(bgSinSaldo);
            Assert.IsNotNull(boleto2, "Segundo viaje sin saldo debería funcionar en día hábil");
        }

        [Test]
        public void BoletoGratuito_SabadoConSaldo_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 19, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.IsNull(boleto, "No debería permitir viaje el sábado aunque tenga saldo");
            Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(5000m), "Saldo no debería cambiar");
        }
    }
}