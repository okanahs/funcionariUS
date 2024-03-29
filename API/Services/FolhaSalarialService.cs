﻿using Funcionarius.Entity;
using Funcionarius.Repository;
using System;
using System.Threading.Tasks;

namespace Funcionarius.Services
{
    public class FolhaSalarialService
    {
        public async Task<FolhaSalarial> CalcularFolhaSalarialFuncionario(int horasTrabalhadas, Funcionario funcionario)
        {
            var calculaFolhaComDescontos = CalcularDescontosFolha(funcionario, horasTrabalhadas);
            return calculaFolhaComDescontos;
        }
        protected FolhaSalarial CalcularDescontosFolha(Funcionario funcionario, int horasTrabalhadas)
        {
            var calculaSalarioBruto = CalculaSalarioBrutoHorasTrabalhadas(funcionario.SalarioBruto, horasTrabalhadas);
            var descontoINSS = CalculaPercentualDescontoINSS(calculaSalarioBruto);
            var descontoIRRF = CalculaDescontoIRRF(calculaSalarioBruto - descontoINSS);
            var salarioLiquido = (calculaSalarioBruto - descontoINSS - descontoIRRF);
            return PreencheFolha(funcionario, horasTrabalhadas, calculaSalarioBruto, descontoINSS, descontoIRRF, salarioLiquido);
        }

        protected FolhaSalarial PreencheFolha(Funcionario funcionario, int horasTrabalhadas, decimal salarioBruto, decimal descontoINSS, decimal descontoIRRF, decimal salarioLiquido)
        {
            FolhaSalarial folha = new FolhaSalarial();
            folha.IdCargoFuncionario = funcionario.IdCargoFuncionario;
            folha.Competencia = DateTime.Now;
            folha.FuncionarioID = funcionario.Id;
            folha.HorasTrabalhadas = horasTrabalhadas;
            folha.INSS = descontoINSS;
            folha.IRRF = descontoIRRF;
            folha.NomeFuncionario = funcionario.NomeCompleto;
            folha.SalarioBruto = salarioBruto;
            folha.SalarioLiquido = salarioLiquido;
            return folha;
        }

        protected decimal CalculaSalarioBrutoHorasTrabalhadas(decimal salarioBruto, int horasTrabalhadas)
        {
            var valorHora = (salarioBruto / 220);
            return valorHora * horasTrabalhadas;
        }

        protected decimal CalculaDescontoIRRF(decimal salario)
        {
            var sal = Convert.ToDouble(salario);
            double resultado;
            if (sal < 1903.98)
                resultado = sal;
            else if (sal > 1903.99 && sal < 2826.65)
                resultado = ((sal * 0.075) - 142);
            else if (sal > 2826.66 && sal < 3751.05)
                resultado = ((sal * 0.15) - 354);
            else if (sal > 3751.06 && sal < 4664.68)
                resultado = ((sal * 0.225) - 636);
            else
                resultado = ((sal * 0.275) - 869);

            return Convert.ToDecimal(resultado);
        }

        protected decimal CalculaPercentualDescontoINSS(decimal salario)
        {
            if (salario <= 1100)
                return CalculaFaixaSalarial(1, salario);
            else if (salario >= 1101 && salario < 2203)
                return CalculaFaixaSalarial(2, salario);
            else if (salario >= 2204 && salario <= 3305)
                return CalculaFaixaSalarial(3, salario);
            else
                return CalculaFaixaSalarial(4, salario);
        }

        protected decimal CalculaFaixaSalarial(int faixa, decimal salario)
        {
            var salarioDouble = Convert.ToDouble(salario);
            double retorno;
            switch (faixa)
            {
                case 1:
                    retorno = (salarioDouble * 0.075);
                    break;
                case 2:
                    retorno = ((1100 * 0.075) + ((salarioDouble - 1100.01) * 0.09));
                    break;
                case 3:
                    retorno = ((1100 * 0.075) + (((2203.48 - 1100.01) * 0.09) + (((salarioDouble - 2203.49) * 0.12))));
                    break;
                default:
                    retorno = (((1100 * 0.075) + (((2203.48 - 1100.01) * 0.09)) + (((3305.22 - 2203.49) * 0.12)) + (((salarioDouble - 3305.23) * 0.14))));
                    break;
            }

            return Convert.ToDecimal(retorno);
        }
    }
}
