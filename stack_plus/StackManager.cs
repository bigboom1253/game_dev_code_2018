using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoSingleton<StackManager>
{
    // Const Value -> 상수

    AnimationManager theAnim;

    private const float BOUND_SIZE_X = 30f;          // 블럭의 크기  //TODO: fix x값 y값 따로
    private const float BOUND_SIZE_Z = 1f;          // 블럭의 크기  //TODO: fix x값 y값 따로
    private const float MOVING_BOUNDS_SIZE_X = 5f;    // 움직임 폭의 배율 
    private const float MOVING_BOUNDS_SIZE_Z = 3f;    // 움직임 폭의 배율 
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float BLOCK_MOVING_SPEED_X = 0.1f;
    private const float BLOCK_MOVING_SPEED_Z = 3f;

    private const float ERROR_MARGIN = 0f;


    // public -> unity 동기화 , 인스펙터에서 셋팅 가능
    public GameObject pfBlock = null;

    // TestCode
    //private bool testFlag = false;
    Transform lastBlockTrans = null;

    float tempMovePositionX = 0;
    public float deltaX = 0;
    private int stackCount = 0;
    private float blockTransitionX = 0f;
    private float blockTransitionZ = 0f;
    //private bool isMovingX = true;
    public float movePositionX;
    public Vector3 stackBounds = new Vector3(BOUND_SIZE_X, 1, BOUND_SIZE_Z);
    public Vector3 stackScales = new Vector3(BOUND_SIZE_X, 1, BOUND_SIZE_Z);
    private Vector3 desiredPosition = Vector3.zero;
    private Vector3 prevBlockPosition = Vector3.zero;
    private Vector3 prevBlockScale = Vector3.zero;
    private Vector3 CutPosition = new Vector3(30f, 0, 0); //써는곳 위치


    private float secondaryPosition = 0f;

    [SerializeField]
    private Color prevColor;
    [SerializeField]
    private Color nextColor;

    private bool isGameOver = false;


    //=========================================
    // Score
    //private const string BEST_SCORE_KEY = "BestScore";
    //private const string BEST_COMBO_KEY = "BestCombo";

    //private int comboCount = 0;
    //private int maxComboCount = 0;
    //private int bestScore = 0;
    //private int bestCombo = 0;




    private void Awake()
    {
        pfBlock = Resources.Load("Prefabs/pf_Block") as GameObject; //*리소시즈 폴더 안에 프리팹스를 게임오브젝트로 불러옴. 
        //pfBlock = Resources.Load("pf_Block", typeof(GameObject)) as GameObject;
        //pfBlock = Resources.Load<GameObject>("pf_Block");

        //bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        //bestCombo = PlayerPrefs.GetInt(BEST_COMBO_KEY, 0);
        //상수 키를 가져옴. 만약 없으면 0으로 가져옴.
    }

    void Start()
    {
        theAnim = FindObjectOfType<AnimationManager>();
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();
        SpawnBlock();

    }

    void Update()
    {
        if (isGameOver == true) //게임오버가 참이되면, 계속 리턴하여, 업데이트를 방해하는것.
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))     // 클릭 & 터치
        {
            if(PlaceBlock())
            {

            }
            else
            {
                SpawnBlock();
                return;
            }
            //theAnim.CutOn();
            //PlaceBlock();
            //theAnim.CutOff();
        }


        if (lastBlockTrans != null)
        {
            MoveBlock();

            //this.transform.position = //TODO:fix 전체 포지션을 아래로 보냄. 필요없음. 카메라가 움직이는 것처럼 하기 위해
            //    Vector3.Lerp(transform.position,
            //                desiredPosition,
            //                Time.deltaTime * STACK_MOVING_SPEED);
        }

    }

    bool SpawnBlock() // TODO: fix 재료 생성.
    {

        GameObject instBlock = null;    // 생성된 블럭
        Transform instTrans = null;     // 블럭 위치    

        instBlock = Instantiate(pfBlock); //프리펩생성

        if (instBlock == null)           // 생성에 실패한 경우
        {
            Debug.Log("Block Prefab Instantiate Failed");
            return false;
        }

        // 블럭의 부모설정, 위치, 회전, 크기 -> transform
        instTrans = instBlock.transform;    // 캐싱 작업
        instTrans.parent = this.transform;  // 부모를 나의 트렌스폼으로 설정
                                            // instTrans.SetParent(this.transform);

        //TODO:fix 블럭 포지션이 위로갈 이유가 없음. 새로만든 블럭의 트랜스폼 세팅
        instTrans.localPosition = prevBlockPosition; // Vector3.up;// new Vector3(0, stackBounds.y);
        instTrans.localRotation = Quaternion.identity; //로테이션 제로세팅
        instTrans.localScale = stackScales; // 스케일 세팅 

        //변한 값들 초기화.
        tempMovePositionX = 0;
        deltaX = 0;
        blockTransitionX = 0f;
        blockTransitionZ = 0f;
        movePositionX =0;
        stackBounds = new Vector3(BOUND_SIZE_X, 1, BOUND_SIZE_Z);

    // desiredPosition = stackCount * Vector3.down; // 전체 트랜스폼을 아래로 이동할떄 사용. 카메라 움직이는것처럼.
        lastBlockTrans = instTrans;         // 마지막에 생성된 블럭 트렌스폼 저장

        ColorChange(instBlock);

        return true;
    }

    void MoveBlock()
    {
        // Lerp
        // Mathf.Lerp(시작, 끝, 시간)
        // Mathf.Lerp(시작, 끝, 누적시간 / 총시간) 시작부터 총시간 동안 끝 도달
        // 시간 -> 퍼센트  0 ~ 1
        movePositionX = Mathf.Lerp(0, BOUND_SIZE_X, blockTransitionX / 2f);
        float movePositionZ
            = Mathf.PingPong(blockTransitionZ, BOUND_SIZE_Z) - BOUND_SIZE_Z / 2f;

        blockTransitionX += Time.deltaTime * BLOCK_MOVING_SPEED_X;     // 블럭 이동 속도
        blockTransitionZ += Time.deltaTime * BLOCK_MOVING_SPEED_Z;     // 블럭 이동 속도

        lastBlockTrans.localPosition = //TODO: fix z값도 movePosition형식으로 수정 x값이 바운드 넘어가면 루블처리되고 떨어지게
            new Vector3(movePositionX * MOVING_BOUNDS_SIZE_X,
            2, movePositionZ * MOVING_BOUNDS_SIZE_Z);


   
    }

    Color GetRandomColor()
    {
        // Random
        // Random.Range(0,100) < 20f -> 당첨
        // Random.Range(0,1000) % 101   < 20f  -> 당첨 
        //     0 ~ 100

        float r = Random.Range(100f, 250f) / 255f;
        float g = ((Random.Range(1000f, 9000f) % 151f) + 100f) / 255f;
        float b = Random.Range(100f, 250f) / 255f; ;

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        Color applyColor = Color.Lerp(prevColor, nextColor,
            (stackCount % 21) / 20f);
        Renderer rn = go.GetComponent<Renderer>();
        // 참조형태 Renderer   실형태 MeshRenderer

        if (rn == null)
        {
            Debug.Log("Renderer is NULL");
            return;
        }

        rn.material.color = applyColor;
        Camera.main.backgroundColor = applyColor //카메라 바탕색과 맞추기.
                                        - new Color(0.1f, 0.1f, 0.1f);

        if (applyColor.Equals(nextColor))
        {
            prevColor = nextColor;
            nextColor = GetRandomColor();
        }
    }
    //placeBlock을 반복시키고, 경우에 따라 스폰블럭 메소드 호출
    bool PlaceBlock()
    {

        Vector3 lastPos = lastBlockTrans.localPosition;

        //movpositionx는 계속 증가하는 값. 일시적으로 저장. gus
        //스폰할떄 밑에 두줄 초기화
        deltaX = movePositionX * 5f - tempMovePositionX;// 현재는 시간당 움직임에 따라 잘림. 5f는 선형보간 총시간
        tempMovePositionX = movePositionX * 5f;

        if (stackBounds.x > 0)
        {
            deltaX = Mathf.Abs(deltaX); //절대값
            stackBounds.x -= deltaX;
        }
        else
        {
            return false;
        }

        float middle = (movePositionX * 5f + lastPos.x) / 2f; //남은 큐브의 포지션x

        lastBlockTrans.localScale = stackBounds;

        lastPos.x = middle - deltaX;

        lastBlockTrans.localPosition = lastPos;

        float rubbleHalfScale = deltaX / 2f;
        float boundHalfScale = stackBounds.x / 2f;

        if (stackBounds.x > 1)
        {
            CreateRubble
            (
                new Vector3(
                lastPos.x + boundHalfScale + rubbleHalfScale + deltaX
            , lastPos.y, lastBlockTrans.localPosition.z)

            , new Vector3(deltaX, 1, stackBounds.z)
            );
        }
        return true;

    }

    void CreateRubble(Vector3 pos, Vector3 scale) //TODO: fix 러블도 한동안 무빙하게.
    {
        GameObject go = Instantiate(lastBlockTrans.gameObject); //마지막블록 트랜스폼(포지션,크기,로테이션)으로 게임오브젝트 생성
        go.transform.SetParent(this.transform);

        go.transform.localPosition = pos; //구한 러블의 포지션으로 수정
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = scale;

        go.AddComponent<Rigidbody>();       // 물리컴포넌트 추가
        go.name = "Rubble";                 // 오브젝트 이름변경
    }

    //TODO: fix 필요없음 
    //void Endeffect()
    //{
    //    // 부모 자식 계층구조 -> transform
    //    // Find(FindChild("PATH")) , GetChild(INDEX) childCount
    //    int childCount = this.transform.childCount;
    //    int maxCnt = 20;        // 최대 적용 수

    //    for(int i = 1; i <= childCount; i++)
    //    {
    //        GameObject child = this.transform.GetChild(childCount - i).gameObject;

    //        if (child.name.Equals("Rubble"))
    //            continue;

    //        maxCnt--;               // 횟수 제한
    //        if (maxCnt <= 0)
    //            break;

    //        Rigidbody rigidbody = child.AddComponent<Rigidbody>();
    //        rigidbody.AddForce(
    //            new Vector3(
    //                Random.Range(-100f, 100f),
    //                Random.Range(500f, 1000f),
    //                Random.Range(-100f, 100f)
    //                ));

    //    }
    //}
    //TODO: fix 필요없음 
    public void Restart()
    {
        //    int childCount = this.transform.childCount;
        //    for (int i = 0; i < childCount; i++)
        //    {
        //        Destroy(this.transform.GetChild(i).gameObject);
        //    }

        //    lastBlockTrans = null;
        //    desiredPosition = Vector3.zero;
        //    stackBounds = new Vector3(BOUND_SIZE, 1, BOUND_SIZE);

        //    stackCount = -1;
        //    isMovingX = true;
        //    blockTransition = 0f;
        //    secondaryPosition = 0f;

        //    comboCount = 0;

        //    prevBlockPosition = Vector3.down;

        //    prevColor = GetRandomColor();
        //    nextColor = GetRandomColor();

        //    isGameOver = false;

        //    SpawnBlock();
        //    SpawnBlock();
    }

}
