using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Diagnostics;




/// <summary>
/// AR 모드 상에서만 물체를 이동시키는 예시
/// </summary>
public class PlaneMgr : MonoBehaviour

{
    //위치에 맞게 앤디를 만들어주는 코드.

    Stopwatch SW = new Stopwatch();
    public Camera arCamera;
    public Camera VirCamera;


    //public GameObject chair;
   // public GameObject Greencube;
    public GameObject Redcube;
    public GameObject Floor;
    public GameObject wall;
    public GameObject RealPosOb;
    public GameObject RealPosOb1;

   


    //public GameObject Point;

    public GameObject LeftArrow;
    public GameObject RightArrow;
    public GameObject UpArrow;
    public GameObject DownArrow;


    public int ButtonMode = 0;//0이 Wall 1이 object
    public int ViewMode = 1;//0일때 AR, 1일 때 미니맵 버전
    int touchtime=0;
    public int StageMode = 0;


    public Text hitPos;
    public Text StageText;


    Vector3[] HitPlanePos = new Vector3[10];
    Vector4[] HitPlaneRot = new Vector4[10];
    Quaternion[] HitPlanQ = new Quaternion[10];

    float Hity=0.0f;//바닥 높이 지정에 사용

    public Vector3[] CrossPos = new Vector3[10];
    public Vector3[] ARCrossPos = new Vector3[10];
    public Vector3[] AR_Real_Position = new Vector3[10];

    float[] Walltheta = new float[10];


    GameObject[] WallArray = new GameObject[10];//가상 오브젝트 배치 어레이
    GameObject[] VirObject = new GameObject[3];
    GameObject[] ARObject = new GameObject[3];

    GameObject[] REALP = new GameObject[5];



    int hitPosNum = 0;//터치횟수

    //float distance = 0.0f;//거리측정
    float ButtonDistance = 0.2f;
    
    float HalfH = 0.0f;
    float HalfBox = 0.16f;

    int WallNum = 0;
    int VirObNum = 0;

    //DRAG용 변수

    private Vector3 screenSpace;
    private Vector3 offset;
    //private bool Isdrag = false;


    //드래그용 버튼

    int IsDragAnd = 0;

    public Button Drag;
    public Button NonDrag;

    Vector3 FirstAR;//첫 클릭위치
    Vector3 FirstVR;


    //Initialize Variables
    GameObject getTarget;
    bool isMouseDragging;
    Vector3 offsetValue;
    Vector3 positionOfScreen;





    // Start is called before the first frame update
    void Start()
    {
        arCamera = Camera.main;

        LeftArrow.SetActive(true);
        RightArrow.SetActive(true);
        UpArrow.SetActive(true);
        DownArrow.SetActive(true);

        ViewMode = 1;

    }



    Vector3 CalCrossPoint(Vector3 p0, Vector3 p1, float theta0, float theta1)
    {

        float a0, b0, a1, b1 = 0.0f;

        float x, z;

        a0 = 0.0f;
        theta0 = theta0 * 3.13f / 180.0f;
        theta1 = theta1 * 3.13f / 180.0f;



        if (0 < theta0 && theta0 <= 90.0f)
        {
            a0 = 1.0f / Mathf.Tan(theta0);
        }
        else if (90.0f < theta0 && theta0 <= 180.0f)
        {

            a0 = -Mathf.Tan(theta0 - 90.0f);
        }
        else if (180.0f < Walltheta[0] && Walltheta[0] <= 270.0f)
        {
            a0 = 1.0f / Mathf.Tan(theta0 - 180.0f);
        }
        else if (270.0f < theta0 && theta0 <= 360.0f)
        {
            a0 = -Mathf.Tan(theta0 - 270.0f);
        }

        a1 = 0.0f;

        if (0 < theta1 && theta1 <= 90.0f)
        {
            a1 = 1.0f / Mathf.Tan(theta1);
        }
        else if (90.0f < theta1 && theta1 <= 180.0f)
        {
            a1 = -Mathf.Tan(theta1 - 90.0f);
        }
        else if (180.0f < theta1 && theta1 <= 270.0f)
        {
            a1 = 1.0f / Mathf.Tan(theta1 - 180.0f);
        }
        else if (270.0f < theta1 && theta1 <= 360.0f)
        {
            a1 = -Mathf.Tan(theta1 - 270.0f);
        }


        b0 = p0.z - a0 * p0.x;
        b1 = p1.z - a1 * p1.x;

        x = ((b1 - b0) / (a0 - a1));
        z = a0 * x + b0;

        return new Vector3(x, 0.5f, z);


    }

