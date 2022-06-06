using PC_Client.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Client.Models
{
    public static class PCMessage
    {
        public enum Type
        {
            pc_connect_server,
            pc_accept,
            pc_reject,
            pc_answer,
            pc_candidate,
            pc_disconnect_android,

        }

        public static string ConnnectToServer(string ip, string name, string os)
        {
            dynamic obj = new ExpandoObject();
            obj.type = Type.pc_connect_server.ToString();
            obj.ip = ip;
            obj.name = name;
            obj.os = os;   

            return Json.Stringify(obj);
        }

        public static string Accept(string mac, string linked_date)
        {
            dynamic obj = new ExpandoObject();
            obj.type = Type.pc_accept.ToString();
            obj.mac = mac;
            obj.linked_date = linked_date;

            return Json.Stringify(obj);
        }

        public static string Reject(string mac)
        {
            dynamic obj = new ExpandoObject();
            obj.type = Type.pc_reject.ToString();
            obj.mac = mac;

            return Json.Stringify(obj);
        }

        public static string Answer(string mac, object answer)
        {
            dynamic obj = new ExpandoObject();
            obj.type = Type.pc_answer.ToString();
            obj.mac = mac;
            obj.answer = answer;

            return Json.Stringify(obj);
        }

        public static string Candidate(string mac, object candidate)
        {
            dynamic obj = new ExpandoObject();
            obj.type = Type.pc_candidate.ToString();
            obj.mac = mac;
            obj.candidate = candidate;

            return Json.Stringify(obj);
        }

        public static string DisconnectAndroid(string ip, string mac, string last_active)
        {
            dynamic obj = new ExpandoObject();
            obj.type = Type.pc_disconnect_android.ToString();
            obj.mac = mac;
            obj.last_active = last_active;

            return Json.Stringify(obj);
        }
    }
}
