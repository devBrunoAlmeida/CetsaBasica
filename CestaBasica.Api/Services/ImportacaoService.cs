// CestaBasica.Api/Services/ImportacaoService.cs
using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;
using OfficeOpenXml;

namespace CestaBasica.Api.Services;

public class ImportacaoService
{
    private readonly FuncionarioRepository _funcionarioRepository;
    private readonly HistoricoImportacaoRepository _historicoRepository;

    public ImportacaoService(FuncionarioRepository funcionarioRepository, HistoricoImportacaoRepository historicoRepository)
    {
        _funcionarioRepository = funcionarioRepository;
        _historicoRepository = historicoRepository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<ImportacaoResultadoDto> ImportarAsync(Stream arquivo, string nomeArquivo)
{
    var resultado = new ImportacaoResultadoDto();

    if (nomeArquivo.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        resultado = await ImportarCsvAsync(arquivo, resultado);
    else
        resultado = await ImportarExcelAsync(arquivo, resultado);

    await _historicoRepository.AdicionarAsync(new HistoricoImportacao
    {
        NomeArquivo = nomeArquivo,
        DataImportacao = DateTime.UtcNow,
        Usuario = "Administrador",
        QuantidadeRegistros = resultado.TotalImportados
    });

    return resultado;
}

    private async Task<ImportacaoResultadoDto> ImportarExcelAsync(Stream arquivo, ImportacaoResultadoDto resultado)
    {
        using var package = new ExcelPackage(arquivo);
        var sheet = package.Workbook.Worksheets[0];
        int totalLinhas = sheet.Dimension?.Rows ?? 0;

        for (int row = 2; row <= totalLinhas; row++)
        {
            var registro = LerLinhaExcel(sheet, row);
            resultado.Registros.Add(registro);

            if (registro.Valido)
                await SalvarAsync(registro);
        }

        Totalizar(resultado);
        return resultado;
    }

    private static RegistroImportacaoDto LerLinhaExcel(ExcelWorksheet sheet, int row)
    {
        var reg = new RegistroImportacaoDto
        {
            Linha        = row,
            Nome         = sheet.Cells[row, 1].Text.Trim(),
            Matricula    = sheet.Cells[row, 2].Text.Trim(),
            CodigoBarras = sheet.Cells[row, 3].Text.Trim(),
            Telefone     = sheet.Cells[row, 4].Text.Trim(),
            Setor        = sheet.Cells[row, 5].Text.Trim(),
        };

        Validar(reg);
        return reg;
    }

    private async Task<ImportacaoResultadoDto> ImportarCsvAsync(Stream arquivo, ImportacaoResultadoDto resultado)
    {
        using var reader = new StreamReader(arquivo);
        await reader.ReadLineAsync(); // pula cabeçalho

        string? linha;
        int row = 1;

        while ((linha = await reader.ReadLineAsync()) is not null)
        {
            row++;
            var cols = linha.Split(';');

            var reg = new RegistroImportacaoDto
            {
                Linha        = row,
                Nome         = Col(cols, 0),
                Matricula    = Col(cols, 1),
                CodigoBarras = Col(cols, 2),
                Telefone     = Col(cols, 3),
                Setor        = Col(cols, 4),
            };

            Validar(reg);
            resultado.Registros.Add(reg);

            if (reg.Valido)
                await SalvarAsync(reg);
        }

        Totalizar(resultado);
        return resultado;
    }

    private static string Col(string[] cols, int i)
        => i < cols.Length ? cols[i].Trim() : string.Empty;

    private static void Validar(RegistroImportacaoDto reg)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(reg.Nome))
            erros.Add("Nome obrigatório");

        if (string.IsNullOrWhiteSpace(reg.Matricula))
            erros.Add("Matrícula obrigatória");

        if (string.IsNullOrWhiteSpace(reg.CodigoBarras))
            erros.Add("Código de barras obrigatório");

        reg.Valido = erros.Count == 0;
        reg.Erro   = erros.Count > 0 ? string.Join("; ", erros) : "-";
    }

    private async Task SalvarAsync(RegistroImportacaoDto reg)
    {
        var funcionario = new Funcionario
        {
            NomeCompleto = reg.Nome,
            Matricula    = reg.Matricula,
            CodigoBarras = reg.CodigoBarras,
            Telefone     = reg.Telefone,
            Setor        = reg.Setor,
            Status       = "Pendente"
        };

        await _funcionarioRepository.CriarAsync(funcionario);
    }
    
    private static void Totalizar(ImportacaoResultadoDto r)
    {
        r.TotalLidos      = r.Registros.Count;
        r.TotalImportados = r.Registros.Count(x => x.Valido);
        r.TotalInvalidos  = r.Registros.Count(x => !x.Valido);
    }
    public async Task<List<HistoricoImportacaoDto>> ObterHistoricoAsync()
{
    var historicos = await _historicoRepository.ListarAsync();

    return historicos.Select(x => new HistoricoImportacaoDto
    {
        Id = x.Id,
        NomeArquivo = x.NomeArquivo,
        DataImportacao = x.DataImportacao,
        Usuario = x.Usuario,
        QuantidadeRegistros = x.QuantidadeRegistros
    }).ToList();
}
}