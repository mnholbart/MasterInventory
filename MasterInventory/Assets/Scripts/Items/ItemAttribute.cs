using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasterInventory
{
    [Serializable]
    public abstract class ItemAttribute
    {
        public string Key = "";
        public bool isStatic = true;
        public bool isRequired = false;

        /// <summary>
        /// Runtime lookup, set by the inventory state, matches a GUID string to item attribute data
        /// </summary>
        public Dictionary<string, ItemAttribute> SerializedAttributeLookup = new Dictionary<string, ItemAttribute>();
        
        [SerializeField] abstract protected object ObjectValue { get; set; }

        public virtual object GetObjectValue(string guid = null)
        {
            var value = (isStatic || guid == null) ? ObjectValue : SerializedAttributeLookup[guid].ObjectValue;
            return value;
        }

        public virtual void SetObjectValue(object newValue, string guid = null)
        {
            if (isStatic || guid == null)
                ObjectValue = newValue;
            else
                SerializedAttributeLookup[guid].ObjectValue = newValue;
        }

        public string GetAttributeName()
        {
            return GetType().ToString().Split('.')[1];
        }
        
        public abstract void InitializeSerializedValue();
        public abstract void SaveSerializedValue();

        protected ItemAttribute() { }
        protected ItemAttribute(ItemAttribute copy)
        {
            Key = copy.Key;
            isStatic = copy.isStatic;
            isRequired = copy.isRequired;
        }
    }

    [Serializable]
    public abstract class ItemAttribute<T> : ItemAttribute
    {
        public ItemAttribute(): base() { }
        public ItemAttribute(ItemAttribute<T> copy) : base(copy)
        {
            SerializedValue = copy.SerializedValue;
            StaticValue = copy.StaticValue;
            InitializeSerializedValue();
        }

        protected T SerializedValue { get; set; }
        protected virtual T StaticValue { get; set; }

        protected override object ObjectValue
        {
            get { return isStatic ? StaticValue : SerializedValue; }
            set {
                StaticValue = (T)value;
                SerializedValue = (T)value;
            }
        }

        public override void InitializeSerializedValue()
        {
            SerializedValue = StaticValue;
        }

        public override void SaveSerializedValue()
        {
            StaticValue = SerializedValue;
        }

        public T GetValue()
        {
            return (T)ObjectValue;
        }

        public void SetValue(T value)
        {
            ObjectValue = value;
        }

        public void SetDefaultValue(T value)
        {
            StaticValue = value;
        }
    }

    [Serializable]
    public class FloatAttribute : ItemAttribute<float>
    {
        public FloatAttribute() { }
        public FloatAttribute(ItemAttribute<float> copy): base(copy) { }

        [SerializeField] private float _staticValue;
        protected override float StaticValue { get { return _staticValue; } set { _staticValue = value; } }
        
    }

    [Serializable]
    public class IntAttribute : ItemAttribute<int>
    {
        public IntAttribute() { }
        public IntAttribute(ItemAttribute<int> copy) : base(copy) { }

        [SerializeField] private int _staticValue;
        protected override int StaticValue { get { return _staticValue; } set { _staticValue = value; } }
    }

    [Serializable]
    public class BoolAttribute : ItemAttribute<bool>
    {
        public BoolAttribute() { }
        public BoolAttribute(ItemAttribute<bool> copy) : base(copy) {  }

        [SerializeField] private bool _staticValue;
        protected override bool StaticValue { get { return _staticValue; } set { _staticValue = value; } }
    }

    [Serializable]
    public class StringAttribute : ItemAttribute<string>
    {
        public StringAttribute() { }
        public StringAttribute(ItemAttribute<string> copy) : base(copy) {  }

        [SerializeField] private string _staticValue;
        protected override string StaticValue { get { return _staticValue; } set { _staticValue = value; } }

    }

    [Serializable]
    public class InventoryItemAttribute : ItemAttribute<InventoryItem>
    {
        public InventoryItemAttribute() { }
        public InventoryItemAttribute(ItemAttribute<InventoryItem> copy) : base(copy) {  }

    [SerializeField] private InventoryItem _staticValue;
        protected override InventoryItem StaticValue { get { return _staticValue; } set { _staticValue = value; } }

    }

    [System.Serializable]
    public class ItemAttributes
    {
        [System.Serializable]
        public class RegisteredAttributeType
        {
            public Type type;
            
            public Func<object, object> EditorDelegate;
        }

        //Register a new RegisteredAttributeType using a new type created above extending ItemAttribute<T>
        //Give it the proper editor display field
        //Add a List<T> TAttributes = new List<T>(); below
        public static List<RegisteredAttributeType> RegisteredTypes = new List<RegisteredAttributeType>() {
            new RegisteredAttributeType() { type = typeof(IntAttribute),
                EditorDelegate = (object value) => 
                { return EditorGUILayout.DelayedIntField((int)value); }
            },
            new RegisteredAttributeType() { type = typeof(BoolAttribute),
                EditorDelegate = (object value) =>
                { return EditorGUILayout.Toggle((bool)value); }
            },
            new RegisteredAttributeType() { type = typeof(StringAttribute),
                EditorDelegate = (object value) =>
                { return EditorGUILayout.DelayedTextField((string)value); }
            },
            new RegisteredAttributeType() { type = typeof(FloatAttribute),
                EditorDelegate = (object value) =>
                { return EditorGUILayout.DelayedFloatField((float)value); }
            },
            new RegisteredAttributeType() { type = typeof(InventoryItemAttribute),
                EditorDelegate = (object value) =>
                { return EditorGUILayout.ObjectField((InventoryItem)value, typeof(InventoryItem), false); }
            },
        };

        public List<FloatAttribute> FloatAttributes = new List<FloatAttribute>();
        public List<IntAttribute> IntAttributes = new List<IntAttribute>();
        public List<BoolAttribute> BoolAttributes = new List<BoolAttribute>();
        public List<StringAttribute> StringAttributes = new List<StringAttribute>();
        public List<InventoryItemAttribute> InventoryItemAttributes = new List<InventoryItemAttribute>();

        public List<ItemAttribute> GetAllAttributes()
        {
            List<ItemAttribute> allAttributes = new List<ItemAttribute>();

            foreach (Type t in RegisteredTypes.Select(t => t.type))
            {
                IList l = GetAttributeList(t);
                foreach (object o in l)
                {
                    allAttributes.Add((ItemAttribute)o);
                }
            }

            return allAttributes;
        }

        public void AddAttribute(ItemAttribute attribute)
        {
            IList collection = GetAttributeList(attribute);

            collection.Add(attribute);
        }

        public void RemoveAttribute(ItemAttribute attribute)
        {
            IList collection = GetAttributeList(attribute);

            collection.Remove(attribute);
        }

        public IList GetAttributeList(ItemAttribute attribute)
        {
            return GetAttributeList(attribute.GetType());
        }

        public IList GetAttributeList(Type type)
        {
            FieldInfo listField = null;
            foreach (FieldInfo f in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object v = f.GetValue(this);
                if (v.GetType().GetGenericArguments()[0] == type)
                {
                    listField = f;
                    break;
                }
            }
            object val = listField.GetValue(this);
            IList collection = (IList)val;
            return collection;
        }

        public void ClearAttributes()
        {
            foreach (Type t in RegisteredTypes.Select(t => t.type))
            {
                IList l = GetAttributeList(t);
                l.Clear();
            }
        }
    }
}