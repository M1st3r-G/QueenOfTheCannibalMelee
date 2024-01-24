public class WinScreenController : PopUpMenu
{
    //ComponentReferences
    //Params
    //Temps
    //Publics
    private void OnEnable()
    {
        BossController.OnWin += FadeIn;
    }

    private void OnDisable()
    {
        BossController.OnWin -= FadeIn;
    }
    
    public void ToMainMenu()
    {
        JumpTo(false);
        SceneController.Instance.CleanLoadMainMenu();
    }
}