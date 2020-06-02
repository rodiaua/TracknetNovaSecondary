using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tree.Models
{
  public class MeterLocationTree
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string LocationElement { get; set; }

    public int[] Path { get; set; }
  }
}
