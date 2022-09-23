using UnityEngine;

namespace Vertx.Debugging
{
	public static partial class Shapes
	{
		public readonly struct Text : IDrawable
		{
			public readonly Vector3 Position;
			public readonly object Value;
			public readonly Camera Camera;

			public Text(Vector3 position, object value, Camera camera = null)
			{
				Camera = camera;
				Position = position;
				Value = value;
			}

#if UNITY_EDITOR
			public void Draw(CommandBuilder commandBuilder, Color color, float duration)
			{
				if (!Application.isPlaying)
					duration = CommandBuilder.EditorUpdateDuration;
				commandBuilder.AppendText(this, color, duration);
			}
#endif
		}

		internal sealed class TextData
		{
			public Vector3 Position;
			public object Value;
			public Camera Camera;
		}
	}
}