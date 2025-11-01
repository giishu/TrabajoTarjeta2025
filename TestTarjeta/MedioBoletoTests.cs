using NUnit.Framework;
using TransporteUrbano;
using System;

public class MedioBoletoTests
{
    private MedioBoleto medioBoleto;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        medioBoleto = new MedioBoleto(5000, tiempoFalso);
        colectivo = new Colectivo("144");
    }

    [Test]
    public void Constructor_SinParametros_CreaTarjetaMedioBoletoConSaldoCero()
    {
        var mb = new MedioBoleto();
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(0m));
    }

    [Test]
    public void Constructor_ConSaldoInicial_CreaTarjetaConSaldo()
    {
        var mb = new MedioBoleto(3000);
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(3000m));
    }

    [Test]
    public void PagarCon_MedioBoleto_CobraMitadDeTarifa()
    {
        Boleto boleto = colectivo.PagarCon(medioBoleto);

        Assert.IsNotNull(boleto);
        Assert.That(boleto.TarifaAbonada, Is.EqualTo(790m)); // 1580 / 2
        Assert.That(medioBoleto.ObtenerSaldo(), Is.EqualTo(4210m)); // 5000 - 790
    }

    [Test]
    public void PagarCon_MedioBoleto_MultiplesViajes_SiempreCobraMitad()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var mb = new MedioBoleto(10000, tiempoFalso);
        var col = new Colectivo("100");

        // Primer viaje
        Boleto boleto1 = col.PagarCon(mb);
        Assert.That(boleto1.TarifaAbonada, Is.EqualTo(790m));
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(9210m));

        // Esperar 6 minutos para el siguiente viaje
        tiempoFalso.AgregarMinutos(6);

        // Segundo viaje
        Boleto boleto2 = col.PagarCon(mb);
        Assert.That(boleto2.TarifaAbonada, Is.EqualTo(790m));
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(8420m));

        // Esperar 6 minutos para el siguiente viaje
        tiempoFalso.AgregarMinutos(6);

        // Tercer viaje - cobra tarifa completa (1580)
        Boleto boleto3 = col.PagarCon(mb);
        Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m)); // Tarifa completa en el 3er viaje
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(6840m)); // 8420 - 1580
    }

    [Test]
    public void CargarSaldo_MedioBoleto_FuncionaIgualQueTarjetaNormal()
    {
        var mb = new MedioBoleto();
        bool resultado = mb.CargarSaldo(5000);

        Assert.IsTrue(resultado);
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(5000m));
    }

    [Test]
    public void MedioBoleto_ConSaldoInsuficiente_NoGeneraBoleto()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var mb = new MedioBoleto(500, tiempoFalso);
        Boleto boleto = colectivo.PagarCon(mb);

        Assert.IsNotNull(boleto);
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(-290m)); 
    }

    [Test]
    public void MedioBoleto_PermiteSaldoNegativoHasta1200()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var mb = new MedioBoleto(400, tiempoFalso);
        Boleto boleto = colectivo.PagarCon(mb);

        Assert.IsNotNull(boleto);
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(-390m));
    }
}