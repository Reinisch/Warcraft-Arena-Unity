using Client.Localization;
using Client.UI;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Client
{
    public class LobbyPanel : UIPanel<LobbyScreen>
    {
        [Inject] private LobbyPresenter presenter;

        [SerializeField, UsedImplicitly] private Button startServerButton;
        [SerializeField, UsedImplicitly] private Button singlePlayerButton;
        [SerializeField, UsedImplicitly] private Button loadAdditiveButton;
        [SerializeField, UsedImplicitly, FormerlySerializedAs("clientServerButton")] private Button startClientButton;
        [SerializeField, UsedImplicitly] private Button leaveButton;
        [SerializeField, UsedImplicitly] private Button closeButton;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI startClientButtonLabel;
        [SerializeField, UsedImplicitly] private Image connectingProgressBar;
        [SerializeField, UsedImplicitly] private Transform mapsContentHolder;
        [SerializeField, UsedImplicitly] private LobbyMapSlot mapSlotPrototype;
        [SerializeField, UsedImplicitly] private Transform sessionsContentHolder;
        [SerializeField, UsedImplicitly] private LobbySessionSlot sessionSlotPrototype;
        [SerializeField, UsedImplicitly] private Button localSessionsTabButton;
        [SerializeField, UsedImplicitly] private Button onlineSessionsTabButton;
        [SerializeField, UsedImplicitly] private Button refreshSessionsButton;
        [SerializeField, UsedImplicitly] private TMP_InputField playerNameInput;
        [SerializeField, UsedImplicitly] private TMP_InputField serverNameInput;
        [SerializeField, UsedImplicitly] private TMP_InputField directConnectAddressInput;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI selectedMapLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI versionName;
        [SerializeField, UsedImplicitly] private LocalizedTextMeshProUGUI statusLabel;
        [SerializeField, UsedImplicitly] private TMP_Dropdown regionDropdown;
        [SerializeField, UsedImplicitly] private GameObject startClientTooltip;
        [SerializeField, UsedImplicitly] private GameObject noSessionsFoundTooltip;

        [SerializeField, UsedImplicitly] private LocalizedString disconnectedReasonString;
        [SerializeField, UsedImplicitly] private LocalizedString connectionStartString;
        [SerializeField, UsedImplicitly] private LocalizedString connectSuccessString;
        [SerializeField, UsedImplicitly] private LocalizedString clientStartString;
        [SerializeField, UsedImplicitly] private LocalizedString serverStartString;
        [SerializeField, UsedImplicitly] private LocalizedString serverStartFailedString;
        [SerializeField, UsedImplicitly] private LocalizedString serverStartSuccessString;
        [SerializeField, UsedImplicitly] private LocalizedString clientStartFailedString;
        [SerializeField, UsedImplicitly] private LocalizedString clientStartSuccessString;

        public Button StartServerButton => startServerButton;
        public Button SinglePlayerButton => singlePlayerButton;
        public Button LoadAdditiveButton => loadAdditiveButton;
        public Button StartClientButton => startClientButton;
        public Button LeaveButton => leaveButton;
        public Button CloseButton => closeButton;
        public TextMeshProUGUI StartClientButtonLabel => startClientButtonLabel;
        public Image ConnectingProgressBar => connectingProgressBar;
        public Transform MapsContentHolder => mapsContentHolder;
        public LobbyMapSlot MapSlotPrototype => mapSlotPrototype;
        public Transform SessionsContentHolder => sessionsContentHolder;
        public LobbySessionSlot SessionSlotPrototype => sessionSlotPrototype;
        public Button LocalSessionsTabButton => localSessionsTabButton;
        public Button OnlineSessionsTabButton => onlineSessionsTabButton;
        public Button RefreshSessionsButton => refreshSessionsButton;
        public TMP_InputField PlayerNameInput => playerNameInput;
        public TMP_InputField ServerNameInput => serverNameInput;
        public TMP_InputField DirectConnectAddressInput => directConnectAddressInput;
        public TextMeshProUGUI SelectedMapLabel => selectedMapLabel;
        public TextMeshProUGUI VersionName => versionName;
        public LocalizedTextMeshProUGUI StatusLabel => statusLabel;
        public TMP_Dropdown RegionDropdown => regionDropdown;
        public GameObject StartClientTooltip => startClientTooltip;
        public GameObject NoSessionsFoundTooltip => noSessionsFoundTooltip;

        public LocalizedString DisconnectedReasonString => disconnectedReasonString;
        public LocalizedString ConnectionStartString => connectionStartString;
        public LocalizedString ConnectSuccessString => connectSuccessString;
        public LocalizedString ClientStartString => clientStartString;
        public LocalizedString ServerStartString => serverStartString;
        public LocalizedString ServerStartFailedString => serverStartFailedString;
        public LocalizedString ServerStartSuccessString => serverStartSuccessString;
        public LocalizedString ClientStartFailedString => clientStartFailedString;
        public LocalizedString ClientStartSuccessString => clientStartSuccessString;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            presenter.Initialize(this);
        }

        protected override void PanelDeinitialized()
        {
            presenter.Deinitialize();

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(float deltaTime)
        {
            base.PanelUpdated(deltaTime);

            presenter.Tick(deltaTime);
        }

        protected override void PanelShown()
        {
            base.PanelShown();

            presenter.Shown();
        }

        protected override void PanelHidden()
        {
            presenter.Hidden();

            base.PanelHidden();
        }
    }
}
