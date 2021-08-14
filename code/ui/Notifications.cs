using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class Notifications : Panel
{
	//private Label Hunger;
	public Notifications()
	{
		/*Panel healthBack = Add.Panel( "notificationBack" );
		var healthIcon = healthBack.Add.Panel( "healthIcon" );
		healthIcon.Style.Background = new PanelBackground
		{
			Texture = Texture.Load( "/icon/health.png", true ),
			SizeX = 24,
			SizeY = 24,
		};
		healthIcon.Style.Opacity = 0.3f;
		healthIcon.Style.Width = 24;
		healthIcon.Style.Height = 24;
		var Health = healthIcon.Add.Label( "0", "vitalText" );*/

	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not MyPlayer LocalPlayer ) return;

		//Health.Text = $"{LocalPlayer.Health.CeilToInt()}";

	}
}
