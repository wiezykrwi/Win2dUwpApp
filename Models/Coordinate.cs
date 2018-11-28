namespace Win2dUwpApp.Models
{
	public struct Coordinate
	{
		public Coordinate(int q, int r)
		{
			Q = q;
			R = r;
		}

		public int Q { get; }
		public int R { get; }

		public override string ToString()
		{
			return $"{Q}, {R}";
		}
	}
}