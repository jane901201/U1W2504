public class CharacterState
{
    public enum EmotionType
    {
        Love = 0,
        Sad = 1
    }
    
    private EmotionType emotion = EmotionType.Love;

    public EmotionType Emotion { get => emotion; set => emotion = value; }

}