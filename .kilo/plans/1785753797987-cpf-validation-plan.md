# Plano: Validação de CPF

## Objetivo

Implementar validação de CPF no sistema com máscara em tempo real, feedback visual válido/inválido ao final da digitação, armazenamento como string limpa no banco de dados e verificação de duplicidade.

## Contexto Atual

- `CpfValidator` já existe no Domain (`ProcessoSelecao.Domain.Helpers.CpfValidator`) e no Blazor (`ProcessoSelecao.Blazor.Helpers.CpfValidator`), com métodos `Clean`, `Format`, `FormatProgressive` e `IsValid`.
- `Pagina2.razor` já aplica máscara progressiva (`FormatProgressive`) e valida no envio do formulário, mas **não exibe feedback válido/inválido ao final da digitação**.
- `Candidato.Cpf` é armazenado como `nvarchar(50)` no banco (string), conforme exigido.
- `CandidatoRepository.GetByCpfAsync` faz comparação direta sem limpar o CPF antes de buscar.
- `InscricaoService` já limpa o CPF com `CpfValidator.Clean` antes de salvar e valida com `CpfValidator.IsValid`.
- `CandidatoRepository.GetByCpfAsync` não limpa o CPF antes da busca, podendo falhar em encontrar duplicatas se o CPF estiver formatado.

## Decisões de Design

- **Máscara**: Usar `FormatProgressive` já existente no Blazor para máscara em tempo real durante a digitação.
- **Validação ao final da digitação**: Adicionar handler `@onblur` no input de CPF que exibe "Válido" ou "Inválido" com a máscara `000.000.000-00` aplicada.
- **Limpeza para salvamento**: O CPF salvo no banco deve conter apenas dígitos (sem pontos, traços ou espaços). O `InscricaoService` já faz isso corretamente.
- **Limpeza para verificação de duplicidade**: `GetByCpfAsync` deve limpar o CPF antes de comparar, e `CandidatoService.CreateAsync` deve verificar duplicidade limpando o CPF antes da busca.

## Tarefas

### 0. Corrigir CSS (antes de nada - bloqueio crítico)

- Adicionar ao `formulario.css` as seguintes classes:
  ```css
  .campo-cpf {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
  }

  .acao {
      margin-top: 0.25rem;
      font-size: 0.75rem;
      font-weight: 600;
  }

  .valido {
      color: #16a34a; /* Verde */
  }

  .invalido {
      color: #dc2626; /* Vermelho */
  }
  ```

### 1. Frontend — Limpar código e garantir validação (Pagina2.razor)

- Remover variável `erroCpf` não utilizada (linha 157).
- Garantir `@onblur="OnCpfBlur"` está funcionando corretamente.
- Garantir que `FormatProgressive` aplica máscara `000.000.000-00` ao digitar.
- Exibir feedback "CPF válido" (verde) ou "CPF inválido" (vermelho) após o blur.
- Tratar o caso inicial quando dados vêm de Pagina1 (CPF já formatado).

### 2. Backend — Limpeza de CPF na verificação de duplicidade (CandidatoRepository)

- Atualizar `GetByCpfAsync` em `CandidatoRepository.cs` para aplicar `CpfValidator.Clean(cpf)` antes da comparação no banco.
- Isso garante que um CPF formatado como `123.456.789-00` seja encontrado como `12345678900`.

### 3. Backend — Verificação de duplicidade ao criar candidato (CandidatoService)

- Adicionar verificação de duplicidade de CPF em `CandidatoService.CreateAsync`.
- Limpar o CPF com `CpfValidator.Clean` antes de chamar `GetByCpfAsync`.
- Se já existir candidato com o mesmo CPF, lançar exceção com mensagem clara.

### 4. Backend — Garantir CPF limpo ao salvar (InscricaoService)

- Verificar que `InscricaoService.CriarInscricaoCompletaAsync` já limpa o CPF antes de atribuir a `Candidato.Cpf` (já está correto na linha 54).
- Nenhuma alteração necessária aqui, apenas confirmar.

### 5. Testes

- Adicionar teste unitário para `CpfValidator.IsValid` cobrando casos limite (CPF válido, inválido, todos os dígitos iguais, null, vazio, formatado).
- Adicionar teste para `CpfValidator.Clean` garantindo que remove todos os caracteres não numéricos.
- Adicionar teste para `CpfValidator.Format` garantindo formatação `000.000.000-00`.
- Adicionar teste para `CandidatoService.CreateAsync` verificando que CPF duplicado lança exceção.

## Fluxo de Dados

1. Usuário digita CPF no campo → `FormatProgressive` aplica máscara `000.000.000-00` em tempo real.
2. Usuário sai do campo (blur) → `OnCpfBlur` limpa, valida → exibe feedback válido/inválido.
3. Ao enviar o formulário → CPF é limpo e validado novamente.
4. No backend → `InscricaoService` limpa o CPF com `CpfValidator.Clean` antes de salvar.
5. Na verificação de duplicidade → `GetByCpfAsync` limpa o CPF antes de comparar.

## Problemas Identificados

1. **CSS Missing**: As classes `.acao`, `.valido` e `.invalido` estão ausentes no `formulario.css`, tornando o feedback visual invisível.
2. **Variável residual**: `erroCpf` está declarada mas não usada - deve ser removida.
3. **Missing CSS**: Classes `.campo-cpf` e `.acao` não estão definidas no CSS.

## Riscos

- O `CandidatoRepository.GetByCpfAsync` atualmente não limpa o CPF, o que pode causar falhas na detecção de duplicatas se o CPF for salvo formatado.
- O `Pagina1.razor` permite selecionar "CPF" como tipo de documento e inserir o número em um campo genérico (`NumeroDocumento`). Esse valor é repassado para `Pagina2` como CPF, mas não passa por validação na página 1.

## Critérios de Aceitação

- Ao sair do campo de CPF (blur), é exibido "Válido" ou "Inválido" com a máscara `000.000.000-00` aplicada.
- CPF com todos os dígitos iguais é rejeitado como inválido.
- CPF salvo no banco contém apenas 11 dígitos numéricos (sem formatação).
- Tentativa de cadastro com CPF já existente retorna erro claro.
- `GetByCpfAsync` encontra candidato independentemente de o CPF estar formatado ou limpo.