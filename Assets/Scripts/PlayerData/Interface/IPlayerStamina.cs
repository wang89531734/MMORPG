public interface IPlayerStamina
{
    string Id { get; set; }
    string PlayerId { get; set; }
    string DataId { get; set; }
    int Amount { get; set; }
    long RecoveredTime { get; set; }
}
