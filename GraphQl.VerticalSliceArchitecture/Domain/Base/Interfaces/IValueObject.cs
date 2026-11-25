namespace Domain.Base.Interfaces;

/// <summary>
/// Interface de marcação.
/// Objetos de valor (Value Objects) não possuem identidade.
/// São imutáveis.
/// São comparados com base em seus atributos ou propriedades.
/// Geralmente requerem um contexto, talvez proveniente de um elemento pai.
/// Aprimoram a linguagem ubíqua.
/// Ajudam a eliminar a "obsessão por tipos primitivos".
/// </summary>
public interface IValueObject;