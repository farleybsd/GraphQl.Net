namespace Domain.Teams;

public static class TeamErrors
{
    public static readonly Error NotOnMission = Error.Conflict(
        "Equipe.ForaDeMissão",
        "A equipe não está, no momento, em uma missão.");

    public static readonly Error NotAvailable = Error.Conflict(
        "Equipe.NãoDisponível",
        "A equipe não está disponível para uma nova missão.");

    public static readonly Error NotFound = Error.NotFound(
        "Equipe.NãoEncontrada",
        "A equipe não foi encontrada.");

    public static readonly Error NoHeroes = Error.Conflict(
        "Equipe.SemHerois",
        "A equipe não possui heróis.");
}