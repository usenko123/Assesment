using Assessment.Domain.Common;

namespace Assessment.Application.Tests.Domain;

public class ResultTests
{
    [Fact]
    public void Ok_Match_returns_success_path()
    {
        var result = Result<int>.Ok(42);
        var mapped = result.Match(
            v => v * 2,
            _ => -1);

        Assert.Equal(84, mapped);
    }

    [Fact]
    public void Fail_Match_returns_error_path()
    {
        var err = new AppError(AppErrorType.NotFound, Title: "missing");
        var result = Result<int>.Fail(err);
        var mapped = result.Match(
            static _ => default!,
            e => e.Title);

        Assert.Equal("missing", mapped);
    }
}
