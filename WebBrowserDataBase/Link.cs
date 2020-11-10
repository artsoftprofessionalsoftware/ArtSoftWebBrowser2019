using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBrowserDataBase
{
    [Table("Link", Schema = "Shared")]
    public class Link
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public bool Added { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> Loaded { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.DateTime> Opened { get; set; }
        public int Time { get; set; }
    }
}
