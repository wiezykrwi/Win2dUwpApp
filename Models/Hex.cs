using System;
using System.Numerics;

namespace Win2dUwpApp.Models
{
	public class Hex
	{
		public Hex(Vector2 center, int size)
		{
			Center = center;
			Corners = new Vector2[6];

			for (int i = 0; i < 6; i++)
			{
				Corners[i] = GetCorner(center, size, i);
			}
		}
		
		public Vector2 Center { get; }
		public Vector2[] Corners { get; }

		public string Tag { get; set; }
		public bool IsSelected { get; set; }
		public bool Passable { get; set; }

		public Pawn Pawn { get; set; }

		private Vector2 GetCorner(Vector2 center, int size, int i)
		{
			var angleDeg = 60 * i - 30;
			var angleRad = Math.PI / 180 * angleDeg;

			return new Vector2(
				(float) (center.X + size * Math.Cos(angleRad)),
				(float) (center.Y + size * Math.Sin(angleRad)));
		}
	}
}