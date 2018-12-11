using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.UI;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
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

			if (_gameManager.Board.Hexes.ContainsKey(_gameManager.Board.Coordinate))
			{
				var currentHex = _gameManager.Board.Hexes[_gameManager.Board.Coordinate];
				HighlightHex(currentHex, drawingSession, Color.FromArgb(100, 255, 0, 100));

//				var neighbours = GetNeighbours(_gameManager.Board.Coordinate);
//				foreach (var neighbour in neighbours)
//				{
//					var neighbourHex = _gameManager.Board.Hexes[neighbour];
//					HighlightHex(neighbourHex, drawingSession, Color.FromArgb(100, 0, 255, 100));
//				}
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

			drawingSession.FillRectangle(0, 0, 150, 90, Colors.White);
			drawingSession.DrawText($"FPS: {fps}", 10, 10, Colors.Black);
			drawingSession.DrawText($"Hex: {_gameManager.Board.Coordinate}", 10, 30, Colors.Black);
			drawingSession.DrawText($"Pnt: {(int)_gameManager.Input.PointerPosition.X}, {(int)_gameManager.Input.PointerPosition.Y}", 10, 50, Colors.Black);

			_signal.Set();
		}

//		private Coordinate[] GetNeighbours(Coordinate coordinate)
//		{
//			var result = new List<Coordinate>(6);
//			
//			var coord1 = new Coordinate(coordinate.Q, coordinate.R - 1);
//			if (_gameManager.Board.Hexes.ContainsKey(coord1))
//			{
//				result.Add(coord1);
//			}
//			var coord2 = new Coordinate(coordinate.Q + 1, coordinate.R - 1);
//			if (_gameManager.Board.Hexes.ContainsKey(coord2))
//			{
//				result.Add(coord2);
//			}
//			var coord3 = new Coordinate(coordinate.Q - 1, coordinate.R);
//			if (_gameManager.Board.Hexes.ContainsKey(coord3))
//			{
//				result.Add(coord3);
//			}
//			var coord4 = new Coordinate(coordinate.Q + 1, coordinate.R);
//			if (_gameManager.Board.Hexes.ContainsKey(coord4))
//			{
//				result.Add(coord4);
//			}
//			var coord5 = new Coordinate(coordinate.Q - 1, coordinate.R + 1);
//			if (_gameManager.Board.Hexes.ContainsKey(coord5))
//			{
//				result.Add(coord5);
//			}
//			var coord6 = new Coordinate(coordinate.Q, coordinate.R + 1);
//			if (_gameManager.Board.Hexes.ContainsKey(coord6))
//			{
//				result.Add(coord6);
//			}
//
//			return result.ToArray();
//		}

		private void HighlightHex(Hex currentHex, CanvasDrawingSession drawingSession, Color highlight)
		{
			var currentHexCorners = (Vector2[]) currentHex.Corners.Clone();
			for (int i = 0; i < 6; i++)
			{
				currentHexCorners[i] -= _gameManager.Camera.Offset;
			}

			drawingSession.FillGeometry(CanvasGeometry.CreatePolygon(drawingSession, currentHexCorners), highlight);
		}

		private void DrawHex(CanvasDrawingSession drawingSession, Hex hex, Camera camera)
		{
			if (hex.Passable)
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

				rect.Y = Math.Floor(rect.Y) - 2.0d;
				rect.Height = Math.Ceiling(rect.Height) + 4.0d;
				drawingSession.DrawImage(_tileImage, rect);

				drawingSession.DrawCircle(centerPoint, 3, hex.IsSelected ? Colors.Blue : Colors.Gold);
				CanvasTextFormat format = new CanvasTextFormat { FontSize = 20.0f, WordWrapping = CanvasWordWrapping.NoWrap };
				CanvasTextLayout textLayout = new CanvasTextLayout(drawingSession, hex.Tag, format, 0.0f, 0.0f);
				drawingSession.DrawTextLayout(textLayout, centerPoint.X - (float) textLayout.DrawBounds.Width / 2.0f, centerPoint.Y, Colors.Black);

				if (hex.Pawn != null)
				{
					drawingSession.FillRectangle(new Rect((centerPoint - new Vector2(25, 25)).ToPoint(), new Size(50, 50)), hex.Pawn.Selected ? Colors.HotPink : Colors.Red);
				}
			}
			else
			{
				HighlightHex(hex, drawingSession, Colors.DarkGray);
			}
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