using System.Collections;
using UnityEngine;


public class PhoneInputHandler : MonoBehaviour
{
    [Header("Phone assets")]
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject phone;

    private Animator animator;
    private string currentAnimation = "";

    //Animations
    private bool isViewing = false;
    private bool isEquiping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PhoneInput();
        CheckAnimation();
    }

    private void PhoneInput()
    {
        //Equip/Unequip phone
        if (Input.GetKeyDown(KeyCode.P))
        {
            isEquiping = !isEquiping;

            if (isEquiping)
            {
                ChangeAnimation("Equip");
            }
            else { ChangeAnimation("Unequip"); 
            }
        }

        //Flashlight toggle
        if (Input.GetKeyDown(KeyCode.F))
        {
            torch.SetActive(!torch.activeSelf);
        }

        //View phone
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isViewing = !isViewing;

            if (isViewing)
            {
                ChangeAnimation("View");
            }
            else { ChangeAnimation("UnView"); }
        }

    }

    private void CheckAnimation()
    {
       // ChangeAnimation("Idle");

        if (currentAnimation == "View" || currentAnimation == "Unview")
        {
            return;
        }
      
    }

    public void ChangeAnimation(string animation, float crossfade = 0.2f, float time = 0f)
    {
        if (time > 0f) StartCoroutine(Wait());
        else Validate();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossfade);
        }

        void Validate()
        {
            if (currentAnimation != animation)
            {
                currentAnimation = animation;

                if (currentAnimation == "")
                {
                    CheckAnimation();
                }
                else { animator.CrossFade(animation, crossfade); }


            }
        }
    }
}
