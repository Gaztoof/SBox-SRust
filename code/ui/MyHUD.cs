using Sandbox;
using Sandbox.UI;

[Library]
public partial class MyHUD : HudEntity<RootPanel>
{
	public MyHUD()
	{
		if ( !IsClient ) return;

		RootPanel.StyleSheet.Load( "/ui/SandboxHud.scss" );
		RootPanel.StyleSheet.Load( "/ui/Vitals.scss" );
		RootPanel.StyleSheet.Load( "/ui/Notifications.scss" );

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<CrosshairCanvas>();
		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<Compass>();
		RootPanel.AddChild<PlayerBelt>();
		RootPanel.AddChild<Notifications>();

		RootPanel.AddChild<SpawnMenu>();

	}
}
