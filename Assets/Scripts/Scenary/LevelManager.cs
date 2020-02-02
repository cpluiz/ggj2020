using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour{

    static LevelManager _instance;
    public static LevelManager instance { get { return _instance; } }

    [Header("Object References")] 
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private PlayerController _player;
    [SerializeField] private Area[] areaPrefabs;
    [SerializeField] private Area startArea;
    [SerializeField] private AudioSource levelAudio;

    [Header("Word Configurations")] 
    [SerializeField] [Range(10,90)] private int proceduralPercentageProbability;

    [SerializeField] [Range(0, 1)] private float minPercentage, maxPercentage;
    [SerializeField] private int minRooms, maxRooms, retryLimit, totalGoals;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Area[,] generatedMap;
    [SerializeField] private bool[,] ocupiedArea;

    //Pool de áreas para construção procedural de cenário
    private List<Area>[] areaPool = new List<Area>[4];
    
    public int timesRegenerated, roomsGenerated, goalsPlaced, resourcesPlaced, challengesPlaced, pump1Placed, pump2Placed, pump3Placed;
    public bool miniGameIsOpened = false;
    public bool insideFireRegion = true;
    public bool enableFireEffect{
        get{ return insideFireRegion && !miniGameIsOpened; }
    }
    public bool canHasWater{
        get{return _player.CanHasWater(); }
    }
    
    void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        }else if (_instance != this) {
            Destroy(gameObject);
        }
    }

    void Start(){
        areaPool[0] = new List<Area>();
        areaPool[1] = new List<Area>();
        areaPool[2] = new List<Area>();
        areaPool[3] = new List<Area>();
        PopulateAreaPool(areaPrefabs);
        ocupiedArea = new bool[mapSize.x,mapSize.y];
        generatedMap = new Area[mapSize.x,mapSize.y];
        minRooms = (int)((mapSize.x * mapSize.y) * minPercentage);
        maxRooms = (int)((mapSize.x * mapSize.y) * maxPercentage);
        GenerateMap();
        PlaceResources();
        MoveCameraToArea(startArea.transform, LinkedDirection.down);
        _player.SetStartPosition(startArea.transform.position);
    }

    //Recebe um conjunto de áreas para popular o pool de construção procedural de cenário vindas de um
    //gerenciador de prefabs
    public void PopulateAreaPool(Area[] areaList){
        foreach (Area area in areaList){
            areaPool[area.waypoints-1].Add(area);
        }
    }

    public void InteractWithMinigame(bool active){
        miniGameIsOpened = active;
        if (active)
            levelAudio.Stop();
        else
            levelAudio.Play();
    }

    public void MoveCameraToArea(Transform targetArea, LinkedDirection direction){
        _virtualCamera.LookAt = targetArea;
        _virtualCamera.Follow = targetArea;
        _virtualCamera.m_Lens.NearClipPlane = 0;
        _player.EnterInArea(direction);
    }
    
    //Função de teste para geração inicial do mapa
    private void GenerateMap(){
        startArea = null;
        ocupiedArea = new bool[mapSize.x,mapSize.y];
        Populate();
        DeleteIfDontHaveNeighbord();
        if (AllRowsConnect() && timesRegenerated < retryLimit && roomsGenerated >= minRooms && roomsGenerated <= maxRooms){
            ConstructMap();
        }
        if(timesRegenerated <= retryLimit && startArea == null){
            timesRegenerated++;
            GenerateMap();
        }
    }

    //Função temporária antes de construir áreas conforme objetivos
    private void ConstructMap(){
        Area prefabArea = areaPool[0][0];
        Vector2 lastArea = Vector2.zero;
        for (int y = 0; y < mapSize.y; y++){
            for (int x = 0; x < mapSize.x; x++){
                if (ocupiedArea[x, y]){
                    bool[] needNeighbor = new bool[4];
                    int portals = 0;
                    needNeighbor[0] = y > 0 && ocupiedArea[x, y - 1]; //vizinho no topo
                    portals += needNeighbor[0] ? 1 : 0;
                    needNeighbor[1] = x < mapSize.x-1 && ocupiedArea[x+1, y]; //vizinho a direita
                    portals += needNeighbor[1] ? 1 : 0;
                    needNeighbor[2] = y < mapSize.y-1 && ocupiedArea[x, y + 1]; //vizinho abaixo
                    portals += needNeighbor[2] ? 1 : 0;
                    needNeighbor[3] = x > 0 && ocupiedArea[x - 1, y];//vizinho a esquerda
                    portals += needNeighbor[3] ? 1 : 0;
                    int index = Random.Range(0, areaPool[portals-1].Count);
                    prefabArea = areaPool[portals-1][index];
                    while (!prefabArea.HasPortals(needNeighbor) ){
                        prefabArea = areaPool[portals-1][Random.Range(0, areaPool[portals-1].Count)];
                    }
                    if (prefabArea == null || generatedMap[x,y] != null){
                        startArea = null;
                        return;
                    }
                    generatedMap[x,y] = Instantiate(prefabArea, lastArea, Quaternion.identity, transform);
                    if(generatedMap[x,y].waypoints == 4 && startArea == null)
                        startArea = generatedMap[x, y];
                }
                lastArea += Vector2.right * prefabArea.GetComponent<SpriteRenderer>().bounds.extents.x * 2;
            }
            lastArea = Vector2.zero;
            lastArea += Vector2.down * prefabArea.GetComponent<SpriteRenderer>().bounds.extents.y * 2 * (y+1);
        }

        ConnectNeighbordAreas();
    }

    private void ConnectNeighbordAreas(){
        for (int y = 0; y < mapSize.y; y++){
            for (int x = 0; x < mapSize.x; x++){
                if (generatedMap[x, y] != null){
                    if(y>0)generatedMap[x,y].SetLinkedArea(generatedMap[x, y-1],LinkedDirection.up);
                    if(x<mapSize.x-1)generatedMap[x,y].SetLinkedArea(generatedMap[x+1, y],LinkedDirection.right);
                    if(y<mapSize.y-1)generatedMap[x,y].SetLinkedArea(generatedMap[x, y+1],LinkedDirection.down);
                    if(x>0)generatedMap[x,y].SetLinkedArea(generatedMap[x-1, y],LinkedDirection.left);
                }
            }
        }
    }    

    private void Populate(){
        roomsGenerated = 0;
        for(int y = 0; y < mapSize.y; y++){
            for (int x = 0; x < mapSize.x; x++){
                float rand = Random.RandomRange(0, 100f);
                int debugValue = (int)((mapSize.x * mapSize.y) * 0.1);
                ocupiedArea[x,y] = rand <= ((roomsGenerated < (debugValue)) ? 95 : proceduralPercentageProbability);
                if (roomsGenerated > 1){
                    ocupiedArea[x, y] &= AreaHasNeighbord(x, y);
                }
                roomsGenerated += ocupiedArea[x,y] ? 1 : 0;
            }
        }
    }

    private void DeleteIfDontHaveNeighbord(){
        for (int i = 0; i < mapSize.x; i++){
            for(int j = 0; j < mapSize.y; j++){
                if (ocupiedArea[i, j]){
                    ocupiedArea[i,j] = AreaHasNeighbord(i,j);
                    if (!ocupiedArea[i, j]) roomsGenerated--;
                }
            }
        }
    }

    private bool AreaHasNeighbord(int x, int y){
        bool left     = (x > 0) && ocupiedArea[x - 1, y];
        bool right    = (x < mapSize.x-1) && ocupiedArea[x+1, y];
        bool up       = (y > 0) && ocupiedArea[x, y-1];
        bool down     = (y < mapSize.y-1) && ocupiedArea[x, y+1];
        return up || down || left || right;
    }

    private bool AllRowsConnect(){
        bool allLinesConnected = true;
        for (int j = 0; j < mapSize.y; j++){
            bool lineConnected = false;
            for (int i = 0; i < mapSize.x; i++){
                if (j > 0)
                    lineConnected |= ocupiedArea[i, j] && ocupiedArea[i, j - 1];
                if(j<mapSize.y-1)
                    lineConnected |= ocupiedArea[i, j] && ocupiedArea[i, j + 1];
            }
            allLinesConnected &= lineConnected;
        }

        if (!allLinesConnected){
            timesRegenerated++;
        }
        return allLinesConnected;
    }

    private void PlaceResources(){
        bool first, middle, final;
        first = middle = final = false;
        int resources = (int)(totalGoals * 3.5f);
        int challenges = totalGoals * 3;
        int horizontalSectorCount = mapSize.x / 3;
        while (goalsPlaced < 3 && challengesPlaced < challenges && resourcesPlaced < resources){
            for (int y = 0; y < mapSize.y; y++){
                for (int x = 0; x < mapSize.x; x++){
                    if (generatedMap[x, y] == null || generatedMap[x, y] == startArea) continue;
                    bool placeSomething = Random.Range(0, 100) <= proceduralPercentageProbability;
                    if (generatedMap[x,y].type == 0 && placeSomething){
                        if (x <= horizontalSectorCount && !first){
                            generatedMap[x,y].SetType(3);
                            first = true;
                            goalsPlaced++;
                        }else if (x > horizontalSectorCount && x <= (horizontalSectorCount * 2) && !middle){
                            generatedMap[x,y].SetType(3);
                            middle = true;
                            goalsPlaced++;
                        }else if (x > horizontalSectorCount * 2 && !final){
                            generatedMap[x,y].SetType(3);
                            final = true;
                            goalsPlaced++;
                        }else{
                            int type = Random.Range(1, 3);
                            generatedMap[x, y].SetType(type);
                            challengesPlaced += (type == 1) ? 1 : 0;
                            resourcesPlaced += (type == 2) ? 1 : 0;
                        }
                    }
                }
            }
        }
    }
    
}
