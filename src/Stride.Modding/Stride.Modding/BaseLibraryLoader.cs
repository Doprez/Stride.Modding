using McMaster.NETCore.Plugins;
using Stride.Core.Collections;

namespace Stride.Modding;
public abstract class BaseLibraryLoader<T>
{
	/// <summary>
	/// Keeps track of all loaded mods.
	/// </summary>
	public TrackingHashSet<T> LoadedMods { get; private set; } = [];

	/// <summary>
	/// Tries to load any DLL files in the specified folder
	/// </summary>
	/// <param name="baseModFolder"></param>
	public virtual void LoadMods(string baseModFolder)
	{
		var loaders = new List<PluginLoader>();

		// create plugin loaders
		foreach (var dir in Directory.GetDirectories(baseModFolder))
		{
			var dirName = Path.GetFileName(dir);
			var pluginDll = Path.Combine(dir, dirName + ".dll");
			if (File.Exists(pluginDll))
			{
				var loader = PluginLoader.CreateFromAssemblyFile(
				pluginDll,
				sharedTypes: [typeof(T)]);
				loaders.Add(loader);
			}
		}

		// Create an instance of plugin types
		foreach (var loader in loaders)
		{
			try
			{
				foreach (var pluginType in loader
					.LoadDefaultAssembly()
					.GetTypes()
					.Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
				{
					// This assumes the implementation of ILoadable has a parameterless constructor
					T plugin = (T)Activator.CreateInstance(pluginType);
					LoadedMods.Add(plugin);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error loading plugin: {e.Message}");
			}
		}
	}

	/// <summary>
	/// Loads a specific DLL file
	/// </summary>
	/// <param name="path"></param>
	public virtual void LoadMod(string path)
	{
		var loaders = new List<PluginLoader>();

		if (File.Exists(path))
		{
			var loader = PluginLoader.CreateFromAssemblyFile(
			path,
			sharedTypes: [typeof(T)]);
			loaders.Add(loader);
		}

		// Create an instance of plugin types
		foreach (var loader in loaders)
		{
			try
			{
				foreach (var pluginType in loader
				.LoadDefaultAssembly()
				.GetTypes()
				.Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
				{
					// This assumes the implementation of ILoadable has a parameterless constructor
					T plugin = (T)Activator.CreateInstance(pluginType);
					LoadedMods.Add(plugin);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error loading plugin: {e.Message}");
			}
		}
	}
}
