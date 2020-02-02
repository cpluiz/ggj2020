using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame1Manager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Transform[] spawnPoint = new Transform[2];
    [SerializeField] private MiniGame1Item[] itemPrefabs;
    [SerializeField] private MiniGame1Player _player;
    [SerializeField] private AudioSource miniGameAudio;
    [SerializeField] public int charges, errors, corrects;
    [SerializeField] private GameObject[] wateringCans = new GameObject[3];
    private bool isEnding = false;

    [Header("Game Options")] public float minSpawnInterval, maxSpawnInterval;
    public bool correctIsCarnivour = false;

    public void SetCarnivour(bool carnivour){
        correctIsCarnivour = carnivour;
    }
    
    void OnEnable(){
        isEnding = false;
        LevelManager.instance.InteractWithMinigame(true);
        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
        charges = 3;
        StartCoroutine(SpawnItem());
    }

    private void OnDisable(){
        LevelManager.instance.InteractWithMinigame(false);
    }

    void Update(){
        if (isEnding) return;
        wateringCans[0].SetActive(charges>=1);
        wateringCans[1].SetActive(charges>=2);
        wateringCans[2].SetActive(charges>=3);
        if (charges <= 0){
            isEnding = true;
            StartCoroutine(EndMinigame());
        }
    }

    private IEnumerator EndMinigame(){
        foreach (MiniGame1Item item in GameObject.FindObjectsOfType<MiniGame1Item>()){
            Destroy(item.gameObject);
        }
        StopCoroutine(SpawnItem());
        //Lógica para tratar casos de vitória e derrota
        //Enrolando 4 segundos para garantir que a animação do regador vai concluir
        yield return new WaitForSecondsRealtime(2);
        gameObject.SetActive(false);
    }

    private IEnumerator SpawnItem(){
        MiniGame1Item itemToSpawn = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        Instantiate(itemToSpawn, spawnPoint[Random.Range(0, 2)].position, Quaternion.identity, transform);
        yield return new WaitForSecondsRealtime(Random.Range(minSpawnInterval, maxSpawnInterval));
        StartCoroutine(SpawnItem());
    }
}
