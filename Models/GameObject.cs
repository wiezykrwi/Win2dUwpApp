using Win2dUwpApp.Managers;

namespace Win2dUwpApp.Models
{
	public abstract class GameObject
	{
		public abstract void Update(GameManager gameManager, int deltaTime);
	}
}