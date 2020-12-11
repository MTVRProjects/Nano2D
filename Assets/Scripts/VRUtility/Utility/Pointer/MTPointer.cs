//================================================
//描述 ：手柄射线与UI交互 
//作者 ：HML
//创建时间 ：2020/11/17 16:01:17  
//版本：1.0 
//================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HMLFramwork;

namespace MiaoTech.VR
{
    [RequireComponent(typeof(Camera), typeof(LineRenderer))]
    public class MTPointer : MonoBehaviour
    {
        [Header("是否启用该功能")]
        [SerializeField] bool isEnablePointer = true;

        [Header("是否需要手柄射线末端的类鼠标对象")]
        [SerializeField] bool isNeedCursor = true;

        [Header("手柄射线末端的类鼠标对象(可为空，为空时自动创建一个小球)")]
        [Tooltip("为空时，会自动创建一个小球")]
        [SerializeField] GameObject Cursor;

        [Header("手柄射线最大长度")]
        [Range(0.1f, 100f)]
        [SerializeField] float lineMaxLength = 3;

        [Header("手柄射线没有碰到对象时的默认长度")]
        [Range(0.1f, 10f)]
        [SerializeField] float lineDefualtLength = 1.5f;

        [Header("手柄射线宽度")]
        [Range(0.001f, 0.01f)]
        [SerializeField] float lineWidth = 0.006f;

        [Header("是否允许类鼠标对象跟随距离远近缩放")]
        [SerializeField] bool isAllowCursorScaleWithDistance = false;

        [Header("允许类鼠标对象初始大小")]
        [SerializeField] Vector3 initialValue = new Vector3(0.02f,0.02f,0.02f);

        [Header("类鼠标对象跟随距离远近缩放系数")]
        [Range(0, 0.05f)]
        [SerializeField] float scaleFactor = 0.001f;
        /// <summary>
        /// 手柄射线
        /// </summary>
        LineRenderer hand_line;

        EventSystem eventSystem;

        /// <summary>
        /// 开启和关闭Pointer功能
        /// </summary>
        public bool IsEnablePointer
        {
            get { return isEnablePointer; }
            set
            {
                isEnablePointer = value;
                if (isEnablePointer == false)
                {
                    eventSystem.enabled = false;
                }
                else
                {
                    checkEventSystemGO();
                    eventSystem.enabled = false;
                }

                hand_line.enabled = isEnablePointer;
                Cursor.SetActive(isEnablePointer);
            }
        }

        StandaloneInputModule _inputModule;
        StandaloneInputModule inputModule
        {
            get
            {
                if (_inputModule == null)
                {
                    _inputModule = FindObjectOfType<StandaloneInputModule>();
                }
                return _inputModule;
            }
        }


        Transform trans;
        private void Awake()
        {
            trans = transform;
        }

        /// <summary>
        /// 检查场景中是否存在EventSystem和必须的组件，如果没有则创建
        /// </summary>
        void checkEventSystemGO()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                var event_sys_go = new GameObject("EventSystem");
                eventSystem = event_sys_go.AddComponent<EventSystem>();
                _inputModule = event_sys_go.AddComponent<StandaloneInputModule>();
            }
            eventSystem.gameObject.requireComponent<MTPointerInput>().eventCamera = event_cam;
        }

        Camera event_cam;

        Transform cursor_trans;
        // Start is called before the first frame update
        void Start()
        {

            event_cam = GetComponent<Camera>();
            event_cam.enabled = false;
#if OVR
            //设置初始位置
            trans.localPosition = new Vector3(0.01f, 0f, 0.03f);
#endif

            checkEventSystemGO();

            if (IsEnablePointer == false) return;

            hand_line = GetComponent<LineRenderer>();
            hand_line.setWidth(lineWidth, lineWidth);
            if (isNeedCursor && Cursor == null)
            {
                Cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(Cursor.GetComponent<SphereCollider>());
                this.Cursor.GetComponent<Renderer>().material.color = Color.white;

            }
            cursor_trans = Cursor.transform;
            cursor_trans.localScale = initialValue;
            cursor_trans.SetParent(transform);
            
        }

        #region Cache

        RaycastHit hit;
        Ray ray;
        Vector3 endPosition;
        GameObject _hitObject;

        #endregion
        // Update is called once per frame
        void LateUpdate()
        {
            if (IsEnablePointer == false) return;

            float targetLength = lineMaxLength;
            float canvasDistance = GetCanvasDistance();
            if (canvasDistance > 0)
            {
                targetLength = canvasDistance;
            }

            ray = new Ray(trans.position, trans.forward);
            Physics.Raycast(ray, out hit, targetLength);

            Cursor.SetActive(isHaveRaycast);

            if (hit.transform != null)
            {
                Cursor.SetActive(true);
                endPosition = hit.point;
                _hitObject = hit.transform.gameObject;

                if (!hit.transform.CompareTag("UI"))
                {
                    eventSystem.enabled = false;
                }
            }
            else
            {
                if (isHaveRaycast) endPosition = trans.position + trans.forward * targetLength;
                else endPosition = trans.position + (trans.forward * lineDefualtLength);

                _hitObject = null;
                eventSystem.enabled = true;
            }

            //设置Cursor大小
            if (isAllowCursorScaleWithDistance)
            {
                cursor_trans.localScale = initialValue*(1+scaleFactor * targetLength);
            }
            endPosition -= trans.forward * 0.05f;
            cursor_trans.position = endPosition;
            hand_line.SetPosition(0, trans.position);
            hand_line.SetPosition(1, endPosition);
        }



        List<RaycastResult> results = new List<RaycastResult>();
        RaycastResult firstRaycast;

        /// <summary>
        /// 是否有碰撞信息
        /// </summary>
        bool isHaveRaycast = true;
        float GetCanvasDistance()
        {
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = inputModule.inputOverride.mousePosition;
            results.Clear();
            eventSystem.RaycastAll(eventData, results);
            firstRaycast = getFirstRaycast(results);

            isHaveRaycast = results.Count > 0 ? true : false;

            float distance = firstRaycast.distance;
            distance = Mathf.Clamp(distance, 0.0f, lineMaxLength);
            return distance;
        }


        //FindFirstRaycast
        RaycastResult getFirstRaycast(List<RaycastResult> results)
        {
            foreach (RaycastResult result in results)
            {
                if (!result.gameObject) continue;
                return result;
            }
            return new RaycastResult();
        }

    }
}

