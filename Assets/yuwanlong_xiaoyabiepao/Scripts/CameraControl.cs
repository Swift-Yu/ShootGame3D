using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets {
    public class CameraControl : MonoBehaviour {


        #region Unity3d设置

        //public float flipTime = 1.5f;

        //public float moveTime = 0.5f;

        public int Count = 5;


        #endregion

        private bool isfound = false;

        //private float time;
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Camera selfCamera;

        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject shield;


        private Queue<Vector3> totalsPoints = new Queue<Vector3>(100);

        private Queue<Quaternion> totlesRotation = new Queue<Quaternion>(100);

        private bool isFllow = false;
        void Start() {
            selfCamera = GetComponent<Camera>();
            mainCamera = Camera.main;
            selfCamera.enabled = false;
        }

        private void Update() {
            if (isFllow) {
                if (totalsPoints.Count == Count) {
                    transform.position = getAvarage();
                    transform.rotation = getAvarageQ();
                    totalsPoints.Dequeue();
                    totlesRotation.Dequeue();
                    //Debug.Log("avarage debug.....................");
                }
                else {
                    selfCamera.transform.position = mainCamera.transform.position;
                    selfCamera.transform.rotation = mainCamera.transform.rotation;
                }
                totalsPoints.Enqueue(mainCamera.transform.position);
                totlesRotation.Enqueue(mainCamera.transform.rotation);
            }
        }


        private Vector3 getAvarage() {

            Vector3 p = Vector3.zero;
            foreach (var totalsPoint in totalsPoints) {
                p += totalsPoint;
            }
            p = p / totalsPoints.Count;
            return p;
        }

        private Quaternion getAvarageQ() {

            Quaternion p = new Quaternion(0, 0, 0, 0);
            foreach (var totalsPoint in totlesRotation) {
                p.x += totalsPoint.x;
                p.y += totalsPoint.y;
                p.z += totalsPoint.z;
                p.w += totalsPoint.w;
            }
            p.x = p.x / totlesRotation.Count;
            p.y = p.y / totalsPoints.Count;
            p.z = p.z / totalsPoints.Count;
            p.w = p.w / totalsPoints.Count;
            return p;

        }


        public void StartFllowMainCamera() {

            transform.position = mainCamera.transform.position;
            transform.rotation = mainCamera.transform.rotation;
            //SwapToSelfCamera();
            isFllow = true;
        }

        public void StopFllowMainCamera() {
            SwapToMainCamera();
            isFllow = false;
            totalsPoints.Clear();
            totlesRotation.Clear();
            
            
        }

        public void SwapToMainCamera() {
            if (mainCamera == null)
                mainCamera = Camera.main;
            selfCamera.enabled = false;
            SetCameraLayout(mainCamera, -1);

        }

        public void SwapToSelfCamera() {

            if (mainCamera == null)
                mainCamera = Camera.main;



            SetCameraLayout(mainCamera, 31);
            SetComponetLayout(mainCamera, mainCamera.transform, 31);
            selfCamera.enabled = true;
            selfCamera.cullingMask = -1;
            CullCameraLayout(selfCamera, 31);
            selfCamera.clearFlags = CameraClearFlags.Depth;
            selfCamera.depth = 10;

            //StartCoroutine(moveToNormalView(selfCamera.transform, srcPosition, srcRotation, Vector3.one));
        }

        public static void SetCameraLayout(Camera camera, params int[] layouts) {
            int num = 0;
            if (layouts.Length == 1) {
                num = layouts[0] == -1 || layouts[0] == 0 ? layouts[0] : 1 << layouts[0];
            }
            else {
                for (int index = 0; index < layouts.Length; ++index)
                    num |= 1 << layouts[index];
            }
            camera.cullingMask = num;
        }

        public static Camera CullCameraLayout(Camera camera, int layout) {
            camera.cullingMask &= ~(1 << layout);
            return camera;
        }

        public static void SetComponetLayout(Camera camera, Transform compont, int layout) {
            foreach (Component componentsInChild in compont.GetComponentsInChildren<Transform>(true))
                componentsInChild.gameObject.layer = layout;
            compont.gameObject.layer = layout;
        }

        //private IEnumerator moveToNormalView(Transform t, Vector3 p, Quaternion r, Vector3 s) {

        //    var ps = t.localPosition;
        //    var pr = t.localRotation;
        //    var pss = t.localScale;

        //    for (float tt = 0; tt < moveTime; tt = tt + Time.deltaTime) {
        //        t.localPosition = Vector3.Lerp(ps, p, tt / moveTime);
        //        t.localRotation = Quaternion.Slerp(pr, r, tt / moveTime);
        //        t.localScale = Vector3.Lerp(pss, s, tt / moveTime);
        //        yield return null;
        //    }

        //    t.localPosition = p;
        //    t.localRotation = r;
        //    t.localScale = s;

        //}


        public void SetPlayerParent(bool isARCamera)
        {
            if (!isARCamera)
            {
                player.transform.parent = selfCamera.transform;
                shield.transform.parent = selfCamera.transform;
            }
        }
    }
}