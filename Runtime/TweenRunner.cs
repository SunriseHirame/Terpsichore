using System.Collections.Generic;
using Hirame.Pantheon.Core;
using UnityEngine;

namespace Hirame.Terpsichore
{
    [AddComponentMenu (null)]
    public class TweenRunner : GameSystem<TweenRunner>
    {
        private readonly List<ITweener> tweenerUpdate = new List<ITweener> (64);
        
        internal void AddTweener (ITweener tweener)
        {
            tweenerUpdate.Add (tweener);
        }

        internal void RemoveTweener (ITweener tweener)
        {
            RemoveTweener (tweenerUpdate.IndexOf (tweener));
        }
        
        private void RemoveTweener (int index)
        {
            var last = tweenerUpdate.Count - 1;
            tweenerUpdate[index] = tweenerUpdate[last];
            tweenerUpdate.RemoveAt (last);
        }
        
        private void Update ()
        {
            for (var i = 0; i < tweenerUpdate.Count; i++)
            {
                var tweener = tweenerUpdate[i];
                
                if (tweener.IsRunning)
                    tweener.OnUpdateTween ();
                else
                    RemoveTweener (i);
            }
        }
    }

}