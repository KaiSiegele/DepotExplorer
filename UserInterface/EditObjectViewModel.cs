using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Basics;
using System.Reflection;
using System.Diagnostics;
using Tools;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für Viewmodels, die die Daten eines
    /// Modells anzeigen, verändern und Fehlerprüfungen durchführen
    /// Die Felder die auch Felder im Modell sind, werden
    /// durch das Attribut BaseDataField gekennzeichnet
    /// </summary>
    public class EditObjectViewModel : ObjectViewModel, IDataErrorInfo, IObjectEditComponent
    {
        /// <summary>
        /// Erzeugt ein neues ViewModels
        /// </summary>
        protected EditObjectViewModel() : base()
        {
        }

        /// <summary>
        /// Erzeugt ein neues ViewModels
        /// </summary>
        /// <param name="bo">Modell, dessen Daten gelesen und verändert werden</param>
        protected EditObjectViewModel(BaseObject bo) : this(bo, false)
        {
        }

        /// <summary>
        /// Erzeugt ein neues ViewModels
        /// </summary>
        /// <param name="bo">Modell dessen Daten gelesen und verändert werden</param>
        /// <param name="enabled">Zeigt an ob die Daten des Modells geändert werden können</param>
        /// <param name="copy">Zeigt an, ob vom Modell eine Kopie zum Verändern ezeugt werden soll</param>
        protected EditObjectViewModel(BaseObject bo, bool enabled) : base(bo)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// Aktualisiert das Modell
        /// </summary>
        /// <param name="bo">Modell dessen Daten gelesen und verändert werden</param>
        /// <param name="enabled">Zeigt an ob die Daten des Modells geändert werden können</param>
        public void Update(BaseObject bo, bool enabled)
        {
            Enabled = enabled;

            Update(bo);
        }

        /// <summary>
        /// Resets properties changed and valid since
        /// underlying object has changed
        /// </summary>
        public void Reset()
        {
            RefreshProperties(false);
        }

        /// <summary>
        /// Aktualisiert die Data-Eigenschaft mit dem übergebenen Namen
        /// </summary>
        /// <param name="propertyName">Geänderte Eigenschaft</param>
        /// <param name="value">Neuer Wert</param>
        protected void UpdateDataProperty(string propertyName, object value)
        {
            Debug.Assert(IsDataProperty(propertyName), "Eigenschaft {0} ist keine DataProperty", propertyName);
            if (Loading == false)
            {
                if (_bo.UpdateValue(propertyName, value))
                {
                    RefreshProperties(false);
                    OnPropertyChanged(propertyName);
                }
                else
                {
                    Debug.Assert(false, "VM und Objekt haben unterschiedliche Typen oder Eigenschaften");
                }
            }
        }

        #region Eigenschaften
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            protected set
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }
        private bool _enabled;

        public bool Valid
        {
            get
            {
                return _valid;
            }
        }
        private bool _valid;

        public bool Changed
        {
            get
            {
                return _changed;
            }
        }
        private bool _changed;
        #endregion

        #region Interface IDataErrorInfo
        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (Enabled)
                {
                    if (IsDataProperty(columnName))
                    {
                        string descripton = string.Empty;
                        if (!_bo.IsValueValid(columnName, ref descripton))
                        {
                            result = descripton;
                        }
                    }
                }
                return result;
            }
        }
        #endregion

        #region Methoden der Basisklasse
        protected override void Refresh()
        {
            RefreshProperties(true);
        }

        public override bool IsValid()
        {
            return Valid;
        }

        public override bool HasChanged()
        {
            return Changed;
        }
        #endregion

        /// <summary>
        /// Aktualsiert die Eigenschaften aus dem Modell
        /// </summary>
        /// <param name="dataProperties">Zeigt an, ob auch die Eigenschaften des Modell gelesen werden sollen</param>
        private void RefreshProperties(bool dataProperties)
        {
            Debug.Assert(_bo != null, "BO not set");
            _valid = _bo.IsValid();
            OnPropertyChanged("Valid");
            _changed = _bo.Changed;
            OnPropertyChanged("Changed");

            if (dataProperties)
            {
                LoadDataProperties();
            }
        }

        private void LoadDataProperties()
        {
            Loading = true;
            IEnumerable<PropertyInfo> props = GetType().GetProperties();
            foreach (var pi in props)
            {
                if (IsDataProperty(pi))
                {
                    object value = _bo.GetValue(pi.Name, pi.PropertyType);
                    try
                    {
                        pi.SetValue(this, value, null);
                        OnPropertyChanged(pi.Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(GetType().Name, "LoadDataProperties", ex);
                    }
                }
            }
            Loading = false;
        }


        private bool IsDataProperty(PropertyInfo pi)
        {
            bool result;
            var customAttribute = pi.GetCustomAttributes(typeof(BaseDataProperty), false).FirstOrDefault();
            if (customAttribute != null)
                result = true;
            else
                result = false;
            return result;
        }

        private bool IsDataProperty(string propertyName)
        {
            bool result;
            PropertyInfo propInfo = GetType().GetProperty(propertyName);
            if (propInfo != null)
            {
                result = IsDataProperty(propInfo);
            }
            else
            {
                Log.Write(TraceLevel.Error, GetType().Name, "Validation", "Cannot get prop" + propertyName);
                result = false;
            }
            return result;
        }


        private bool Loading { get; set; }
    }
}
