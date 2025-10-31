using NUnit.Framework;
using TransporteUrbano;
using System;

public class MedioBoletoLimitesTests
{
    [Test]
    public void MedioBoleto_NoPermiteViajeAntesDe5Minutos()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var medioBoleto = new MedioBoleto(tiempoFalso);
        medioBoleto.CargarSaldo(5000);
        var colectivo = new Colectivo("144");

        // Primer viaje - debería funcionar
        Boleto boleto1 = colectivo.PagarCon(medioBoleto);
        Assert.IsNotNull(boleto1, "Primer viaje debería funcionar");

        // Intentar segundo viaje a los 3 minutos - debería fallar
        tiempoFalso.AgregarMinutos(3);
        Boleto boleto2 = colectivo.PagarCon(medioBoleto);
        Assert.IsNull(boleto2, "No debería permitir viaje antes de 5 minutos");

        // Esperar 5 minutos completos - debería funcionar
        tiempoFalso.AgregarMinutos(2);
        Boleto boleto3 = colectivo.PagarCon(medioBoleto);
        Assert.IsNotNull(boleto3, "Debería permitir viaje después de 5 minutos");
    }

    [Test]
    public void MedioBoleto_TercerViajeDelDia_CobraTarifaCompleta()
    {
        var tiempoFalso = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
        var medioBoleto = new MedioBoleto(10000, tiempoFalso);
        var colectivo = new Colectivo("144");

        // Primer viaje - tarifa media
        Boleto boleto1 = colectivo.PagarCon(medioBoleto);
        Assert.IsNotNull(boleto1);
        Assert.That(boleto1.TarifaAbonada, Is.EqualTo(790m));
        tiempoFalso.AgregarMinutos(6);

        // Segundo viaje - tarifa media
        Boleto boleto2 = colectivo.PagarCon(medioBoleto);
        Assert.IsNotNull(boleto2);
        Assert.That(boleto2.TarifaAbonada, Is.EqualTo(790m));
        tiempoFalso.AgregarMinutos(6);

        // Tercer viaje - tarifa completa
        Boleto boleto3 = colectivo.PagarCon(medioBoleto);
        Assert.IsNotNull(boleto3);
        Assert.That(boleto3.TarifaAbonada, Is.EqualTo(1580m));
    }
}