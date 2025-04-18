public class CharacterState
{
    public enum EmotionType
    {
        Love = 0,
        Sad = 1
    }

    public enum RoleType
    {
        Oni = 67123, 
        Human = 67124
    }
    
    private EmotionType emotion = EmotionType.Love;
    private RoleType role = RoleType.Human;

    public EmotionType Emotion { get => emotion; set => emotion = value; }
    public RoleType Role { get => role; set => role = value; }

}