using NUnit.Framework;
using TransporteUrbano;

public class BoletoGratuitoTests
{
    private BoletoGratuito boletoGratuito;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        boletoGratuito = new BoletoGratuito();
        colectivo = new Colectivo("144");
    }

    [Test]
    public void Constructor_SinParametros_CreaTarjetaBoletoGratuitoConSaldoCero()
    {
        var bg = new BoletoGratuito();
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoInicial_CreaTarjetaConSaldo()
    {
        var bg = new BoletoGratuito(3000);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(3000m));
    }

    [Test]
    public void PagarCon_BoletoGratuito_SiemprePuedePagar()
    {
        Boleto boleto = colectivo.PagarCon(boletoGratuito);

        Assert.IsNotNull(boleto);
        Assert.That(boletoGratuito.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void PagarCon_BoletoGratuito_NoDescontaSaldo()
    {
        var bg = new BoletoGratuito(5000);
        Boleto boleto = colectivo.PagarCon(bg);

        Assert.IsNotNull(boleto);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(5000m)); // Mantiene el saldo
    }

    [Test]
    public void PagarCon_BoletoGratuito_MultiplesViajes_NuncaDescontaSaldo()
    {
        var bg = new BoletoGratuito(2000);

        colectivo.PagarCon(bg);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(2000m));

        colectivo.PagarCon(bg);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(2000m));

        colectivo.PagarCon(bg);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(2000m));
    }

    [Test]
    public void PagarCon_BoletoGratuito_SinSaldo_SiguePudiendoPagar()
    {
        var bgSinSaldo = new BoletoGratuito(0);

        Boleto boleto1 = colectivo.PagarCon(bgSinSaldo);
        Assert.IsNotNull(boleto1);

        Boleto boleto2 = colectivo.PagarCon(bgSinSaldo);
        Assert.IsNotNull(boleto2);

        Assert.That(bgSinSaldo.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void CargarSaldo_BoletoGratuito_FuncionaNormalmente()
    {
        var bg = new BoletoGratuito();
        bool resultado = bg.CargarSaldo(5000);

        Assert.IsTrue(resultado);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void DescontarSaldo_BoletoGratuito_SiempreRetornaTrue()
    {
        var bg = new BoletoGratuito(100);

        bool resultado = bg.DescontarSaldo(1580);
        Assert.IsTrue(resultado);
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(100m)); // No se descuenta
    }
}