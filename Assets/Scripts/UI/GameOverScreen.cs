namespace UI
{
    public class GameOverScreen : PopUpMenu
    {
        private void OnEnable() => PlayerController.OnGameOver += FadeIn;
        private void OnDisable() => PlayerController.OnGameOver -= FadeIn;
    
        public void ToMainMenu()
        {
            JumpTo(false);
            SceneController.Instance.CleanLoadMainMenu();
        }
    }
}