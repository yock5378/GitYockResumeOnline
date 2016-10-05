using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;



namespace YockResume.Models
{
    public class SampleContext : DbContext
    {
        public DbSet<UserContact> UserContacts { get; set; }

        public SampleContext()
            : base("name=SQL_AppHarbor")
        {

        }

    }
}