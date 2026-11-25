namespace Domain.Teams;

public static class MissionErrors
{
    public static readonly Error AlreadyCompleted = Error.Conflict(
        "Missão.JáConcluída",
        "Missão já concluída");
}