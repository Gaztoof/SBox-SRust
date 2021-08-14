using Sandbox;
using System;

public partial class PlayerMetabolism : NetworkComponent
{
	[ConVar.ClientData( "sv_metabolism_speed" )] // MAKE THIS REPLICATED!
	public float metabolismSpeed { get; set; } = 1f;
	//private Label Hunger;
	[Net]
	public float Calories { get; set; } = 500;
	[Net]
	public float Hydration { get; set; } = 250;
	[Net]
	public float Bleeding { get; set; } = 0;
	/*public static float MoveTowards( float current, float target, float maxDelta )
	{
		bool flag = Mathf.Abs( target - current ) <= maxDelta;
		float result;
		if ( flag )
		{
			result = target;
		}
		else
		{
			result = current + Mathf.Sign( target - current ) * maxDelta;
		}
		return result;
	}*/

	public float MoveTowards( float current, float fTarget, float fRate, float max )
	{
		if ( fRate == 0f )
		{
			return 0f;
		}
		float result;
		if ( Math.Abs( fTarget - current ) <= fRate )
		{
			result = fTarget;
		}
		else
		{
			result = current + Math.Sign( fTarget - current ) * fRate;
		}

		return Math.Clamp( result, 0f, max);
	}

	public PlayerMetabolism()
	{
		Reset();
	}
	public void Reset()
	{
		Calories = Rand.Float( 0f, 100f );
		Hydration= Rand.Float( 0f, 200f );
		Bleeding= 0;

	}
	private int serverTickCount;
	public void RunMetabolism(bool server)
	{
		if ( server )
		{
			serverTickCount++;

			Calories = MoveTowards( Calories, 0f, 0.0625f * 0.008333334f * metabolismSpeed, 250 );// 		this.calories.MoveTowards(0f, delta * 0.0166666675f);
			Hydration = MoveTowards( Hydration, 0f, 0.0166666675f * 0.0166666675f * metabolismSpeed, 250 ); // this.hydration.MoveTowards(0f, delta * 0.008333334f
			//testin shows 60-62 ticks per second, so i round to 60srvticks/s

		}
	}
}
