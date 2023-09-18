using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class CutObj : GrabbableEvents
    {
        public Transform LassoFigure;
        public bool isDrawObj;


        public override void OnGrab(Grabber grabber)
        {
            transform.GetComponent<Collider>().isTrigger = false;

            base.OnGrab(grabber);
        }
        public override void OnRelease()
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;

            DrawCollageObj();

            base.OnRelease();
        }
       
        void DrawCollageObj()
        {
            if(isDrawObj)
            {
                LassoFigure.transform.gameObject.layer = LayerMask.NameToLayer("WhiteBoard");
                transform.GetComponent<Rigidbody>().isKinematic = true;

                Invoke("DestroyInvoke", 0.05f);
            }
        }
        void DestroyInvoke()//캔버스에 그려질 시간 지연용
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.name.Equals("PaintCanvas"))
            {
                isDrawObj = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            if (collision.transform.name.Equals("PaintCanvas"))
            {
                isDrawObj = false;
            }
        }
    }
}
