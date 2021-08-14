using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
public enum NotifierType
{
	tooCold,
	tooHot,
	bleeding,
	hungry,
	dehydrated,
	comfort,
	wet,
	drowning
}
public enum NotificationType
{
	pickedUp,
}
public class Notification
{
	public Panel back;
	public Label strLabel;
	public Label rightLabel;
	public bool active { get; set; } = false;
	public NotificationType notificationType { get; set; }
	public Notification( Panel panel, NotificationType _notificationType, int value, string name = null)
	{
		back = panel.Add.Panel( "notificationBack" );
		notificationType = _notificationType;
		var notifIcon = back.Add.Panel( );
		switch (notificationType)
		{
			case NotificationType.pickedUp:
				notifIcon.Style.Background = new PanelBackground
				{
					Texture = Texture.Load( "/icon/pickup.png", true ),
					SizeX = 35,
					SizeY = 35,
				};


				break;
		}
		strLabel = back.Add.Label( name, "notificationLeftText" );
		rightLabel = back.Add.Label( $"+{value}", "notificationRightText" );

		updateValue( value, false, name );

		notifIcon.Style.Width = 35;
		notifIcon.Style.Height = 35;
		notifIcon.Style.Opacity = 0.6f;
		Disable();
	}
	public Notification Enable()
	{
		back.SetClass( "active", true );
		return this;
	}
	public Notification Disable()
	{
		back.SetClass( "active", false );
		return this;
	}
	public void updateValue( int value, bool enabled, string strValue = null )
	{
		if ( strValue != null ) strLabel.SetText( strValue );
		switch ( notificationType )
		{

			case NotificationType.pickedUp:
				rightLabel.SetText( $"x1{value}" );
				back.SetClass( "pickedUpItem", true );
				break;
		}
		if ( enabled ) Enable(); else { Disable(); return; }

	}

}
public class Notifier
{
	public Panel panel;
	public Panel back;
	public Label strLabel;
	public Label rightLabel;
	public bool active { get; set; } = false;
	public NotifierType notificationType { get; set; }
	public Notifier( Panel _panel, NotifierType _notifierType, int value, string name = null )
	{
		panel = _panel;
		notificationType = _notifierType;
		back = panel.Add.Panel( "notifierBack" );

		var notifIcon = back.Add.Panel();

		switch ( notificationType )
		{

			case NotifierType.tooCold:
				break;
			case NotifierType.tooHot:
				break;
			case NotifierType.bleeding:
				break;
			case NotifierType.hungry:
				notifIcon.Style.Background = new PanelBackground
				{
					Texture = Texture.Load( "/icon/fork_and_spoon2.png", true ),
					SizeX = 24,
					SizeY = 24,
					Repeat = BackgroundRepeat.NoRepeat,
				};
				notifIcon.Style.Left = 7;
				notifIcon.Style.Top = 7;

				break;
			case NotifierType.dehydrated:
				notifIcon.Style.Background = new PanelBackground
				{
					Texture = Texture.Load( "/icon/cup_water2.png", true ),
					SizeX = 24,
					SizeY = 24,
					Repeat = BackgroundRepeat.NoRepeat,
				};
				notifIcon.Style.Left = 7;
				notifIcon.Style.Top = 7;

				break;
			case NotifierType.comfort:
				break;
			case NotifierType.wet:
				notifIcon.Style.Background = new PanelBackground
				{
					Texture = Texture.Load( "/icon/embrella.png", true ),
					SizeX = 20,
					SizeY = 20,
					Repeat = BackgroundRepeat.NoRepeat,
				};
				notifIcon.Style.Left = 10;
				notifIcon.Style.Top = 7;
				
				
				break;
			case NotifierType.drowning:
				break;
		}

		strLabel = back.Add.Label( "", "notificationLeftText" );
		rightLabel = back.Add.Label( "", "notificationRightText" );


		updateValue( value, false, name );

		notifIcon.Style.Width = 35;
		notifIcon.Style.Height = 35;
		notifIcon.Style.Opacity = 0.8f;
	}
	public Notifier SetState( bool b )
	{
		if ( b ) Enable(); else Disable();
		return this;
	}
	public void Enable()
	{
		back.SetClass( "active", true );
		active = true;
	}
	public void Disable()
	{
		back.SetClass( "active", false );
		active = false;
	}
	public void updateValue( int value, bool enabled, string strValue = null)
	{

		if(strValue != null) strLabel.SetText(strValue);
		switch ( notificationType )
		{

			case NotifierType.tooCold:
				rightLabel.SetText( $"{value}°c" );
				back.SetClass( "redNotif", true );
				back.SetClass( "comfort", false );
				break;
			case NotifierType.tooHot:
				rightLabel.SetText( $"{value}°c" );
				back.SetClass( "redNotif", true );
				back.SetClass( "comfort", false );
				break;
			case NotifierType.bleeding:
				rightLabel.SetText( $"{value}" );
				back.SetClass( "redNotif", true );
				back.SetClass( "comfort", false );
				break;
			case NotifierType.hungry:
				rightLabel.SetText($"{value}");
				back.SetClass( "redNotif", true );  // hungry = just calories amount clamped at 1/39, starts at 39
				back.SetClass( "comfort", false );
				break;
			case NotifierType.dehydrated:
				rightLabel.SetText($"{value}");
				back.SetClass( "redNotif", true );  // dehydrated = just thirst amount clamped at 1/39, starts at 39
				back.SetClass( "comfort", false );
				break;
			case NotifierType.comfort:
				rightLabel.SetText( $"{value}%" );
				back.SetClass( "redNotif", false );
				back.SetClass( "comfort", true );
				break;
			case NotifierType.wet:
				rightLabel.SetText( $"{value}%" );
				back.SetClass( "redNotif", true );
				back.SetClass( "comfort", false );
				break;
			case NotifierType.drowning:
				rightLabel.SetText($"{value}%");
				back.SetClass( "redNotif", true ); 
				back.SetClass( "comfort", false );
				break;
		}
		if ( enabled ) Enable(); else { Disable(); return; }

	}
}

