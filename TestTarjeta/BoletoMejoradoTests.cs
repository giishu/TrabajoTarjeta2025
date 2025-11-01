using System;
using NUnit.Framework;
using TransporteUrbano;

public class BoletoMejoradoTests
{
    [Test]
    public void Boleto_NuevoConstructor_GuardaTodaLaInformacion()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2025, 4, 7, 10, 0, 0)); // Lunes 10:00
        var tarjeta = new Tarjeta(5000, tiempoFalso);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(boleto);
        Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
        Assert.That(boleto.IdTarjeta, Is.EqualTo(tarjeta.Id));
        Assert.That(boleto.LineaColectivo, Is.EqualTo("144"));
        Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m));
    }

    [Test]
    public void Boleto_ConMedioBoleto_MuestraTipoCorrecto()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2025, 4, 7, 10, 0, 0)); // Lunes 10:00
        var medioBoleto = new MedioBoleto(5000, tiempoFalso);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(medioBoleto);

        Assert.IsNotNull(boleto); // Añadido para evitar NullReference
        Assert.That(boleto.TipoTarjeta, Is.EqualTo("Medio Boleto"));
        Assert.That(boleto.TarifaAbonada, Is.EqualTo(790m));
    }

    [Test]
    public void Boleto_ConSaldoNegativo_CalculaMontoTotalCorrecto()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2025, 4, 7, 10, 0, 0));
        var tarjeta = new Tarjeta(1000, tiempoFalso); // Saldo insuficiente
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(boleto);
        Assert.IsTrue(boleto.TieneSaldoNegativo);
        Assert.That(boleto.MontoTotalAbonado, Is.EqualTo(1580m + 580m)); // 1580 + | -580 |
    }

    [Test]
    public void Boleto_SinSaldoNegativo_MontoTotalIgualTarifa()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2025, 4, 7, 10, 0, 0));
        var tarjeta = new Tarjeta(5000, tiempoFalso);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(boleto);
        Assert.IsFalse(boleto.TieneSaldoNegativo);
        Assert.That(boleto.MontoTotalAbonado, Is.EqualTo(1580m));
    }

    [Test]
    public void Boleto_ToString_MuestraInformacionCompleta()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2025, 4, 7, 10, 0, 0));
        var tarjeta = new Tarjeta(5000, tiempoFalso);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);
        string resultado = boleto.ToString();

        Assert.That(resultado, Does.Contain("Tipo Tarjeta: Normal"));
        Assert.That(resultado, Does.Contain("ID Tarjeta: TARJ-"));
        Assert.That(resultado, Does.Contain("Línea: 144"));
    }

    [Test]
    public void PagarCon_BoletoGratuito_NoDescontaSaldo()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2025, 4, 7, 10, 0, 0)); // Lunes 10:00
        var bg = new BoletoGratuito(5000, tiempoFalso);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(bg);

        Assert.IsNotNull(boleto);
        Assert.That(boleto.TarifaAbonada, Is.EqualTo(0m));
        Assert.That(bg.ObtenerSaldo(), Is.EqualTo(5000m));
    }
}