using CestaBasica.Api.Models;
using CestaBasica.Api.Repositories;
using CestaBasica.Shared.DTOs;
using OfficeOpenXml;

namespace CestaBasica.Api.Services;

public class FuncionarioService
{
    private readonly FuncionarioRepository _funcionarioRepository;
    private readonly RetiradaRepository _retiradaRepository;
    private readonly CestaRepository _cestaRepository;

    public FuncionarioService(
        FuncionarioRepository funcionarioRepository,
        RetiradaRepository retiradaRepository,
        CestaRepository cestaRepository
        )
    {
        _cestaRepository = cestaRepository;
        _funcionarioRepository = funcionarioRepository;
        _retiradaRepository = retiradaRepository;
    }

    public async Task<FuncionarioDto> CriarAsync(FuncionarioDto dto)
    {
        var funcionario = new Funcionario
        {
            NomeCompleto = dto.NomeCompleto,
            Matricula = dto.Matricula,
            CodigoBarras = dto.CodigoBarras,
            Telefone = dto.Telefone,
            Setor = dto.Setor,
        };

        var criado = await _funcionarioRepository.CriarAsync(funcionario);

        return new FuncionarioDto
        {
            Id = criado.Id,
            NomeCompleto = criado.NomeCompleto,
            Matricula = criado.Matricula,
            CodigoBarras = criado.CodigoBarras,
            Telefone = criado.Telefone,
            Setor = criado.Setor,
            Status = "Pendente"
        };
    }

    public async Task<List<FuncionarioDto>> ListarAsync()
    {
        var funcionarios = await _funcionarioRepository.ListarAsync();

        var lista = new List<FuncionarioDto>();

        foreach (var funcionario in funcionarios)
        {
            var historico = await ObterHistoricoAsync(funcionario.Id);

            lista.Add(new FuncionarioDto
            {
                Id = funcionario.Id,
                NomeCompleto = funcionario.NomeCompleto,
                Matricula = funcionario.Matricula,
                CodigoBarras = funcionario.CodigoBarras,
                Telefone = funcionario.Telefone,
                Setor = funcionario.Setor,
                Status = historico.Status
            });
        }

        return lista;
    }

    public async Task ExcluirAsync(int id)
    {
        await _funcionarioRepository.ExcluirAsync(id);
    }

    public async Task<Funcionario> AtualizarAsync(FuncionarioDto dto)
    {
        var funcionario = await _funcionarioRepository.BuscarPorIdAsync(dto.Id);

        if (funcionario is null)
            throw new Exception("Funcionário não encontrado.");

        funcionario.NomeCompleto = dto.NomeCompleto;
        funcionario.Telefone = dto.Telefone;
        funcionario.Matricula = dto.Matricula;
        funcionario.CodigoBarras = dto.CodigoBarras;
        funcionario.Setor = dto.Setor;

        return await _funcionarioRepository.AtualizarAsync(funcionario);
    }

    public async Task<HistoricoFuncionarioDto> ObterHistoricoAsync(int funcionarioId)
    {
        var cestasAtivas = await _cestaRepository.ListarAtivasAsync();

        if (!cestasAtivas.Any())
        {
            return new HistoricoFuncionarioDto
            {
                Status = "Sem lote ativo",
                LotesAtivos = "-"
            };
        }

        var statusList = new List<string>();
        var lotesList = new List<string>();

        foreach (var cesta in cestasAtivas)
        {
            var retirada = await _retiradaRepository
                .BuscarPorFuncionarioECestaAsync(funcionarioId, cesta.Id);

            statusList.Add(retirada is null ? "Pendente" : "Retirado");
            lotesList.Add(cesta.Nome);
        }

        return new HistoricoFuncionarioDto
        {
            Status = string.Join(", ", statusList),
            LotesAtivos = string.Join(", ", lotesList)
        };
    }
    public async Task<FuncionarioDto?> BuscarPorCodigoAsync(string codigoBarras)
    {
        var funcionario = await _funcionarioRepository.BuscarPorCodigoBarrasAsync(codigoBarras);

        if (funcionario is null)
            return null;

        return new FuncionarioDto
        {
            Id = funcionario.Id,
            NomeCompleto = funcionario.NomeCompleto,
            Matricula = funcionario.Matricula,
            CodigoBarras = funcionario.CodigoBarras,
            Telefone = funcionario.Telefone,
            Setor = funcionario.Setor,
            Status = funcionario.Status
        };
    }
    public async Task<List<FuncionarioDto>> BuscarManualAsync(string termo)
    {
        var funcionarios = await _funcionarioRepository.BuscaManualAsync(termo);

        return funcionarios.Select(f => new FuncionarioDto
        {
            Id = f.Id,
            NomeCompleto = f.NomeCompleto,
            Matricula = f.Matricula,
            CodigoBarras = f.CodigoBarras,
            Telefone = f.Telefone,
            Setor = f.Setor,
            Status = f.Status
        }).ToList();
    }
    public async Task<FuncionarioDto?> BuscarPorIdAsync(int id)
    {
        var funcionario = await _funcionarioRepository.BuscarPorIdAsync(id);

        if (funcionario is null)
            return null;

        var historico = await ObterHistoricoAsync(funcionario.Id);

        return new FuncionarioDto
        {
            Id = funcionario.Id,
            NomeCompleto = funcionario.NomeCompleto,
            Matricula = funcionario.Matricula,
            CodigoBarras = funcionario.CodigoBarras,
            Telefone = funcionario.Telefone,
            Setor = funcionario.Setor,
            Status = historico.Status
        };
    }
    public async Task<byte[]> ExportarExcelAsync()
    {
        var funcionarios = await _funcionarioRepository.ListarAsync();

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Funcionários");

        sheet.Cells[1, 1].Value = "Nome";
        sheet.Cells[1, 2].Value = "Matrícula";
        sheet.Cells[1, 3].Value = "Código de Barras";
        sheet.Cells[1, 4].Value = "Telefone";
        sheet.Cells[1, 5].Value = "Setor";
        sheet.Cells[1, 6].Value = "Status";

        int linha = 2;

        foreach (var f in funcionarios)
        {
            sheet.Cells[linha, 1].Value = f.NomeCompleto;
            sheet.Cells[linha, 2].Value = f.Matricula;
            sheet.Cells[linha, 3].Value = f.CodigoBarras;
            sheet.Cells[linha, 4].Value = f.Telefone;
            sheet.Cells[linha, 5].Value = f.Setor;
            sheet.Cells[linha, 6].Value = f.Status;

            linha++;
        }

        sheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }
}
