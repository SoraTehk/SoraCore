using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [field: SerializeField] public GameplayUIController GameplayUIController { get; private set; }

    private void Start()
    {
        GameplayUIController.enabled = true;
    }
}