using UnityEngine;

public class AlchemistNPC : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] PickupManager _ingredientPickup;
        Animator _anim;
        void Start()
        {
		_anim = GetComponent<Animator>();
        }

    // Update is called once per frame
        void Update()
        {
                _anim.SetBool("creating", _ingredientPickup.GetItemCount() > 0); 
        }
}
