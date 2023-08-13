using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Transform[] PlayerSlots;
    [SerializeField] private float SnapDistance;
    [SerializeField] private float MaxLux;
    [SerializeField] private Slider LuxMeter;
    [SerializeField] private float MaxUmbra;
    [SerializeField] private Slider UmbraMeter;
    float Lux;
    float Umbra;

    private void Start()
    {
        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
    }

    public bool PlaceCard(CardTemplate card, GameObject obj)
    {
        float Min = SnapDistance;
        Transform Slot = null;
        int ind = 0;
        for (int i = 0; i < PlayerSlots.Length; i++)
        {
            if(Vector2.Distance(obj.transform.position, PlayerSlots[i].position) < Min)
            {
                Min = Vector2.Distance(obj.transform.position, PlayerSlots[i].position);
                Slot = PlayerSlots[i];
                ind = i;
            }
        }
        if(Slot != null && (card.Primary == ind >= 4) && ((card.element == CardTemplate.elements.light && Lux >= card.cost) || (card.element == CardTemplate.elements.dark && Umbra >= card.cost)))
        {
            obj.transform.position = Slot.position;
            if (card.element == CardTemplate.elements.light)
            {
                Lux -= card.cost;
                LuxMeter.value = Lux;
            }
            else
            {
                Umbra -= card.cost;
                UmbraMeter.value = Umbra;
            }

            return true;
        }
        return false;
    }
}
