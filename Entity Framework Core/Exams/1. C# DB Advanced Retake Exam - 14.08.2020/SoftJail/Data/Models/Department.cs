namespace SoftJail.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Department
    {
        public Department()
        {
            this.Cells = new HashSet<Cell>();
            this.Officers = new HashSet<Officer>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }

        public virtual IEnumerable<Cell> Cells { get; set; }

        public virtual IEnumerable<Officer> Officers { get; set; }
    }
}
