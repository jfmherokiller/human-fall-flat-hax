using UnityEngine;
namespace human_fall_flat_hax
{
   public class hookies
    {
        void playerhook()
        {
            var Playerinstance = GameObject.Find("Player");
            if (Playerinstance != null)
            {
                Debug.Log(Playerinstance.ToString());
            }
        }

        void scenetest()
        {
            helperfunctions.CreateGameObjectAndAttachClass(new KeypressGameObject());
        }

        public void allhooks()
        {
            //playerhook();
            scenetest();
        }
    }
}
