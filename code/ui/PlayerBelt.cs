﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
public partial class PlayerBelt : Panel
{
	readonly List<InventoryIcon> slots = new();

	public PlayerBelt()
	{
		for ( int i = 0; i < 6; i++ )
		{
			var icon = new InventoryIcon( i + 1, this );
			slots.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not MyPlayer LocalPlayer || LocalPlayer.Inventory ==null) return;

		for ( int i = 0; i < 6; i++ )
		{
			UpdateIcon( LocalPlayer.Inventory.GetSlot( i ), slots[i], i );
		}

	}
	private static void UpdateIcon( Entity ent, InventoryIcon inventoryIcon, int i )
	{
		if ( ent == null )
		{
			inventoryIcon.Clear();
			return;
		}

		inventoryIcon.TargetEnt = ent;
		inventoryIcon.SetClass( "active", ent.IsActiveChild() );
	}

	[Event( "buildinput" )]
	public void ProcessClientInput( InputBuilder input )
	{
		var player = Local.Pawn as MyPlayer;
		if ( player == null )
			return;

		var inventory = player.Inventory;
		if ( inventory == null )
			return;
		
		/*if ( player.ActiveChild is PhysGun physgun && physgun.BeamActive )
		{
			return;
		}*/

		if ( input.Pressed( InputButton.Slot1 ) ) SetActiveSlot( input, inventory, 0 );
		if ( input.Pressed( InputButton.Slot2 ) ) SetActiveSlot( input, inventory, 1 );
		if ( input.Pressed( InputButton.Slot3 ) ) SetActiveSlot( input, inventory, 2 );
		if ( input.Pressed( InputButton.Slot4 ) ) SetActiveSlot( input, inventory, 3 );
		if ( input.Pressed( InputButton.Slot5 ) ) SetActiveSlot( input, inventory, 4 );
		if ( input.Pressed( InputButton.Slot6 ) ) SetActiveSlot( input, inventory, 5 );

		if ( input.MouseWheel != 0 ) SwitchActiveSlot( input, inventory, -input.MouseWheel );
	}

	private static void SetActiveSlot( InputBuilder input, IBaseInventory inventory, int i )
	{
		var player = Local.Pawn;
		if ( player == null )
			return;

		if(inventory.GetActiveSlot() == i) // doesnt works for soem reasons
		{
			inventory.SetActiveSlot( -1, true );
			input.ActiveChild = null;
			return;
		}

		Log.Info( inventory.GetActiveSlot() + " : " + input.ActiveChild );
		var ent = inventory.GetSlot( i );
		if ( player.ActiveChild == ent )
			return;
		
		if ( ent == null )
			return;

		input.ActiveChild = ent;
	}

	private static void SwitchActiveSlot( InputBuilder input, IBaseInventory inventory, int idelta )
	{
		var count = inventory.Count();
		if ( count == 0 ) return;

		var slot = inventory.GetActiveSlot();
		var nextSlot = slot + idelta;

		while ( nextSlot < 0 ) nextSlot += count;
		while ( nextSlot >= count ) nextSlot -= count;

		SetActiveSlot( input, inventory, nextSlot );
	}
}
