using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using Windows.Foundation;

using Win2dUwpApp.Managers;

namespace Win2dUwpApp.Models
{
	public class Board : GameObject
	{
		private const int HexSize = 100;

		private readonly int _rows;
		private readonly int _cols;
		private readonly float _hexWidth;
		private readonly float _hexHeigth;

		public Board(int rows, int cols)
		{
			_rows = rows;
			_cols = cols;

			_hexWidth = (float) (Math.Sqrt(3) * HexSize);
			_hexHeigth = 2 * HexSize;

			Hexes = new Hex[rows * cols];
			Width = (int) (_hexWidth * _cols + _hexWidth * 2);
			Heigth = (int) (_hexHeigth * _rows + _hexHeigth * 2);
	        
			bool evenRow = false;
			float heigthMultiplier = 0.75f;
			
			for (int i = 0; i < rows; i++)
			{
				float widthMultiplier = evenRow ? 0.5f : 1.0f;

				for (int j = 0; j < cols; j++)
				{
					Hexes[j + i * cols] = new Hex(new Vector2(widthMultiplier * _hexWidth, heigthMultiplier * _hexHeigth), HexSize)
					{
						Tag = $"{j}, {i}"
					};

					widthMultiplier += 1.0f;
				}

				evenRow = !evenRow;
				heigthMultiplier += 0.75f;
			}
		}

		public Hex[] Hexes { get; }

		public int Width { get; }
		public int Heigth { get; }

		public Hex[] GetVisibleHexes(Camera camera)
		{
			var visibleHexes = new List<Hex>();
			var minimumWidth = _hexWidth / 2.0f - 1.0f;
			var minimumHeigth = _hexHeigth / 2.0f - 1.0f;

			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					var hex = Hexes[j + i * _cols];
					if (hex.Center.X < camera.Offset.X - minimumWidth ||
					    hex.Center.X > camera.Offset.X + camera.Size.Width + minimumWidth ||
					    hex.Center.Y < camera.Offset.Y - minimumHeigth ||
					    hex.Center.Y > camera.Offset.Y + camera.Size.Height + minimumHeigth)
					{
						continue;
					}

					visibleHexes.Add(hex);
				}
			}

			return visibleHexes.ToArray();
		}

		private Point _clickPoint;
		private bool _dragging;

		public override void Update(GameManager gameManager, int deltaTime)
		{
			if (gameManager.Input.IsLeftButtonPressed)
			{
				if (_dragging)
				{
					return;
				}

				_clickPoint = gameManager.Input.PointerPosition;
				_dragging = true;
			}
			else if (_dragging)
			{
				_dragging = false;
				var delta = _clickPoint.X - gameManager.Input.PointerPosition.X + _clickPoint.Y - gameManager.Input.PointerPosition.Y;
				if (delta > 4.0d)
				{
					return;
				}

				var x = _clickPoint.X + gameManager.Camera.Offset.X;
				var y = _clickPoint.Y + gameManager.Camera.Offset.Y;

				var q = (int) (x / _hexWidth) - 1;
				var r = (int) (y / _hexHeigth) - 1;
				
				Hexes[q + r * _cols].IsSelected = true;
			}
		}
	}
}