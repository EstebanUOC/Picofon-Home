using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public readonly struct CountriesData
{
    public readonly CountryDTO[] Records { get; init; }
}

public readonly struct CountryDTO
{
    public readonly int Id { get; init; }
    public readonly string Name { get; init; }
}

public readonly struct CentersData
{
    public readonly CenterRegisterDTO[] Records { get; init; }
}

public readonly struct CenterRegisterDTO
{
    public readonly int Id { get; init; }
    public readonly string Name { get; init; }
}

public readonly struct GradeDTO
{
    public readonly int Id { get; init; }
    public readonly string LocalName { get; init; }
}

public readonly struct AcademicService
{
    public async UniTask<ApiResult<CountriesData>> GetCountries(CancellationToken token = default)
    {
        string url = $"{ApiConfig.BaseUrl}/countries/get_active_countries";

        byte[] rawResponse;

        try
        {
            rawResponse = await HttpClientUnity.GetAsyncBytes(
                url: url,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception)
        {
            return ApiResult<CountriesData>.Fail(
                "Network error occurred while fetching activities."
            );
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<CountriesData> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<CountriesData>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<CountriesData>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult<CentersData>> GetCenters(
        int countryId,
        CancellationToken token = default
    )
    {
        string url = $"{ApiConfig.BaseUrl}/centers/?country_id={countryId}";

        byte[] rawResponse;

        try
        {
            rawResponse = await HttpClientUnity.GetAsyncBytes(
                url: url,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception)
        {
            return ApiResult<CentersData>.Fail("Network error occurred while fetching activities.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<CentersData> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<CentersData>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<CentersData>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult<GradeDTO[]>> GetGrades(
        int countryId,
        CancellationToken token = default
    )
    {
        string url = $"{ApiConfig.BaseUrl}/grades/list_grades?country_id={countryId}";

        byte[] rawResponse;

        try
        {
            rawResponse = await HttpClientUnity.GetAsyncBytes(
                url: url,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception)
        {
            return ApiResult<GradeDTO[]>.Fail("Network error occurred while fetching activities.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<GradeDTO[]> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<GradeDTO[]>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<GradeDTO[]>.Ok(responseView.Data);
    }
}
