using System.Collections;

namespace TerrainComposer2
{
	public class EditorCoroutine
	{
		public bool pause;

		private readonly IEnumerator routine;

		private EditorCoroutine(IEnumerator _routine)
		{
			routine = _routine;
		}

		public static EditorCoroutine Start(IEnumerator _routine)
		{
			EditorCoroutine editorCoroutine = new EditorCoroutine(_routine);
			editorCoroutine.Start();
			return editorCoroutine;
		}

		private void Start()
		{
		}

		public void Stop()
		{
		}

		private void Update()
		{
			if (!pause && !routine.MoveNext())
			{
				Stop();
			}
		}
	}
}
