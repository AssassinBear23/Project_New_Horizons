namespace UI.Elements.Buttons
{
    public class QuitGameButton : Button
    {
        public void QuitGame()
        {
            StaticMethods.QuitApplication();
        }
    }
}