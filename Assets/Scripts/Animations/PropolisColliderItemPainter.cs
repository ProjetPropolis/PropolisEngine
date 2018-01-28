using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Propolis
{


    public class PropolisColliderItemPainter : MonoBehaviour
    {
        public PropolisStatus StatusToPaint;
        [Range(0.0f, 10.0f)]
        public float DelayInSeconds = 0.0f;
        [Range(0.0f, 10.0f)]
        public float RestoreStatusAfterSeconds = 0.0f;
        public bool WillRestoreStatusAfter = false;
        private PropolisStatus _lastStatus;
        private void OnTriggerEnter2D(Collider2D other)
        {
            AbstractItem item = other.GetComponent<AbstractItem>();
            if (item != null)
            {
                StartCoroutine(StartPainting(item));
            }

        }


        private IEnumerator StartPainting(AbstractItem item)
        {
            yield return  new WaitForSecondsRealtime(DelayInSeconds);

            if (item != null)
            {
                _lastStatus = item.status;
                item.ParentGroup.parentGameController.SendItemData(item.ParentGroup.ID, item.ID, StatusToPaint);

                if (WillRestoreStatusAfter)
                {
                    yield return new WaitForSecondsRealtime(RestoreStatusAfterSeconds);
                    item.ParentGroup.parentGameController.SendItemData(item.ParentGroup.ID, item.ID, _lastStatus);

                }
            }
        }

    }


}


