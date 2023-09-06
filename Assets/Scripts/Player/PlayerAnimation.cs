using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator _playerAnim;

    private void Start()
    {
        

    }
    public void Run()
    {
        _playerAnim.SetBool("isRunning", true);
    }
    public void Idle() => _playerAnim.SetBool("isRunning", false);
    public void dash() => _playerAnim.SetTrigger("dash");
    public void jump() => _playerAnim.SetTrigger("jump");
    public  void die()
    {
        _playerAnim.SetTrigger("die");
        gameObject.SetActive(false);
    }


}
