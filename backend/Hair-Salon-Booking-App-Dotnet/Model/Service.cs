using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("services")]
public class Service
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("duration")]
    public TimeSpan Duration { get; set; }

    [Column("price")]
    public decimal Price { get; set; }
}
