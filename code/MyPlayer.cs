using Sandbox;
using System.Collections.Generic;

public class PlayerCam : Camera
{
	Vector3 lastPos;

	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Pos = pawn.EyePos;
		Rot = pawn.EyeRot;

		lastPos = Pos;
	}
	
	public override void Update()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		var eyePos = pawn.EyePos;
		if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
		{
			Pos = Vector3.Lerp( eyePos.WithZ( lastPos.z ), eyePos, 20.0f * Time.Delta );
		}
		else
		{
			Pos = eyePos;
		}

		Rot = pawn.EyeRot;

		FieldOfView = 90;

		Viewer = pawn;
		lastPos = Pos;
	}
}

partial class MyPlayer : Player
{
	[ConVar.ClientData( "tool_current" )]
	public static string UserToolCurrent { get; set; } = "boxgun";
	public WeaponData WeaponData { get; set; }
	private TimeSince timeSinceDropped;


	[Net, Predicted]
	public ICamera MainCamera { get; set; }
	[Net, Local]
	public PlayerMetabolism playerMetabolism { get; set; }
	public MyPlayer()
	{
		Inventory = new Inventory( this );

	}
	public override void Spawn()
	{
		MainCamera = new FirstPersonCamera();
		base.Spawn();
	}
	public override void Respawn()
	{

		playerMetabolism = new PlayerMetabolism();
		playerMetabolism.Reset();

		//WeaponData = Resource.FromPath<WeaponData>( "config/MyWeapon.weapon" );
		SetModel( "models/citizen/citizen.vmdl" );

		// Use WalkController for movement (you can make your own PlayerController for 100% control)
		//Controller = new PlayerWalkController();
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		
		// Use ThirdPersonCamera (you can make your own Camera for 100% control)
		MainCamera = new PlayerCam();
		Camera = MainCamera;
		//Camera = new ThirdPersonCamera();

		if ( DevController is NoclipController )
		{
			DevController = null;
		}

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		//Inventory.Add( new PhysGun(), true );
		//Inventory.Add( new GravGun() );
		//Inventory.Add( new Tool() );
		//Inventory.Add( new Pistol() );

		// currentTool = ConsoleSystem.GetValue( "tool_current" );
		base.Respawn();
	}
	public override PawnController GetActiveController()
	{
		//if ( VehicleController != null ) return VehicleController;
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}
	public ICamera GetActiveCamera()
	{
		//if ( VehicleCamera != null ) return VehicleCamera;

		return MainCamera;
	}
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}
		if ( LifeState != LifeState.Alive )
			return;
		var controller = GetActiveController();
		if ( controller != null)
		EnableSolidCollisions = !controller.HasTag( "noclip" );

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRot.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				timeSinceDropped = 0;
			}
		}
		if ( Input.Pressed( InputButton.View ) )
		{
			if ( MainCamera is not PlayerCam )
			{
				MainCamera = new PlayerCam();
				EnableHideInFirstPerson = true;
			}
			else
			{
				MainCamera = new ThirdPersonCamera();
				EnableHideInFirstPerson = false;
			}
		}

		Camera = GetActiveCamera();

		playerMetabolism.RunMetabolism(!IsClient);

		if ( IsClient )
		{
			/*
tooCold,
tooHot,
bleeding,
hungry,
dehydrated,
comfort,
wet,
drowning*/
			Vitals.notifiers.Find( x => x.notificationType == NotifierType.hungry ).updateValue( (int)playerMetabolism.Calories.Clamp( 1f, 39f ), playerMetabolism.Calories < 40f );
			Vitals.notifiers.Find( x => x.notificationType == NotifierType.dehydrated ).updateValue( (int)playerMetabolism.Hydration.Clamp( 1f, 39f ), playerMetabolism.Hydration < 40f );
			Vitals.notifiers.Find( x => x.notificationType == NotifierType.wet ).updateValue( (int)(WaterLevel.Fraction * 100f), WaterLevel.Fraction > 0 );

		}

	}

	private DamageInfo lastDamage;

	public override void OnKilled()
	{
		base.OnKilled();
		
		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );


		MainCamera = new SpectateRagdollCamera();
		Camera = MainCamera;
		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		Inventory.DropActive();
		Inventory.DeleteContents();
	}
	public override void TakeDamage( DamageInfo info )
	{
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage *= 10.0f;
		}

		lastDamage = info;

		TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

		base.TakeDamage( info );
	}

	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
	}

	[ServerCmd( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( !slot.ClassInfo.IsNamed( entName ) )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}
}
