[SerializeField] private MonsterCatalog _monsterCatalog;
    [SerializeField] private Button _catalogButton;

    private void Start()
    {
        _client.RegisterEvent(GameEvents.EVT_INITIALIZE_PLAYERS, OnInitializePlayers);
        _client.RegisterEvent(GameEvents.EVT_TURN_BEGIN, OnBeginTurn);
        _client.RegisterEvent(GameEvents.EVT_TURN_TRANSITION, OnTurnTransition);

        _monsterCatalog.Initialize();
        _catalogButton.onClick.AddListener(() => _monsterCatalog.Show());
    }