﻿using System;

namespace TransporteUrbano
{
    public class Boleto
    {
        public DateTime Fecha { get; private set; }
        public decimal TarifaAbonada { get; private set; }
        public decimal SaldoRestante { get; private set; }
        public string LineaColectivo { get; private set; }
        
        // NUEVAS PROPIEDADES
        public string TipoTarjeta { get; private set; }
        public string IdTarjeta { get; private set; }
        public decimal MontoTotalAbonado { get; private set; }
        public bool TieneSaldoNegativo { get; private set; }

        // CONSTRUCTOR ACTUALIZADO
        public Boleto(decimal tarifaAbonada, decimal saldoRestante, string lineaColectivo, 
                     string tipoTarjeta, string idTarjeta, bool tieneSaldoNegativo)
        {
            Fecha = DateTime.Now;
            TarifaAbonada = tarifaAbonada;
            SaldoRestante = saldoRestante;
            LineaColectivo = lineaColectivo;
            TipoTarjeta = tipoTarjeta;
            IdTarjeta = idTarjeta;
            TieneSaldoNegativo = tieneSaldoNegativo;
            
            // Calcular monto total abonado
            MontoTotalAbonado = tieneSaldoNegativo ? 
                tarifaAbonada + Math.Abs(saldoRestante) : tarifaAbonada;
        }

        public override string ToString()
        {
            string infoSaldoNegativo = TieneSaldoNegativo ? 
                $"\nMonto Total Abonado: ${MontoTotalAbonado} (incluye saldo negativo)" : "";
            
            return $"Boleto - Fecha: {Fecha:dd/MM/yyyy HH:mm:ss}\n" +
                   $"Tarifa: ${TarifaAbonada}\n" +
                   $"Saldo Restante: ${SaldoRestante}\n" +
                   $"Línea: {LineaColectivo}\n" +
                   $"Tipo Tarjeta: {TipoTarjeta}\n" +
                   $"ID Tarjeta: {IdTarjeta}" +
                   infoSaldoNegativo;
        }
    }
}