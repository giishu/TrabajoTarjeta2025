using NUnit.Framework;
using TransporteUrbano;

public class BoletoMejoradoTests
{
    [Test]
    public void Boleto_NuevoConstructor_GuardaTodaLaInformacion()
    {
        var tarjeta = new Tarjeta(5000);
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
        var medioBoleto = new MedioBoleto(5000);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(medioBoleto);

        Assert.That(boleto.TipoTarjeta, Is.EqualTo("Medio Boleto"));
        Assert.That(boleto.TarifaAbonada, Is.EqualTo(790m));
    }

    [Test]
    public void Boleto_ConSaldoNegativo_CalculaMontoTotalCorrecto()
    {
        var tarjeta = new Tarjeta(1000); // Saldo insuficiente
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsTrue(boleto.TieneSaldoNegativo);
        Assert.That(boleto.MontoTotalAbonado, Is.EqualTo(1580m + 580m)); // Tarifa + saldo negativo
    }

    [Test]
    public void Boleto_SinSaldoNegativo_MontoTotalIgualTarifa()
    {
        var tarjeta = new Tarjeta(5000);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsFalse(boleto.TieneSaldoNegativo);
        Assert.That(boleto.MontoTotalAbonado, Is.EqualTo(1580m));
    }

    [Test]
    public void Boleto_ToString_MuestraInformacionCompleta()
    {
        var tarjeta = new Tarjeta(5000);
        var colectivo = new Colectivo("144");

        Boleto boleto = colectivo.PagarCon(tarjeta);
        string resultado = boleto.ToString();

        Assert.That(resultado, Does.Contain("Tipo Tarjeta: Normal"));
        Assert.That(resultado, Does.Contain("ID Tarjeta: TARJ-"));
        Assert.That(resultado, Does.Contain("Línea: 144"));
    }
}