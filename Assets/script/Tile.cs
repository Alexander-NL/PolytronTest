using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Tile : MonoBehaviour{
    [Header("Grid Settings")]
    public GameObject[] prefabArray;
    public int gridSize = 7;      
    public float cellSpacing = 1.5f;

    [Header("Tree Planting Settings")]
    public GameObject treePrefab;
    public Vector3 treeScale = Vector3.one;
    public float treePlantingInterval = 1f; 

    [Header("House Placement Settings")]
    public GameObject housePrefab;
    public Vector3 houseScale = Vector3.one;
    public Vector3 houseOffset = Vector3.zero;

    [Header("Grid Spawn")]
    public Vector3 gridOrigin = Vector3.zero;

    [Header("Score")]
    public TMP_Text Text;
    int score = 0;

    private void Start(){
        GenerateRandomGrid();
        StartCoroutine(PlantTrees());
    }

    void GenerateRandomGrid(){
        List<GameObject> shuffledPrefabs = new List<GameObject>(prefabArray);
        ShuffleList(shuffledPrefabs);

        int prefabIndex = 0;

        for (int row = 0; row < gridSize; row++){
            for (int col = 0; col < gridSize; col++){
                Vector3 spawnPosition = gridOrigin + new Vector3(col * cellSpacing, 0, row * cellSpacing);

                GameObject prefabToInstantiate;

                if (prefabIndex < shuffledPrefabs.Count){
                    prefabToInstantiate = shuffledPrefabs[prefabIndex];
                    prefabIndex++;
                }
                else{
                    prefabToInstantiate = prefabArray[Random.Range(0, prefabArray.Length)];
                }

                Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    System.Collections.IEnumerator PlantTrees(){
        while (true){
            GameObject[] dirtTiles = GameObject.FindGameObjectsWithTag("Dirt");

            if (dirtTiles.Length > 0){
                GameObject randomDirtTile = dirtTiles[Random.Range(0, dirtTiles.Length)];

                if (randomDirtTile.transform.childCount == 0){
                    GameObject newTree = Instantiate(treePrefab, randomDirtTile.transform.position, Quaternion.identity, randomDirtTile.transform);

                    newTree.transform.localScale = treeScale;
                }
            }

            yield return new WaitForSeconds(treePlantingInterval);
        }
    }

    public void PlaceHouse(GameObject tile){
        if (tile.CompareTag("Dirt") || tile.CompareTag("Desert")){
            if (tile.transform.childCount == 0){
                Vector3 placementPosition = tile.transform.position + houseOffset;
                GameObject newHouse = Instantiate(housePrefab, placementPosition, Quaternion.identity, tile.transform);

                newHouse.transform.localScale = houseScale;
                Score(tile);
            }else{
                Debug.Log("Nope something there");
            }
        }
        else{
            Debug.Log("Nope not dirt or sand");
        }
    }

    //to make sure all prefab spawned
    void ShuffleList<T>(List<T> list){
        for (int i = list.Count - 1; i > 0; i--){
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)){
                PlaceHouse(hit.collider.gameObject);
            }
        }
    }

    void Score(GameObject tile){
        if(tile.name.Contains("Dirt")){
            score = score + 10;
        }
        
        if(tile.name.Contains("Desert")){
            score = score + 2;
        }
        UpdateScore();
    }

    void UpdateScore(){
        Text.text = "Score: " + score;
    }
}