public partial class Vitals : Panel
{
	private Label Hunger;
	private Label Thirst;
	private Label Health;
	private Panel HungerBar;
	private Panel ThirstBar;
	private Panel HealthBar;
	public static List<Notification> notifications { get; set; }
	public static List<Notifier> notifiers { get; set; }

	public Vitals()
	{

		Panel hungerBackBack = Add.Panel( "vitalBack" );
		var hungerIcon = hungerBackBack.Add.Panel(  );
		hungerIcon.Style.Background = new PanelBackground
		{
			Texture = Texture.Load( "/icon/fork_and_spoon.png", true ),
			SizeX = 24,
			SizeY = 24,

		};
		hungerIcon.Style.Opacity = 0.3f;
		hungerIcon.Style.Width = 24;
		hungerIcon.Style.Height = 24;
		HungerBar = hungerBackBack.Add.Panel( "hungerBar" );
		Hunger = HungerBar.Add.Label( "0", "vitalText" );

		Panel thirstBarBack = Add.Panel( "vitalBack" );
		var thirstIcon = thirstBarBack.Add.Panel( "thirstIcon" );
		thirstIcon.Style.Background = new PanelBackground
		{
			Texture = Texture.Load( "/icon/cup_water.png", true ),
			SizeX = 24,
			SizeY = 24,
		};

		thirstIcon.Style.Opacity = 0.3f;
		thirstIcon.Style.Width = 24;
		thirstIcon.Style.Height = 24;
		ThirstBar = thirstBarBack.Add.Panel( "thirstBar" );
		Thirst = ThirstBar.Add.Label( "0", "vitalText" );

		Panel healthBack = Add.Panel( "vitalBack" );
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
		HealthBar = healthBack.Add.Panel( "healthBar" );
		Health = HealthBar.Add.Label( "0", "vitalText" );

		notifiers = new List<Notifier>();
		notifications = new List<Notification>();

		notifiers.Add( new Notifier( this, NotifierType.tooCold, 0, "TOO COLD" ) );
		notifiers.Add( new Notifier( this, NotifierType.tooHot, 0, "TOO HOT" ) );
		notifiers.Add( new Notifier( this, NotifierType.bleeding, 0, "BLEEDING" ) );
		notifiers.Add( new Notifier( this, NotifierType.hungry, 0, "STARVING" ) );
		notifiers.Add( new Notifier( this, NotifierType.dehydrated, 0, "DEHYDRATED" ) );
		notifiers.Add( new Notifier( this, NotifierType.comfort, 0, "COMFORT" ) );
		notifiers.Add( new Notifier( this, NotifierType.wet, 0, "WET" ) );
		notifiers.Add( new Notifier( this, NotifierType.drowning, 0, "DROWNING" ) );

		notifications.Add( new Notification( this, NotificationType.pickedUp, 0, "" ) );
	}
	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not MyPlayer LocalPlayer || LocalPlayer.playerMetabolism == null ) return;

		Health.Text = $"{LocalPlayer.Health.CeilToInt()}";
		HealthBar.Style.Dirty();
		HealthBar.Style.Width = Length.Percent( LocalPlayer.Health.Clamp( 0f, 100f ) ).Value.Value * 2.465f; // cuz width is 246.5px, and max percent is 100, so 100*2.465=246.5
		
		Thirst.Text = $"{LocalPlayer.playerMetabolism.Hydration.Clamp( 0f, 250f ).CeilToInt()}";
		ThirstBar.Style.Dirty();
		ThirstBar.Style.Width = Length.Percent( LocalPlayer.playerMetabolism.Hydration.Clamp( 0f, 250f ) / 2.5f ).Value.Value * 2.465f;
		
		Hunger.Text = $"{LocalPlayer.playerMetabolism.Calories.Clamp( 0f, 500f ).CeilToInt()}";
		HungerBar.Style.Dirty();
		HungerBar.Style.Width = Length.Percent( LocalPlayer.playerMetabolism.Calories.Clamp(0f,500f) / 5.0f ).Value.Value * 2.465f;

		/*Panel notificationBack = Add.Panel( "notificationBack" );
		var notifIcon = notificationBack.Add.Panel( "healthIcon" );
		notifIcon.Style.Background = new PanelBackground
		{
			Texture = Texture.Load( "/icon/pickup.png", true ),
			SizeX = 35,
			SizeY = 35,
		};
		//notifIcon.Style.Opacity = 0.3f;
		notifIcon.Style.Width = 35;
		notifIcon.Style.Height = 35;
		notificationBack.Add.Label( "TOO COLD", "notificationLeftText" );
		notificationBack.Add.Label( "+1", "notificationRightText" );

		if ( true )
		{
			notificationBack.SetClass( "tooCold", true );
			notificationBack.SetClass( "comfort", false );
		}*/

	}
}
