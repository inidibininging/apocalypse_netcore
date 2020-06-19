using Apocalypse.Any.Core.Behaviour;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core
{
    /// <summary>
    /// This class describes all the main common functionality and properties that describe a game object.
    /// In this little library a game object is a sort of object that can contain a tree structure of objects.
    /// This means: A game object in this library has the possibility of holding other game objects that have the same functionality.
    ///
    /// The reason I did this was a sort of flexibility purpouse. I'm certainly not that deep in game programming but
    /// I'd guess that this pattern would be exposed in other engines with a component design pattern
    ///
    /// Moreover I introduced all the base loop methods that a game object needs in order to be processed by the game.
    /// e.g Update,LoadContent etc.
    /// </summary>
    public abstract class GameObject : IGameObjectDictionary//, IMapLocatable
    {
        public GameObject()
        {
        }

        #region IGameObject

        /// <summary>
        /// This method loads the content of every object in this object with the content manager.
        /// </summary>
        /// <param name="manager"></param>
        public virtual void LoadContent(ContentManager manager)
        {
            if (_objects == null)
                return;
            foreach (var obj in this)
                obj.Value.LoadContent(manager);
        }

        /// <summary>
        /// This method unloads the content of every object in this object with the content manager.
        /// </summary>
        public virtual void UnloadContent()
        {
            Clear();
        }

        /// <summary>
        /// This method should be used on the initialization of the object
        /// </summary>
        public virtual void Initialize()
        {
            Add(new CleanDeletedGameObjectsBehaviour(this));
            //Do nothing
        }

        /// <summary>
        /// This method is used as a base loop for updating all the data of this object. No drawing stuff here!
        /// </summary>
        /// <param name="time"></param>
        public abstract void Update(GameTime time);

        private void CheckIfGameObjectsAreLoaded()
        {
            if (_objects == null)
            {
                _objects = new Dictionary<string, IGameObject>();
            }
        }

        #endregion IGameObject

        #region GameObjectList Implementation

        private Dictionary<string, IGameObject> _objects;

        /// <summary>
        /// This is the list of objects that are inside this game object. The dictionary is lazy loaded
        /// </summary>
        private Dictionary<string, IGameObject> Objects
        {
            get
            {
                CheckIfGameObjectsAreLoaded();
                return _objects;
            }
            set { _objects = value; }
        }

        public ICollection<string> Keys => Objects.Keys;

        public ICollection<IGameObject> Values => Objects.Values;

        public int Count => _objects == null ? 0 : Objects.Count;

        public bool IsReadOnly => false;

        IGameObject IDictionary<string, IGameObject>.this[string key]
        {
            get
            {
                return Objects[key];
            }

            set
            {
                Objects[key] = value;
            }
        }

        public virtual bool ContainsKey(string key)
        {
            return Objects.ContainsKey(key);
        }

        /// <summary>
        /// Adds a gameobject here
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(string key, IGameObject value)
        {
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"{key} added");
            //Console.ForegroundColor = ConsoleColor.White;
            Objects.Add(key, value);
        }

        public virtual void Add(IGameObject value)
        {
            Add(value.GetType().Name, value);
        }

        /// <summary>
        /// Adds or replaces a game object.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void AddReplace(string key, IGameObject value)
        {
            if (Objects.ContainsKey(key))
                Objects.Remove(key);
            Add(key, value);
        }

        /// <summary>
        /// Removes a game object of this game object. The game object is unloaded first and after that -> removed!
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Remove(string key)
        {
            Objects[key].UnloadContent();
            var removed = Objects.Remove(key);
            //if (!removed)
            //    Console.WriteLine($"{key} not deleted");
            return removed;
        }

        /// <summary>
        /// Removes all game objects inside this object that equal the game object supplemented
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public virtual bool Remove(IGameObject gameObject)
        {
            return (from kv in Objects
                    where kv.Value.Equals(gameObject)
                    select kv.Key)
             .ToList().All(k => Remove(k));
        }

        public bool TryGetValue(string key, out IGameObject value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, IGameObject> item)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            lock (Objects)
            {
                Objects.Keys.ToList().ForEach(k =>
                {
                    if (Objects.ContainsKey(k))
                    {
                        Objects[k]?.UnloadContent();
                        Objects.Remove(k);
                    }
                });
            }
        }

        public bool Contains(KeyValuePair<string, IGameObject> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, IGameObject>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, IGameObject> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, IGameObject>> GetEnumerator()
        {
            return Objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Objects.GetEnumerator();
        }

        public IEnumerable<TResult> AllOfType<TResult>()
        {
            foreach (var item in Objects)
            {
                if (item.Value is TResult)
                    yield return (TResult)item.Value;
            }
        }

        /// <summary>
        /// Gives you a casted representation of a game object inside this one. This does not go through the whole tree structure. Only the first game objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T As<T>(string name) where T : IGameObject => (T)Objects[name];

        /// <summary>
        /// Iterates through the whole tree structure and tries to find game objects of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<T> AllOfTypeInTree<T>(T current) where T : GameObject
        {
            yield return current;
            foreach (var item in AllOfType<T>())
            {
                foreach (var subItem in AllOfTypeInTree<T>(item))
                {
                    yield return subItem;
                }
            }
        }

        /// <summary>
        /// This is the buggy messy for each iteration method implemented inside every game object. I don't recommend using this
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<IGameObject> action)
        {
            ForEach<IGameObject>(action);
        }

        /// <summary>
        /// This is the buggy messy for each iteration method implemented inside every game object. I don't recommend using this
        /// </summary>
        /// <param name="action"></param>
        public void ForEach<T>(Action<T> action)
            where T : IGameObject
        {
            List<string> keys = new List<string>(Keys);
            foreach (var key in keys)
                if (ContainsKey(key))
                    if (Objects[key] is T)
                        action((T)Objects[key]);
        }

        #endregion GameObjectList Implementation
    }
}