using Windows.Foundation;
using Windows.UI.Xaml;

namespace Win2dUwpApp.Managers
{
	public class RenderManager
	{
		private readonly MainPage _mainPage;

		public RenderManager(MainPage mainPage)
		{
			_mainPage = mainPage;
		}

		public void Render(GameManager gameManager)
		{
			_mainPage.Render(gameManager);
		}

		public Size GetSize()
		{
			return new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
		}
	}
}