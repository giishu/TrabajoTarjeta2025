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
            // Iniciar en un horario permitido (10:00 AM)
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            franquicia = new FranquiciaCompleta(5000, tiempoFalso);
            colectivo = new Colectivo("144");
        }

        // ===== TESTS DE HORARIOS PERMITIDOS =====

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_10AM_PermiteViaje()
        {
            // 10:00 AM está en el rango permitido (6:00 - 22:00)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 10:00 AM");
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(0m));
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_6AM_PermiteViaje()
        {
            // 6:00 AM es el inicio del horario permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 6:00 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_6_30AM_PermiteViaje()
        {
            // 6:30 AM está en el rango permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 30, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 6:30 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_12PM_PermiteViaje()
        {
            // 12:00 PM (mediodía) está en el rango permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 12, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 12:00 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_6PM_PermiteViaje()
        {
            // 18:00 (6:00 PM) está en el rango permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 18, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 6:00 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_21_59PM_PermiteViaje()
        {
            // 21:59 (9:59 PM) es el último minuto del horario permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 21:59 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_21_59_59PM_PermiteViaje()
        {
            // 21:59:59 es el último segundo del horario permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 59));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 21:59:59 PM");
        }

        // ===== TESTS DE HORARIOS NO PERMITIDOS =====

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_22PM_NoPermiteViaje()
        {
            // 22:00 (10:00 PM) ya no está permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 22:00 PM");
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(5000m), "No debería haber descontado saldo");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_23PM_NoPermiteViaje()
        {
            // 23:00 (11:00 PM) no está permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 23:00 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_Medianoche_NoPermiteViaje()
        {
            // 0:00 (medianoche) no está permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 0, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a medianoche");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_2AM_NoPermiteViaje()
        {
            // 2:00 AM no está permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 2, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 2:00 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_5_59AM_NoPermiteViaje()
        {
            // 5:59 AM es el último minuto antes del horario permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 5:59 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_5_59_59AM_NoPermiteViaje()
        {
            // 5:59:59 AM es el último segundo antes del horario permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 59));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 5:59:59 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_22_01PM_NoPermiteViaje()
        {
            // 22:01 PM ya no está permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 1, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 22:01 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_3AM_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 3, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 3:00 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_4AM_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 4, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 4:00 AM");
        }

        // ===== TESTS DE CASOS LÍMITE Y TRANSICIONES =====

        [Test]
        public void FranquiciaCompleta_TransicionHoraria_De21_59a22_00_BloqueoEnSegundoViaje()
        {
            // Primer viaje a las 21:59 - debería funcionar
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto1, "Primer viaje a las 21:59 debería funcionar");

            // Avanzar a las 22:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto2, "Segundo viaje a las 22:00 no debería funcionar");
        }

        [Test]
        public void FranquiciaCompleta_TransicionHoraria_De5_59a6_00_PermiteEnSegundoViaje()
        {
            // Primer intento a las 5:59 - no debería funcionar
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto1, "Viaje a las 5:59 no debería funcionar");

            // Avanzar a las 6:00
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto2, "Viaje a las 6:00 debería funcionar");
        }

        [Test]
        public void FranquiciaCompleta_MultiplesViajesEnHorarioPermitido_TodosFuncionan()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            // Realizar 5 viajes en horario permitido
            for (int i = 0; i < 5; i++)
            {
                Boleto boleto = colectivo.PagarCon(franquicia);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} en horario permitido debería funcionar");
                tiempoFalso.AgregarMinutos(30);
            }
        }

        [Test]
        public void FranquiciaCompleta_MultiplesIntentosEnHorarioNoPermitido_TodosFallan()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            // Intentar 3 viajes en horario no permitido
            for (int i = 0; i < 3; i++)
            {
                Boleto boleto = colectivo.PagarCon(franquicia);
                Assert.IsNull(boleto, $"Viaje {i + 1} en horario no permitido debería fallar");
                tiempoFalso.AgregarMinutos(10);
            }
        }

        // ===== TESTS DE MÉTODOS AUXILIARES =====

        [Test]
        public void PuedeViajarEnEsteHorario_HorarioPermitido_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_HorarioNoPermitido_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_6AM_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_21_59PM_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 21, 59, 59));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_22PM_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 0, 0));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_5_59AM_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 5, 59, 59));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_Medianoche_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 0, 0, 0));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarEnEsteHorario_15PM_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 15, 0, 0));

            bool resultado = franquicia.PuedeViajarEnEsteHorario();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void ObtenerHorarioPermitido_RetornaRangoCorrecto()
        {
            var (horaInicio, horaFin) = franquicia.ObtenerHorarioPermitido();

            Assert.That(horaInicio, Is.EqualTo(6));
            Assert.That(horaFin, Is.EqualTo(22));
        }

        // ===== TESTS DE INTEGRACIÓN CON OTRAS FUNCIONALIDADES =====

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_SinSaldo_SigueFuncionando()
        {
            var franquiciaSinSaldo = new FranquiciaCompleta(0, tiempoFalso);
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquiciaSinSaldo);

            Assert.IsNotNull(boleto, "Franquicia sin saldo debería funcionar en horario permitido");
            Assert.That(franquiciaSinSaldo.ObtenerSaldo(), Is.EqualTo(0m));
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_ConSaldo_NoPermiteViaje()
        {
            var franquiciaConSaldo = new FranquiciaCompleta(10000, tiempoFalso);
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquiciaConSaldo);

            Assert.IsNull(boleto, "Franquicia con saldo no debería funcionar en horario no permitido");
            Assert.That(franquiciaConSaldo.ObtenerSaldo(), Is.EqualTo(10000m), "Saldo no debería cambiar");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_SaldoNoSeModifica()
        {
            decimal saldoInicial = franquicia.ObtenerSaldo();
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            colectivo.PagarCon(franquicia);

            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(saldoInicial), "Saldo no debería cambiar");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_GeneraBoletoConTarifaCero()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto);
            Assert.That(boleto.TarifaAbonada, Is.EqualTo(0m));
            Assert.That(boleto.TipoTarjeta, Is.EqualTo("Franquicia Completa"));
        }

        // ===== TESTS DE TODOS LOS HORARIOS DEL DÍA =====

        [Test]
        [TestCase(0, false)]  // Medianoche
        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(3, false)]
        [TestCase(4, false)]
        [TestCase(5, false)]
        [TestCase(6, true)]   // Inicio permitido
        [TestCase(7, true)]
        [TestCase(8, true)]
        [TestCase(9, true)]
        [TestCase(10, true)]
        [TestCase(11, true)]
        [TestCase(12, true)]
        [TestCase(13, true)]
        [TestCase(14, true)]
        [TestCase(15, true)]
        [TestCase(16, true)]
        [TestCase(17, true)]
        [TestCase(18, true)]
        [TestCase(19, true)]
        [TestCase(20, true)]
        [TestCase(21, true)]
        [TestCase(22, false)] // Fin permitido
        [TestCase(23, false)]
        public void FranquiciaCompleta_VerificaTodosLosHorarios(int hora, bool deberiaPermitir)
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, hora, 0, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            if (deberiaPermitir)
            {
                Assert.IsNotNull(boleto, $"Debería permitir viaje a las {hora}:00");
            }
            else
            {
                Assert.IsNull(boleto, $"No debería permitir viaje a las {hora}:00");
            }
        }

        // ===== TEST DE DESCONTAR SALDO DIRECTO =====

        [Test]
        public void DescontarSaldo_HorarioNoPermitido_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));

            bool resultado = franquicia.DescontarSaldo(1580m);

            Assert.IsFalse(resultado);
        }

        [Test]
        public void DescontarSaldo_HorarioPermitido_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));

            bool resultado = franquicia.DescontarSaldo(1580m);

            Assert.IsTrue(resultado);
        }

        [Test]
        public void DescontarSaldo_HorarioNoPermitido_Medianoche_RetornaFalse()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 0, 0, 0));

            bool resultado = franquicia.DescontarSaldo(1580m);

            Assert.IsFalse(resultado);
        }

        [Test]
        public void DescontarSaldo_HorarioPermitido_6AM_RetornaTrue()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 6, 0, 0));

            bool resultado = franquicia.DescontarSaldo(1580m);

            Assert.IsTrue(resultado);
        }

        // ===== TESTS DE DÍAS DIFERENTES =====

        [Test]
        public void FranquiciaCompleta_RestriccionAplicaTodosLosDias()
        {
            // Probar varios días de la semana
            for (int dia = 14; dia <= 20; dia++)
            {
                tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, dia, 23, 0, 0));
                Boleto boleto = colectivo.PagarCon(franquicia);
                Assert.IsNull(boleto, $"No debería permitir viaje a las 23:00 el día {dia}");
            }
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitidoAplicaTodosLosDias()
        {
            // Probar varios días de la semana
            for (int dia = 14; dia <= 20; dia++)
            {
                tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, dia, 10, 0, 0));
                Boleto boleto = colectivo.PagarCon(franquicia);
                Assert.IsNotNull(boleto, $"Debería permitir viaje a las 10:00 el día {dia}");
            }
        }

        // ===== TESTS DE MINUTOS ESPECÍFICOS =====

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_7_30AM_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 7, 30, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 7:30 AM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioPermitido_20_45PM_PermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 20, 45, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(boleto, "Debería permitir viaje a las 20:45 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_22_30PM_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 22, 30, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a las 22:30 PM");
        }

        [Test]
        public void FranquiciaCompleta_HorarioNoPermitido_1_30AM_NoPermiteViaje()
        {
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 1, 30, 0));

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.IsNull(boleto, "No debería permitir viaje a la 1:30 AM");
        }

        // ===== TEST DE TIPO DE TARJETA =====

        [Test]
        public void FranquiciaCompleta_TipoTarjeta_EsCorrecto()
        {
            Assert.That(franquicia.ObtenerTipoTarjeta(), Is.EqualTo("Franquicia Completa"));
        }

        // ===== TESTS ADICIONALES PARA COBERTURA =====

        [Test]
        public void FranquiciaCompleta_ViajesConsecutivosEnHorariosPermitidos_TodosFuncionan()
        {
            decimal saldoInicial = franquicia.ObtenerSaldo();

            // Viaje a las 8 AM
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 8, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto1);

            // Viaje a las 12 PM
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 12, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto2);

            // Viaje a las 18 PM
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 18, 0, 0));
            Boleto boleto3 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto3);

            // El saldo no debería cambiar
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }

        [Test]
        public void FranquiciaCompleta_IntentosConsecutivosEnHorariosNoPermitidos_TodosFallan()
        {
            decimal saldoInicial = franquicia.ObtenerSaldo();

            // Intento a las 2 AM
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 2, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto1);

            // Intento a las 4 AM
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 4, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto2);

            // Intento a las 23 PM
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));
            Boleto boleto3 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto3);

            // El saldo no debería cambiar
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }

        [Test]
        public void FranquiciaCompleta_MixViajesPermitidosYNoPermitidos_ComportamientoCorrecto()
        {
            decimal saldoInicial = franquicia.ObtenerSaldo();

            // Viaje 1: Permitido (10:00)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 10, 0, 0));
            Boleto boleto1 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto1, "Viaje en horario permitido debería funcionar");

            // Viaje 2: No permitido (23:00)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));
            Boleto boleto2 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto2, "Viaje en horario no permitido no debería funcionar");

            // Viaje 3: Permitido (15:00)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 15, 0, 0));
            Boleto boleto3 = colectivo.PagarCon(franquicia);
            Assert.IsNotNull(boleto3, "Viaje en horario permitido debería funcionar");

            // Viaje 4: No permitido (3:00)
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 3, 0, 0));
            Boleto boleto4 = colectivo.PagarCon(franquicia);
            Assert.IsNull(boleto4, "Viaje en horario no permitido no debería funcionar");

            // El saldo no debería cambiar
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(saldoInicial));
        }

        [Test]
        public void FranquiciaCompleta_CargarSaldo_FuncionaEnCualquierHorario()
        {
            // Cargar saldo en horario no permitido
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, 23, 0, 0));
            bool resultadoCarga = franquicia.CargarSaldo(5000);

            Assert.IsTrue(resultadoCarga, "Debería poder cargar saldo en cualquier horario");
            Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(10000m));
        }

        [Test]
        public void PuedeViajarEnEsteHorario_TodasLasHorasPermitidas_RetornaTrue()
        {
            // Verificar todas las horas permitidas (6-21)
            for (int hora = 6; hora < 22; hora++)
            {
                tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, hora, 0, 0));
                bool resultado = franquicia.PuedeViajarEnEsteHorario();
                Assert.IsTrue(resultado, $"Debería retornar true para las {hora}:00");
            }
        }

        [Test]
        public void PuedeViajarEnEsteHorario_TodasLasHorasNoPermitidas_RetornaFalse()
        {
            // Verificar horas de medianoche a 5:59
            for (int hora = 0; hora < 6; hora++)
            {
                tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, hora, 0, 0));
                bool resultado = franquicia.PuedeViajarEnEsteHorario();
                Assert.IsFalse(resultado, $"Debería retornar false para las {hora}:00");
            }

            // Verificar horas de 22:00 a 23:59
            for (int hora = 22; hora < 24; hora++)
            {
                tiempoFalso.EstablecerTiempo(new DateTime(2024, 10, 14, hora, 0, 0));
                bool resultado = franquicia.PuedeViajarEnEsteHorario();
                Assert.IsFalse(resultado, $"Debería retornar false para las {hora}:00");
            }
        }
    }
}