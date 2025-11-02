using NUnit.Framework;
using TransporteUrbano;
using System;

public class TarjetaTests
{
    private Tarjeta tarjeta;

    [SetUp]
    public void Setup()
    {
        tarjeta = new Tarjeta();
    }

    [Test]
    public void Constructor_SinParametros_CreaTarjetaConSaldoCero()
    {
        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoValido_CreaTarjetaConSaldoCorreto()
    {
        var tarjetaConSaldo = new Tarjeta(5000);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void Constructor_ConSaldoNegativo_CreaTarjetaConSaldoCero()
    {
        var tarjetaNegativa = new Tarjeta(-1000);
        Assert.That(tarjetaNegativa.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoMayorAlLimite_CreaTarjetaConSaldoCero()
    {
        var tarjetaExcedida = new Tarjeta(60000);
        Assert.That(tarjetaExcedida.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void ObtenerSaldo_RetornaElSaldoActual()
    {
        var tarjetaConSaldo = new Tarjeta(3000);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(3000m));
    }

    [TestCase(2000)]
    [TestCase(3000)]
    [TestCase(4000)]
    [TestCase(5000)]
    [TestCase(8000)]
    [TestCase(10000)]
    [TestCase(15000)]
    [TestCase(20000)]
    [TestCase(25000)]
    [TestCase(30000)]
    public void CargarSaldo_ConMontosValidos_CargaExitosamente(decimal monto)
    {
        bool resultado = tarjeta.CargarSaldo(monto);
        Assert.IsTrue(resultado);
        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(monto));
    }

    [Test]
    public void CargarSaldo_ConMontoInvalido_RetornaFalso()
    {
        bool resultado = tarjeta.CargarSaldo(1000);
        Assert.IsFalse(resultado);
        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void CargarSaldo_ExcedeElLimite_RetornaTrue()
    {
        var tarjetaConSaldo = new Tarjeta(50000);
        bool resultado = tarjetaConSaldo.CargarSaldo(10000);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(56000m));
        Assert.That(tarjetaConSaldo.SaldoPendiente, Is.EqualTo(4000m));
    }

    [Test]
    public void CargarSaldo_MultiplesCargasValidas_AcumulaSaldo()
    {
        tarjeta.CargarSaldo(5000);
        tarjeta.CargarSaldo(4000);
        tarjeta.CargarSaldo(3000);
        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(12000m));
    }

    [Test]
    public void DescontarSaldo_ConMontoValido_DescuentaExitosamente()
    {
        var tarjetaConSaldo = new Tarjeta(5000);
        bool resultado = tarjetaConSaldo.DescontarSaldo(1580);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(3420m));
    }

    [Test]
    public void DescontarSaldo_ConSaldoInsuficiente_PermiteSaldoNegativoHastaElLimite()
    {
        var tarjetaConSaldo = new Tarjeta(1000);
        bool resultado = tarjetaConSaldo.DescontarSaldo(2000);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(-1000m));
    }

    [Test]
    public void DescontarSaldo_ConMontoNegativo_RetornaFalso()
    {
        var tarjetaConSaldo = new Tarjeta(5000);
        bool resultado = tarjetaConSaldo.DescontarSaldo(-100);
        Assert.IsFalse(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void DescontarSaldo_DescuentoExacto_DejaElSaldoEnCero()
    {
        var tarjetaConSaldo = new Tarjeta(1580);
        bool resultado = tarjetaConSaldo.DescontarSaldo(1580);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void ObtenerCargasValidas_RetornaListaConTodosLosMontosValidos()
    {
        var cargasValidas = tarjeta.ObtenerCargasValidas();
        Assert.That(cargasValidas.Count, Is.EqualTo(10));
        Assert.Contains(2000m, cargasValidas);
        Assert.Contains(30000m, cargasValidas);
    }

    [Test]
    public void DescontarSaldo_PermiteSaldoNegativoHasta1200()
    {
        var tarjetaConSaldo = new Tarjeta(500);
        bool resultado = tarjetaConSaldo.DescontarSaldo(1580);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(-1080m));
    }

    [Test]
    public void DescontarSaldo_NoPermiteSaldoNegativoMayorA1200()
    {
        var tarjetaConSaldo = new Tarjeta(200);
        bool resultado = tarjetaConSaldo.DescontarSaldo(1580);
        Assert.IsFalse(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(200m));
    }

    [Test]
    public void CargarSaldo_ConSaldoNegativo_DescuentaElDebitoAntesDeSumar()
    {
        var tarjetaConSaldo = new Tarjeta(500);
        tarjetaConSaldo.DescontarSaldo(1580);
        bool resultado = tarjetaConSaldo.CargarSaldo(3000);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(1920m));
    }

    [Test]
    public void DescontarSaldo_MultiplesViajesPlus_NoExcedeLimiteNegativo()
    {
        var tarjetaConSaldo = new Tarjeta(500);

        bool primerViaje = tarjetaConSaldo.DescontarSaldo(1580);
        Assert.IsTrue(primerViaje);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(-1080m));

        bool segundoViaje = tarjetaConSaldo.DescontarSaldo(1580);
        Assert.IsFalse(segundoViaje);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(-1080m));
    }

