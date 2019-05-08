using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using front_end.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace front_end.DAL
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(): base("ApplicationContext")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}