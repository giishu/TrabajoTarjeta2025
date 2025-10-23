using NUnit.Framework;
using TransporteUrbano;

public class MedioBoletoTests
{
    private MedioBoleto medioBoleto;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        medioBoleto = new MedioBoleto(5000);
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
        var mb = new MedioBoleto(10000);
        var col = new Colectivo("100");

        Boleto boleto1 = col.PagarCon(mb);
        Assert.That(boleto1.TarifaAbonada, Is.EqualTo(790m));
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(9210m));

        Boleto boleto2 = col.PagarCon(mb);
        Assert.That(boleto2.TarifaAbonada, Is.EqualTo(790m));
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(8420m));

        Boleto boleto3 = col.PagarCon(mb);
        Assert.That(boleto3.TarifaAbonada, Is.EqualTo(790m));
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(7630m));
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
        var mb = new MedioBoleto(500);
        Boleto boleto = colectivo.PagarCon(mb);

        Assert.IsNotNull(boleto);
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(-290m)); // 500 - 790
    }

    [Test]
    public void MedioBoleto_PermiteSaldoNegativoHasta1200()
    {
        var mb = new MedioBoleto(400);
        Boleto boleto = colectivo.PagarCon(mb);

        Assert.IsNotNull(boleto);
        Assert.That(mb.ObtenerSaldo(), Is.EqualTo(-390m));
    }
}