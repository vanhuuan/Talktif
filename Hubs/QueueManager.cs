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
            // RoomManager.Instance.RemoveRoom(usr);
            for (int i = 0; i < UserQueue.Count; i++)
            {
                // TODO: Filter
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
            UserQueue.Add(usr);
            return null;
        }
    }
}