    [Test]
    public void CargarSaldo_ConSaldoNegativoYCargaMayor_NoExcedeLimite()
    {
        var tarjetaConSaldo = new Tarjeta(500);
        tarjetaConSaldo.DescontarSaldo(1580);

        bool resultado = tarjetaConSaldo.CargarSaldo(30000);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(28920m));
    }

    [Test]
    public void Constructor_ConSaldoMayorAlNuevoLimite_CreaTarjetaConSaldoCero()
    {
        var tarjetaExcedida = new Tarjeta(60000m);
        Assert.That(tarjetaExcedida.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void CargarSaldo_ExcedeNuevoLimite_RetornaTrue()
    {
        var tarjetaConSaldo = new Tarjeta(55000m);
        bool resultado = tarjetaConSaldo.CargarSaldo(2000m);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(56000m));
        Assert.That(tarjetaConSaldo.SaldoPendiente, Is.GreaterThan(0m));
    }

    [Test]
    public void Tarjeta_TienePropiedadSaldoPendiente()
    {
        var tarjeta = new Tarjeta();
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0m));
    }

    // ===== NUEVOS TESTS PARA AUMENTAR COBERTURA =====

    [Test]
    public void Constructor_ConTiempo_CreaTarjetaCorrectamente()
    {
        var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var tarjetaConTiempo = new Tarjeta(tiempo);

        Assert.That(tarjetaConTiempo.ObtenerSaldo(), Is.EqualTo(0m));
        Assert.That(tarjetaConTiempo.Id, Is.Not.Null);
    }

    [Test]
    public void Constructor_ConSaldoYTiempo_CreaTarjetaCorrectamente()
    {
        var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var tarjetaConTiempo = new Tarjeta(5000m, tiempo);

        Assert.That(tarjetaConTiempo.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void ObtenerTipoTarjeta_TarjetaNormal_RetornaNormal()
    {
        Assert.That(tarjeta.ObtenerTipoTarjeta(), Is.EqualTo("Normal"));
    }

    [Test]
    public void Id_EsUnicoParaCadaTarjeta()
    {
        var tarjeta1 = new Tarjeta();
        var tarjeta2 = new Tarjeta();

        Assert.That(tarjeta1.Id, Is.Not.EqualTo(tarjeta2.Id));
    }

    [Test]
    public void Id_ComienzaConPrefijo()
    {
        Assert.That(tarjeta.Id, Does.StartWith("TARJ-"));
    }

    [Test]
    public void ObtenerSaldoNegativoPermitido_Retorna1200()
    {
        Assert.That(tarjeta.ObtenerSaldoNegativoPermitido(), Is.EqualTo(-1200m));
    }

    [Test]
    public void ObtenerLimiteSaldo_Retorna56000()
    {
        Assert.That(tarjeta.ObtenerLimiteSaldo(), Is.EqualTo(56000m));
    }

    [Test]
    public void CargarSaldo_ConMontoCero_RetornaFalso()
    {
        bool resultado = tarjeta.CargarSaldo(0);
        Assert.IsFalse(resultado);
    }

    [Test]
    public void DescontarSaldo_ConMontoCero_DescuentaExitosamente()
    {
        var tarjetaConSaldo = new Tarjeta(5000);
        bool resultado = tarjetaConSaldo.DescontarSaldo(0);
        Assert.IsTrue(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void ObtenerCargasValidas_ContieneTodasLasCargasEsperadas()
    {
        var cargas = tarjeta.ObtenerCargasValidas();

        Assert.Contains(2000m, cargas);
        Assert.Contains(3000m, cargas);
        Assert.Contains(4000m, cargas);
        Assert.Contains(5000m, cargas);
        Assert.Contains(8000m, cargas);
        Assert.Contains(10000m, cargas);
        Assert.Contains(15000m, cargas);
        Assert.Contains(20000m, cargas);
        Assert.Contains(25000m, cargas);
        Assert.Contains(30000m, cargas);
    }

    [Test]
    public void AcreditarCarga_ConSaldoPendienteYEspacioDisponible_AcreditaParcialmente()
    {
        var tarjeta = new Tarjeta(55000);
        tarjeta.CargarSaldo(5000); // Saldo: 56000, Pendiente: 4000

        tarjeta.DescontarSaldo(2000); // Saldo: 54000

        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m)); // Acreditó 2000 de pendiente
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2000m));
    }

    [Test]
    public void DescontarSaldo_ExactamenteEnElLimiteNegativo_Funciona()
    {
        var tarjeta = new Tarjeta(380);
        bool resultado = tarjeta.DescontarSaldo(1580);

        Assert.IsTrue(resultado);
        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(-1200m));
    }

    [Test]
    public void DescontarSaldo_UnPesoMasDelLimiteNegativo_NoPermite()
    {
        var tarjeta = new Tarjeta(379);
        bool resultado = tarjeta.DescontarSaldo(1580);

        Assert.IsFalse(resultado);
        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(379m));
    }

    [Test]
    public void CargarSaldo_MultiplesVecesHastaExceder_AcumulaSaldoPendiente()
    {
        var tarjeta = new Tarjeta(50000);

        tarjeta.CargarSaldo(5000); // 55000
        tarjeta.CargarSaldo(3000); // 56000 + 2000 pendiente
        tarjeta.CargarSaldo(4000); // 56000 + 6000 pendiente

        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(56000m));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(6000m));
    }

    [Test]
    public void Constructor_ConSaldoNegativoYTiempo_CreaSaldoCero()
    {
        var tiempo = new TiempoFalso();
        var tarjeta = new Tarjeta(-500m, tiempo);

        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoExcediendoLimiteYTiempo_CreaSaldoCero()
    {
        var tiempo = new TiempoFalso();
        var tarjeta = new Tarjeta(70000m, tiempo);

        Assert.That(tarjeta.ObtenerSaldo(), Is.EqualTo(0m));
    }
}