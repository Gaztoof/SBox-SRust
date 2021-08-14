using Sandbox;

[Library( "weapon" )]
public class WeaponData : Asset
{
	// Data from weapon.asset
	public string Name { get; set; } = "Weapon Name";
	public string Description { get; set; } = "This is my weapon.";
	public float Damage { get; set; } = 5.0f;
}
