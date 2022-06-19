using System.ComponentModel.DataAnnotations;

namespace RateLimiterLibrary.Options;

public class RouteLimiterOptions
{
    // TODO в принципе все делается по привычке, но если вспомнить первую работу)) то тут можно использовать массив

    // TODO так же можно выделить интерфейс обобщенный типа ILimitterOptions... написал ниже в комменте
	// Таким образом можно объединить тип описания для Options
    [Required]
    public List<RouteLimiterOption> Options { get; set; }
}


// public interface ILimiterOptions<TOptions>
// {
//     TOptions[] Options { get; set; }
// }