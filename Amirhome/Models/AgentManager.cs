using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Entity;


namespace Amirhome.Models
{
    public class AgentManager
    {
        public Amirhome.Models.User getAgentById(int id)
        {
            Amirhome.Models.User agent = null;
            using (var context = new AmirhomeEntities())
            {
                agent = (from U in context.Users
                         where U.UserID == id
                         select U).FirstOrDefault();
            }
            return agent;
        }
        public List<Amirhome.Models.User> getAllAgent()
        {
            List<Amirhome.Models.User> res = null;
            using (var context = new AmirhomeEntities())
            {
                res = (from A in context.Users
                       select A).ToList();
            }
            return res;
        }
        public bool deleteAgent(User agent)
        {
            return true;
        }
    }
}