using System;
using System.Threading.Tasks;

using Win2dUwpApp.Models;

namespace Win2dUwpApp.Managers
{
	public class GameManager
	{
		public InputManager Input { get; }
		public RenderManager Render { get; }

		public Board Board { get; }
		public Camera Camera { get; }

		public GameManager(InputManager inputManager, RenderManager renderManager)
		{
			Input = inputManager;
			Render = renderManager;

			Board = new Board(50, 60);
			Camera = new Camera(Render.GetSize());
		}

		public void Start()
		{
			Task.Run(() =>
			{
				int previousTime = 0;
				float frameTime = 60f / 1000f;
				int lastFrameTime = 0;

				while (true)
				{
					int deltaTime = Environment.TickCount - previousTime;
					previousTime = Environment.TickCount;

					Board.Update(this, deltaTime);
					Camera.Update(this, deltaTime);

					lastFrameTime += deltaTime;
					if (lastFrameTime > frameTime)
					{
						Render.Render(this);
						lastFrameTime = 0;
					}
				}
			});
		}
	}
}