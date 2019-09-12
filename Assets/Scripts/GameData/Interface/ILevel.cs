public interface ILevel
{
    int Level { get; }
    int MaxLevel { get; }
    int CollectExp { get; }
    int NextExp { get; }
}

public static class LevelExpExtention
{
    public static float ExpRate(this ILevel level)
    {
        return (float)level.CollectExp / (float)level.NextExp;
    }

    public static int RequireExp(this ILevel level)
    {
        var requireExp = level.NextExp - level.CollectExp;
        if (requireExp < 0)
            return 0;
        return requireExp;
    }
}