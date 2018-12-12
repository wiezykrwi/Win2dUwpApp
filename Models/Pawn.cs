using Win2dUwpApp.Managers;

namespace Win2dUwpApp.Models
{
	public class Pawn : GameObject
	{
		public bool Selected { get; set; }
		public int MoveSpeed { get; set; }

		public override void Update(GameManager gameManager, int deltaTime)
		{
		}
	}
}