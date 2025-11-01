using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TestTransporteUrbano
{
    public class SaldoPendienteTests
    {
        [Test]
        public void CargarSaldo_Excedente_CreaSaldoPendiente()
        {
            var tarjeta = new Tarjeta(55000m);

            bool resultado = tarjeta.CargarSaldo(3000m);

            Assert.IsTrue(resultado);
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2000m));
        }

        [Test]
        public void CargarSaldo_MultipleExcedente_AcumulaSaldoPendiente()
        {
            var tarjeta = new Tarjeta(54000m);

            tarjeta.CargarSaldo(3000m); 
            tarjeta.CargarSaldo(2000m); 

            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(3000m));
        }

        [Test]
        public void AcreditarCarga_RecargaSaldoPendienteAlUsarTarjeta()
        {
            var tarjeta = new Tarjeta(55000m);
            tarjeta.CargarSaldo(3000m); 
            var colectivo = new Colectivo("144");

            tarjeta.DescontarSaldo(1580m); 

            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m - 1580m + 1580m));
            Assert.That(tarjeta.SaldoPendiente, Is.LessThan(2000m));
        }

        [Test]
        public void AcreditarCarga_RecargaCompletaSaldoPendiente()
        {
            var tarjeta = new Tarjeta(55000m);
            tarjeta.CargarSaldo(3000m); 
            decimal saldoPendienteInicial = tarjeta.SaldoPendiente;

            tarjeta.DescontarSaldo(3000m);

            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0m));
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m - 3000m + saldoPendienteInicial));
        }

        [Test]
        public void CargarSaldo_SinExcedente_NoCreaSaldoPendiente()
        {
            var tarjeta = new Tarjeta(50000m);

            bool resultado = tarjeta.CargarSaldo(5000m);

            Assert.IsTrue(resultado);
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(55000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0m));
        }

        [Test]
        public void CargarSaldo_EnElLimite_NoCreaSaldoPendiente()
        {
            var tarjeta = new Tarjeta(56000m - 3000m);

            bool resultado = tarjeta.CargarSaldo(3000m);

            Assert.IsTrue(resultado);
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0m));
        }

        [Test]
        public void AcreditarCarga_SinSaldoPendiente_NoHaceNada()
        {
            var tarjeta = new Tarjeta(50000m);

            tarjeta.AcreditarCarga();


            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(50000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0m));
        }

        [Test]
        public void AcreditarCarga_SinEspacioDisponible_NoHaceNada()
        {
            var tarjeta = new Tarjeta(56000m);
            tarjeta.CargarSaldo(3000m);

            tarjeta.AcreditarCarga();

            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(3000m));
        }

        [Test]
        public void ObtenerLimiteSaldo_Retorna56000()
        {
            var tarjeta = new Tarjeta();

            Assert.That(tarjeta.ObtenerLimiteSaldo(), Is.EqualTo(56000m));
        }

        [Test]
        public void CargarSaldo_ConSaldoPendienteYAcreditarAutomatico()
        {
            var tarjeta = new Tarjeta(55000m);
            tarjeta.CargarSaldo(3000m);

            tarjeta.DescontarSaldo(1580m);

            bool resultado = tarjeta.CargarSaldo(2000m);

            Assert.IsTrue(resultado);
            Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2420m));
        }
    }
}