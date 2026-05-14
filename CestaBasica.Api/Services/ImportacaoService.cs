using ClosedXML.Excel;
using CestaBasica.Api.DTOs;
using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;

namespace CestaBasica.Api.Services;

public class ImportacaoService
{
    private readonly FuncionarioRepository _repository;

    public ImportacaoService(FuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ImportacaoResultadoDto> ImportarFuncionariosAsync(IFormFile arquivo)
    {
        var resultado = new ImportacaoResultadoDto();

        using var stream = new MemoryStream();

        await arquivo.CopyToAsync(stream);

        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheet(1);

        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            try
            {
                var funcionario = new Funcionario
                {
                    Nome = row.Cell(1).GetString(),
                    Matricula = row.Cell(2).GetString(),
                    CodigoBarras = row.Cell(3).GetString(),
                    Telefone = row.Cell(4).GetString(),
                    Setor = row.Cell(5).GetString()
                };

                await _repository.CriarAsync(funcionario);

                resultado.TotalImportados++;
            }
            catch (Exception ex)
            {
                resultado.Erros.Add(
                    $"Erro na linha {row.RowNumber()}: {ex.Message}");
            }
        }

        return resultado;
    }
}