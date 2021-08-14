using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Compass : Panel
{
	public Panel compass;

	public Compass()
	{
		compass = Add.Panel( "ploom" );
		
	}

	public override void Tick()
	{
		base.Tick();
		if ( compass == null || Local.Pawn is not MyPlayer LocalPlayer || LocalPlayer.GetActiveController() == null ) return;

		int wishedWidth = 800;
		compass.Style.Width = wishedWidth;

		int maxPercent = 360; // thats the style's background-size

		maxPercent /= 2;
		//Log.Info( "Max Percent: " + maxPercent + " Yaw: " + LocalPlayer.GetActiveController().EyeRot.Yaw() + " Width: " + wishedWidth );
		float targetCompassAngle = ((LocalPlayer.GetActiveController().EyeRot.Yaw()) / 180f) * -(maxPercent);
		
		if ( targetCompassAngle < 0f ) targetCompassAngle += maxPercent*2f;

		//Log.Info( targetCompassAngle );
		compass.Style.Set( "background-position", $"-{(targetCompassAngle + 130f).ToString()}% 0%" );

		// 1600 = 256 800 = 512
	}
}
