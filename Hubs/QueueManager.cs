using System.Collections.Generic;
using Talktif.Models;

namespace Talktif.Hubs
{
    public class QueueManager
    {
        public List<WaitUser> UserQueue { get; set; }
        private QueueManager()
        {
            UserQueue = new List<WaitUser>();
        }
        private static QueueManager _Instance;
        public static QueueManager Instance
        {
            get
            {
                if (_Instance == null) _Instance = new QueueManager();
                return _Instance;
            }
            private set
            {
            }
        }

        public void Dequeue(int index)
        {
            UserQueue.RemoveAt(index);
        }
        public void Dequeue(string id)
        {
            for (int i = 0; i < UserQueue.Count; i++)
            {
                if (UserQueue[i].ConnectionID == id)
                {
                    Dequeue(i);
                    return;
                }
            }
        }
        public RandomRoom Enqueue(WaitUser usr)
        {
            int maxScore = 0;
            WaitUser targetUser = new WaitUser();
            if (usr.Filter != null)
            {
                string[] filterData = usr.Filter.Split(",");
                for (int i = 0; i < UserQueue.Count; i++)
                {
                    // Match conditions with filter
                    if (usr.ConnectionID != UserQueue[i].ConnectionID && !usr.SkipID.Contains(UserQueue[i].ConnectionID)
                        && !UserQueue[i].SkipID.Contains(usr.ConnectionID))
                    {
                        // Filter
                        if (UserQueue[i].Filter != null)
                        {
                            string[] filterDataI = UserQueue[i].Filter.Split(",");
                            int score = Similarity(filterData, filterDataI);
                            if (score > maxScore)
                            {
                                maxScore = score;
                                targetUser = UserQueue[i];
                            }
                        }

                        // RandomRoom room = RoomManager.Instance.CreateRoom(new WaitUser[] {
                        //     usr,
                        //     UserQueue[i]
                        // });
                        // Dequeue(i);
                        // return room;
                    }
                }
            }

            if (maxScore == 0)
            {
                // In case no filter found
                for (int i = 0; i < UserQueue.Count; i++)
                {
                    if (usr.ConnectionID != UserQueue[i].ConnectionID && !usr.SkipID.Contains(UserQueue[i].ConnectionID)
                        && !UserQueue[i].SkipID.Contains(usr.ConnectionID))
                    {
                        RandomRoom room = RoomManager.Instance.CreateRoom(new WaitUser[] {
                            usr,
                            UserQueue[i]
                        });
                        Dequeue(i);
                        return room;
                    }
                }
            }
            else
            {
                // In case match filter found
                RandomRoom room = RoomManager.Instance.CreateRoom(new WaitUser[] {
                    usr,
                    targetUser
                });
                Dequeue(targetUser.ConnectionID);
                return room;
            }
            UserQueue.Add(usr);
            return null;
        }
        private int Similarity(string[] arr1, string[] arr2)
        {
            int equalCount = 0;
            for (int i = 0; i < arr1.Length; i++)
            {
                for (int j = 0; j < arr2.Length; j++)
                {
                    if (arr1[i] == arr2[j])
                    {
                        equalCount++;
                        break;
                    }
                }
            }
            return equalCount;
        }
    }
}