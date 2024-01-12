using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Tools;

namespace Basics
{
    /// <summary>
    /// Status der anzeigt, ob der Inhalt des Objektes
    /// auch in der Datenbank steht
    /// </summary>
    public enum ObjectState
    {
        Stored,   // Inhalt vom Objekt im System und in der Datenbank gleich
        Inserted, // Objekt im System eingefügt, Inhalt noch nicht gepeichert
        Updated,  // Objekt im System verändert, Änderungen noch nicht gespeichert
        Removed   // Objekt im System gelöscht, Inhalt in der Datenbank noch nicht gespeichert
    }

    /// <summary>
    /// Basisklasse fuer die Objekte, die vom System verwaltet werden
    /// und deren Inhalt aus einer Datenbank gelesen und in eine Datenbank
    /// geschrieben wird
    /// Jedes Objekt hat eine Id zur eindeutigen Identifizierung, einen Namen
    /// und einen Status
    /// </summary>
    public abstract class BaseObject : IEquatable<BaseObject>
    {
        public static readonly int NotSpecified = 0;

        /// <summary>
        /// Hilfsmethode, um festzustellen ob eine id
        /// für ein BaseObject verwendet werden darf
        /// </summary>
        /// <param name="id">Zu prüfende Id</param>
        /// <returns>Id ist gültig</returns>
        [Conditional("DEBUG")]
        public static void CheckId(int id)
        {
           Debug.Assert(id > NotSpecified);
        }

        public bool Equals(BaseObject other)
        {
            if (other == null)
            {
                return false;
            }
            BaseObject objAsBO = other as BaseObject;
            if (objAsBO.GetType() != GetType())
            {
                return false;
            }
            else
            {
                return objAsBO.Id == Id;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Erzeugt ein neues Default-Objekt
        /// vom uebergebenen Typ mit dem uebergebenen Status
        /// </summary>
        /// <typeparam name="T">Typ des neuen Objekts</typeparam>
        /// <param name="objectState=">Status des Objects</param>
        /// <returns>Erzeugtes Objekt</returns>
        public static T CreateDefault<T>(ObjectState objectState = ObjectState.Stored) where T : BaseObject
        {
            T obj = Activator.CreateInstance(typeof(T)) as T;
            obj.ObjectState = objectState;
            return obj;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ObjectState ObjectState { get; set; }

        /// <summary>
        /// Constructor
        /// Erzeugt ein neues, leeres Objekt bspw. beim Lesen von Informationen
        /// aus der Datenbank. Die Werte koennen anschließend mit der Methode
        /// UpdateValue gesetzt werden
        /// </summary>
        protected BaseObject()
          : this(BaseObject.NotSpecified, string.Empty, ObjectState.Stored)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Id des Objekts</param>
        /// <param name="name">Name</param>
        /// <param name="objectState">Status</param>
        protected BaseObject(int id, string name, ObjectState objectState)
        {
            Id = id;
            Name = name;
            ObjectState = objectState;
        }

 
        /// <summary>
        /// Gibt den Inhalt des Objekts in lesbarer Form aus
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}", GetType().Name, Id, Name);
        }

        /// <summary>
        /// Stellt fest, ob ein Objekt gelöscht wurde
        /// </summary>
        public bool Removed
        {
            get
            {
                return ObjectState == ObjectState.Removed;
            }
        }

        /// <summary>
        /// Stellt fest, ob ein Objekt geändert wurde
        /// </summary>
        public bool Changed
        {
            get
            {
                return (ObjectState == ObjectState.Inserted || ObjectState == ObjectState.Updated);
            }
        }

        /// <summary>
        /// Stellt fest, ob das uebergebene Objekt das gleiche ist
        /// Zwei BaseObjekt-Objekte sind gleich, wenn sie vom
        /// gleichen Typ sind und die Id gleich ist
        /// </summary>
        /// <param name="obj">Objekt zum Vergleich</param>
        /// <returns>Objekt ist gleich</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (object.ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            BaseObject bo = obj as BaseObject;
            Debug.Assert(bo != null);
            return (Id == bo.Id);
        }

