using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    [Serializable]
    public class User
    {
        private string name;
        private string nick;
        private string pass;
        private Status status;
        private IPEndPoint endpoint;
        
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

        public IPEndPoint Endpoint
        {
            get
            {
                return endpoint;
            }

            set
            {
                endpoint = value;
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
    }
}
