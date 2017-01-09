using System.ComponentModel.DataAnnotations;

namespace vote.Models
{
    public class AdditionViewModel
    {
        [Required]
        [Display(Name = "Название мероприятия")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Текст")]
        public string Text { get; set; }

        [Required]
        [Display(Name = "Дата проведения")]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        [Display(Name = "Место")]
        public string Place { get; set; }

        [Required]
        [Display(Name = "Вид соревнования")]
        public string Kind { get; set; }

    }

}