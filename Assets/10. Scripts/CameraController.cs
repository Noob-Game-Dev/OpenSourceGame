using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject camera1;
    [SerializeField] GameObject camera2;
    [SerializeField] GameObject camera3;
    [SerializeField] GameObject camera4;

    [SerializeField] AnimationAndMovementController hero;
    [SerializeField] Animator anim;

    [SerializeField] RuntimeAnimatorController animator1;
    [SerializeField] RuntimeAnimatorController animator2;
    [SerializeField] RuntimeAnimatorController animator3;

    [SerializeField] GameObject panel;

    [SerializeField] Material greenMat;
    [SerializeField] Material grassMat;
    [SerializeField] Material brownMat;
    [SerializeField] Material brokeMat;
    [SerializeField] Material heroMat;

    bool withOutline = false;

    [SerializeField] GameObject notCloudLight;
    [SerializeField] GameObject withCloudLight;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera1.SetActive(true); camera2.SetActive(false); camera3.SetActive(false); camera4.SetActive(false); hero.onMouseCameraRotate = false;
        anim.runtimeAnimatorController = animator1;

        Application.targetFrameRate = 400;
    }

    int rCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            camera1.SetActive(true); camera2.SetActive(false); camera3.SetActive(false); camera4.SetActive(false);
            hero.onMouseCameraRotate = false; hero.deactivateRotation = false; hero.animator.applyRootMotion = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera1.SetActive(false); camera2.SetActive(true); camera3.SetActive(false); camera4.SetActive(false);
            hero.onMouseCameraRotate = false; hero.deactivateRotation = false; hero.animator.applyRootMotion = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camera1.SetActive(false); camera2.SetActive(false); camera3.SetActive(true); camera4.SetActive(false);
            hero.onMouseCameraRotate = true; hero.deactivateRotation = false; hero.animator.applyRootMotion = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            camera1.SetActive(false); camera2.SetActive(false); camera3.SetActive(false); camera4.SetActive(true);
            hero.onMouseCameraRotate = true; hero.deactivateRotation = true; hero.animator.applyRootMotion = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rCount++;
            if (rCount > 2) { rCount = 0; }

            if (rCount == 0) { anim.runtimeAnimatorController = animator1; animatorTypeText = "Standart"; }
            if (rCount == 1) { anim.runtimeAnimatorController = animator2; animatorTypeText = "Кривые"; }
            if (rCount == 2) { anim.runtimeAnimatorController = animator3; animatorTypeText = "Кривые + кадры"; }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CanvasActiveSwitch();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            onCloud = !onCloud;
            if (onCloud == true) { withCloudLight.SetActive(true); notCloudLight.SetActive(false); cloudEnableText = "On"; }
            else { withCloudLight.SetActive(false); notCloudLight.SetActive(true); cloudEnableText = "Off"; }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            oCount++;
            if (oCount > 2) { oCount = 0; }

            if (oCount == 0)
            {
                greenMat.SetFloat("_OUTLINE", 0f);
                greenMat.SetFloat("_Outline_Width", 0f);

                brownMat.SetFloat("_OUTLINE", 0f);
                brownMat.SetFloat("_Outline_Width", 0f);

                brokeMat.SetFloat("_OUTLINE", 0f);
                brokeMat.SetFloat("_Outline_Width", 0f);

                heroMat.SetFloat("_OUTLINE", 0f);
                heroMat.SetFloat("_Outline_Width", 0f);

                outlineTypeText = "None";
            }

            if (oCount == 1)
            {
                greenMat.SetFloat("_OUTLINE", 0f);
                greenMat.SetFloat("_Outline_Width", 25f);

                brownMat.SetFloat("_OUTLINE", 0f);
                brownMat.SetFloat("_Outline_Width", 25f);

                brokeMat.SetFloat("_OUTLINE", 0f);
                brokeMat.SetFloat("_Outline_Width", 25f);

                heroMat.SetFloat("_OUTLINE", 0f);
                heroMat.SetFloat("_Outline_Width", 0.7f);

                outlineTypeText = "Normal Dir";
            }

            if (oCount == 2)
            {
                greenMat.SetFloat("_OUTLINE", 1f);
                greenMat.SetFloat("_Outline_Width", 25f);

                brownMat.SetFloat("_OUTLINE", 1f);
                brownMat.SetFloat("_Outline_Width", 25f);

                brokeMat.SetFloat("_OUTLINE", 1f);
                brokeMat.SetFloat("_Outline_Width", 25f);

                heroMat.SetFloat("_OUTLINE", 1f);
                heroMat.SetFloat("_Outline_Width", 25f);

                outlineTypeText = "Scale Pos";
            }
        }

        FPSCalculate();
    }

    [SerializeField] GameObject canvasObject;

    void CanvasActiveSwitch()
    {
        canvasObject.SetActive(!canvasObject.activeSelf);

        if (canvasObject.activeSelf == true) { Cursor.lockState = CursorLockMode.None; }
        else { Cursor.lockState = CursorLockMode.Locked; }
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    bool onCloud = false;

    int oCount = 0;


    // for ui.
    private int screenLongSide;
    private Rect boxRect;
    private GUIStyle style = new GUIStyle();

    // for fps calculation.
    private int frameCount;
    private float elapsedTime;
    private double frameRate;

    void FPSCalculate()
    {
        // FPS calculation
        frameCount++;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 0.5f)
        {
            frameRate = System.Math.Round(frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero);
            frameCount = 0;
            elapsedTime = 0;

            // Update the UI size if the resolution has changed
            //if (screenLongSide != Mathf.Max(Screen.width, Screen.height))
            //{
            //    UpdateUISize();
            //}
        }
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        //UpdateUISize();
    }

    /// <summary>
    /// Resize the UI according to the screen resolution
    /// </summary>
    //private void UpdateUISize()
    //{
    //    screenLongSide = Mathf.Max(Screen.width, Screen.height);
    //    var rectLongSide = screenLongSide / 10;
    //    boxRect = new Rect(1, 1, rectLongSide, rectLongSide / 3);
    //    style.fontSize = (int)(screenLongSide / 36.8);
    //    style.normal.textColor = Color.white;

    //    styleMy.normal.textColor = Color.white;
    //    styleMy.fontSize = fontGUISize;
    //}

    /// <summary>
    /// Display FPS
    /// </summary>
    private void OnGUI()
    {
        GUI.Box(animatorType, "");
        GUI.Label(animatorType, "Анимация: " + animatorTypeText, styleMy);

        GUI.Box(outlineType, "");
        GUI.Label(outlineType, "Тип обводки: " + outlineTypeText, styleMy);

        GUI.Box(cloudEnable, "");
        GUI.Label(cloudEnable, "Облака включены: " + cloudEnableText, styleMy);

        GUI.Box(fps, "");
        GUI.Label(fps, "FPS: " + frameRate, styleMy);
    }

    [SerializeField] Rect animatorType;
    [SerializeField] Rect outlineType;
    [SerializeField] Rect cloudEnable;
    [SerializeField] Rect fps;
    [SerializeField] GUIStyle styleMy = new GUIStyle();
    [SerializeField] int fontGUISize = 12;
    [SerializeField] Vector2 correctTextinBox;

    string animatorTypeText = "Standart";
    string outlineTypeText = "None";
    string cloudEnableText = "Off";
}