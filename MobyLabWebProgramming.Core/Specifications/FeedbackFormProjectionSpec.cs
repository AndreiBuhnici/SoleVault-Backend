using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FeedbackFormProjectionSpec : BaseSpec<FeedbackFormProjectionSpec, FeedbackForm, FeedbackFormDTO>
{
    protected override Expression<Func<FeedbackForm, FeedbackFormDTO>> Spec => e => new()
    {
        Id = e.Id,
        Feedback = e.Feedback,
        OverallRating = e.OverallRating,
        DeliveryRating = e.DeliveryRating,
        FavoriteFeatures = e.FavoriteFeatures,
        CreatedAt = e.CreatedAt
    };

    public FeedbackFormProjectionSpec(Guid id) : base(id)
    {
    }

    public FeedbackFormProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Feedback, searchExpr) ||
                         EF.Functions.ILike(e.OverallRating.ToString(), searchExpr) ||
                         EF.Functions.ILike(e.DeliveryRating.ToString(), searchExpr) ||
                         EF.Functions.ILike(e.FavoriteFeatures, searchExpr));
    }
}   