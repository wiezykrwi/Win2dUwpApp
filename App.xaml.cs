using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

using Win2dUwpApp.Managers;

namespace Win2dUwpApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
		    var mainPage = new MainPage();
		    Window.Current.Content = mainPage;
			Window.Current.Activate();
			
			var inputManager = new InputManager(Window.Current.CoreWindow);
		    var renderManager = new RenderManager(mainPage);
	        var gameManager = new GameManager(inputManager, renderManager);

	        gameManager.Start();
        }
    }
}
