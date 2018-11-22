using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.UI;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

using Win2dUwpApp.Managers;
using Win2dUwpApp.Models;

namespace Win2dUwpApp
{
	public sealed partial class MainPage
	{
		private readonly AutoResetEvent _signal = new AutoResetEvent(false);

		public MainPage()
		{
			InitializeComponent();
		}

		private int frames;
		private int fps;
		private int time;
		private int previousTime;
		private GameManager _gameManager;
		private CanvasBitmap _tileImage;

		private void Canvas_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
		{
			Debug.WriteLine($"drawing {frames}");
			var drawingSession = args.DrawingSession;

			foreach (var hex in _gameManager.Board.GetVisibleHexes(_gameManager.Camera))
			{
				DrawHex(drawingSession, hex, _gameManager.Camera);
			}

			frames++;
			time += Environment.TickCount - previousTime;
			previousTime = Environment.TickCount;
			if (time >= 1000)
			{
				fps = frames;
				frames = 0;
				time = 0;
			}

			drawingSession.DrawText($"FPS: {fps}", 10, 10, Colors.Black);

			_signal.Set();
		}

		private void DrawHex(CanvasDrawingSession drawingSession, Hex hex, Camera camera)
		{
			drawingSession.DrawLine(hex.Corners[0] - camera.Offset, hex.Corners[1] - camera.Offset, Colors.Black);
			drawingSession.DrawLine(hex.Corners[1] - camera.Offset, hex.Corners[2] - camera.Offset, Colors.Black);
			drawingSession.DrawLine(hex.Corners[2] - camera.Offset, hex.Corners[3] - camera.Offset, Colors.Black);
			drawingSession.DrawLine(hex.Corners[3] - camera.Offset, hex.Corners[4] - camera.Offset, Colors.Black);
			drawingSession.DrawLine(hex.Corners[4] - camera.Offset, hex.Corners[5] - camera.Offset, Colors.Black);
			drawingSession.DrawLine(hex.Corners[5] - camera.Offset, hex.Corners[0] - camera.Offset, Colors.Black);

			var centerPoint = hex.Center - camera.Offset;
			
			var rect = new Rect(centerPoint.ToPoint(), centerPoint.ToPoint());
			for (int i = 0; i < 6; i++)
			{
				rect.Union((hex.Corners[i] - camera.Offset).ToPoint());
			}
			drawingSession.DrawImage(_tileImage, rect);

			drawingSession.DrawCircle(centerPoint, 3, hex.IsSelected ? Colors.Blue : Colors.Gold);
			CanvasTextFormat format = new CanvasTextFormat { FontSize = 20.0f, WordWrapping = CanvasWordWrapping.NoWrap };
			CanvasTextLayout textLayout = new CanvasTextLayout(drawingSession, hex.Tag, format, 0.0f, 0.0f);
			drawingSession.DrawTextLayout(textLayout, centerPoint.X - (float) textLayout.DrawBounds.Width / 2.0f, centerPoint.Y, Colors.Black);
		}

		public void Render(GameManager gameManager)
		{
			_gameManager = gameManager;
			Canvas.Invalidate();

			_signal.WaitOne();
		}

		private void Canvas_OnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
		{
			args.TrackAsyncAction(Task.Run(async () =>
			{
				_tileImage = await CanvasBitmap.LoadAsync(sender, "tile.png");
			}).AsAsyncAction());
		}
	}
}