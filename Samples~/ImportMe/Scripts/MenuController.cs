using UnityEngine;

public class MenuController : MonoBehaviour
{
    [field: SerializeField] public MenuUIController MenuUIController { get; private set; }

    private void Start()
    {
        MenuUIController.enabled = true;
    }
}