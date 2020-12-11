
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using HMLFramwork.Log;
using System.Security.Cryptography;
using System.Text;
using Application = UnityEngine.Application;

namespace HMLFramwork
{
    /// <summary>
    /// 常用算法管理
    /// </summary>
    public class ArithmeticHelper
    {

        /// <summary>
        /// 二分查找
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="low">开始索引 0</param>
        /// <param name="high">结束索引 </param>
        /// <param name="key">要查找的对象</param>
        /// <returns></returns>
        public static int BinarySearch(int[] arr, int low, int high, int key)
        {
            int mid = (low + high) / 2;
            if (low > high)
                return -1;
            else
            {
                if (arr[mid] == key)
                    return mid;
                else if (arr[mid] > key)
                    return BinarySearch(arr, low, mid - 1, key);
                else
                    return BinarySearch(arr, mid + 1, high, key);
            }
        }

        /// <summary>
        /// 获取一个对象距离一组位置点的最近点的下标
        /// </summary>
        /// <param name="target"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static int GetNearestPointIndex(Transform target, Vector3[] arr)
        {
            int pointIndex = -1;
            if (target)
            {
                Vector3 targetOriPos = target.position;
                float dis = 0;
                if (arr != null && arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0) dis = Vector3.SqrMagnitude(arr[0] - targetOriPos);
                        else
                        {
                            if (dis >= Vector3.SqrMagnitude(arr[i] - targetOriPos))
                            {
                                dis = Vector3.SqrMagnitude(arr[i] - targetOriPos);
                                pointIndex = i;
                            }
                            else continue;
                        }
                    }
                }
            }
            return pointIndex;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt32(string target)
        {
            //实例化一个md5对像
            MD5 md5 = MD5.Create();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] strBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(target));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            int length = strBytes.Length;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                builder.Append(strBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
        /// <summary>
        /// 64位MD5加密
        /// </summary>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt64(string target)
        {
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(target));
            return Convert.ToBase64String(s);
        }
    }

    /// <summary>
    /// 通用功能
    /// </summary>
    public class GeneralHelper
    {

        /// <summary>
        /// 隐藏指定的对象数组的所有子元素
        /// </summary>
        /// <param name="arr">指定的对象数组</param>
        public static void HideArrEle(GameObject[] arr)
        {
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    HideGO(arr[i]);
                }
            }
        }

        /// <summary>
        /// 隐藏指定的对象数组的所有子元素
        /// </summary>
        /// <param name="arr">指定的对象数组</param>
        public static void HideArrEle(Transform[] arr)
        {
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    HideGO(arr[i]);
                }
            }
        }

        /// <summary>
        /// 隐藏指定的对象数组的所有子元素
        /// </summary>
        /// <param name="arr">指定的对象数组</param>
        public static void HideListEle(List<GameObject> go_list)
        {
            if (go_list != null && go_list.Count > 0)
            {
                for (int i = 0; i < go_list.Count; i++)
                {
                    HideGO(go_list[i]);
                }
            }
        }
        /// <summary>
        /// 隐藏指定的对象数组的所有子元素
        /// </summary>
        /// <param name="arr">指定的对象数组</param>
        public static void HideListEle(List<Transform> go_list)
        {
            if (go_list != null && go_list.Count > 0)
            {
                for (int i = 0; i < go_list.Count; i++)
                {
                    HideGO(go_list[i]);
                }
            }
        }

        /// <summary>
        /// 激活指定的对象数组的所有子元素
        /// </summary>
        /// <param name="arr">指定的对象数组</param>
        public static void ActiveListEle(List<Transform> go_list)
        {
            if (go_list != null && go_list.Count > 0)
            {
                for (int i = 0; i < go_list.Count; i++)
                {
                    ActivateGO(go_list[i]);
                }
            }
        }

        /// <summary>
        /// 激活指定的对象数组的所有子元素
        /// </summary>
        /// <param name="arr">指定的对象数组</param>
        public static void ActiveListEle(List<GameObject> go_list)
        {
            if (go_list != null && go_list.Count > 0)
            {
                for (int i = 0; i < go_list.Count; i++)
                {
                    ActivateGO(go_list[i]);
                }
            }
        }


        /// <summary>
        /// 数组转List
        /// </summary>
        /// <typeparam name="T">数组存储的对象类型</typeparam>
        /// <param name="arr">数组</param>
        /// <returns>List</returns>
        public static List<T> ArrToList<T>(T[] arr)
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
        public static void ArrToList<T>(T[] arr, out List<T> arrList)
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
        /// 隐藏指定对象
        /// </summary>
        /// <param name="obj">需隐藏对象</param>
        public static void HideGO(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
            else
                Debug.Log("警告：指定对象为空，请检查赋值！");
        }
      
        /// <summary>
        /// 隐藏指定对象
        /// </summary>
        /// <param name="obj">需隐藏对象</param>
        public static void HideGO(Component obj)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }
            else
                Debug.Log("警告：指定对象为空，请检查赋值！");
        }
        /// <summary>
        /// 激活指定对象
        /// </summary>
        /// <param name="obj">需激活对象</param>
        public static void HideGO(params UnityEngine.Object[] objs)
        {
            if (objs != null && objs.Length > 0)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i].GetType().Equals(typeof(GameObject)))
                        HideGO(objs[i] as GameObject);
                    else if (objs[i].GetType().Equals(typeof(Component)))
                        HideGO(objs[i] as Transform);
                }
            }
            else Debug.Log("警告：指定对象为空，请检查赋值！");
        }

        /// <summary>
        /// 激活指定对象
        /// </summary>
        /// <param name="obj">需激活对象</param>
        public static void ActivateGO(GameObject obj)
        {
            if (obj != null)
            {
                if (!obj.activeInHierarchy) obj.SetActive(true);
            }
            else Debug.Log("警告：指定对象为空，请检查赋值！");
        }

        /// <summary>
        /// 激活指定对象
        /// </summary>
        /// <param name="obj">需激活对象</param>
        public static void ActivateGO(Component obj)
        {
            if (obj != null) obj.gameObject.SetActive(true);
            else Debug.Log("警告：指定对象为空，请检查赋值！");
        }

        /// <summary>
        /// 激活指定对象
        /// </summary>
        /// <param name="obj">需激活对象</param>
        public static void ActivateGO(params UnityEngine.Object[] objs)
        {
            if (objs != null && objs.Length > 0)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i].GetType().Equals(typeof(GameObject)))
                        ActivateGO(objs[i] as GameObject);
                    else if (objs[i].GetType().Equals(typeof(Component)))
                        ActivateGO(objs[i] as Component);
                }
            }
            else Debug.Log("警告：指定对象为空，请检查赋值！");
        }

        /// <summary>
        /// 自转
        /// </summary>
        /// <param name="target">需要自转的对象</param>
        /// <param name="RotateSpeed">旋转速度</param>
        /// <param name="MyRotate_Axle">旋转轴</param>
        public static void RotateAroundSelf(Transform selfObj, float RotateSpeed, RotateAxle MyRotate_Axle)
        {
            switch (MyRotate_Axle)
            {
                case RotateAxle.X:
                    selfObj.Rotate(new Vector3(1f, 0, 0) * RotateSpeed * Time.deltaTime, Space.Self);
                    break;
                case RotateAxle.Y:
                    selfObj.Rotate(new Vector3(0, 1f, 0) * RotateSpeed * Time.deltaTime, Space.Self);
                    break;
                case RotateAxle.Z:
                    selfObj.Rotate(new Vector3(0, 0, 1f) * RotateSpeed * Time.deltaTime, Space.Self);
                    break;
            }
        }

        /// <summary>
        /// 绕目标旋转
        /// </summary>
        /// <param name="selfObj">作旋转的对象</param>
        /// <param name="RotateTargetObj">旋转运动目标</param>
        /// <param name="RotateSpeed">旋转速度</param>
        /// <param name="MyRotate_Axle">旋转轴</param>
        public static void RotateAroundTarget(Transform selfObj, GameObject RotateTargetObj, float RotateSpeed, RotateAxle MyRotate_Axle)
        {
            switch (MyRotate_Axle)
            {
                case RotateAxle.X:
                    selfObj.RotateAround(RotateTargetObj.transform.position, Vector3.right, RotateSpeed * Time.deltaTime);
                    break;
                case RotateAxle.Y:
                    selfObj.RotateAround(RotateTargetObj.transform.position, Vector3.up, RotateSpeed * Time.deltaTime);
                    break;
                case RotateAxle.Z:
                    selfObj.RotateAround(RotateTargetObj.transform.position, Vector3.forward, RotateSpeed * Time.deltaTime);
                    break;
            }
        }

        /// <summary>
        /// 自转
        /// </summary>
        /// <param name="target">需要自转的对象</param>
        /// <param name="RotateSpeed">旋转速度</param>
        /// <param name="MyRotate_Axle">旋转轴</param>
        public static void RotateAroundSelf_Fixed(Transform selfObj, float RotateSpeed, RotateAxle MyRotate_Axle)
        {
            switch (MyRotate_Axle)
            {
                case RotateAxle.X:
                    selfObj.Rotate(new Vector3(1f, 0, 0) * RotateSpeed * Time.fixedDeltaTime, Space.Self);
                    break;
                case RotateAxle.Y:
                    selfObj.Rotate(new Vector3(0, 1f, 0) * RotateSpeed * Time.fixedDeltaTime, Space.Self);
                    break;
                case RotateAxle.Z:
                    selfObj.Rotate(new Vector3(0, 0, 1f) * RotateSpeed * Time.fixedDeltaTime, Space.Self);
                    break;
            }
        }

        /// <summary>
        /// 绕目标旋转
        /// </summary>
        /// <param name="selfObj">作旋转的对象</param>
        /// <param name="RotateTargetObj">旋转运动目标</param>
        /// <param name="RotateSpeed">旋转速度</param>
        /// <param name="MyRotate_Axle">旋转轴</param>
        public static void RotateAroundTarget_Fixed(Transform selfObj, GameObject RotateTargetObj, float RotateSpeed, RotateAxle MyRotate_Axle)
        {
            switch (MyRotate_Axle)
            {
                case RotateAxle.X:
                    selfObj.RotateAround(RotateTargetObj.transform.position, Vector3.right, RotateSpeed * Time.fixedDeltaTime);
                    break;
                case RotateAxle.Y:
                    selfObj.RotateAround(RotateTargetObj.transform.position, Vector3.up, RotateSpeed * Time.fixedDeltaTime);
                    break;
                case RotateAxle.Z:
                    selfObj.RotateAround(RotateTargetObj.transform.position, Vector3.forward, RotateSpeed * Time.fixedDeltaTime);
                    break;
            }
        }

        /// <summary>
        /// 将指定父节点的一级子节点添加到数组里面
        /// </summary>
        /// <param name="parent">指定父节点</param>
        /// <param name="arr">指定对象</param>
        public static void AddChildToArr(GameObject parent, ref GameObject[] arr)
        {
            if (parent)
            {
                if (arr == null)
                {
                    arr = new GameObject[parent.transform.childCount];
                }
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = parent.transform.GetChild(i).gameObject;
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
            }
        }

        /// <summary>
        /// 将指定父节点的一级子节点添加到数组里面
        /// </summary>
        /// <param name="parent">指定父节点</param>
        /// <returns>返回存储子节点的数组</returns>
        public static GameObject[] AddChildToArr(GameObject parent)
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
        /// 删除父节点下特定子对象后的所有子节点
        /// </summary>
        /// <param name="father">父节点</param>
        /// <param name="index">特定子对象的下标</param>
        public static void DestroyChildrenAfterIndex(Transform father, int index)
        {
            if (father != null)
            {
                for (int i = 0; i < father.childCount; i++)
                {
                    if (i > index)
                    {
                        MonoBehaviour.Destroy(father.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
            }
        }

        /// <summary>
        /// 删除父节点下特定子对象后的所有子节点
        /// </summary>
        /// <param name="father">父节点</param>
        /// <param name="index">特定子对象的下标</param>
        public static void DestroyChildrenAfterIndex(GameObject father, int index)
        {
            if (father != null)
            {
                for (int i = 0; i < father.transform.childCount; i++)
                {
                    if (i > index)
                    {
                        MonoBehaviour.Destroy(father.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
            }
        }

        /// <summary>
        /// 清除指定对象的所有一级子对象
        /// </summary>
        /// <param name="father"></param>
        public static void ClearChildren(Transform father)
        {
            if (father != null)
            {
                for (int i = 0; i < father.childCount; i++)
                {
                    GameObject.DestroyImmediate(father.GetChild(i));
                }
            }
            else
            {
                Debug.Log("警告：父对象没有赋值！");
            }
        }

        /// <summary>
        /// 获取两个3D坐标的水平面距离
        /// </summary>
        /// <param name="v1">3D坐标1</param>
        /// <param name="v2">3D坐标2</param>
        /// <returns></returns>
        public static double GetXZ_Distance(Vector3 v1, Vector3 v2)
        {
            return Math.Sqrt((v2.x - v1.x) * (v2.x - v1.x) + (v2.z - v1.z) * (v2.z - v1.z));
        }

        /// <summary>
        /// 清除指定游戏对象数组的所有元素
        /// </summary>
        /// <param name="arr">指定的游戏对象数组</param>
        public static void DestroyAllEle(GameObject[] arr)
        {
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(arr[i]);
                }
            }
        }

        /// <summary>
        /// 清除指定数组的所有元素
        /// </summary>
        /// <param name="arr">指定数组</param>
        public static void DestroyAllEle(Component[] arr)
        {
            if (arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(arr[i]);
                }
            }
        }

        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <returns>返回加载起来的GameObject</returns>
        public static GameObject LoadAsset(string path)
        {
            if (path != null)
            {
                try
                {
                    return Resources.Load(path, typeof(GameObject)) as GameObject;
                }
                catch
                {
                    Debug.Log("警告：加载失败...");
                    return null;
                }
            }
            else
            {
                Debug.Log("警告：路径为空！");
                return null;
            }
        }

        /// <summary>
        /// 隐藏指定对象的子对象
        /// </summary>
        /// <param name="father">指定对象</param>
        public static void HideAChildren(Transform father)
        {
            if (father != null)
            {
                foreach (Transform child in father)
                {
                    HideGO(child);
                }
            }
        }

        /// <summary>
        /// 隐藏指定对象的子对象
        /// </summary>
        /// <param name="father">指定对象</param>
        public static void HideChildren(GameObject father)
        {
            if (father != null)
            {
                foreach (Transform child in father.transform)
                {
                    HideGO(child.gameObject);
                }
            }
        }
        /// <summary>
        /// 复制UI元素（参数为：被复制的UI元素，localPosition是否一致，localRotation是否一致，localScale是否一致）
        /// </summary>
        /// <param name="target">被复制的UI元素</param>
        /// <returns></returns>
        public static GameObject Clone(GameObject target, params bool[] args)
        {
            if (target)
            {
                GameObject obj = MonoBehaviour.Instantiate(target);
                obj.transform.SetParent(target.transform.parent);
                if (args != null)
                {
                    if (args.Length == 1)
                    {
                        if (args[0])
                            obj.transform.localPosition = target.transform.localPosition;
                    }
                    else if (args.Length == 2)
                    {
                        if (args[0])
                            obj.transform.localPosition = target.transform.localPosition;
                        if (args[1])
                            obj.transform.localRotation = target.transform.localRotation;
                    }
                    else if (args.Length == 3)
                    {
                        if (args[0])
                            obj.transform.localPosition = target.transform.localPosition;
                        if (args[1])
                            obj.transform.localRotation = target.transform.localRotation;
                        if (args[2])
                            obj.transform.localScale = target.transform.localScale;
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
        /// 获取当前对象及当前对象的所有子对象
        /// </summary>
        /// <param name="father">当前对象</param>
        /// <param name="ObjList">用来存储当前对象及所有子对象的List</param>
        /// <returns>返回存储对象的List</returns>
        public static void GetAllChildren(GameObject father, ref List<GameObject> ObjList)
        {
            if (ObjList != null)
            {
                if (father)
                {
                    ObjList.Add(father);
                    foreach (Transform item in father.transform)
                        GetAllChildren(item.gameObject, ref ObjList);
                }
                else Debug.Log("警告：指定对象为空，请检查赋值！");
            }
            else Debug.Log("警告：请先实例化List...");
        }

        /// <summary>
        /// 获取当前对象及当前对象的所有子对象
        /// </summary>
        /// <param name="father">当前对象</param>
        /// <param name="ObjList">用来存储当前对象及所有子对象的List</param>
        /// <returns>返回存储对象的List</returns>
        public static void GetAllChildren(Transform father, ref List<Transform> ObjList)
        {
            if (ObjList != null)
            {
                if (father)
                {
                    ObjList.Add(father);
                    foreach (Transform item in father)
                        GetAllChildren(item, ref ObjList);
                }
                else Debug.Log("警告：指定对象为空，请检查赋值！");
            }
            else Debug.Log("警告：请先实例化List...");
        }

        /// <summary>
        /// 获取当前对象所有一级子对象
        /// </summary>
        /// <param name="father">当前对象</param>
        /// <param name="ObjList">用来存储当前对象及所有子对象的List</param>
        /// <returns>返回存储对象的List</returns>
        public static List<Transform> GetOne_LevelChidren(Transform father, out List<Transform> ObjList)
        {
            ObjList = new List<Transform>();
            if (father)
            {
                for (int i = 0; i < father.childCount; i++)
                {
                    ObjList.Add(father.GetChild(i));
                }
                return ObjList;
            }
            else
            {
                Debug.Log("警告：传进的参数为空...");
                return ObjList;
            }
        }

        /// <summary>
        /// 获取当前对象及当前对象的所有子对象
        /// </summary>
        /// <param name="father">当前对象</param>
        /// <param name="ObjList">用来存储当前对象及所有子对象的List</param>
        /// <returns>返回存储对象的List</returns>
        public static List<Transform> GetAllChildren(Transform father, List<Transform> ObjList)
        {
            if (ObjList == null) ObjList = new List<Transform>();
            ObjList.Add(father);
            foreach (Transform item in father)
            {
                GetAllChildren(item, ObjList);
            }
            return ObjList;
        }

        /// <summary>
        /// 获取指定父对象下的指定子对象
        /// </summary>
        /// <param name="father">指定父对象</param>
        /// <param name="childName">指定子对象</param>
        /// <returns></returns>
        public static GameObject GetChild(GameObject father, string childName)
        {
            if (father)
            {
                if (father.transform.Find(childName)) return father.transform.Find(childName).gameObject;
                else
                {
                    Debug.Log("警告：未能找到" + childName + "子对象！");
                    return null;
                }
            }
            else
            {
                Debug.Log("警告：指定父对象为空，请检查赋值！");
                return null;
            }
        }

        /// <summary>
        /// 获取指定父对象下的指定子对象
        /// </summary>
        /// <param name="father">指定父对象</param>
        /// <param name="childName">指定子对象</param>
        /// <returns></returns>
        public static Transform GetChild(Transform father, string childName)
        {
            if (father)
            {
                if (father.Find(childName)) return father.Find(childName);
                else
                {
                    Debug.Log("警告：未能找到" + childName + "子对象！");
                    return null;
                }
            }
            else
            {
                Debug.Log("警告：指定父对象为空，请检查赋值！");
                return null;
            }
        }

        /// <summary>
        /// 获取父对象下的同名子对象
        /// </summary>
        /// <param name="father">指定父对象</param>
        /// <param name="childName">子对象名字</param>
        /// <param name="childrenList">存储指定子对象的List</param>
        /// <returns></returns>
        public static void GetAllTheChild(Transform father, string childName, List<Transform> childrenList)
        {
            if (childrenList == null) childrenList = new List<Transform>();
            if (father)
            {
                foreach (Transform item in father)
                {
                    if (item != null && item.name.Equals(childName)) childrenList.Add(item);
                    GetAllTheChild(item, childName, childrenList);
                }
            }
            else Debug.Log("警告：指定父对象为空，请检查赋值！");
        }

        /// <summary>
        /// 获取父对象下的同名子对象
        /// </summary>
        /// <param name="father">指定父对象</param>
        /// <param name="childName">子对象名字</param>
        /// <param name="childrenList">存储指定子对象的List</param>
        /// <returns></returns>
        public static void GetAllTheChild(Transform father, string childName, out List<Transform> childrenList)
        {
            childrenList = new List<Transform>();
            if (father)
            {
                foreach (Transform item in father)
                {
                    if (item != null && item.name.Equals(childName)) childrenList.Add(item);
                    GetAllTheChild(item, childName, childrenList);
                }
            }
            else Debug.Log("警告：指定父对象为空，请检查赋值！");
        }

        /// <summary>
        /// 获取随机颜色
        /// </summary>
        /// <returns></returns>
        public static Color getRandomColor()
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);
            return new Color(r, g, b);
        }


        /// <summary>
        /// 删除指定的对象数组的所有子元素
        /// </summary>
        /// <param name="go">指定的对象</param>
        public static void ClearChildren(GameObject go)
        {
            if (go && go.transform.childCount > 0)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                    MonoBehaviour.Destroy(go.transform.GetChild(i).gameObject);
            }
        }


        /// <summary>
        /// 删除指定的对象数组的所有子元素
        /// </summary>
        /// <param name="go">指定的对象</param>
        public static void ClearChildren(Component go)
        {
            if (go && go.transform.childCount > 0)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                    MonoBehaviour.DestroyImmediate(go.transform.GetChild(i).gameObject);
            }
        }

        public static void ChangeChildrenOrder(GameObject go)
        {
            if (go && go.transform.childCount>0)
            {
                Transform trans = go.transform;
                int count = trans.childCount;
                for (int i = 0; i < count; i++)
                {
                    trans.GetChild(i).SetSiblingIndex(count-i-1);
                }
            }
        }


    }


}
