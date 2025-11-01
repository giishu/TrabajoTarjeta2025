using NUnit.Framework;
using TransporteUrbano;
using System;

public class FranquiciaCompletaTests
{
    private FranquiciaCompleta franquicia;
    private Colectivo colectivo;
    private TiempoFalso tiempoFalso;

    [SetUp]
    public void Setup()
    {
        // Usar TiempoFalso en horario permitido (10:00 AM)
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        franquicia = new FranquiciaCompleta(tiempoFalso);
        colectivo = new Colectivo("144");
    }

    [Test]
    public void Constructor_SinParametros_CreaTarjetaFranquiciaConSaldoCero()
    {
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fc = new FranquiciaCompleta(tiempoFalso);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoInicial_CreaTarjetaConSaldo()
    {
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fc = new FranquiciaCompleta(3000, tiempoFalso);
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
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fc = new FranquiciaCompleta(5000, tiempoFalso);
        Boleto boleto = colectivo.PagarCon(fc);

        Assert.IsNotNull(boleto);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(5000m)); // Mantiene el mismo saldo
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_MultiplesViajes_NuncaDescontaSaldo()
    {
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fc = new FranquiciaCompleta(1000, tiempoFalso);

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
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fcSinSaldo = new FranquiciaCompleta(0, tiempoFalso);

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
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fc = new FranquiciaCompleta(tiempoFalso);
        bool resultado = fc.CargarSaldo(5000);

        Assert.IsTrue(resultado);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void DescontarSaldo_FranquiciaCompleta_SiempreRetornaTrue()
    {
        tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var fc = new FranquiciaCompleta(100, tiempoFalso);

        bool resultado = fc.DescontarSaldo(1580);
        Assert.IsTrue(resultado);
        Assert.That(fc.ObtenerSaldo(), Is.EqualTo(100m)); // No se descuenta
    }

    [Test]
    public void ObtenerTipoTarjeta_RetornaFranquiciaCompleta()
    {
        Assert.That(franquicia.ObtenerTipoTarjeta(), Is.EqualTo("Franquicia Completa"));
    }
}