using System;
using System.Numerics;

using Windows.Foundation;

using Win2dUwpApp.Managers;

namespace Win2dUwpApp.Models
{
	public class Camera : GameObject
	{
		private bool _isDragging;
		private Vector2 _previousPoint;

		public Camera(Size size)
		{
			Size = size;
		}
		
		public Vector2 Offset { get; private set; }
		public Size Size { get; }

		public override void Update(GameManager gameManager, int deltaTime)
		{
			if (gameManager.Input.IsLeftButtonPressed)
			{
				var position = gameManager.Input.PointerPosition.ToVector2();
				if (_isDragging)
				{
					var newX = Math.Max(Math.Min((float) (gameManager.Board.Width - Size.Width), Offset.X - (position.X - _previousPoint.X)), 0.0f);
					var newY = Math.Max(Math.Min((float) (gameManager.Board.Heigth - Size.Height), Offset.Y - (position.Y - _previousPoint.Y)), 0.0f);
					Offset = new Vector2(newX, newY);

					_previousPoint = position;
				}
				else
				{
					_isDragging = true;
					_previousPoint = position;
				}
			}
			else
			{
				_isDragging = false;
			}
		}
	}
}