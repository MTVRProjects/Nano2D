using HMLFramwork;
using HMLFramwork.FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiaoTech.VR
{
    public class Teleport :MonoBehaviour, IState
    {
        public event Action enterEventHandle = () => { };
        public event Action exitEventHandle = () => { };
        public void Enter()
        {
            if (DisableFunction) return;

            CalculateArc();
            EnableGuideLine();
            enterEventHandle();
        }

        public void Exit()
        {
            if (DisableFunction) return;

            exitEventHandle();
            TeleportToTargetPos();
            DisableGuideLine();
        }

        [Header("是否禁用功能")]
        public bool DisableFunction = false;
        [Header("射线目标Tag")]
        public List<string> RayHitTag = new List<string>();

        [Header("传送最远距离")]
        /// <summary>
        /// 传送的最远距离
        /// </summary>
        [Range(0f,50f)]
        public float maxDistance = 5f;

        [Header("可传送时引导线的材质")]
        /// <summary>
        /// 当可瞬移目的地为可移动时，抛物线使用的材质
        /// </summary>
        public Material goodTeleMat;

        [Header("不可传送时引导线的材质")]
        /// <summary>
        /// 当可瞬移目的地为不可移动时，抛物线使用的材质
        /// </summary>
        public Material badTeleMat;
        public float matScale = 5;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [Header("引导线移动速度")]
        /// <summary>
        /// 抛物线贴图的移动速度
        /// </summary>
        public Vector2 texMovementSpeed = new Vector2(-0.05f, 0);
        [Header("可传送时，抛物线的颜色")]
        /// <summary>
        /// 可传送时，抛物线的颜色
        /// </summary>
        public Color goodSpotColor = new Color(0, 0.6f, 1f, 0.2f);
        [Header("不可传送时，抛物线的颜色")]
        /// <summary>
        /// 不可传送时，抛物线的颜色
        /// </summary>
        public Color badSpotColor = new Color(0.8f, 0, 0, 0.2f);
        [Header("抛物线的宽度")]
        /// <summary>
        /// 抛物线的宽度
        /// </summary>
        public float arcLineWidth = 0.02F;
        [Header("传送目标点引导对象")]
        /// <summary>
        /// 传送位置引导对象
        /// </summary>
        public GameObject TeleportPosGuideGO;
        /// <summary>
        /// 传送位置引导对象实例
        /// </summary>
        GameObject _teleportPosGuideGOInstance;

        /// <summary>
        /// VR相机对象
        /// </summary>
        private Transform _vrCameraTransform;

        /// <summary>
        /// 线条渲染器1
        /// </summary>
        private LineRenderer _lineRenderer;
        /// <summary>
        /// 传输点
        /// </summary>
        private Vector3 _teleportSpot;
        /// <summary>
        /// 当前选择的点是否可以移动
        /// </summary>
        private bool _goodSpot;
        /// <summary>
        /// 传送是否是激活状态
        /// </summary>
        private bool _teleportActive;
        public bool teleportActive
        {
            get { return _teleportActive; }
        }

        void Start()
        {
            JudgeVRCamera();
          
            //初始化
            Init();
        }

        /// <summary>
        /// 初始化操作（包括初始化一些对象）
        /// </summary>
        public void Init()
        {
            GameObject arcLine1 = new GameObject("ArcLine1");
            arcLine1.transform.SetParent(transform);
            _lineRenderer = arcLine1.AddComponent<LineRenderer>();

            _lineRenderer.setWidth(arcLineWidth, arcLineWidth);
            
            if (goodTeleMat == null)
            {
                _lineRenderer.setColor(goodSpotColor, goodSpotColor);
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
            else
            {
                _lineRenderer.material = goodTeleMat;
            }
            _lineRenderer.enabled = false;

            if (TeleportPosGuideGO != null)
            {
                _teleportPosGuideGOInstance = (GameObject)Instantiate(TeleportPosGuideGO, Vector3.zero, Quaternion.identity);
                _teleportPosGuideGOInstance.transform.SetParent(transform);
                _teleportPosGuideGOInstance.transform.localScale = Vector3.one;
                _teleportPosGuideGOInstance.SetActive(false);
            }
        }

        //计算抛物线弧度
        private void CalculateArc()
        {
            List<Vector3> positions1 = new List<Vector3>();
            List<Vector3> positions2 = new List<Vector3>();

            Quaternion currentRotation = transform.rotation;
            Vector3 currentPosition = transform.position;
            positions1.Add(currentPosition);
            Vector3 lastPostion = transform.position - transform.forward;
            Vector3 currentDirection = transform.forward;
            Vector3 downForward = new Vector3(transform.forward.x * 0.01f, -1, transform.forward.z * 0.01f);
            RaycastHit hit = new RaycastHit();
            float totalDistance1 = 0;
            float totalDistance2 = 0;

            bool useFirstArray = true;

            int i = 0;
            while (i < 500)
            {
                i++;
                Quaternion downQuat = Quaternion.LookRotation(downForward);
                currentRotation = Quaternion.RotateTowards(currentRotation, downQuat, 1f);
                Ray newRay = new Ray(currentPosition, currentPosition - lastPostion);
                float length = maxDistance * 0.01f;
                if (currentRotation == downQuat)
                {
                    useFirstArray = false;
                    length = maxDistance * matScale;
                    positions2.Add(currentPosition);
                }

                //判断射线是否碰撞到指定对象
                bool hitSomething = false;
                if (RayHitTag.Count < 1)
                {
                    Debug.Log("警告：射线无目标Tag！");
                    return;
                }
                hitSomething = Physics.Raycast(newRay, out hit, length);
                /* 在这对抛物线碰撞的物体进行判断，当碰撞的是可移动的位置时，则移动；否则改变抛物线箭头颜色为不可移动    */
                if (hitSomething)//如果碰撞对象不为空
                {
                    //取决于我们是否有切换到第一或第二线渲染器添加点并完成总距离的计算。
                    if (useFirstArray)
                    {
                        totalDistance1 += (currentPosition - hit.point).magnitude;
                        positions1.Add(hit.point);
                    }
                    else
                    {
                        totalDistance2 += (currentPosition - hit.point).magnitude;
                        positions2.Add(hit.point);
                    }
                    break;
                }

                //将旋转转换为一个正前方向量，然后应用到我们的当前位置
                currentDirection = currentRotation * Vector3.forward;
                lastPostion = currentPosition;
                currentPosition += currentDirection * length;

                if (useFirstArray)//使用第一个渲染器
                {
                    totalDistance1 += length;
                    positions1.Add(currentPosition);
                }
                else//使用第二个渲染器
                {
                    totalDistance2 += length;
                    positions2.Add(currentPosition);
                }

                if (currentRotation == downQuat) break;
            }

            if (useFirstArray)//如果用第一个渲染器
            {
                _teleportSpot = positions1[positions1.Count - 1];
            }
            else//否则用第二个渲染器
            {
                _teleportSpot = positions2[positions2.Count - 1];
            }

            //如果当前碰撞点可以传送，则传送到此点
            _goodSpot = IsGoodSpot(hit);

            if (_goodSpot)
            {
                if (goodTeleMat == null)
                {
                    _lineRenderer.setColor(goodSpotColor, goodSpotColor);
                }
                else
                {
                    if (_lineRenderer.material.mainTexture.name != goodTeleMat.mainTexture.name) _lineRenderer.material = goodTeleMat;

                }

            }
            else
            {
                if (badTeleMat == null)
                {
                    _lineRenderer.setColor(badSpotColor, badSpotColor);
                }
                else
                {
                    if (_lineRenderer.material.mainTexture.name != badTeleMat.mainTexture.name) _lineRenderer.material = badTeleMat;
                }
            }
            //确定引导对象的位置
            if (_teleportPosGuideGOInstance != null)
            {
                if (_goodSpot)
                {
                    _teleportPosGuideGOInstance.SetActive(true);
                    _teleportPosGuideGOInstance.transform.position = _teleportSpot + (hit.normal * 0.05f);
                    if (hit.normal == Vector3.zero)
                        _teleportPosGuideGOInstance.transform.rotation = Quaternion.identity;
                    else
                        _teleportPosGuideGOInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
                else
                {
                    _teleportPosGuideGOInstance.SetActive(false);
                }
            }

            //渲染弧线
            _lineRenderer.positionCount = positions1.Count;
            _lineRenderer.SetPositions(positions1.ToArray());
            _lineRenderer.material.mainTextureScale = new Vector2(totalDistance1 * matScale, 1);
            _lineRenderer.material.mainTextureOffset = new Vector2(_lineRenderer.material.mainTextureOffset.x + texMovementSpeed.x, _lineRenderer.material.mainTextureOffset.y + texMovementSpeed.y);
        }

        //改变位置到指定的良好着陆点
        protected bool IsGoodSpot(RaycastHit hit)
        {
            if (hit.transform == null) return false;
            if (RayHitTag != null)
            {
                for (int i = 0; i < RayHitTag.Count; i++)
                {
                    if (hit.transform.tag.Equals(RayHitTag[i]))
                        return true;
                    else return false;
                }
            }
            else return false;
            return true;
        }

        //可传送，开始渲染抛物线,在Update中更新
         public void EnableGuideLine()
        {
            _lineRenderer.enabled = true;
            _teleportActive = true;
        }
      
        //不可传送
         public void DisableGuideLine()
        {
            _lineRenderer.enabled = false;
            _teleportActive = false;
            if (_teleportPosGuideGOInstance != null) _teleportPosGuideGOInstance.SetActive(false);
        }

        //传送
         public void TeleportToTargetPos()
        {
            if (teleportActive && _goodSpot)
            {
                Vector3 camSpot = new Vector3(_vrCameraTransform.position.x, 0, _vrCameraTransform.position.z);
                Vector3 roomSpot = new Vector3(MTVRInput.Ins.VRCamRoot.position.x, 0, MTVRInput.Ins.VRCamRoot.position.z);
                Vector3 offset = roomSpot - camSpot;
                MTVRInput.Ins.VRCamRoot.position = _teleportSpot + offset;
            }
        }

        /// <summary>
        /// 判断VR相机是否存在
        /// </summary>
        void JudgeVRCamera()
        {
            _vrCameraTransform = MTVRInput.Ins.VRCam;

            if (_vrCameraTransform == null)
            {
                Debug.LogError("警告：传送功能找不到VR相机!");
                enabled = false;
                return;
            }
        }

    }
}
