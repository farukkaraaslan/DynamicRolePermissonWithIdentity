using Core.Utilities.Results;

namespace Core.Utilities.Business;

public class BusinessRules
{
    public static IResult Run(params IResult[] logics)
    {
        foreach (var result in logics)
        {
            if (!result.Success)
            {
                return result;
            }
        }

        return null;
    }

    public static async Task<IResult> RunAsync(params Task<IResult>[] logics)
    {
        foreach (var logic in logics)
        {
            var result = await logic;
            if (!result.Success)
            {
                return result;
            }
        }

        return new SuccessResult(); // Burada SuccessResult dönebilirsiniz, çünkü tüm kurallar geçerliyse başarılı sayılabilir.
    }
}
