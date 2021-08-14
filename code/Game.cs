using Sandbox;
using System.Linq;
using System;
using System.Collections.Generic;


[Library( "srust", Title = "SRust" )]
public partial class MyGame : Sandbox.Game
{
	public static Dictionary<Client, SpawnList> spawnLists = new Dictionary<Client, SpawnList>();
	public MyHUD MyHUD;
	public MyGame()
	{
		if ( IsClient ) MyHUD = new MyHUD();
	}
	[Event.Hotload]
	public void HotloadUpdate()
	{
		if ( !IsClient ) return;
		MyHUD?.Delete();
		MyHUD = new MyHUD();
	}
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn and assign it to the client.
		var player = new MyPlayer();
		client.Pawn = player;

		player.Respawn();
		spawnLists.Add( client, new SpawnList() );
	}
	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client,reason );
		var spawnList = MyGame.spawnLists.Where( x => x.Key == client );
		if ( spawnList.Count() < 1 )
			return;
		while ( spawnList.First().Value.Count() > 0 ) spawnList.First().Value.RemoveLast();
	}
	

	[ServerCmd( "setrank" )]
	public static void SetRank(string player, string rankName)
	{
		Log.Info( $"Attempting to give \"{player}\" the \"{rankName}\" rank!" );
		foreach ( var ply in All.OfType<MyPlayer>() )
			Log.Info( "\"" + ply.GetClientOwner().Name + "\"" );
	}

	[ServerCmd( "noclip" )]
	public static void Noclip(  )
	{
		Game.Current?.DoPlayerNoclip( ConsoleSystem.Caller );
	}

	[ServerCmd( "teleport" )]
	public static void Teleport( string playerName, string teleportTo = null )
	{
		if ( ConsoleSystem.Caller.Pawn == null ) return;
		bool tpToPlayer = teleportTo != null;

		var tpToEnt = ConsoleSystem.Caller.Pawn;
		if(teleportTo != null)
		{
			tpToEnt = All.OfType<MyPlayer>().Where(x => x.GetClientOwner().Name.ToLower().Contains(teleportTo)).FirstOrDefault();
		}


		var ply = All.OfType<MyPlayer>().Where( x => x.GetClientOwner().Name.ToLower().Contains( playerName ) ).FirstOrDefault();
		if ( ply == null || tpToEnt == null )
		{
			Log.Info( "Couldn't find this player!" );
			return;
		}
		if( ply == tpToEnt)
		{
			Log.Info( "You can not teleport to yourself!" );
			return;
		}

		Log.Info( $"Teleported \"{ply.GetClientOwner().Name}\" to \"{tpToEnt.GetClientOwner().Name}\" !" );

		if ( tpToPlayer )
		{
			ply.Position = tpToEnt.Position;
			return;
		}
		var ray = Trace.Ray( tpToEnt.EyePos, tpToEnt.EyePos + tpToEnt.EyeRot.Forward * 5000 ).UseHitboxes().Ignore( tpToEnt ).Run();
		ply.Position = ray.EndPos;
	}
	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null || owner.Health <= 0 )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 1000 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var ent = new Prop();
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );
		ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;

		spawnLists.First().Value.AddElem( ent );
	}
	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		spawnLists.First().Value.AddElem( ent );

	}

	[ServerCmd( "undo" )]
	public static void Undo(  )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null || owner.Health <= 0 )
			return;

		var localPly = (Local.Pawn as MyPlayer);
		var foundList = MyGame.spawnLists.Where( x => (x.Key as MyPlayer) == localPly );
		if ( foundList.Count() < 1 )
			return;
		string removed = foundList.First().Value.RemoveLast();
		if(removed != null)
		ShowUndo( removed );
	}


	[ServerCmd( "sethealth" )]
	public static void SetHealth( string playerName, int health )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var ply = All.OfType<MyPlayer>().Where( x => x.GetClientOwner().Name.ToLower().Contains( playerName ) ).FirstOrDefault();
		if ( ply == null )
		{
			Log.Info( "Couldn't find this player!" );
			return;
		}
		ply.Health = health;
	}
	[ServerCmd( "setthirst" )]
	public static void SetThirst( string playerName, int thirst )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var ply = All.OfType<MyPlayer>().Where( x => x.GetClientOwner().Name.ToLower().Contains( playerName ) ).FirstOrDefault();
		if ( ply == null )
		{
			Log.Info( "Couldn't find this player!" );
			return;
		}
		ply.playerMetabolism.Hydration = thirst;
	}

	[ServerCmd( "sethunger" )]
	public static void SetHunger( string playerName, int hunger )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var ply = All.OfType<MyPlayer>().Where( x => x.GetClientOwner().Name.ToLower().Contains( playerName ) ).FirstOrDefault();
		if ( ply == null )
		{
			Log.Info( "Couldn't find this player!" );
			return;
		}

		ply.playerMetabolism.Calories = hunger;
	}
	[ClientRpc]
	public static void ShowUndo( string message )
	{
		Sound.FromScreen( "undo" );
		// message = either model name, or entname
	}

	//public override bool HasPermission( string mode )
	//{
	//	if ( mode == "noclip" ) return true;
	//	if ( mode == "devcam" ) return true;
	//	if ( mode == "suicide" ) return true;
	//
	//	return base.HasPermission( mode );
	//}

}
