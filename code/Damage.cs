using Sandbox;

public static class GameEvent
{
	public const string Custom = "custom";
	public class CustomAttribute : EventAttribute
	{
		public CustomAttribute() : base( Custom ) { }
	}
}

