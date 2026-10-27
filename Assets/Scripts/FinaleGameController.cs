using System;
using UnityEngine;

public enum GamePhase { Menu, Ready, Spinning, Reward, Advancing, Bomb, Summary }

public class FinaleGameController : MonoBehaviour
{
    [SerializeField] private SpinRewardPool bronzePool;
    [SerializeField] private SpinRewardPool silverPool;
    [SerializeField] private SpinRewardPool goldenPool;
    [SerializeField, Min(0)] private float rewardGrowthPerZone = .05f;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject gameplay;
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private UnityEngine.UI.Button spinButton;
    [SerializeField] private UnityEngine.UI.Button continueButton;
    [SerializeField] private WheelSpinner spinner;
    [SerializeField] private ZoneBarController zoneBar;
    [SerializeField] private CollectedItemsPanel collectedItems;
    [SerializeField] private CollectedItemsPanel summaryItems;
    [SerializeField] private BombExplosionPanel bombPanel;
    [SerializeField] private PopupTransition rewardPopup;
    [SerializeField] private PopupTransition summaryPopup;
    [SerializeField] private UnityEngine.UI.Image indicator;
    [SerializeField] private UnityEngine.UI.Image rewardIcon;
    [SerializeField] private TMPro.TMP_Text rewardText;
    [SerializeField] private TMPro.TMP_Text spinTitle;
    [SerializeField] private TMPro.TMP_Text summaryEmpty;
    [SerializeField] private UnityEngine.UI.Image[] slotIcons;
    [SerializeField] private TMPro.TMP_Text[] slotAmounts;

    private readonly RunSession session = new RunSession();
    public GamePhase Phase { get; private set; } = GamePhase.Menu;
    public int CurrentZone => session.Zone;
    public SpinRewardResult[] CurrentRewards { get; private set; }
    public RunSession Session => session;

