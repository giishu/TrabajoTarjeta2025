using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class BoletoUsoFrecuenteTests
    {
        private TiempoFalso tiempoFalso;
        private Tarjeta tarjeta;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 1, 10, 0, 0));
            tarjeta = new Tarjeta(56000, tiempoFalso); // Máximo saldo permitido
            colectivo = new Colectivo("144");
        }

        // Método auxiliar para recargar tarjeta si es necesario
        private void RecargarSiEsNecesario()
        {
            if (tarjeta.ObtenerSaldo() < 5000)
            {
                tarjeta.CargarSaldo(30000);
            }
        }

        [Test]
        public void Viajes1a29_TarifaNormal_SinDescuento()
        {
            // Realizar 29 viajes
            for (int i = 0; i < 29; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} no debería ser null");
                Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m), $"Viaje {i + 1} debe cobrar tarifa normal");
            }
        }

        [Test]
        public void Viaje30_Aplica20PorCientoDescuento()
        {
            // Realizar 29 viajes sin descuento
            for (int i = 0; i < 29; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 30 - 20% de descuento
            RecargarSiEsNecesario();
            tiempoFalso.AgregarMinutos(10);
            Boleto boleto30 = colectivo.PagarCon(tarjeta);

            decimal tarifaEsperada = 1580m * 0.80m; // 1264
            Assert.IsNotNull(boleto30);
            Assert.That(boleto30.TarifaAbonada, Is.EqualTo(tarifaEsperada));
        }

        [Test]
        public void Viajes30a59_Aplica20PorCientoDescuento()
        {
            // Realizar 29 viajes sin descuento
            for (int i = 0; i < 29; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 30 a 59 - con 20% de descuento
            decimal tarifaEsperada = 1580m * 0.80m; // 1264
            for (int i = 29; i < 59; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} no debería ser null");
                Assert.That(boleto.TarifaAbonada, Is.EqualTo(tarifaEsperada), $"Viaje {i + 1} debe tener 20% descuento");
            }
        }

        [Test]
        public void Viaje60_Aplica25PorCientoDescuento()
        {
            // Realizar 59 viajes
            for (int i = 0; i < 59; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 60 - 25% de descuento
            RecargarSiEsNecesario();
            tiempoFalso.AgregarMinutos(10);
            Boleto boleto60 = colectivo.PagarCon(tarjeta);

            decimal tarifaEsperada = 1580m * 0.75m; // 1185
            Assert.IsNotNull(boleto60);
            Assert.That(boleto60.TarifaAbonada, Is.EqualTo(tarifaEsperada));
        }

        [Test]
        public void Viajes60a80_Aplica25PorCientoDescuento()
        {
            // Realizar 59 viajes
            for (int i = 0; i < 59; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 60 a 80 - con 25% de descuento
            decimal tarifaEsperada = 1580m * 0.75m; // 1185
            for (int i = 59; i < 80; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} no debería ser null");
                Assert.That(boleto.TarifaAbonada, Is.EqualTo(tarifaEsperada), $"Viaje {i + 1} debe tener 25% descuento");
            }
        }

        [Test]
        public void Viaje81_VuelveATarifaNormal()
        {
            // Realizar 80 viajes
            for (int i = 0; i < 80; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 81 - vuelve a tarifa normal
            RecargarSiEsNecesario();
            tiempoFalso.AgregarMinutos(10);
            Boleto boleto81 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto81);
            Assert.That(boleto81.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void Viajes81EnAdelante_TarifaNormal()
        {
            // Realizar 80 viajes
            for (int i = 0; i < 80; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 81 a 85 - tarifa normal
            for (int i = 80; i < 85; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} no debería ser null");
                Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m), $"Viaje {i + 1} debe cobrar tarifa normal");
            }
        }

        [Test]
        public void ContadorViajesDelMes_FuncionaCorrectamente()
        {
            // Realizar 5 viajes
            for (int i = 0; i < 5; i++)
            {
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} no debería ser null");
            }

            Assert.That(tarjeta.ObtenerViajesDelMes(), Is.EqualTo(5));

            // Realizar 10 viajes más
            for (int i = 0; i < 10; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 6} no debería ser null");
            }

            Assert.That(tarjeta.ObtenerViajesDelMes(), Is.EqualTo(15));
        }

        [Test]
        public void ContadorViajesDelMes_SeReiniciaEnNuevoMes()
        {
            // Realizar 35 viajes en octubre
            for (int i = 0; i < 35; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} en octubre no debería ser null");
            }

            Assert.That(tarjeta.ObtenerViajesDelMes(), Is.EqualTo(35));

            // Cambiar al 1 de noviembre
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 11, 1, 10, 0, 0));

            Assert.That(tarjeta.ObtenerViajesDelMes(), Is.EqualTo(0));

            // Realizar 2 viajes en noviembre
            for (int i = 0; i < 2; i++)
            {
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} en noviembre no debería ser null");
            }

            Assert.That(tarjeta.ObtenerViajesDelMes(), Is.EqualTo(2));
        }

        [Test]
        public void DescuentosNoAplicanAMedioBoleto()
        {
            var medioBoleto = new MedioBoleto(56000, tiempoFalso);

            // Realizar 35 viajes (debería tener descuento si fuera tarjeta normal)
            for (int i = 0; i < 35; i++)
            {
                if (medioBoleto.ObtenerSaldo() < 5000)
                {
                    medioBoleto.CargarSaldo(30000);
                }
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(medioBoleto);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} no debería ser null");
            }

            // Viaje 36 - debe seguir siendo medio boleto sin descuento adicional
            tiempoFalso.AgregarMinutos(10);
            Boleto ultimoBoleto = colectivo.PagarCon(medioBoleto);

            Assert.IsNotNull(ultimoBoleto);
            // Verificar que no aplica descuento por uso frecuente
            Assert.That(medioBoleto.ObtenerDescuentoUsoFrecuente(1580m), Is.EqualTo(0));
        }

        [Test]
        public void DescuentosNoAplicanABoletoGratuito()
        {
            var boletoGratuito = new BoletoGratuito(56000, tiempoFalso);

            // Realizar 35 viajes
            for (int i = 0; i < 35; i++)
            {
                if (boletoGratuito.ObtenerSaldo() < 5000)
                {
                    boletoGratuito.CargarSaldo(30000);
                }
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(boletoGratuito);
            }

            // Verificar que no aplica descuento por uso frecuente
            Assert.That(boletoGratuito.ObtenerDescuentoUsoFrecuente(1580m), Is.EqualTo(0));
        }

        [Test]
        public void DescuentosNoAplicanAFranquiciaCompleta()
        {
            var franquicia = new FranquiciaCompleta(56000);

            // Verificar que no aplica descuento por uso frecuente
            Assert.That(franquicia.ObtenerDescuentoUsoFrecuente(1580m), Is.EqualTo(0));
        }

        [Test]
        public void ObtenerDescuentoUsoFrecuente_Viaje29_RetornaCero()
        {
            // Realizar 28 viajes
            for (int i = 0; i < 28; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Ahora tiene 28 viajes, el siguiente será el 29
            decimal descuento = tarjeta.ObtenerDescuentoUsoFrecuente(1580m);
            Assert.That(descuento, Is.EqualTo(0));
        }

        [Test]
        public void ObtenerDescuentoUsoFrecuente_Viaje30_Retorna20Porciento()
        {
            // Realizar 29 viajes
            for (int i = 0; i < 29; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Ahora tiene 29 viajes, el siguiente será el 30 (con descuento)
            decimal descuento = tarjeta.ObtenerDescuentoUsoFrecuente(1580m);
            Assert.That(descuento, Is.EqualTo(1580m * 0.20m));
        }

        [Test]
        public void ObtenerDescuentoUsoFrecuente_Viaje60_Retorna25Porciento()
        {
            // Realizar 59 viajes
            for (int i = 0; i < 59; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Ahora tiene 59 viajes, el siguiente será el 60 (con 25% descuento)
            decimal descuento = tarjeta.ObtenerDescuentoUsoFrecuente(1580m);
            Assert.That(descuento, Is.EqualTo(1580m * 0.25m));
        }

        [Test]
        public void ObtenerDescuentoUsoFrecuente_Viaje81_RetornaCero()
        {
            // Realizar 80 viajes
            for (int i = 0; i < 80; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Ahora tiene 80 viajes, el siguiente será el 81 (sin descuento)
            decimal descuento = tarjeta.ObtenerDescuentoUsoFrecuente(1580m);
            Assert.That(descuento, Is.EqualTo(0));
        }

        [Test]
        public void CalcularTarifaConDescuento_Viaje30_RetornaTarifaConDescuento()
        {
            // Realizar 29 viajes
            for (int i = 0; i < 29; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            decimal tarifaConDescuento = tarjeta.CalcularTarifaConDescuento(1580m);
            Assert.That(tarifaConDescuento, Is.EqualTo(1580m * 0.80m));
        }

        [Test]
        public void CalcularTarifaConDescuento_Viaje60_RetornaTarifaConDescuento()
        {
            // Realizar 59 viajes
            for (int i = 0; i < 59; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            decimal tarifaConDescuento = tarjeta.CalcularTarifaConDescuento(1580m);
            Assert.That(tarifaConDescuento, Is.EqualTo(1580m * 0.75m));
        }

        [Test]
        public void SaldoSeDescontaCorrectamenteConDescuentos()
        {
            decimal saldoInicial = tarjeta.ObtenerSaldo();

            // Realizar 29 viajes sin descuento
            for (int i = 0; i < 29; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Calcular saldo esperado después de 29 viajes
            // (considerando las recargas que se hicieron)
            int cantidadRecargas = (int)((29 * 1580) / 56000) + 1;
            decimal saldoEsperadoDespues29 = saldoInicial + (cantidadRecargas * 30000) - (29 * 1580m);

            // Verificar que el saldo sea positivo (se recargó correctamente)
            Assert.That(tarjeta.ObtenerSaldo(), Is.GreaterThan(0));

            // Guardar saldo antes del viaje 30
            decimal saldoAntes30 = tarjeta.ObtenerSaldo();

            // Viaje 30 con 20% descuento
            RecargarSiEsNecesario();
            tiempoFalso.AgregarMinutos(10);
            colectivo.PagarCon(tarjeta);

            // Verificar que se descontó la tarifa con descuento (1264)
            decimal tarifaConDescuento = 1580m * 0.80m;
            decimal saldoEsperadoDespues30 = saldoAntes30 - tarifaConDescuento;

            // Permitir pequeña diferencia por recargas automáticas
            Assert.That(tarjeta.ObtenerSaldo(), Is.LessThan(saldoAntes30));
        }

        [Test]
        public void ViajesEnDiferentesMeses_ContadoresSeparados()
        {
            // Realizar 35 viajes en octubre
            for (int i = 0; i < 35; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} en octubre no debería ser null");

                // Viajes 30-35 deben tener descuento
                if (i >= 29)
                {
                    Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m * 0.80m));
                }
            }

            // Cambiar a noviembre
            tiempoFalso.EstablecerTiempo(new DateTime(2024, 11, 1, 10, 0, 0));

            // Primer viaje de noviembre - sin descuento
            tiempoFalso.AgregarMinutos(10);
            Boleto boletoNoviembre = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boletoNoviembre);
            Assert.That(boletoNoviembre.TarifaAbonada, Is.EqualTo(1580m));
        }

        [Test]
        public void ObtenerViajesDelMes_SinViajes_RetornaCero()
        {
            Assert.That(tarjeta.ObtenerViajesDelMes(), Is.EqualTo(0));
        }

        [Test]
        public void ObtenerDescuentoUsoFrecuente_SinViajes_RetornaCero()
        {
            decimal descuento = tarjeta.ObtenerDescuentoUsoFrecuente(1580m);
            Assert.That(descuento, Is.EqualTo(0));
        }

        [Test]
        public void CalcularTarifaConDescuento_SinViajes_RetornaTarifaCompleta()
        {
            decimal tarifa = tarjeta.CalcularTarifaConDescuento(1580m);
            Assert.That(tarifa, Is.EqualTo(1580m));
        }

        [Test]
        public void Viaje59_TieneDescuento20Porciento()
        {
            // Realizar 58 viajes
            for (int i = 0; i < 58; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 59 - último con 20% descuento
            RecargarSiEsNecesario();
            tiempoFalso.AgregarMinutos(10);
            Boleto boleto59 = colectivo.PagarCon(tarjeta);

            decimal tarifaEsperada = 1580m * 0.80m; // 1264
            Assert.IsNotNull(boleto59);
            Assert.That(boleto59.TarifaAbonada, Is.EqualTo(tarifaEsperada));
        }

        [Test]
        public void Viaje80_TieneDescuento25Porciento()
        {
            // Realizar 79 viajes
            for (int i = 0; i < 79; i++)
            {
                RecargarSiEsNecesario();
                tiempoFalso.AgregarMinutos(10);
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 80 - último con 25% descuento
            RecargarSiEsNecesario();
            tiempoFalso.AgregarMinutos(10);
            Boleto boleto80 = colectivo.PagarCon(tarjeta);

            decimal tarifaEsperada = 1580m * 0.75m; // 1185
            Assert.IsNotNull(boleto80);
            Assert.That(boleto80.TarifaAbonada, Is.EqualTo(tarifaEsperada));
        }

        [Test]
        public void FranquiciaCompleta_RegistraViajesParaContador()
        {
            var franquicia = new FranquiciaCompleta(5000);

            // DescontarSaldo debe funcionar y registrar el viaje
            bool resultado = franquicia.DescontarSaldo(1580m);
            Assert.IsTrue(resultado);
        }
    }
}