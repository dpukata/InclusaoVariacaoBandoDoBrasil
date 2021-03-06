﻿using System;
using NUnit.Framework;

namespace Boleto2Net.Testes
{
    public class BancoSantanderCarteira101Tests
    {
        readonly Banco _banco;
        public BancoSantanderCarteira101Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "12345678",
                DigitoConta = "9",
                CarteiraPadrao = "101",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = new Banco(033)
            {
                Cedente = Utils.GerarCedente("1234567", "", "123400001234567", contaBancaria)
            };
            _banco.FormataCedente();
        }

        [Test]
        public void Santander_101_REM240()
        {
            Utils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoSantanderCarteira101Tests), 5, true, "N");
        }

        [TestCase(2717.16, "456", "BB874A", "1", "0000456-1", "03391693400002717169123456700000000045610101", "03399.12347 56700.000005 00456.101013 1 69340000271716", 2016, 10, 1)]
        [TestCase(649.32, "414", "BB815A", "2", "0000414-6", "03392687300000649329123456700000000041460101", "03399.12347 56700.000005 00414.601013 2 68730000064932", 2016, 8, 1)]
        [TestCase(297.22, "444", "BB834A", "3", "0000444-8", "03393690500000297229123456700000000044480101", "03399.12347 56700.000005 00444.801013 3 69050000029722", 2016, 9, 2)]
        [TestCase(297.46, "13724", "BB834A", "4", "0013724-3", "03394690500000297469123456700000001372430101", "03399.12347 56700.000005 13724.301018 4 69050000029746", 2016, 9, 2)]
        [TestCase(297.34, "12428", "BB834A", "5", "0012428-1", "03395690500000297349123456700000001242810101", "03399.12347 56700.000005 12428.101013 5 69050000029734", 2016, 9, 2)]
        [TestCase(297.21, "443", "BB833A", "6", "0000443-0", "03396690500000297219123456700000000044300101", "03399.12347 56700.000005 00443.001011 6 69050000029721", 2016, 9, 2)]
        [TestCase(2924.11, "445", "BB874A", "7", "0000445-6", "03397690500002924119123456700000000044560101", "03399.12347 56700.000005 00445.601016 7 69050000292411", 2016, 9, 2)]
        [TestCase(141.50, "453", "BB943A", "8", "0000453-7", "03398690400000141509123456700000000045370101", "03399.12347 56700.000005 00453.701013 8 69040000014150", 2016, 9, 1)]
        [TestCase(297.45, "16278", "BB834A", "9", "0016278-7", "03399690500000297459123456700000001627870101", "03399.12347 56700.000005 16278.701012 9 69050000029745", 2016, 9, 2)]
        public void Santander_101_BoletoOK(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Sacado = Utils.GerarSacado()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}