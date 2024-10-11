using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using RangersSDK.CSLib.Foundation;
using RangersSDK.Hedgehog.Foundation;
using RangersSDK.Hedgehog.Game;

namespace RangersSDK.Reflection
{
    public class SpawnTypeAttribute : Attribute
    {
        public string SpawnType { get; private set; }

        public SpawnTypeAttribute(string spawnType)
        {
            SpawnType = spawnType;
        }
    }

    public class CategoryAttribute : Attribute
    {
        public string Category { get; private set; }

        public CategoryAttribute(string category)
        {
            Category = category;
        }
    }

    public class SpawnerDataAttribute : Attribute
    {
        public string SpawnerDataClass { get; private set; }

        public SpawnerDataAttribute(string spawnerDataClass)
        {
            SpawnerDataClass = spawnerDataClass;
        }
        public SpawnerDataAttribute(Type spawnerDataClass)
        {
            SpawnerDataClass = spawnerDataClass.Name;
        }
    }

    public static class GameObjectReflector
    {
        private static List<GameObjectClass.CreateFunction> instantiators = new List<GameObjectClass.CreateFunction>();
        public unsafe static void DiscoverGameObjects()
        {
            foreach (var obj in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly != Assembly.GetExecutingAssembly()).SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(GameObject))))
            {
                var spawnerData = (SpawnerDataAttribute)Attribute.GetCustomAttribute(obj, typeof(SpawnerDataAttribute));

                GameObjectClass.CreateFunction instantiator = allocator =>
                {
                    return ((GameObject)obj.GetConstructor(new Type[] { typeof(IAllocator) }).Invoke(new object[] { IAllocator.__GetOrCreateInstance(allocator, false, true) })).__Instance;
                };

                instantiators.Add(instantiator);

                var objClass = new GameObjectClass
                {
                    Name = obj.Name,
                    ScopedName = obj.Name,
                    ObjectSize = (ulong)sizeof(GameObject.__Internal),
                    Instantiator = instantiator,
                    SpawnerDataRflClass = spawnerData != null ? Singleton<RflClassNameRegistry>.instance.GetByName(spawnerData.SpawnerDataClass) : null,
                };

                Singleton<GameObjectSystem>.instance.GameObjectRegistry.AddObject(objClass);
            }
        }
    }
}
