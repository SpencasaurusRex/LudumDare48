using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour {
    public Image[] Digits;
    public Sprite[] Numbers;
    public float CountSpeed = 50;

    int amount;
    public float fluidAmount;
    int maxAmount;
    public bool Done;

    public void SetAmount(int amt, float fluid = 0) {
        amount = amt;
        fluidAmount = fluid;
    }

    void Start() {
        Digits = GetComponentsInChildren<Image>();
        maxAmount = Mathf.RoundToInt(Mathf.Pow(10, Digits.Length)) - 1;
    }

    public void Reset() {
        Done = false;
        amount = 0;
        fluidAmount = 0;
        UpdateNumbers(fluidAmount);
    }

    public void UpdateNumbers(float fl) {
        int i = 0;
        int f = (int)fl;
        for (int place = Digits.Length - 1; place >= 0; place--) {
            int a = Mathf.RoundToInt(Mathf.Pow(10, place));
            int r = (f / a) % 10;
            Digits[i].sprite = Numbers[r];
            i++;
        }
    }

    void Update() {
        if (Done) return;
        if (amount == 0) return;
        fluidAmount += CountSpeed * Time.deltaTime;
        
        if (fluidAmount >= Mathf.Min(maxAmount, amount)) {
            fluidAmount = Mathf.Min(maxAmount, amount);
            Done = true;
            // TODO? Animation
        }

        UpdateNumbers(fluidAmount);
    }
}
