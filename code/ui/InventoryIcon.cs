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

	}
	public override void Tick()
	{
		base.Tick();

		if ( TargetEnt is not Weapon targetWeapon )
		{
			itemIcon.Style.Display = DisplayMode.None;
			healthBar.SetClass( "active", false );

			return;
		}

		if ( targetWeapon.Icon == null) return;

		itemIcon.Style.Background = new PanelBackground
		{
			Texture = Texture.Load( $"/entity/{targetWeapon.Icon}", true ),
			SizeX = 90,
			SizeY = 90,
		};

		itemIcon.Style.Display = DisplayMode.Flex;
		itemIcon.Style.Width = 90;
		itemIcon.Style.Height = 90;

		healthBar.SetClass( "active", targetWeapon.HasHealth );
		healthBar.Style.Height = (targetWeapon.Health / targetWeapon.MaxHealth) * 100 * 0.9f; // style height = 90px.

		//Log.Info( TargetEnt.EntityName );

	}
	public void Clear()
	{
		healthBar.SetClass( "active", false );
		itemIcon.Style.Display = DisplayMode.None;
		SetClass( "active", false );
	}
}
