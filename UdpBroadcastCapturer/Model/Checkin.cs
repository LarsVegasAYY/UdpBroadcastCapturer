using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpBroadcastCapture.Model
{
    public class Checkin
    {
        public Checkin(int scannerkey, int classroom, DateTime checkinTime, bool checkingIn)
        {
            Scannerkey = scannerkey;
            Classroom = classroom;
            CheckinTime = checkinTime;
            CheckingIn = checkingIn;
        }
        public Checkin(int scannerkey, int classroom, DateTime checkinTime)
        {
            Scannerkey = scannerkey;
            Classroom = classroom;
            CheckinTime = checkinTime;
        }
        public int Scannerkey { get; set; }
        public int Classroom { get; set; }
        public DateTime CheckinTime { get; set; }
        public bool CheckingIn { get; set; }
    }
}
