#region

using System.Collections.Generic;
using System.Configuration;

#endregion

namespace DBLayer.Persistence.Configuration.Base
{
    public abstract class ConfigurationElementCollection<TKey, TElement> : ConfigurationElementCollection, IEnumerable<TElement>
      where TElement : ConfigurationElement, new()
    {
        #region Properties

        public abstract override ConfigurationElementCollectionType CollectionType { get; }

        protected abstract override string ElementName { get; }

        #endregion


        #region Indexers

        public virtual TElement Get(TKey key)
        {
            return (TElement)BaseGet(key);
        }

        public virtual TElement Get(int index)
        {
            return (TElement)BaseGet(index);
        }

        #endregion

        #region Public Methods

        public virtual void Add(TElement path)
        {
            BaseAdd(path);
        }

        public virtual void Remove(TKey key)
        {
            BaseRemove(key);
        }

        public virtual void Clear()
        {
            foreach (var keyItem in this.BaseGetAllKeys())
            {
                BaseRemove(keyItem);  
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IEnumerable<TElement>

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return new ConfigurationElementEnumerator<TKey, TElement>(this);
        }

        #endregion

        #endregion

        #region Methods
        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return GetElementKey((TElement)element);
        }

        protected abstract TKey GetElementKey(TElement element);

        #endregion
    }
}