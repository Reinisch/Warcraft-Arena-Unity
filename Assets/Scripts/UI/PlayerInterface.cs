using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PivotX { Left, Right, Center }
public enum PivotY { Bottom, Center, Top }

public delegate void TargetChangeEvent(Unit target);
public delegate void ScreenResizeEvent();

public class PlayerInterface : MonoBehaviour
{
    [HideInInspector]
    public ArenaManager world;
    [HideInInspector]
    public Camera mainCamera;

    public int ActualSkillNameFontSize { get; set; }
    public int ActualDamageFontSize { get; set; }
    public float relativeSkillIconSize;
    public float relativeSkillNameFontSize;
    public float relativeDamageFontSize;
    public GUIStyle skillUseNameStyle;
    public GUIStyle damageLabelStyle;

    public ButtonController ButtonController { get; private set; }
    public Unit PlayerUnit { get; private set; }

    GameObject humanPlayer;
    Unit lastTarget;
    RaycastHit hitInfo = new RaycastHit();
    int lastScreenWidth;
    int lastScreenHeight;

    public GameObject canvasUiOverlay;
    public ActionBar actionBarNewUI;
    public ActionBar newActionBarNewUI;
    public UnitFrame playerFrameNewUI;
    public UnitFrame targetFrameNewUI;
    public BuffBar playerBuffsNewUI;
    public BuffBar targetBuffsNewUI;
    public CastFrame playerCastFrameNewUI;

    public event ScreenResizeEvent ScreenResized;    
    public event TargetChangeEvent TargetLost;
    public event TargetChangeEvent TargetSwitch;
    public event TargetChangeEvent TargetSet;

    public void Initialize()
    {
        actionBarNewUI.Initialize();
        newActionBarNewUI.Initialize();

        if (ScreenResized != null)
            ScreenResized();
    }

    void Awake()
    {
        world = ArenaManager.Instanse;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        humanPlayer = GameObject.FindGameObjectWithTag("Player");

        PlayerUnit = humanPlayer.GetComponent<Unit>();
        ButtonController = GetComponent<ButtonController>();

        lastScreenWidth = 0;
        lastScreenHeight = 0;
        lastTarget = null;

        playerFrameNewUI.Initialize();
        playerFrameNewUI.SetUnit(PlayerUnit);

        targetFrameNewUI.Initialize();
        targetFrameNewUI.SetUnit(null);
        TargetLost += targetFrameNewUI.OnTargetLost;
        TargetSet += targetFrameNewUI.OnTargetSet;
        TargetSwitch += targetFrameNewUI.OnTargetSwitch;

        playerBuffsNewUI.Initialize(PlayerUnit);
        ScreenResized += playerBuffsNewUI.OnScreenResize;

        targetBuffsNewUI.Initialize(PlayerUnit.character.target);
        TargetLost += targetBuffsNewUI.OnTargetLost;
        TargetSet += targetBuffsNewUI.OnTargetSet;
        TargetSwitch += targetBuffsNewUI.OnTargetSwitch;
        targetBuffsNewUI.gameObject.SetActive(false);
        ScreenResized += targetBuffsNewUI.OnScreenResize;

        ScreenResized += OnScreenResize;
    }
    void OnGUI()
    {
        if (lastScreenHeight != Screen.height || lastScreenWidth != Screen.width)
        {
            if (ScreenResized != null)
                ScreenResized();

            lastScreenHeight = Screen.height;
            lastScreenWidth = Screen.width;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ArenaManager.GetHostileTargets(PlayerUnit, 10);

        CheckActionBarButtons();

        if (Input.GetMouseButtonDown(0))
            if (!EventSystem.current.IsPointerOverGameObject())
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1 << LayerMask.NameToLayer("Characters")))
                    CheckTargetSelection(hitInfo.transform.GetComponent<Unit>());

        /*if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(mainCamera.ScreenToWorldPoint(Input.mousePosition).x,
                mainCamera.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0, 1 << LayerMask.NameToLayer("Characters"));

                if (hit.transform != null)
                    CheckTargetSelection(hit.transform.GetComponent<Unit>());
                else
                    CheckTargetSelection(null);
            }
        }*/

        playerCastFrameNewUI.UpdateUnit();
        playerFrameNewUI.UpdateFrame();
        if (targetFrameNewUI.gameObject.activeSelf)
            targetFrameNewUI.UpdateFrame();
    }

    public void CheckTargetSelection(Unit newTarget)
    {
        if (newTarget != null)
        {
            lastTarget = PlayerUnit.Character.target;
            PlayerUnit.Character.target = newTarget;

            if (TargetSet != null)
                TargetSet(PlayerUnit.Character.target);
        }
    }
    public void ClearTargetSelection()
    {
        if (PlayerUnit.Character.target != null)
        {
            lastTarget = PlayerUnit.Character.target;

            if (TargetLost != null)
                TargetLost(PlayerUnit.Character.target);

            PlayerUnit.Character.target = null;
        }
    }

    public void CheckTargetUnitDeath(int id)
    {
        if (PlayerUnit.character.target != null && PlayerUnit.character.target.id == id)
        {
            lastTarget = PlayerUnit.Character.target;
            PlayerUnit.character.target = null;
            TargetLost(lastTarget);
        }
    }
    public void CheckActionBarButtons()
    {
        actionBarNewUI.CheckButtons(world);
        newActionBarNewUI.CheckButtons(world);
    }

    public void OnScreenResize()
    {
        ActualSkillNameFontSize = (int)Math.Round(relativeSkillNameFontSize * Screen.height);
        ActualDamageFontSize = (int)Math.Round(relativeDamageFontSize * Screen.height);
        damageLabelStyle.fontSize = ActualDamageFontSize;
        skillUseNameStyle.fontSize = ActualSkillNameFontSize;
    }
}