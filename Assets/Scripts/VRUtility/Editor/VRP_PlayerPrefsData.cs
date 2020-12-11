using UnityEngine;

namespace MiaoTech.VR
{
    public class VRP_PlayerPrefsData<T> where T : System.IConvertible
    {
        string _saveKey;
        private T _data = default(T);
        private System.IConvertible _defaultVal = null;
        private bool _get = false;

        public VRP_PlayerPrefsData(string saveKey, System.IConvertible defaultVal)
        {
            _saveKey = saveKey;
            _defaultVal = defaultVal;
            //Save();
        }

        public T Data
        {
            get
            {
                if (_get == false)
                {
                    _get = true;
                    _data = Load();
                }
                return _data;
            }
            set
            {
                if (_get == false || value.Equals(_data) == false)
                {
                    _get = true;
                    _data = value;
                    Save(true);
                }
            }
        }

        private T Load()
        {
            var val = PlayerPrefs.GetString(_saveKey, _defaultVal.ToString());
            try
            {
                var retVal = (T)System.Convert.ChangeType(val, typeof(T));
                return retVal;
            }
            catch
            {
                return default(T);
            }
        }

        private void Save(bool doSave = true)
        {
            PlayerPrefs.SetString(_saveKey, _data.ToString());
            if (doSave)
            {
                PlayerPrefs.Save();
            }
        }
    }
}