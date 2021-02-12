using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeViewBot.Models
{
    public class ProxyDBContext:DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseSqlite("Data Source=youtubeviewbot.db");

        public DbSet<ProxyDetail> ProxyDetails { get; set; }
    }
}
