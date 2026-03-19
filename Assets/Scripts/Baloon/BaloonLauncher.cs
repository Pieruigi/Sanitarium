using Baloon;
using TMM;
using UnityEngine;
using UnityEngine.Events;

public class BaloonLauncher : MonoBehaviour
{
    public delegate void DirectionChangedDelegate(BaloonLauncher baloonLauncher);
    public static DirectionChangedDelegate OnDirectionChanged;

    [SerializeField]
    Vector4 directions = Vector4.zero;

    [SerializeField]
    int initialDirection = 0;

    int currentDirection;
    public int CurrentDirection => currentDirection;

    bool[] internalDirections;

    private void Awake()
    {
        internalDirections = new bool[] { directions.x > 0, directions.y > 0, directions.z > 0, directions.w > 0 };
        currentDirection = initialDirection;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TrySwitchDirection()
    {
        Debug.Log("Switching direction");     
        int length = internalDirections.Length;
        
        for(int i=1; i<length; i++)
        {
            int next = (currentDirection + i) % length;

            if (internalDirections[next])
            {
                currentDirection = next;
                Debug.Log("New direction " + currentDirection);
                OnDirectionChanged?.Invoke(this);
                return true;
            }
        
        }

        return false;
        
    }
   
}
