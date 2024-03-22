# Stride.Modding
Usefull functions for runtime modding Stride

## Script loading
- [x] Loading DLLs at runtime
- [ ] Loading standalone scripts

## Model importing
- [ ] GLTF loading (will likely use the code from [here](https://github.com/ykafia/StrideGltf/blob/master/GltfImport/LoadGLTF.cs) or just use the built in lib from Stride once its cross platform)
- [ ] FBX loading (I may just wait for the work to be done on the Stride importer https://github.com/stride3d/stride/pull/2163)

## Misc
At this point any other moddable features can be a seperate project since the base will be able to come from Stride.Modding.

# Examples

## Library Loading

Create an interface as a contract for mods to follow

``` csharp
using Stride.Core;

public interface ILoadable
{
	public string Name { get; }
	public string Description { get; }

	public void Initialize(IServiceRegistry services);
}
```

Create a class to access the ModLoader
``` csharp
public class ModLoader : BaseLibraryLoader<ILoadable>
{
	// You can override some methods here to implement your own loaders.
}
```

finally you can create the instance how you want to enable loading mods at runtime. I usualy register everything in a Custom Game class and register it to the service registry from Stride.
``` csharp
public class CustomGame : Game
{
	protected override void BeginRun()
	{
		base.BeginRun();
		
		ModLoader _modLoader = new();
		Services.AddService(_modLoader);
	}
}
```

You can also register to events to be able to see when a mod is loaded and run a command. With the example above you can run `Initialize` whenever a mod is added.
``` csharp
private readonly ModLoader _modLoader

// subscribe somewhere
public Test(IServiceRegistry services)
{
	_modLoader = services.GetService<ModLoader>()
	_modLoader.LoadedMods.CollectionChanged += InitializeMod;
}

private void InitializeMod(object? sender, TrackingCollectionChangedEventArgs e)
{
	if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
	{
		var mod = (ILoadable)e.Item;
		mod.Initialize(_services);
	}
}
```
