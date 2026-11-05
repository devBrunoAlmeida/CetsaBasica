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
    private readonly NotificacaoRepository _notificacaoRepository;

    public FuncionarioService(
        FuncionarioRepository funcionarioRepository,
        RetiradaRepository retiradaRepository,
        CestaRepository cestaRepository,
        NotificacaoRepository notificacaoRepository
        )
    {
        _cestaRepository = cestaRepository;
        _funcionarioRepository = funcionarioRepository;
        _retiradaRepository = retiradaRepository;
        _notificacaoRepository = notificacaoRepository;
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
            Status = "Pendente",
            CriadoEm = DateTime.UtcNow
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
            Status = "Pendente",
            DataRetirada = criado.DataRetirada,
            CriadoEm = criado.CriadoEm
        };
    }

    private async Task<(bool TemLoteAtivo, HashSet<int> FuncionariosComRetirada)> ObterMapaStatusAtivoAsync()
    {
        var cestasAtivas = await _cestaRepository.ListarAtivasAsync();
        if (cestasAtivas.Count == 0)
            return (false, new HashSet<int>());

        var cestaIds = cestasAtivas.Select(c => c.Id).ToList();
        var retiraram = await _retiradaRepository.ListarFuncionariosComRetiradaEmCestasAsync(cestaIds);
        return (true, retiraram);
    }

    private static string ResolverStatus(int funcionarioId, bool temLoteAtivo, HashSet<int> funcionariosComRetirada)
    {
        if (!temLoteAtivo)
            return "Sem lote ativo";

        return funcionariosComRetirada.Contains(funcionarioId) ? "Retirado" : "Pendente";
    }

    public async Task<List<FuncionarioDto>> ListarAsync()
    {
        var funcionarios = await _funcionarioRepository.ListarAsync();
        var (temLoteAtivo, retiraram) = await ObterMapaStatusAtivoAsync();

        return funcionarios.Select(funcionario => new FuncionarioDto
        {
            Id = funcionario.Id,
            NomeCompleto = funcionario.NomeCompleto,
            Matricula = funcionario.Matricula,
            CodigoBarras = funcionario.CodigoBarras,
            Telefone = funcionario.Telefone,
            Setor = funcionario.Setor,
            Status = ResolverStatus(funcionario.Id, temLoteAtivo, retiraram),
            DataRetirada = funcionario.DataRetirada,
            CriadoEm = funcionario.CriadoEm
        }).ToList();
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
        var funcionario = await _funcionarioRepository.BuscarPorIdAsync(funcionarioId);
        var cestasAtivas = await _cestaRepository.ListarAtivasAsync();
        var retiradas = await _retiradaRepository.ListarPorFuncionarioAsync(funcionarioId);
        var notificacoes = await _notificacaoRepository.ListarPorFuncionarioAsync(funcionarioId);

        var eventos = new List<EventoHistoricoDto>();

        if (funcionario is not null)
        {
            eventos.Add(new EventoHistoricoDto
            {
                DataHora = funcionario.CriadoEm,
                Tipo = "CADASTRO",
                Titulo = "Cadastro no Sistema",
                Descricao = $"Funcionário vinculado à {funcionario.Setor} (Matrícula {funcionario.Matricula}).",
                Responsavel = "Sistema"
            });
        }

        foreach (var r in retiradas)
        {
            eventos.Add(new EventoHistoricoDto
            {
                DataHora = r.DataRetirada,
                Tipo = "RETIRADA",
                Titulo = "Retirado",
                Descricao = $"Entrega realizada — Lote: {r.Cesta?.Nome ?? $"#{r.CestaId}"}",
                Responsavel = r.Cesta?.Nome ?? "Atendimento"
            });
        }

        foreach (var n in notificacoes)
        {
            eventos.Add(new EventoHistoricoDto
            {
                DataHora = n.DataEnvio ?? DateTime.UtcNow,
                Tipo = "NOTIFICACAO",
                Titulo = n.Status,
                Descricao = $"{n.Canal} ({n.Telefone}): {n.Mensagem}",
                Responsavel = n.Canal
            });
        }

        eventos = eventos.OrderByDescending(e => e.DataHora).ToList();

        var ultimaRetirada = retiradas.FirstOrDefault()?.DataRetirada;

        if (!cestasAtivas.Any())
        {
            return new HistoricoFuncionarioDto
            {
                Status = "Sem lote ativo",
                LotesAtivos = "-",
                UltimaRetirada = ultimaRetirada,
                Eventos = eventos
            };
        }

        var idsCestasAtivas = cestasAtivas.Select(c => c.Id).ToHashSet();
        var retirouLoteAtivo = retiradas.Any(r => idsCestasAtivas.Contains(r.CestaId));

        return new HistoricoFuncionarioDto
        {
            Status = retirouLoteAtivo ? "Retirado" : "Pendente",
            LotesAtivos = string.Join(", ", cestasAtivas.Select(c => c.Nome)),
            UltimaRetirada = ultimaRetirada,
            ProximaRetirada = ultimaRetirada?.AddMonths(1),
            Eventos = eventos
        };
    }

    public async Task<FuncionarioDto?> BuscarPorCodigoAsync(string codigoBarras)
    {
        var funcionario = await _funcionarioRepository.BuscarPorCodigoBarrasAsync(codigoBarras);

        if (funcionario is null)
            return null;

        var (temLoteAtivo, retiraram) = await ObterMapaStatusAtivoAsync();

        return new FuncionarioDto
        {
            Id = funcionario.Id,
            NomeCompleto = funcionario.NomeCompleto,
            Matricula = funcionario.Matricula,
            CodigoBarras = funcionario.CodigoBarras,
            Telefone = funcionario.Telefone,
            Setor = funcionario.Setor,
            Status = ResolverStatus(funcionario.Id, temLoteAtivo, retiraram),
            DataRetirada = funcionario.DataRetirada,
            CriadoEm = funcionario.CriadoEm
        };
    }

    public async Task<List<FuncionarioDto>> BuscarManualAsync(string termo)
    {
        var funcionarios = await _funcionarioRepository.BuscaManualAsync(termo);
        var (temLoteAtivo, retiraram) = await ObterMapaStatusAtivoAsync();

        return funcionarios.Select(f => new FuncionarioDto
        {
            Id = f.Id,
            NomeCompleto = f.NomeCompleto,
            Matricula = f.Matricula,
            CodigoBarras = f.CodigoBarras,
            Telefone = f.Telefone,
            Setor = f.Setor,
            Status = ResolverStatus(f.Id, temLoteAtivo, retiraram),
            DataRetirada = f.DataRetirada,
            CriadoEm = f.CriadoEm
        }).ToList();
    }

    public async Task<FuncionarioDto?> BuscarPorIdAsync(int id)
    {
        var funcionario = await _funcionarioRepository.BuscarPorIdAsync(id);

        if (funcionario is null)
            return null;

        var (temLoteAtivo, retiraram) = await ObterMapaStatusAtivoAsync();

        return new FuncionarioDto
        {
            Id = funcionario.Id,
            NomeCompleto = funcionario.NomeCompleto,
            Matricula = funcionario.Matricula,
            CodigoBarras = funcionario.CodigoBarras,
            Telefone = funcionario.Telefone,
            Setor = funcionario.Setor,
            Status = ResolverStatus(funcionario.Id, temLoteAtivo, retiraram),
            DataRetirada = funcionario.DataRetirada,
            CriadoEm = funcionario.CriadoEm
        };
    }

    public async Task<byte[]> ExportarExcelAsync()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var funcionarios = await ListarAsync();

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

    public async Task<List<string>> ListarSetoresAsync()
    {
        return await _funcionarioRepository.ListarSetoresAsync();
    }
}
