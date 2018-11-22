using Windows.Foundation;
using Windows.UI.Core;

namespace Win2dUwpApp.Managers
{
	public class InputManager
	{
		public InputManager(CoreWindow coreWindow)
		{
			coreWindow.KeyDown += OnCoreWindowOnKeyDown;
			coreWindow.KeyUp += OnCoreWindowOnKeyUp;
			coreWindow.PointerMoved += OnCoreWindowOnPointer;
			coreWindow.PointerPressed += OnCoreWindowOnPointer;
			coreWindow.PointerReleased += OnCoreWindowOnPointer;
		}

		public Point PointerPosition { get; private set; }
		public bool IsLeftButtonPressed { get; private set; }
		public bool IsRightButtonPressed { get; private set; }

		private readonly bool[] _keys = new bool[256];
		
		private void OnCoreWindowOnPointer(CoreWindow sender, PointerEventArgs eventArgs)
		{
			PointerPosition = eventArgs.CurrentPoint.Position;

			IsLeftButtonPressed = eventArgs.CurrentPoint.Properties.IsLeftButtonPressed;
			IsRightButtonPressed = eventArgs.CurrentPoint.Properties.IsRightButtonPressed;
		}
		
		private void OnCoreWindowOnKeyUp(CoreWindow sender, KeyEventArgs eventArgs)
		{
			_keys[(int) eventArgs.VirtualKey] = false;
		}

		private void OnCoreWindowOnKeyDown(CoreWindow sender, KeyEventArgs eventArgs)
		{
			_keys[(int) eventArgs.VirtualKey] = true;
		}
	}
}