        /// <summary>
        /// Prüft ob das Objekt in einem korrekten Zustand ist
        /// Basisimplementierung prueft, ob alle Eigenschaften einen gueltige Wert gesetzt wurde
        /// </summary>
        /// <returns>Werte aller Eigenschaft sind gueltig</returns>
        public virtual bool IsValid()
        {
            bool result = true;
            IEnumerable<PropertyInfo> propInfos = GetType().GetProperties();
            foreach (var pi in propInfos)
            {
                string desc = string.Empty;
                if (!BaseValidations.ValidateInput(pi.Name, pi.GetValue(this, null), this, ref desc))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Kopiert ein Objekt
        /// </summary>
        /// <returns>Kopie des Obekts</returns>
        public BaseObject Copy()
        {
            return (BaseObject)MemberwiseClone();
        }

        #region Setzen und Abfragen der Eigenschaften
        /// <summary>
        /// Prueft, ob fuer eine Eigenschaft ein gueltiger Wert gesetzt wurde
        /// Basisimplementierung prueft, ob für die Eigenschaft einen gueltige Wert gesetzt wurde
        /// </summary>
        /// <param name="propName">Name der Eigenschaft</param>
        /// <param name="description">Gibt eine Fehlermeldung zurueck, falls der eingegebene Wert ungueltig war</param>
        /// <returns>Wert der Eigenschaft war gueltig</returns>
        public virtual bool IsValueValid(string propName, ref string description)
        {
            bool result = false;
            PropertyInfo propInfo = GetType().GetProperty(propName);
            if (propInfo != null)
            {
                result = BaseValidations.ValidateInput(propName, propInfo.GetValue(this, null), this, ref description);
            }
            else
            {
                TracePropertyError(false, propName, "Property not found");
            }
            return result;
        }

        /// <summary>
        /// Gibt den Wert einer Eigenschaft zurück und prueft dabei den Typ
        /// </summary>
        /// <param name="propName">Name der Eigenschaft</param>
        /// <param name="propType">Typ der Eigenschaft</param>
        /// <returns>Wert der Eigenschaft, null falls die Eigenschaft nicht vorhanden oder der Typ falsch ist</returns>
        public object GetValue(string propName, Type propType)
        {
            return GetValue(propName, propType, true);
        }

        /// <summary>
        ///  Gibt den Wert einer Eigenschaft zurück 
        /// </summary>
        /// <param name="propName">Name der Eigenschaft</param>
        /// <returns>Wert der Eigenschaft, null falls die Eigenschaft nicht vorhanden ist</returns>
        public object GetValue(string propName)
        {
            return GetValue(propName, default(Type), false);
        }

        /// <summary>
        /// Setzt den Wert der Eigenschaft
        /// </summary>
        /// <param name="propName">Name der Eigenschaft</param>
        /// <param name="value">Neuer Wert</param>
        /// <returns>Eigenschaft erfolgreich aktualisiert</returns>
        public bool UpdateValue(string propName, object value)
        {
            bool result = false;
            PropertyInfo propInfo = GetType().GetProperty(propName);
            if (propInfo != null)
            {
                try
                {
                    propInfo.SetValue(this, value);
                    SetChanged();
                    result = true;
                }
                catch (Exception e)
                {
                    TracePropertyError(false, propName, e.Message);
                }
            }
            else
            {
                TracePropertyError(false, propName, "Property not found");
            }
            return result;
        }

        /// <summary>
        /// Aktualisiert den Wert der Eigenschaft
        /// </summary>
        /// <param name="propName">Name der Eigenschaft</param>
        /// <param name="propType">Typ der Eigenschaft</param>
        /// <param name="value">Neuer Wert</param>
        /// <returns>Eigenschaft erfolgreich aktualisiert</returns>
        public bool SetValue(string propName, Type propType, object value)
        {
            bool result = false;
            PropertyInfo propInfo = GetType().GetProperty(propName);
            if (propInfo != null)
            {
                if (propInfo.PropertyType == propType || (propType == typeof(UInt16) || propType == typeof(Int16) || propType == typeof(DateTime)))
                {
                    try
                    {
                        propInfo.SetValue(this, value);
                        result = true;
                    }
                    catch (Exception e)
                    {
                        TracePropertyError(false, propName, e.Message);
                    }
                }
                else
                {
                    TracePropertyError(false, propName, "Property has wrong type (expect " + propInfo.PropertyType.Name + ")");
                }
            }
            else
            {
                TracePropertyError(false, propName, "Property not found");
            }
            return result;
        }
        #endregion

        [Conditional("DEBUG")]
        public static void CheckParameter<T>(string method, BaseObject bo) where T : BaseObject
        {
            if (bo.GetType() != typeof(T))
            {
                Debug.Assert(
                  false,
                  "Falscher Typ beim Aufruf der Methode " + method + " (Object ist " + bo.GetType().Name + ", erwartet " + typeof(T).Name + " ).");
            }
        }

        private object GetValue(string propName, Type propType, bool checkType)
        {
            object value = null;
            PropertyInfo propInfo = GetType().GetProperty(propName);
            if (propInfo != null)
            {
                if (!checkType || propInfo.PropertyType == propType)
                {
                    value = propInfo.GetValue(this, null);
                }
                else
                {
                    TracePropertyError(true, propName, "Property has wrong type (expect " + propInfo.PropertyType.Name + ")");
                }
            }
            else
            {
                TracePropertyError(true, propName, "Property not found");
            }
            return value;
        }

        /// <summary>
        /// Zeigt an, dass ein Objekt im System geaendert wurde
        /// </summary>
        private void SetChanged()
        {
            if (ObjectState == ObjectState.Stored)
            {
                ObjectState = ObjectState.Updated;
            }
        }

        private void TracePropertyError(bool read, string propName, string message)
        {
            string error;
            if (read)
            {
                error = string.Format("Cannot get prop {0} from {1}: {2}", propName, ToString(), message);
            }
            else
            {
                error = string.Format("Cannot set prop {0} in {1}: {2}", propName, ToString(), message);
            }
            Log.Write(TraceLevel.Error, "BaseObject", read ? "GetValue" : "UpdateValue", error);
        }
    }
}
