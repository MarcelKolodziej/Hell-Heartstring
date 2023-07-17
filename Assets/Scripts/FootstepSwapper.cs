using System.Collections;
using UnityEngine;

    public class FootstepSwapper : MonoBehaviour
    {
        private TerrainChecker terrainChecker;
        private FirstPersonController firstPersonController;
        private string currentLayer;
        FootstepsCollection[] terrainFootstepsCollections;
        // Use this for initialization
        void Start()
            {
                  terrainChecker = new TerrainChecker();
                  firstPersonController= GetComponent<FirstPersonController>();
        print(terrainFootstepsCollections);
            }

            // Update is called once per frame
            void Update() 
            {
        //      Debug.Log(CheckLayers().collection);
            }
            
        public void CheckLayers() {
            // raycast down
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3 ))
            {
                // check if terrain exist
                if (hit.transform.GetComponent<Terrain>() != null)
                {
                Terrain t = hit.transform.GetComponent<Terrain>();
                    // if layer match our currentLayer
                    if (currentLayer != terrainChecker.GetLayerName(transform.position, t)) 
                    {
                        currentLayer = terrainChecker.GetLayerName(transform.position, t);
                        // swap Footsteps
                       foreach (FootstepsCollection collection in terrainFootstepsCollections) {
                            if (currentLayer == collection.name) {
                                firstPersonController.SwapFootsteps(collection);
                                print(collection);
                            }
                        }
                    }
                }
            }
        }
    
    }
