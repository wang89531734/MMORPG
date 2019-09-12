public interface IPlayer
{
    string Id { get; set; }
    string ProfileName { get; set; }
    string LoginToken { get; set; }
    int Exp { get; set; }
    string SelectedFormation { get; set; }
    string MainCharacter { get; set; }
    int MainCharacterExp { get; set; }
}
