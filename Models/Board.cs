using System;
using System.Collections.Generic;
using System.Numerics;

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

			Hexes = new Dictionary<Coordinate, Hex>();
			HexArray = new Hex[rows * cols];
			Width = (int) (_hexWidth * _cols + _hexWidth * 2);
			Heigth = (int) (_hexHeigth * _rows + _hexHeigth * 2);
	        
			bool evenRow = true;
			float heigthMultiplier = 0.5f;
			
			for (int i = 0; i < rows; i++)
			{
				float widthMultiplier = evenRow ? 0.5f : 1.0f;

				for (int j = 0; j < cols; j++)
				{
					var center = new Vector2(widthMultiplier * _hexWidth, heigthMultiplier * _hexHeigth);
					var coordinate = GetCoordinate(center);
					var hex = new Hex(center, HexSize)
					{
						Tag = coordinate.ToString()
					};

					Hexes.Add(coordinate, hex);
					HexArray[j + i * cols] = hex;

					widthMultiplier += 1.0f;
				}

				evenRow = !evenRow;
				heigthMultiplier += 0.75f;
			}
		}

		private Coordinate GetCoordinate(Vector2 point)
		{
			var pointY = point.Y + HexSize / 2;
			var q = (float) (Math.Sqrt(3f) / 3f * point.X - 1f / 3f * pointY) / HexSize;
			var r = (2f / 3f * pointY) / HexSize;

			return HexRound(q, r, -q -r);
			//return HexRound(q, -q -r, r);
		}

		private Coordinate HexRound(float x, float y, float z)
		{
			var rx = (int) Math.Round(x);
			var ry = (int) Math.Round(y);
			var rz = (int) Math.Round(z);

			var xDiff = Math.Abs(rx - x);
			var yDiff = Math.Abs(ry - y);
			var zDiff = Math.Abs(rz - z);

			if (xDiff > yDiff && xDiff > zDiff)
			{
				rx = -ry - rz;
			}
			else if (yDiff > zDiff)
			{
				ry = -rx - rz;
			}
			else
			{
				rz = -rx - ry;
			}

			return new Coordinate(rx, ry);
		}

		public Dictionary<Coordinate, Hex> Hexes { get; }
		public Hex[] HexArray { get; }

		public int Width { get; }
		public int Heigth { get; }

		public Coordinate Coordinate { get; set; }

		public Hex[] GetVisibleHexes(Camera camera)
		{
			var visibleHexes = new List<Hex>();
			var minimumWidth = _hexWidth / 2.0f - 1.0f;
			var minimumHeigth = _hexHeigth / 2.0f - 1.0f;
			
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					var hex = HexArray[j + i * _cols];
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
		
		public override void Update(GameManager gameManager, int deltaTime)
		{
			var realPoint = gameManager.Input.PointerPosition.ToVector2() + gameManager.Camera.Offset;
			Coordinate = GetCoordinate(realPoint);
		}
	}
}