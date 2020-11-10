using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBrowserDataBase
{
    [Table("Function", Schema = "Shared")]
    public class Function
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
