using System.Collections.Generic;
using UnityEngine;

public class Frames : MonoBehaviour
{
    private List<Vector2> frameSizes = new List<Vector2>
    {
        new Vector2(0.1f, 0.15f), // 4x6 inches (10 x 15 cm)
        new Vector2(0.13f, 0.18f), // 5x7 inches (13 x 18 cm)
        new Vector2(0.20f, 0.25f), // 8x10 inches (20 x 25 cm)
        new Vector2(0.30f, 0.30f), // 12x12 inches (30 x 30 cm)
        new Vector2(0.30f, 0.40f)  // 12x16 inches (30 x 40 cm)
    };

    public void SetFrameSize(int index)
    {
        if (index >= 0 && index < frameSizes.Count)
        {
            Vector2 selectedSize = frameSizes[index];
            Debug.Log("Creating object of frame size: " + selectedSize);
        }
    }

}
