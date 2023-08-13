using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardManager : MonoBehaviour
{
    private Card[] TheBoard;

    public Animator turnAnouncerAnim;
    public TextMeshProUGUI turnAnouncerText;
    public bool PlayersTurn = true;

    //player stuff
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
        TheBoard = new Card[16];

        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) NextTurn();  //testing
    }

    public void NextTurn()
    {


        PlayersTurn = !PlayersTurn;
        turnAnouncerText.text = (PlayersTurn ? "Player's" : "Enenemy's") + " turn!";
        turnAnouncerAnim.SetTrigger("go");
    }

    public bool PlaceCard(Card card, GameObject obj)
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
        if(Slot != null && (card.Primary == ind >= 4) && ((card.element == Card.elements.light && Lux >= card.cost) || (card.element == Card.elements.dark && Umbra >= card.cost)))
        {
            obj.transform.position = Slot.position;
            if (card.element == Card.elements.light)
            {
                Lux -= card.cost;
                LuxMeter.value = Lux;
            }
            else
            {
                Umbra -= card.cost;
                UmbraMeter.value = Umbra;
            }
            TheBoard[9 + ind] = card;

            return true;
        }
        return false;
    }
}
