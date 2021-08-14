using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class InventoryIcon : Panel
{
	public Entity TargetEnt;
	public Panel healthBar;
	public Panel itemIcon;
	public Label Number;

	public InventoryIcon( int i, Panel parent )
	{
		Parent = parent;
		healthBar = Add.Panel( "health-bar" );
		itemIcon = Add.Panel(  );
		itemIcon.Style.Background = new PanelBackground
		{
			Texture = null,
			SizeX = 90,
			SizeY = 90,
		};
	}
	public override void Tick()
	{
		base.Tick();

		if ( TargetEnt is not Weapon targetWeapon )
		{
			itemIcon.Style.Display = DisplayMode.None;
			healthBar.SetClass( "active", false );

			var background = itemIcon.Style.Background.GetValueOrDefault();
			background.Texture = null;
			itemIcon.Style.Background = background;

			return;
		}

		itemIcon.Style.Display = DisplayMode.Flex;
		itemIcon.Style.Width = 90;
		itemIcon.Style.Height = 90;

		healthBar.SetClass( "active", targetWeapon.HasHealth );
		healthBar.Style.Height = (targetWeapon.Health / targetWeapon.MaxHealth) * 100 * 0.9f; // style height = 90px.

		if ( targetWeapon.Icon == null )
		{
			var background = itemIcon.Style.Background.GetValueOrDefault();
			background.Texture = null;
			itemIcon.Style.Background = background;
			return; }

		// make it so that when the targetWeapon.Icon changes, it sets Texture to null so that it auto updates
		if ( itemIcon.Style.Background.GetValueOrDefault().Texture == null )
		{
			var background = itemIcon.Style.Background.GetValueOrDefault();
			background.Texture = Texture.Load( $"/entity/{targetWeapon.Icon}", true );
			itemIcon.Style.Background = background;
		}
		
		//Log.Info( TargetEnt.EntityName );

	}
	public void Clear()
	{
		healthBar.SetClass( "active", false );
		itemIcon.Style.Display = DisplayMode.None;
		SetClass( "active", false );
	}
}
