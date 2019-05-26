namespace LBCService
{
    public interface IBacklightControl
    {
        uint GetKeyboardBackLightLevel(out int level);

        uint GetKeyboardBackLightStatus(out int nStatus);

        uint SetKeyboardBackLightStatus(int nStatus);
    }
}