public class GameOverScreen : PopUpMenu
{
    //ComponentReferences
    //Params
    //Temps
    //Public

    private void OnEnable()
    {
        PlayerController.OnGameOver += FadeIn;
    }

    private void OnDisable()
    {
        PlayerController.OnGameOver -= FadeIn;
    }
    
    public void ToMainMenu()
    {
        JumpTo(false);
        SceneController.Instance.CleanLoadMainMenu();
    }
}