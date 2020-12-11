//================================================
//描述 ： 静态扩展方法
//作者 ：HML
//创建时间 ：2017/07/02 
//版本： 1.0
//================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HMLFramwork.Log;

namespace HMLFramwork
{

    public static partial class Extent
    {
        #region Struct

        /// <summary>
        /// 数组转List
        /// </summary>
        /// <typeparam name="T">数组存储的对象类型</typeparam>
        /// <param name="arr">数组</param>
        /// <returns>List</returns>
        public static List<T> toList<T>(this T[] arr)
        {
            List<T> arrList = new List<T>();
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arrList.Add(arr[i]);
                }
            }
            else Debug.Log("警告：传入的数组为空，或者长度小于1...");
            return arrList;
        }

        /// <summary>
        /// 数组转List
        /// </summary>
        /// <typeparam name="T">数组存储的对象类型</typeparam>
        /// <param name="arr">数组</param>
        public static void toList<T>(this T[] arr, out List<T> arrList)
        {
            arrList = new List<T>();
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arrList.Add(arr[i]);
                }
            }
            else Debug.Log("警告：传入的数组为空，或者长度小于1...");
        }

        /// <summary>
        /// 获取List中的指定元素
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="insure">是否确保获取值</param>
        public static T getValue<T>(this List<T> list, int index, bool insure = false)
        {
            T _value = default(T);
            if (insure) index = index < list.Count ? index : list.Count - 1;
            if (list != null && list.Count > 0 && index < list.Count)
            {
                _value = list[index];
            }
            return _value;
        }


        /// <summary>
        /// 数组转List
        /// </summary>
        /// <typeparam name="T">数组存储的对象类型</typeparam>
        /// <param name="arr">数组</param>
        /// <returns>List</returns>
        public static T[] toArr<T>(this List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                T[] arr = new T[list.Count];

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = list[i];
                }
                return arr;
            }
            else
            {
                Debug.Log("警告：传入的数组为空，或者长度小于1...");
                return null;
            }
        }
        // <summary>
        /// 数组转List
        /// </summary>
        /// <typeparam name="T">数组存储的对象类型</typeparam>
        /// <param name="arr">数组</param>
        /// <returns>List</returns>
        public static T[] toArr<T>(this List<T> list, out T[] arr)
        {
            arr = new T[0];
            if (list != null && list.Count > 0)
            {
                arr = new T[list.Count];

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = list[i];
                }
                return arr;
            }
            else
            {
                Debug.Log("警告：传入的数组为空，或者长度小于1...");
                return null;
            }

        }

        /// <summary>
        /// 获取字典的所有Key值
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static List<K> getKeys<K, V>(this Dictionary<K, V> dic)
        {
            if (dic != null && dic.Count > 0) return new List<K>(dic.Keys);
            else return null;
        }

        /// <summary>
        /// 获取字典的所有Value值
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static List<V> getValues<K, V>(this Dictionary<K, V> dic)
        {
            if (dic != null && dic.Count > 0) return new List<V>(dic.Values);
            else return null;
        }
        /// <summary>
        /// 获取一个向量的一半
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 getHalf(this Vector3 v3)
        {
            return v3 * 0.5f;
        }

        /// <summary>
        /// 获取字典中指定Value值的下标
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int getIndexOf<K, V>(this Dictionary<K, V> dic, V value)
        {
            int index = -1;
            if (dic.ContainsValue(value))
            {
                index = dic.getValues().IndexOf(value);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", value.ToString()));
            return index;
        }
        /// <summary>
        /// 获取字典中指定Key值的下标
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int getIndexOf<K, V>(this Dictionary<K, V> dic, K key)
        {
            int index = -1;
            if (dic.ContainsKey(key))
            {
                index = dic.getKeys().IndexOf(key);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", key.ToString()));
            return index;
        }

        /// <summary>
        /// 移除指定Value值
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        public static void Remove<K, V>(this Dictionary<K, V> dic, V value)
        {
            if (dic.ContainsValue(value))
            {
                K key = default(K);
                foreach (var item in dic)
                {
                    if (item.Value.Equals(value))
                    {
                        key = item.Key;
                        break;
                    }
                }
                dic.Remove(key);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", value.ToString()));
        }

        /// <summary>
        /// 将和枚举元素同名的字符串转为对应枚举元素
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enum_element_str"></param>
        /// <returns></returns>
        public static TEnum toEnum<TEnum>(this string enum_element_str) where TEnum : struct
        {
            TEnum element;
            Enum.TryParse<TEnum>(enum_element_str, out element);
            return element;
        }



        #endregion
        #region Unity
        /// <summary>
        /// 复制自身（参数为：被复制的UI元素，localPosition是否一致，localRotation是否一致，localScale是否一致）
        /// </summary>
        /// <returns></returns>
        public static GameObject Clone(this GameObject target, params bool[] args)
        {
            if (target)
            {
                GameObject obj = MonoBehaviour.Instantiate(target);
                Transform trans = obj.transform;
                Transform _targetTrans = target.transform;
                trans.SetParent(target.transform.parent);
                if (args != null)
                {
                    if (args.Length == 1)
                    {
                        if (args[0]) trans.localPosition = _targetTrans.localPosition;
                    }
                    else if (args.Length == 2)
                    {
                        if (args[0]) trans.localPosition = _targetTrans.localPosition;
                        if (args[1]) trans.localRotation = _targetTrans.localRotation;
                    }
                    else if (args.Length == 3)
                    {
                        if (args[0]) trans.localPosition = _targetTrans.localPosition;
                        if (args[1]) trans.localRotation = _targetTrans.localRotation;
                        if (args[2]) trans.localScale = _targetTrans.localScale;
                    }
                }
                return obj;
            }
            else
            {
                Debug.Log("警告：需要克隆的目标体没有赋值！");
                return null;
            }
        }

        /// <summary>
        /// 将指定父节点的一级子节点添加到数组里面
        /// </summary>
        /// <param name="parent">指定父节点</param>
        /// <returns>返回存储子节点的数组</returns>
        public static GameObject[] getChildrenArr(this GameObject parent)
        {
            if (parent)
            {
                if (parent.transform.childCount < 1) return null;
                else
                {
                    GameObject[] arr = new GameObject[parent.transform.childCount];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = parent.transform.GetChild(i).gameObject;
                    }
                    return arr;
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
                return null;
            }
        }

        /// <summary>
        /// 将指定父节点的一级子节点添加到数组里面
        /// </summary>
        /// <param name="parent">指定父节点</param>
        /// <returns>返回存储子节点的数组</returns>
        public static Transform[] getChildrenArr(this Transform parent)
        {
            if (parent)
            {
                if (parent.childCount < 1) return null;
                else
                {
                    Transform[] arr = new Transform[parent.transform.childCount];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = parent.transform.GetChild(i);
                    }
                    return arr;
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
                return null;
            }
        }

        /// <summary>
        /// 将指定父节点的一级子节点添加到List里面
        /// </summary>
        /// <param name="parent">指定父节点</param>
        /// <returns>返回存储子节点的数组</returns>
        public static List<GameObject> getChildren(this GameObject parent)
        {
            List<GameObject> goList = new List<GameObject>();
            if (parent)
            {
                int childCount = parent.transform.childCount;
                if (childCount > 0)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        goList.Add(parent.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
            }
            return goList;
        }

        /// <summary>
        /// 将指定父节点的一级子节点添加到List里面
        /// </summary>
        /// <param name="parent">指定父节点</param>
        /// <returns>返回存储子节点的数组</returns>
        public static List<Transform> getChildren(this Transform parent)
        {
            List<Transform> goList = new List<Transform>();
            if (parent)
            {
                int childCount = parent.childCount;
                if (childCount > 0)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        goList.Add(parent.GetChild(i));
                    }
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
            }
            return goList;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static T requireComponent<T>(this GameObject go) where T : Component
        {
            if (go)
            {
                T comp = go.GetComponent<T>();
                if (comp == null)
                {
                    comp = go.AddComponent<T>();
                }
                return comp;
            }
            else
            {
                Debug.Log("error：对象为空...");
                return null;
            }
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static T requireComponent<T>(this Component target) where T : Component
        {
            if (target)
            {
                T comp = target.GetComponent<T>();
                if (comp == null && comp.gameObject != null)
                {
                    comp = comp.gameObject.AddComponent<T>();
                }
                return comp;
            }
            else
            {
                Debug.Log("error：对象为空...");
                return null;
            }
        }

        /// <summary>
        /// 获取当前对象及当前对象的所有子对象
        /// </summary>
        /// <param name="father">当前对象</param>
        /// <param name="ObjList">用来存储当前对象及所有子对象的List</param>
        /// <returns>返回存储对象的List</returns>
        public static List<Transform> getAllChildren(this Transform father)
        {
            if (father)
            {
                List<Transform> children = new List<Transform>();
                GeneralHelper.GetAllChildren(father, ref children);
                return children;
            }
            else return null;
        }

        /// <summary>
        /// 获取当前对象及当前对象的所有子对象
        /// </summary>
        /// <param name="father">当前对象</param>
        /// <param name="ObjList">用来存储当前对象及所有子对象的List</param>
        /// <returns>返回存储对象的List</returns>
        public static List<GameObject> getAllChildren(this GameObject father)
        {
            if (father)
            {
                List<GameObject> children = new List<GameObject>();
                GeneralHelper.GetAllChildren(father, ref children);
                return children;
            }
            else return null;
        }

        /// <summary>
        /// 音频播放（带回调）
        /// </summary>
        /// <param name="m_as"></param>
        /// <param name="callback">回调</param>
        public static void Play(this AudioSource m_as, Action callback)
        {
            if (callback != null)
            {
                if (m_as.clip != null && m_as.clip.length > 0)
                {
                    m_as.Play();
                    TimerEvent.Add(m_as.clip.length, callback);
                }
            }
        }

        /// <summary>
        /// 把录音转换为Byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] getByteArr(this AudioClip clip)
        {
            if (clip == null)
            {
                Debug.Log("警告：录音数据为空...");
                return null;
            }
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);
            byte[] outData = new byte[samples.Length * 2];
            int rescaleFactor = 32767; //to convert float to Int16   
            for (int i = 0; i < samples.Length; i++)
            {
                short temshort = (short)(samples[i] * rescaleFactor);
                byte[] temdata = System.BitConverter.GetBytes(temshort);
                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];
            }
            if (outData == null || outData.Length <= 0)
            {
                Debug.Log("警告：录音数据为空...");
                return null;
            }
            return outData;
        }

        /// <summary>
        /// 获取游戏对象的模型尺寸
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Vector3 getModelSize(this GameObject go)
        {
            if (go != null)
            {
                var mf = go.GetComponentInChildren<MeshFilter>(true);
                if (mf && mf.sharedMesh)
                {
                    return mf.sharedMesh.bounds.size;
                }
            }
            else Debug.Log("提醒：对象为空，或者没有网格...");
            return Vector3.zero;
        }
        /// <summary>
        /// 获取游戏对象模型的长宽尺寸
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Vector2 getModel_SizeXZ(this GameObject go)
        {
            if (go != null && go.GetComponentInChildren<MeshFilter>())
            {
                Vector3 v3 = go.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size;
                return new Vector2(v3.x, v3.z);
            }
            else
            {
                Debug.Log("提醒：对象为空，或者没有网格...");
                return Vector2.zero;
            }
        }

        /// <summary>
        /// 获取游戏对象模型的高
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static float getModel_SingleSize(this GameObject go, ComAxle ca)
        {
            if (go != null && go.GetComponentInChildren<MeshFilter>())
            {
                switch (ca)
                {
                    case ComAxle.X:
                        return go.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.x;
                    case ComAxle.Y:
                        return go.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.y;
                    case ComAxle.Z:
                        return go.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.z;
                    default: return 0;
                }
            }
            else
            {
                Debug.Log("提醒：对象为空，或者没有网格...");
                return 0;
            }
        }
        /// <summary>
        /// 获取游戏对象模型的高
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static float GetModel_Height(this GameObject go)
        {
            return getModel_SingleSize(go, ComAxle.Y);
        }

        /// <summary>
        /// 设置材质球的RenderingMode
        /// </summary>
        /// <param name="material"></param>
        /// <param name="renderingMode"></param>
        public static void setRenderingMode(this Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        /// <summary>
        /// 添加BoxCollider
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static BoxCollider addBoxCollider(this GameObject go)
        {
            var bc = go.GetComponent<BoxCollider>();
            if (bc == null) bc = go.AddComponent<BoxCollider>();
            var _bounds = go.GetComponentInChildren<MeshFilter>(true).sharedMesh.bounds;
            if (_bounds != null)
            {
                bc.center = _bounds.center;
                bc.size = _bounds.size;
            }
            return bc;
        }

        /// <summary>
        /// 激活或者隐藏挂有指定组件的游戏对象
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="isActive"></param>
        public static void setActive(this Component comp, bool isActive)
        {
            if (comp != null && comp.gameObject)
            {
                comp.gameObject.SetActive(isActive);
            }
        }


        /// <summary>
        /// 获取线的总长度
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static float getPerimeter(this LineRenderer line)
        {
            float p = 0;
            if (line != null)
            {
                int num = line.positionCount;
                if (num > 1)
                {
                    for (int i = 1; i < num; i++)
                    {
                        p += Vector3.Distance(line.GetPosition(i), line.GetPosition(i - 1));
                    }
                }
            }
            else Debug.Log("警告：线渲染为空...");

            return p;
        }

        /// <summary>
        /// 获取线渲染的指定边长
        /// </summary>
        /// <param name="line"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float getLength(this LineRenderer line, int index)
        {
            float l = 0;
            if (line != null)
            {
                int num = line.positionCount;
                if (num > 1)
                {
                    index = index >= num - 1 ? num - 1 : (index < 1 ? 1 : index);
                    l += Vector3.Distance(line.GetPosition(index), line.GetPosition(index - 1));
                }
            }
            else Debug.Log("警告：线渲染为空...");
            return l;
        }

        /// <summary>
        /// 设置线渲染器的宽度
        /// </summary>
        /// <param name="line"></param>
        /// <param name="startWidth"></param>
        /// <param name="endWidth"></param>
        public static void setWidth(this LineRenderer line, float startWidth, float endWidth)
        {
            if (line != null)
            {
                line.startWidth = startWidth;
                line.endWidth = endWidth;
            }
            else Debug.Log("警告：线渲染为空...");
        }

        /// <summary>
        /// 设置线渲染器的颜色
        /// </summary>
        /// <param name="line"></param>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        public static void setColor(this LineRenderer line, Color startColor, Color endColor)
        {
            if (line != null)
            {
                line.startColor = startColor;
                line.endColor = endColor;
            }
            else Debug.Log("警告：线渲染为空...");
        }

        /// <summary>
        /// 获取指定对象的父对象层级，排序为：自身到根节点
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static List<Transform> getParentLevel(this Transform trans)
        {
            List<Transform> parent_level = null;
            if (trans)
            {
                parent_level = new List<Transform>();
                if (trans.root.name.Equals(trans.name))
                {
                    parent_level.Add(trans);
                }
                else
                {
                    Transform trans_temp = trans;
                    while (!trans_temp.Equals(trans.root))
                    {
                        parent_level.Add(trans_temp);
                        trans_temp = trans_temp.parent;
                    }
                    parent_level.Add(trans_temp);
                }
            }
            else
            {
                LogQueue.Add("警告：需要获取的父节点层级的对象为空！");
            }
            return parent_level;
        }

        #endregion
        #region String

        /// <summary>
        /// 将字符串转成单个字符串数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] toStringArr(this string str)
        {
            string[] strArr = null;
            if (!string.IsNullOrEmpty(str))
            {
                int length = str.Length;
                strArr = new string[length];
                for (int i = 0; i < length; i++)
                {
                    strArr[i] = str[i].ToString();
                }
            }
            return strArr;
        }

        /// <summary>
        /// 将字符串转成单个字符串List
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> toStringList(this string str)
        {
            List<string> strArr = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                int length = str.Length;
                for (int i = 0; i < length; i++)
                {
                    strArr.Add(str[i].ToString());
                }
            }
            return strArr;
        }

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool hasChinese(this string str)
        {
            bool has = false;
            if (str != null)
            {
                int length = str.Length;
                if (length > 0)
                {
                    Regex rx = new Regex("^[\u4e00-\u9fa5]$");
                    for (int i = 0; i < length; i++)
                    {
                        if (rx.IsMatch(str[i].ToString()))
                        {
                            has = true;
                            break;
                        }
                    }
                }
            }
            return has;
        }

        /// <summary>
        /// 给字符串设定颜色和大小
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string getStrWithColor(this string str, Color? color, int size)
        {
            Color col = (Color)(color == null ? GeneralHelper.getRandomColor() : color);
            return string.Format("<color=#{0}><size={1}>{2}</size></color>", ColorUtility.ToHtmlStringRGB(col), size.ToString(), str);
        }
        /// <summary>
        /// 获取目标字符串经过32位MD5加密后的字符串
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string getMD5Encrypt32(this string target)
        {
            return ArithmeticHelper.MD5Encrypt32(target);
        }
        /// <summary>
        /// 获取目标字符串经过64位MD5加密后的字符串
        /// </summary>
        /// <param name="target"></param>
        public static string getMD5Encrypt64(this string target)
        {
            return ArithmeticHelper.MD5Encrypt64(target);
        }

        /// <summary>
        /// 删除字符串中的所有的指定字符
        /// </summary>
        /// <param name="target"></param>
        /// <param name="charStr"></param>
        /// <returns></returns>
        public static string deleteAllTheChar(this string target, string charStr)
        {
            return Regex.Replace(target, charStr, "");
        }
        #endregion
        #region value-type

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float getAbs(this float value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int getAbs(this int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double getAbs(this double value)
        {
            return Math.Abs(value);
        }

        #endregion
    }
    #region enum

    /// <summary>
    /// 渲染模式
    /// </summary>
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public enum ColorType
    {
        Red,
        Green,
        Blue,
        Yellow,
        White,
        Black,
        Random
    }
    #endregion
}
