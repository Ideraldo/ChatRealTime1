using System.Collections.Generic;

namespace ChatAPI.Services
{
    public class ChatService
    {
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();

        public bool AddUserToList(string usertoAdd)
        {
            lock(Users)
            {
                foreach(var user in Users)
                    if(user.Key.ToLower() == usertoAdd.ToLower())
                    {
                        return false;
                    }
            }
            Users.Add(usertoAdd, null);
            return true;
        }
    }
}
