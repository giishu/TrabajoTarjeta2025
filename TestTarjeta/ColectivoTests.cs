using NUnit.Framework;
using TransporteUrbano;

public class ColectivoTests
{
    private Colectivo colectivo;
    private Tarjeta tarjeta;

    [SetUp]
    public void Setup()
    {
        colectivo = new Colectivo("144");
        tarjeta = new Tarjeta(5000);
    }

    [Test]
    public void Constructor_ConLineaValida_CreasColectivoConLaLinea()
    {
        var colectivo144 = new Colectivo("144");
        Assert.That(colectivo144.Linea, Is.EqualTo("144"));
    }

    [Test]
    public void Constructor_ConLineaNula_CreasColectivoConLineaNA()
    {
        var colectivoNulo = new Colectivo(null);
        Assert.That(colectivoNulo.Linea, Is.EqualTo("N/A"));
    }

    [Test]
    public void Constructor_ConLineaVacia_CreasColectivoConLineaNA()
    {
        var colectivoVacio = new Colectivo("");
        Assert.That(colectivoVacio.Linea, Is.EqualTo("N/A"));
    }

    [Test]
    public void ObtenerTarifaBasica_RetornaTarifaBasicaCorrecta()
    {
        decimal tarifa = colectivo.ObtenerTarifaBasica();
        Assert.That(tarifa, Is.EqualTo(1580m));
    }

    [Test]
    public void PagarCon_ConTarjetaConSaldoSuficiente_GeneraBoleto()
    {
        Boleto boleto = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(boleto);
        Assert.That(boleto.TarifaAbonada, Is.EqualTo(1580m));
        Assert.That(boleto.SaldoRestante, Is.EqualTo(3420m));
    }

    [Test]
    public void PagarCon_ConTarjetaSinSaldo_RetornaNull()
    {
        var tarjetaVacia = new Tarjeta(0);
        Boleto boleto = colectivo.PagarCon(tarjetaVacia);
        Assert.IsNull(boleto);
    }

    [Test]
    public void PagarCon_ConTarjetaConSaldoInsuficiente_RetornaNull()
    {
        var tarjetaBaja = new Tarjeta(1000);
        Boleto boleto = colectivo.PagarCon(tarjetaBaja);
        Assert.IsNull(boleto);
    }

    [Test]
    public void PagarCon_ConTarjetaNula_RetornaNull()
    {
        Boleto boleto = colectivo.PagarCon(null);
        Assert.IsNull(boleto);
    }

    [Test]
    public void PagarCon_DescontaLaTarifaDeLaTarjeta()
    {
        decimal saldoAntes = tarjeta.ObtenerSaldo();
        colectivo.PagarCon(tarjeta);
        decimal saldoDespues = tarjeta.ObtenerSaldo();
        Assert.That(saldoDespues, Is.EqualTo(saldoAntes - 1580m));
    }

    [Test]
    public void PagarCon_ElBoletoContieneLaLineaDelColectivo()
    {
        Boleto boleto = colectivo.PagarCon(tarjeta);
        Assert.That(boleto.LineaColectivo, Is.EqualTo("144"));
    }

    [Test]
    public void PagarCon_MultiplesViajes_DescontaPorCadaUno()
    {
        var tarjetaConMuchoDinero = new Tarjeta(10000);
        var col = new Colectivo("100");

        col.PagarCon(tarjetaConMuchoDinero);
        Assert.That(tarjetaConMuchoDinero.ObtenerSaldo(), Is.EqualTo(8420m));

        col.PagarCon(tarjetaConMuchoDinero);
        Assert.That(tarjetaConMuchoDinero.ObtenerSaldo(), Is.EqualTo(6840m));

        col.PagarCon(tarjetaConMuchoDinero);
        Assert.That(tarjetaConMuchoDinero.ObtenerSaldo(), Is.EqualTo(5260m));
    }
}