    public void SynPos() {

        //dlfeks 0에 대해서
        
            
            Vector3 CurPos = VirObject[0].transform.position;

            Vector3 tempPos = CurPos - FirstVR;

            Vector3 tempy = new Vector3(tempPos.x, 0.0f, tempPos.z); 

            Vector3 newPos = FirstAR;

            newPos = newPos + tempy;

            ARObject[0].transform.rotation = VirObject[0].transform.rotation;

            ARObject[0].transform.position = newPos;



    }


    public void SynPosAtoV()
    {

        //dlfeks 0에 대해서
      
            Vector3 CurPos = ARObject[0].transform.position;

            Vector3 tempPos = CurPos - FirstAR;

            Vector3 tempy = new Vector3(tempPos.x, 0.0f, tempPos.z);

            Vector3 newPos = FirstVR;

            newPos = newPos + tempy;

            VirObject[0].transform.position = newPos;


    }

  
    // Update is called once per frame
    void Update()
    {
        //Mouse Button Press Down
        if (Input.GetMouseButtonDown(0) && IsDragAnd == 1 )
        {
            
            touchtime++;

            RaycastHit hitInfo;

            getTarget = ReturnClickedObject(out hitInfo);
            
            touchtime++;
            if (getTarget != null)
            {
                //hitPos.text += "객체클릭" + touchtime;
                isMouseDragging = true;

                //Converting world position to screen position.
                positionOfScreen = Camera.main.WorldToScreenPoint(getTarget.transform.position);
                offsetValue = getTarget.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, positionOfScreen.z));
            }
            else
            {
               // hitPos.text += "안됨" + touchtime;
                touchtime++;
            }
        }

        //Mouse Button Up
        else if (Input.GetMouseButtonUp(0) && IsDragAnd == 1 )
        {
            isMouseDragging = false;
        }

        //Is mouse Moving
        if (isMouseDragging && IsDragAnd == 1  )
        {
            //tracking mouse position.
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, positionOfScreen.z);

            //converting screen position to world position with offset changes.
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offsetValue;
            currentPosition.y = FirstAR.y;
            //It will update target gameobject's current postion.
            ARObject[0].transform.position = currentPosition;

            SynPosAtoV();
            //SynPos();
        }

        Touch touch = Input.GetTouch(0);


        if (Input.touchCount > 0 && touch.phase == TouchPhase.Began )//1이상 눌리고, phase가 시작상태면.버튼 범위는 제외하자
        {
            TrackableHit hit;//터치에 해당하는 정보
            TrackableHitFlags flags = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

           // hitPos.text += "click:\n" + "x:" + touch.position.x + "y:" + touch.position.y + "z:" + "\n";

            if (Frame.Raycast(touch.position.x, touch.position.y, flags, out hit) && touch.position.x < 2000.0f)
            {//생성이 가능한지 판단, 평면 위인지를 판단하는 함수
                //&& touch.position.x < 2000.0f
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);//이게 클릭된 위치값인듯

                HitPlanePos[hitPosNum] = hit.Pose.position;//클릭한 좌표값 정보 입력


                //위치 계산 부분

                HitPlanePos[hitPosNum].x = HitPlanePos[hitPosNum].x + 100.0f;//Floor 위치 기반으로 X두배
                HitPlanePos[hitPosNum].y = Floor.transform.position.y + HalfH; //높이 맞추려고 1 더했음
                HitPlanePos[hitPosNum].z = HitPlanePos[hitPosNum].z + 100.0f;//Floor 위치 기반으로 X두배

                Vector3 HRotation = hit.Pose.rotation.eulerAngles;
                HRotation.x = 0.0f;
                HRotation.z = 0.0f;//회전 오차 잡아줌


                Quaternion hQ = Quaternion.identity;
                hQ = Quaternion.Euler(HRotation);

                HitPlanQ[hitPosNum] = hQ;

               

                Vector3 HRotation1 = hit.Pose.rotation.eulerAngles;

                Quaternion hQ1 = Quaternion.identity;
                hQ1 = Quaternion.Euler(HRotation1);


                if (HitPlanePos[hitPosNum].y > 0)
                {
                    HRotation1.x = 0.0f;
                    hQ1 = Quaternion.Euler(HRotation1);
                }
                else
                {
                    hQ1 = Quaternion.Euler(HRotation1);

                }

                //생성코드 위치, 형태연산하기

                if (WallNum == 0 && ButtonMode == 0)
                {
                    //벽이 0개고 벽을 하나 세워야한다면
                    // ARobject[0] = Instantiate(Greencube, hit.Pose.position, hQ1, anchor.transform);//AR 화면상
                    WallArray[0] = Instantiate(wall);

                    WallArray[0].transform.localScale = new Vector3(0.1f, 0.1f, 2.0f);
                    WallArray[0].transform.position = HitPlanePos[hitPosNum];

                    WallArray[0].transform.rotation = hQ;

                    WallNum++;

                    Walltheta[0] = HRotation.y;

                }
                else if (WallNum == 1 && ButtonMode == 0)
                {
                    //벽이 하나 있고 벽을 생성
                    WallArray[1] = Instantiate(wall);

                    WallArray[1].transform.localScale = new Vector3(0.1f, 0.1f, 4.0f);
                    WallArray[1].transform.position = HitPlanePos[hitPosNum];
                    WallArray[1].transform.rotation = hQ;

                   

                    Walltheta[1] = HRotation.y;



                    CrossPos[0] = CalCrossPoint(HitPlanePos[0], HitPlanePos[1], Walltheta[0], Walltheta[1]);//첫교점

                    ARCrossPos[WallNum - 1] = new Vector3(CrossPos[WallNum - 1].x - 100f, Hity, CrossPos[WallNum - 1].z - 100f);

                    Vector3 distance = CrossPos[0] - HitPlanePos[0];

                    float dis = distance.magnitude;

                    WallArray[0].transform.localScale = new Vector3(0.1f, 0.1f, 2 * dis);
                    WallArray[0].transform.position = HitPlanePos[0];
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[0].transform.rotation = HitPlanQ[0];
                    WallNum++;

                }
                else if (WallNum == 2 && ButtonMode == 0)
                {
                    //ARobject[2] = Instantiate(Greencube, hit.Pose.position, hQ1, anchor.transform);//AR 화면상

                    WallArray[2] = Instantiate(wall);
                    //  WallArray[2].transform.localScale = new Vector3(0.1f, 1.0f, 4.0f);
                    WallArray[2].transform.position = HitPlanePos[hitPosNum];
                    WallArray[2].transform.rotation = hQ;

                    
                    Walltheta[2] = HRotation.y;

                    CrossPos[1] = CalCrossPoint(HitPlanePos[1], HitPlanePos[2], Walltheta[1], Walltheta[2]);
                    ARCrossPos[WallNum - 1] = new Vector3(CrossPos[WallNum - 1].x - 100f, Hity, CrossPos[WallNum - 1].z - 100f);

                    Vector3 distance = CrossPos[1] - CrossPos[0];
                    float dis = distance.magnitude;

                    WallArray[1].transform.localScale = new Vector3(0.1f, 0.1f, dis);
                    WallArray[1].transform.position = (CrossPos[1] + CrossPos[0]) * 0.5f;
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[1].transform.rotation = HitPlanQ[1];

                    distance = HitPlanePos[2] - CrossPos[1];
                    dis = distance.magnitude;

                    WallArray[2].transform.localScale = new Vector3(0.1f, 0.1f, 2 * dis);
                    WallArray[2].transform.position = HitPlanePos[2];
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[2].transform.rotation = HitPlanQ[2];

                    float DisX = WallArray[2].transform.position.x - WallArray[0].transform.position.x;

                    //ShitPos.text += "Touch:+" + DisX;

                    WallNum++;
                }
                else if (WallNum == 3 && ButtonMode == 0)
                {
                    //ARobject[2] = Instantiate(Greencube, hit.Pose.position, hQ1, anchor.transform);//AR 화면상

                    WallArray[WallNum] = Instantiate(wall);
                    //  WallArray[2].transform.localScale = new Vector3(0.1f, 1.0f, 4.0f);
                    WallArray[WallNum].transform.position = HitPlanePos[hitPosNum];
                    WallArray[WallNum].transform.rotation = hQ;
                    // WallArray[2].GetComponent<MeshRenderer>().material.color = Color.yellow;

                    Walltheta[WallNum] = HRotation.y;

                    CrossPos[WallNum - 1] = CalCrossPoint(HitPlanePos[WallNum - 1], HitPlanePos[WallNum], Walltheta[WallNum - 1], Walltheta[WallNum]);
                    ARCrossPos[WallNum - 1] = new Vector3(CrossPos[WallNum - 1].x - 100f, Hity, CrossPos[WallNum - 1].z - 100f);

                    Vector3 distance = CrossPos[WallNum - 1] - CrossPos[WallNum - 2];
                    float dis = distance.magnitude;

                    WallArray[WallNum - 1].transform.localScale = new Vector3(0.1f, 0.1f, dis);
                    WallArray[WallNum - 1].transform.position = (CrossPos[WallNum - 1] + CrossPos[WallNum - 2]) * 0.5f;
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[WallNum - 1].transform.rotation = HitPlanQ[WallNum - 1];

                    distance = HitPlanePos[WallNum] - CrossPos[WallNum - 1];
                    dis = distance.magnitude;

                    WallArray[WallNum].transform.localScale = new Vector3(0.1f, 0.1f, 2 * dis);
                    WallArray[WallNum].transform.position = HitPlanePos[WallNum];
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[WallNum].transform.rotation = HitPlanQ[WallNum];




                    WallNum++;

                }
                else if (WallNum == 4 && ButtonMode == 0)
                {
                    //ARobject[2] = Instantiate(Greencube, hit.Pose.position, hQ1, anchor.transform);//AR 화면상

                    WallArray[WallNum] = Instantiate(wall);
                    //  WallArray[2].transform.localScale = new Vector3(0.1f, 1.0f, 4.0f);
                    WallArray[WallNum].transform.position = HitPlanePos[hitPosNum];
                    WallArray[WallNum].transform.rotation = hQ;
                    // WallArray[2].GetComponent<MeshRenderer>().material.color = Color.yellow;

                    Walltheta[WallNum] = HRotation.y;

                    CrossPos[WallNum - 1] = CalCrossPoint(HitPlanePos[WallNum - 1], HitPlanePos[WallNum], Walltheta[WallNum - 1], Walltheta[WallNum]);
                    

                    Vector3 distance = CrossPos[WallNum - 1] - CrossPos[WallNum - 2];
                    float dis = distance.magnitude;

                    WallArray[WallNum - 1].transform.localScale = new Vector3(0.1f, 0.1f, dis);
                    WallArray[WallNum - 1].transform.position = (CrossPos[WallNum - 1] + CrossPos[WallNum - 2]) * 0.5f;
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[WallNum - 1].transform.rotation = HitPlanQ[WallNum - 1];

                    distance = HitPlanePos[WallNum] - CrossPos[WallNum - 1];
                    dis = distance.magnitude;

                    WallArray[WallNum].transform.localScale = new Vector3(0.1f, 0.1f, 2 * dis);
                    WallArray[WallNum].transform.position = HitPlanePos[WallNum];
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[WallNum].transform.rotation = HitPlanQ[WallNum];




                    WallNum++;

                }
                else if (WallNum == 5 && ButtonMode == 0)
                {
                    //ARobject[2] = Instantiate(Greencube, hit.Pose.position, hQ1, anchor.transform);//AR 화면상

                    WallArray[WallNum] = Instantiate(wall);
                    //  WallArray[2].transform.localScale = new Vector3(0.1f, 1.0f, 4.0f);
                    WallArray[WallNum].transform.position = HitPlanePos[hitPosNum];
                    WallArray[WallNum].transform.rotation = hQ;
                    // WallArray[2].GetComponent<MeshRenderer>().material.color = Color.yellow;

                    Walltheta[WallNum] = HRotation.y;

                    CrossPos[WallNum - 1] = CalCrossPoint(HitPlanePos[WallNum - 1], HitPlanePos[WallNum], Walltheta[WallNum - 1], Walltheta[WallNum]);
                    ARCrossPos[WallNum - 1] = new Vector3(CrossPos[WallNum - 1].x - 100f, Hity, CrossPos[WallNum - 1].z - 100f);

                    Vector3 distance = CrossPos[WallNum - 1] - CrossPos[WallNum - 2];
                    float dis = distance.magnitude;

                    WallArray[WallNum - 1].transform.localScale = new Vector3(0.1f, 0.1f, dis);
                    WallArray[WallNum - 1].transform.position = (CrossPos[WallNum - 1] + CrossPos[WallNum - 2]) * 0.5f;
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[WallNum - 1].transform.rotation = HitPlanQ[WallNum - 1];

                    distance = HitPlanePos[WallNum] - CrossPos[WallNum - 1];
                    dis = distance.magnitude;

                    WallArray[WallNum].transform.localScale = new Vector3(0.1f, 0.1f, 2 * dis);
                    WallArray[WallNum].transform.position = HitPlanePos[WallNum];
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[WallNum].transform.rotation = HitPlanQ[WallNum];

                    float DIS = CrossPos[0].z - CrossPos[4].z;

                    WallArray[0].transform.localScale = new Vector3(0.08f, 0.1f, DIS);
                    WallArray[0].transform.position = new Vector3(HitPlanePos[0].x, HitPlanePos[0].y, (CrossPos[4].z + CrossPos[0].z) / 2);
                    //hitPos.text += "Touch:" + hitPosNum + "\n" + "x:" + HitPlanePos[hitPosNum].x + "y:" + HitPlanePos[hitPosNum].y + "z:" + HitPlanePos[hitPosNum].z + "\n";
                    WallArray[0].transform.rotation = HitPlanQ[0];





                    WallNum++;

                }

                else if (ButtonMode == 0)
                {   //Wall Mode

                    //Instantiate(Greencube, hit.Pose.position, hQ, anchor.transform);//AR 화면상
                    //Instantiate(Greencube, HitPlanePos[hitPosNum], hit.Pose.rotation, anchor.transform);//Minicam
                    //Instantiate(Greencube, HitPlanePos[hitPosNum], hQ);//가상환경상

                    WallNum++;

                }
                else if (ButtonMode == 1 && VirObNum==0)
                {
                    //Object 모드
                    

                    ARObject[0] = Instantiate(Redcube, hit.Pose.position, hit.Pose.rotation, anchor.transform);
                    Hity = hit.Pose.position.y;
                    //hitPos.text += "Hity:\n"+ Hity;

                    Vector3 temp = ARObject[0].transform.position;
                    temp.y = temp.y + 0.1f;
                    ARObject[0].transform.position = temp;//높이상승

                    FirstAR = ARObject[0].transform.position;//오브젝트 초기 위치저장

                    VirObject[VirObNum] = Instantiate(Redcube, HitPlanePos[hitPosNum], hit.Pose.rotation, anchor.transform);

                    FirstVR = HitPlanePos[hitPosNum];

                    VirObNum++;
                }




                hitPosNum++;
            }
        }





      



    }



    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            target = hit.collider.gameObject;
        }
        return target;
    }

  

    public void IsRLocateButton()
    {
        Vector3 checkReal = new Vector3(ARCrossPos[0].x + HalfBox, ARCrossPos[0].y, ARCrossPos[0].z - HalfBox);
        if (StageMode == 0)
        {
            IsRLocate(AR_Real_Position[0], ARObject[0].transform.position, 0.1f);
        }
        else if (StageMode == 1) {
            IsRLocate(AR_Real_Position[1], ARObject[0].transform.position, 0.1f);
        }
        else if (StageMode == 2)
        {
            IsRLocate(AR_Real_Position[2], ARObject[0].transform.position, 0.1f);
        }
        else if (StageMode == 3)
        {
            IsRLocate(AR_Real_Position[3], ARObject[0].transform.position, 0.1f);
        }
        else if (StageMode == 4)
        {
            IsRLocate(AR_Real_Position[4], ARObject[0].transform.position, 0.15f);
        }
        // hitPos.text += "외부함수끝";
    }

    public void IsRLocate(Vector3 goalPos, Vector3 curPoss, float bound)
    {
        //확인하기 누르면 맞는 장소인지 확인해줌

        //정답거리
        //float realDis = 0.1414f;


        float Dis = Mathf.Sqrt((goalPos.x - curPoss.x) * (goalPos.x - curPoss.x) + (goalPos.z - curPoss.z) * (goalPos.z - curPoss.z));

        //hitPos.text += "Dis:\n" + Dis;
        //hitPos.text += "이프문직전";
        if (bound > Dis)
        {
           
            hitPos.text += "위치성공";

           // REALP[StageMode].SetActive(false);

            if (StageMode == 4) {
                StageText.color = Color.red;
                StageText.text = "Finish";


                SW.Stop();
                StageText.text += "총 수행시간 : "+ SW.ElapsedMilliseconds.ToString() +"ms";
            }

            StageMode++;

            MapSetting();
            

        }
        else{

            hitPos.text += "위치실패";
           // hitPos.text += "RX:"+ goalPos.x+"Rz:"+ goalPos.z+"\n";
            //hitPos.text += "CX:" + curPoss.x + "Cz:" + curPoss.z+ "\n";

        }


    }



    public void RealPosCalculate()
    {
        //1번
        AR_Real_Position[0] = new Vector3(ARCrossPos[0].x + HalfBox, Hity, ARCrossPos[0].z - HalfBox);
       // hitPos.text += "A0X:" + AR_Real_Position[0].x + "A0z:" + AR_Real_Position[0].z + "\n";
        //2번

        AR_Real_Position[1] = new Vector3((ARCrossPos[0].x + ARCrossPos[1].x)/2, Hity, ((ARCrossPos[0].z + ARCrossPos[1].z) / 2)- HalfBox);
       // hitPos.text += "A1X:" + AR_Real_Position[0].x + "A1z:" + AR_Real_Position[0].z + "\n";
        
        //3번
        AR_Real_Position[2] = new Vector3(ARCrossPos[2].x - HalfBox, Hity, ARCrossPos[2].z - HalfBox);

        
        float Gpx = (ARCrossPos[0].x + ARCrossPos[4].x) / 2;
        float Gpz = (ARCrossPos[2].z + ARCrossPos[4].z) / 2;
        //4번
        AR_Real_Position[3] = new Vector3(Gpx + HalfBox + 0.27f, Hity, Gpz);

        //5번

        AR_Real_Position[4] = new Vector3(ARCrossPos[0].x + HalfBox , Hity, Gpz);
        
    }

    public void MapSetting() {

        //Cross point로 계산

        //Vector3 Temp = new Vector3(ARCrossPos[0].x + HalfBox, Hity, ARCrossPos[0].z  - HalfBox);

        RealPosCalculate();

        if (StageMode == 0)
        {

            //REALP[0] =Instantiate(RealPosOb, AR_Real_Position[0], HitPlanQ[0]);
            StageText.text = "STAGE : 1";
            // Instantiate(Redcube, new Vector3(CrossPos[0].x - 100.0f , ARObject[0].transform.position.y, CrossPos[0].z - 100.0f), HitPlanQ[0]);

        }
        else if (StageMode == 1)
        {
            //REALP[1] = Instantiate(RealPosOb1, AR_Real_Position[1], HitPlanQ[0]);
            StageText.text = "STAGE : 2";


        }
        else if (StageMode == 2)
        {
           // REALP[2] = Instantiate(RealPosOb, AR_Real_Position[2], HitPlanQ[0]);
            StageText.text = "STAGE : 3";


        }
        else if (StageMode == 3)
        {
            //REALP[3] = Instantiate(RealPosOb, AR_Real_Position[3], HitPlanQ[0]);
            StageText.text = "STAGE : 4";
        }
        else if (StageMode == 4)
        {
            //REALP[4] = Instantiate(RealPosOb, AR_Real_Position[4], HitPlanQ[0]);
            StageText.text = "STAGE : 5";
        }
    }


    public void ChangeModeWall()
    {

        ButtonMode = 0;
    }

    public void ChangeModeObject()
    {
        ButtonMode = 1;
    }

    public void Change2AR()
    {
        //AR:1
        if (ViewMode == 0)
        {
            LeftArrow.SetActive(false);
            RightArrow.SetActive(false);
            UpArrow.SetActive(false);
            DownArrow.SetActive(false);
            ViewMode = 1;
        }
    }

    public void Change2Plane()
    {
        //AR:0
        if (ViewMode == 1)
        {

            LeftArrow.SetActive(true);
            RightArrow.SetActive(true);
            UpArrow.SetActive(true);
            DownArrow.SetActive(true);
            ViewMode = 0;
        }
    }


    public void VirObTransDA()
    {

        VirObject[0].transform.position += new Vector3(0.0f, 0.0f, -ButtonDistance);
        ARObject[0].transform.position += new Vector3(0.0f, 0.0f, -ButtonDistance);
        //SynPos();


    }
    public void VirObTransRA()
    {

        VirObject[0].transform.position += new Vector3(ButtonDistance, 0.0f, 0.0f);
        ARObject[0].transform.position += new Vector3(ButtonDistance, 0.0f, 0.0f);
        //SynPos();


    }
    public void VirObTransLA()
    {

        VirObject[0].transform.position += new Vector3(-ButtonDistance, 0.0f, 0.0f);
        ARObject[0].transform.position += new Vector3(-ButtonDistance, 0.0f, 0.0f);
        //SynPos();


    }
    public void VirObTransUA()
    {

        VirObject[0].transform.position += new Vector3(0.0f, 0.0f, ButtonDistance);
        ARObject[0].transform.position += new Vector3(0.0f, 0.0f, ButtonDistance);
        //SynPos();


    }
    public void Rotate()
    {
        /*Quaternion temp = VirObject[0].transform.rotation;
        temp.y = temp.y + 5.0f;
        VirObject[0].transform.rotation = temp;*/
        VirObject[0].transform.Rotate(0.0f, 5.0f, 0.0f);

        /* Quaternion tempA = ARObject[0].transform.rotation;
         tempA.y = tempA.y + 5.0f;
         ARObject[0].transform.rotation = tempA;*/
        ARObject[0].transform.Rotate(0.0f, 5.0f, 0.0f);
    }

    //카메라 이동 코드
    public void CameraPlus()
    {

        VirCamera.transform.position += new Vector3(0.0f, 1.0f, 0.0f);

    }
    public void CameraMinus()
    {

        VirCamera.transform.position -= new Vector3(0.0f, 1.0f, 0.0f);

    }
    public void Retry()
    {

        Application.Quit();
    }

    public void Dragmode()
    {

        //SceneManager.LoadScene(0);
        //지금 드래그 모드가 아니면 
     
     
      IsDragAnd = 1;

      //hitPos.text += "드래그모드" + IsDragAnd;
      SW.Start();
    }

    public void NonDragmode()
    {
      
       
       IsDragAnd = 0;

          /*  Color newColor = Color.red;
            ColorBlock cb = NonDrag.colors;
            cb.normalColor = newColor;
            NonDrag.colors = cb;*/
  
       // hitPos.text += "드래그아님"+ IsDragAnd;
    }
    /*
    void OnMouseDown()
    {
        //translate the cubes position from the world to Screen Point
        screenSpace = arCamera.WorldToScreenPoint(ARObject[0].transform.position);

        //calculate any difference between the cubes world position and the mouses Screen position converted to a world point  
        offset = ARObject[0].transform.position - arCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));

    }



    void OnMouseDrag()
    {

        //keep track of the mouse position
        var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);

        //convert the screen mouse position to world point and adjust with offset
        var curPosition = arCamera.ScreenToWorldPoint(curScreenSpace) + offset;
        curPosition.y = 0.5f;

        //update the position of the object in the world
        VirObject[0].transform.position = curPosition;
        //hitPos.text += "Rot:\n" + "x:" + VirObject[0].transform.position.x + "y:" + VirObject[0].transform.position.y + "z:" + VirObject[0].transform.position.z + "\n";
        // ARobject[0].transform.position += offset;
    }

    */
    




}