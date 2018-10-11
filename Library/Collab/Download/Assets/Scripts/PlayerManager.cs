﻿using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    int playerHealth = 3;
    public float speed = 2f;
    public GameObject[] heart;
    private static PlayerManager _instance;
    public float recoverTime = .2f;
    bool canHurtPlayer = true;
    public static PlayerManager Instance { get { return _instance; } }
    private IEnumerator coroutine;
    public bool facingRight = true;
    public Animator anim;
    public SpriteRenderer playerSprite;
    bool movementEnabled = true;
    public RPGTalk rpgTalk;
    public bool swordEquipped =true;
    public GameObject sword;
    // Use this for initialization
    private void Awake()
    {
        rpgTalk = GameObject.Find("RPGTalk Holder").GetComponent<RPGTalk>();

        if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        
    }
    void Start () {
        if (swordEquipped)
            EquipSword();
        else if(!swordEquipped)
        {
            UnequipSword();

        }
    }
    void OnEnable()
    {
        rpgTalk.OnNewTalk += DisableMovement;
        rpgTalk.OnEndTalk += EnableMovement;
    }
    private void OnDisable()
    {
        rpgTalk.OnNewTalk -= DisableMovement;
        rpgTalk.OnEndTalk -= EnableMovement;
    }
    public void DisableMovement()
    {
        movementEnabled = false;
    }
    public void EnableMovement()
    {
        movementEnabled = true;
    }
    void Flip()
    {
        if (facingRight == false)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        if (facingRight == true)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void UnequipSword()
    {
        Debug.Log("unequpp");
        sword.SetActive(false);
        swordEquipped = false;
    }
    public void EquipSword()
    {
        sword.SetActive(true);
        swordEquipped = true;
    }
    // Update is called once per frame
    void Update () {
        if(Input.GetMouseButtonDown(0) && swordEquipped)
        {
            anim.SetTrigger("Slash");
        }
       else if (GameManager.Instance.mobileControls && movementEnabled)
        {
            Debug.Log("using android.");
            GetComponent<Rigidbody2D>().velocity = Vector2.right * Input.acceleration.x * speed;
            if (Input.acceleration.x < 0)
                facingRight = false;
            else if(Input.acceleration.x > 0)
                facingRight = true;
        }
       else if(movementEnabled)
        {

            var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            GetComponent<Rigidbody2D>().velocity = Vector2.right * Input.GetAxis("Horizontal") * speed;
            if(Input.GetAxis("Horizontal") > 0)
            {
                anim.SetBool("Running",true);
                facingRight = true;
                Flip();
            }
            else if(Input.GetAxis("Horizontal") < 0)
            {
                anim.SetBool("Running", true);
                facingRight = false;
                Flip();
            }
            else
            {
                anim.SetBool("Running", false);
            }
        }
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP_8_1

#endif


    }
    public void SetPlayerImmunity()
    {

        coroutine = SetImmune(recoverTime);
        StartCoroutine(coroutine);
    }
    private IEnumerator SetImmune(float waitTime)
    {
        
        //Color tmp = playerSprite.color;
        //tmp.a = .3f;
        //playerSprite.color= tmp;
        canHurtPlayer = false;
        yield return new WaitForSeconds(waitTime);
        canHurtPlayer = true;
        //tmp.a = 1f;
        //playerSprite.color = tmp;
    }
    

    void DamagePlayer()
    {
        if(canHurtPlayer)
        {
            if(playerHealth <= 0)
            {
                GameManager.Instance.LoseGame();
            }
            else
            {
                playerHealth--;
                heart[playerHealth].SetActive(false);
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<ICharacterEnter>() != null)
            collision.gameObject.GetComponent<ICharacterEnter>().Interact();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            DamagePlayer();
        if (collision.gameObject.GetComponent<ICharacterEnter>() != null)
            collision.gameObject.GetComponent<ICharacterEnter>().Interact();
    }
}