    private void OnValidate()
    {
        menu = transform.Find("ui_menu")?.gameObject;
        gameplay = transform.Find("ui_gameplay")?.gameObject;
        startButton = transform.Find("ui_menu/ui_button_start")?.GetComponent<UnityEngine.UI.Button>();
        spinButton = transform.Find("ui_gameplay/ui_wheel_area/ui_button_spin")?.GetComponent<UnityEngine.UI.Button>();
        spinner = transform.Find("ui_gameplay/ui_wheel_area")?.GetComponent<WheelSpinner>();
        zoneBar = transform.Find("ui_gameplay/ui_zone_bar")?.GetComponent<ZoneBarController>();
        collectedItems = transform.Find("ui_gameplay/ui_panel_collected_items")?.GetComponent<CollectedItemsPanel>();
        rewardPopup = transform.Find("ui_reward_popup")?.GetComponent<PopupTransition>();
        summaryPopup = transform.Find("ui_summary_popup")?.GetComponent<PopupTransition>();
        bombPanel = transform.Find("ui_bomb_demo")?.GetComponent<BombExplosionPanel>();
        continueButton = transform.Find("ui_reward_popup/ui_animator_content/ui_button_continue")?.GetComponent<UnityEngine.UI.Button>();
        rewardIcon = transform.Find("ui_reward_popup/ui_animator_content/ui_image_reward_value")?.GetComponent<UnityEngine.UI.Image>();
        rewardText = transform.Find("ui_reward_popup/ui_animator_content/ui_text_reward_value")?.GetComponent<TMPro.TMP_Text>();
        summaryItems = transform.Find("ui_summary_popup/ui_animator_content/ui_panel_collected_items")?.GetComponent<CollectedItemsPanel>();
        summaryEmpty = transform.Find("ui_summary_popup/ui_animator_content/ui_text_empty_value")?.GetComponent<TMPro.TMP_Text>();
        spinTitle = transform.Find("ui_gameplay/ui_text_spin_title_value")?.GetComponent<TMPro.TMP_Text>();
        indicator = transform.Find("ui_gameplay/ui_wheel_area/ui_image_indicator_value")?.GetComponent<UnityEngine.UI.Image>();
        if (spinner == null) return;
        slotIcons = new UnityEngine.UI.Image[8];
        slotAmounts = new TMPro.TMP_Text[8];
        for (int i = 0; i < 8; i++)
        {
            var slot = spinner.transform.Find("ui_image_wheel_value/ui_slot_" + i);
            slotIcons[i] = slot?.Find("ui_image_icon_value")?.GetComponent<UnityEngine.UI.Image>();
            slotAmounts[i] = slot?.Find("ui_text_amount_value")?.GetComponent<TMPro.TMP_Text>();
        }
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartRun);
        spinButton.onClick.AddListener(Spin);
        continueButton.onClick.AddListener(Continue);
        spinner.Completed += OnSpinCompleted;
        collectedItems.ExitRequested += ExitRun;
        summaryItems.ExitRequested += ReturnToMenu;
        bombPanel.GiveUpRequested += ReturnToMenu;
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartRun);
        spinButton.onClick.RemoveListener(Spin);
        continueButton.onClick.RemoveListener(Continue);
        spinner.Completed -= OnSpinCompleted;
        collectedItems.ExitRequested -= ExitRun;
        summaryItems.ExitRequested -= ReturnToMenu;
        bombPanel.GiveUpRequested -= ReturnToMenu;
    }

    private void Start() => ReturnToMenu();

    public void StartRun()
    {
        if (Phase != GamePhase.Menu) return;
        session.Reset();
        menu.SetActive(false);
        gameplay.SetActive(true);
        collectedItems.ClearItems();
        summaryItems.ClearItems();
        zoneBar.SetZone(1);
        PrepareZone();
    }

    private void PrepareZone()
    {
        spinner.ResetWheel();
        var type = RunSession.GetSpinType(session.Zone);
        var pool = type == SpinType.Golden ? goldenPool : type == SpinType.Silver ? silverPool : bronzePool;
        CurrentRewards = SpinRewardGenerator.Generate(pool);
        var wheel = spinner.transform.Find("ui_image_wheel_value").GetComponent<UnityEngine.UI.Image>();
        wheel.sprite = pool.wheelSprite;
        indicator.sprite = pool.indicatorSprite;
        spinTitle.text = type.ToString().ToUpperInvariant() + " SPIN";
        for (int i = 0; i < CurrentRewards.Length; i++)
        {
            var reward = CurrentRewards[i];
            if (!reward.IsBomb)
            {
                int amount = Mathf.Max(1, Mathf.CeilToInt(reward.Amount * (1f + (session.Zone - 1) * rewardGrowthPerZone)));
                reward = new SpinRewardResult(reward.ItemId, reward.DisplayName, reward.Icon, amount);
                CurrentRewards[i] = reward;
            }
            slotIcons[i].sprite = reward.Icon;
            slotIcons[i].rectTransform.sizeDelta = reward.IsBomb ? new Vector2(66, 66) : new Vector2(60, 42);
            slotIcons[i].rectTransform.anchoredPosition = reward.IsBomb ? Vector2.zero : new Vector2(0, 10);
            slotAmounts[i].text = reward.IsBomb ? "" : "x" + reward.Amount;
        }
        Phase = GamePhase.Ready;
        SetControls(true);
    }

    public void Spin()
    {
        if (Phase != GamePhase.Ready) return;
        Phase = GamePhase.Spinning;
        SetControls(false);
        spinner.Spin();
    }

    private void OnSpinCompleted(int index)
    {
        if (Phase != GamePhase.Spinning) return;
        if (index < 0 || index >= CurrentRewards.Length)
        {
            Debug.LogError("Çark durdu ama kazanan delik algılanamadı.", this);
            Phase = GamePhase.Ready;
            SetControls(true);
            return;
        }
        var reward = CurrentRewards[index];
        if (reward.IsBomb)
        {
            Phase = GamePhase.Bomb;
            session.Reset();
            collectedItems.ClearItems();
            bombPanel.ShowBombExplosion();
            return;
        }
        session.Award(reward);
        collectedItems.AddItem(reward.ItemId, reward.Icon, reward.Amount);
        Phase = GamePhase.Reward;
        rewardIcon.sprite = reward.Icon;
        rewardText.text = reward.DisplayName + "\nx" + reward.Amount.ToString("N0");
        rewardPopup.Show();
    }

    public void Continue()
    {
        if (Phase != GamePhase.Reward) return;
        Phase = GamePhase.Advancing;
        rewardPopup.Hide();
        session.Advance();
        zoneBar.AdvanceZone(() =>
        {
            if (Phase == GamePhase.Advancing) PrepareZone();
        });
    }

    public void ExitRun()
    {
        if (Phase != GamePhase.Ready) return;
        Phase = GamePhase.Summary;
        SetControls(false);
        summaryItems.ClearItems();
        foreach (var reward in session.Rewards)
            summaryItems.AddItem(reward.ItemId, reward.Icon, reward.Amount);
        summaryEmpty.gameObject.SetActive(session.Rewards.Count == 0);
        summaryPopup.Show();
    }

    public void ReturnToMenu()
    {
        Phase = GamePhase.Menu;
        session.Reset();
        CurrentRewards = null;
        spinner.ResetWheel();
        rewardPopup.Hide();
        summaryPopup.Hide();
        bombPanel.CloseImmediately();
        collectedItems.ClearItems();
        summaryItems.ClearItems();
        gameplay.SetActive(false);
        menu.SetActive(true);
    }

    private void SetControls(bool interactable)
    {
        spinButton.interactable = interactable;
        collectedItems.transform.Find("ui_button_exit").GetComponent<UnityEngine.UI.Button>().interactable = interactable;
    }
}
