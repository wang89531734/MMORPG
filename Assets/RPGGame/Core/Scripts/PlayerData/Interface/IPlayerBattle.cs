public interface IPlayerBattle
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string DataId { get; set; }
    string Session { get; set; }
    uint BattleResult { get; set; }
    int Rating { get; set; }
}
