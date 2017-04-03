using System;

namespace Shared
{
    [Serializable]
    public class User : IEquatable<User>
    {
        private string name;
        private string nick;
        private string pass;
        private Status status;
        
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Nick
        {
            get
            {
                return nick;
            }

            set
            {
                nick = value;
            }
        }

        public string Pass
        {
            get
            {
                return pass;
            }

            set
            {
                pass = value;
            }
        }

        public Status Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public User(string name, string nick, string pass)
        {
            this.Name = name;
            this.Nick = nick;
            this.Pass = pass;
            this.Status = Status.Online;
        }

        public User(string name, string nick, string pass, Status status)
        {
            this.name = name;
            this.nick = nick;
            this.pass = pass;
            this.status = status;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User user) 
        {
            return this.Nick == user.Nick;
        }

        public override int GetHashCode()
        {
            return this.Nick.GetHashCode();
        }
    }
}
