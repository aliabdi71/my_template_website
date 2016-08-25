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
        public UserAccouunt getAgentById(int id)
        {
            UserAccouunt agent = null;
            using (var context = new AmirhomeEntities())
            {
                agent = (from U in context.UserAccouunts
                         where U.ID == id
                         select U).FirstOrDefault();
            }
            return agent;
        }
        public List<UserAccouunt> getAllAgent()
        {
            List<UserAccouunt> res = null;
            using (var context = new AmirhomeEntities())
            {
                res = (from A in context.UserAccouunts
                       select A).ToList();
            }
            return res;
        }
        public bool deleteAgent(int id)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    User agent = (from A in context.Users
                                  where A.UserID == id
                                  select A).FirstOrDefault();
                    context.Users.Remove(agent);
                    context.Entry(agent).State = EntityState.Deleted;
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool editAgent(User agent)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    context.Users.Attach(agent);
                    context.Entry(agent).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}