using Project.Booking.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Model
{
    public class Email
    {

        public  string host = Constant.Email.HOST;
        public string username = Constant.Email.USER_NAME;
        public string password = Constant.Email.PASSWORD;
        public int port = Constant.Email.PORT;
        public string from = Constant.Email.FROM;
        private List<string> _To;
        public List<string> To
        {
            get
            {
                if (_To == null)
                {
                    _To = new List<string>();
                }
                return _To;
            }
            set { _To = value; }
        }

        private List<string> _Bcc;
        public List<string> Bcc
        {
            get
            {
                if (_Bcc == null)
                {
                    _Bcc = new List<string>();
                }
                return _Bcc;
            }
            set { _Bcc = value; }
        }

        private List<string> _CC;
        public List<string> CC
        {
            get
            {
                if (_CC == null)
                {
                    _CC = new List<string>();
                }
                return _CC;
            }
            set { _CC = value; }
        }        

        public string Body { get; set; }
        public string Subject { get; set; }
    }
}
