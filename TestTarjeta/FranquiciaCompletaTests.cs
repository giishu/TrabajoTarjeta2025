using NUnit.Framework;
using TransporteUrbano;

public class FranquiciaCompletaTests
{
    private FranquiciaCompleta franquicia;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        franquicia = new FranquiciaCompleta();
        colectivo = new Colectivo("144");
    }

    [Test]
    public void Constructor_SinParametros_CreaTarjetaFranquiciaConSaldoCero()
    {
        var fc = new FranquiciaCompleta();
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoInicial_CreaTarjetaConSaldo()
    {
        var fc = new FranquiciaCompleta(3000);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(3000m));
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_SiemprePuedePagar()
    {
        Boleto boleto = colectivo.PagarCon(franquicia);

        Assert.IsNotNull(boleto);
        Assert.That(franquicia.ObtenerSaldo(), Is.EqualTo(0m)); // No descuenta saldo
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_NoDescontaSaldo()
    {
        var fc = new FranquiciaCompleta(5000);
        Boleto boleto = colectivo.PagarCon(fc);

        Assert.IsNotNull(boleto);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(5000m)); // Mantiene el mismo saldo
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_MultiplesViajes_NuncaDescontaSaldo()
    {
        var fc = new FranquiciaCompleta(1000);

        colectivo.PagarCon(fc);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(1000m));

        colectivo.PagarCon(fc);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(1000m));

        colectivo.PagarCon(fc);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(1000m));
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_SinSaldo_SiguePudiendoPagar()
    {
        var fcSinSaldo = new FranquiciaCompleta(0);

        Boleto boleto1 = colectivo.PagarCon(fcSinSaldo);
        Assert.IsNotNull(boleto1);

        Boleto boleto2 = colectivo.PagarCon(fcSinSaldo);
        Assert.IsNotNull(boleto2);

        Boleto boleto3 = colectivo.PagarCon(fcSinSaldo);
        Assert.IsNotNull(boleto3);

        Assert.That(fcSinSaldo.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void CargarSaldo_FranquiciaCompleta_FuncionaNormalmente()
    {
        var fc = new FranquiciaCompleta();
        bool resultado = fc.CargarSaldo(5000);

        Assert.IsTrue(resultado);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void DescontarSaldo_FranquiciaCompleta_SiempreRetornaTrue()
    {
        var fc = new FranquiciaCompleta(100);

        bool resultado = fc.DescontarSaldo(1580);
        Assert.IsTrue(resultado);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(100m)); // No se descuenta
    }
}