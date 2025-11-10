//using UnityEngine;

//public class PlayerShootsss : MonoBehaviour
//{

//    public float m_speed = 2;
    
//    private void Update()
//    {
//        transform.position += m_speed* Time.deltaTime * transform.forward;
//        PlayerShoot();
//    }

//    public void PlayerShoot()
//    {

//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;
//        if (Input.GetMouseButtonDown(0))
//        {


            

//            // Health.cs
//            if (Physics.Raycast(ray, out hit, 100f))
//            {
                 
//               if(hit.collider.TryGetComponent<IDamagable>(out IDamagable c))
//               {
//                    c?.TakeHit(8);
//               }
//               else
//                {
//                    hit.collider.GetComponentInChildren<IDamagable>()?.TakeHit(8);
//                }

               
               
//            }
//        }

//        if (Physics.Raycast(ray, out hit, 100f))
//        {

            
//      var b = hit.collider.GetComponent<LockOnDodgeEnemy>();
//      if (b != null)
//      {
//         b.TriggerDodge();
//      }

//    }
//    }
//}
