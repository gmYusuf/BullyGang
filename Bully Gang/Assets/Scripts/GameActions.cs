using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions :  HMSSingleton<GameActions>
{
    public GameObject character1, character2;
    Animator playerAnimator;
    private static bool characterSide = true;

    private GamePlayer player;

    // Start is called before the first frame update
    void Start()
    {
        player = GamePlayer.Left;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCharacter(bool leftOrRight)
    {
        if(leftOrRight)
            player = GamePlayer.Left;
        else
            player = GamePlayer.Left;
    }

    public void FightTrigger()
    { 
        playerAnimator = character1.GetComponent<Animator>();
        playerAnimator.SetTrigger("fight");  
    }

    enum GamePlayer
    {
        Left,
        Right
    }
}
