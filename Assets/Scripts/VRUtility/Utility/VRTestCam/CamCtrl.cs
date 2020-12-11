//================================================
//描述 ： 
//作者 ：
//创建时间 ：2020/11/17 17:17:27  
//版本： 
//================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HMLFramwork.TEST
{
    public class CamCtrl : MonoBehaviour
    {
        [Header("水平移动速度")]
        [Range(0, 100f)]
        public float MoveSpeed = 0.5F;

        [Header("上下移动速度")]
        [Range(0, 100f)]
        public float UpDownSpeed = 1F;

        [Header("旋转速度")]
        [Range(0, 100f)]
        public float RotateSpeed = 0.5F;


        Transform trans;
        Camera cam;
        CharacterController controller;


        void Start()
        {
            trans = transform;
            cam = GetComponent<Camera>();
            controller = GetComponent<CharacterController>();
        }

        Vector3 move_value;
        // Update is called once per frame
        Vector3 forward_temp_H;
        Vector3 forward_temp_V;


        void Update()
        {
            forward_temp_H = trans.forward * Input.GetAxis("Horizontal") * MoveSpeed;
            forward_temp_V = trans.forward * Input.GetAxis("Vertical") * MoveSpeed;
            float up_down_value = Input.GetAxis("Mouse ScrollWheel")* UpDownSpeed;

            move_value.x = forward_temp_V.x;
            move_value.z = forward_temp_V.z;
            controller.Move(move_value);

            move_value.x = forward_temp_H.z;
            move_value.z = -forward_temp_H.x;
            move_value.y = up_down_value;
            controller.Move(move_value);

            if (Input.GetMouseButton(1))
            {

                float yRot = Input.GetAxis("Mouse X") ;
                float xRot = Input.GetAxis("Mouse Y");

                trans.Rotate(Vector3.up, yRot* RotateSpeed, Space.World);
                trans.Rotate(Vector3.right, -xRot* RotateSpeed, Space.Self);

            }
          
        }

    }
}
