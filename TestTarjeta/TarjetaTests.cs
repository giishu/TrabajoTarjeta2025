using NUnit.Framework;
using TransporteUrbano;

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
        var tarjetaExcedida = new Tarjeta(50000);
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
    public void CargarSaldo_ExcedeElLimite_RetornaFalso()
    {
        var tarjetaConSaldo = new Tarjeta(35000);
        bool resultado = tarjetaConSaldo.CargarSaldo(10000);
        Assert.IsFalse(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(35000m));
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
    public void DescontarSaldo_ConSaldoInsuficiente_RetornaFalso()
    {
        var tarjetaConSaldo = new Tarjeta(1000);
        bool resultado = tarjetaConSaldo.DescontarSaldo(2000);
        Assert.IsFalse(resultado);
        Assert.That(tarjetaConSaldo.ObtenerSaldo(), Is.EqualTo(1000m));
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
}