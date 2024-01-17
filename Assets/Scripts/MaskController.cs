using UnityEngine;
using UnityEngine.InputSystem;

public class MaskController : MonoBehaviour
{
    //ComponentReferences
    private MaskController currentMask;
    //Params
    //Temps
    //Publics

    public void OnButton1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ChangeMask(1);
    }

    private void ChangeMask(int index)
    {
        return;
    }
}