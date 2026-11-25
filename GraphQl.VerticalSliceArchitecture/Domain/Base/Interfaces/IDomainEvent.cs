namespace Domain.Base.Interfaces;

/// <summary>
//Interface de marcação para algo que ocorreu no domínio,
//disparado por um agregado e publicado após a persistência da alteração.
/// </summary>
/// Declarado aqui, em vez de ser obtido de uma biblioteca de mensageria,
/// para que o domínio permaneça livre de pacotes de framework.
/// O contrato do *handler* e o *dispatcher* que o executa residem no projeto de API,
/// que é a camada autorizada a ter conhecimento sobre injeção de dependência.
public interface IDomainEvent;