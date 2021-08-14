using Sandbox;
using System.Collections.Generic;
using System.Linq;

public partial class SpawnedElement
{
	private Entity spawnedEnt { get; set; }
	private Prop spawnedProp { get; set; }
	public SpawnedElement(Prop prop)
	{
		spawnedProp = prop;
	}
	public SpawnedElement( Entity entity)
	{
		spawnedEnt = entity;
	}
	public string Delete()
	{
		string ret = "";
		if ( spawnedEnt != null ) { ret = spawnedEnt.EntityName; spawnedEnt.Delete(); }
		else if ( spawnedProp != null ) { ret = spawnedProp.GetModelName(); spawnedProp.Delete(); }
		return ret;
	}
}
public partial class SpawnList
{
	private List<SpawnedElement> spawnedElements = new List<SpawnedElement>();
	public void AddElem( Prop prop)
	{
		spawnedElements.Add( new SpawnedElement( prop ) );
	}
	public void AddElem(Entity ent)
	{
		spawnedElements.Add( new SpawnedElement( ent ) );
	}
	public int Count()
	{
		return spawnedElements.Count;
	}
	public string RemoveLast()
	{
		if ( spawnedElements.Count == 0 ) return null;
		var lastElement = spawnedElements.Last();
		spawnedElements.Remove( lastElement );
		
		return lastElement.Delete();
	}
}
