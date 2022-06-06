using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Client.Models
{
    public class PhoneHistory
    {
        public PhoneHistory(string path)
        {
            this.Path = path;
            this.phones = Load(path);
        }

        public string Path { get; set; }

        public Dictionary<string, Phone> GetPhones()
        {
            return this.phones;
        }

        public Dictionary<string, Phone> Load(string path)
        {
            Dictionary<string, Phone> phones = new Dictionary<string, Phone>();
            return phones;
        }

        public bool Check(string mac)
        {
            return this.phones.ContainsKey(mac);
        }

        public void Add(Phone phone)
        {
            this.phones.Add(phone.Mac, phone);
        }

        public void Remove(string phone)
        {
            this.phones.Remove(phone);
        }

        public void Save()
        {
            
        }

        private Dictionary<string, Phone> phones;
    }
}
